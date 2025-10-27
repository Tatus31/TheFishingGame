using UnityEngine;
using UnityEditor;
using DG.Tweening;

[CustomEditor(typeof(AnimationDataSO))]
public class AnimationDataSOEditor : Editor
{
    AnimationDataSO so;
    bool isPreviewing = false;
    float previewTime = 0f;
    float totalDuration = 0f;
    float lastEditorUpdateTime = 0f;

    static Material s_GhostMaterial;

    void OnEnable()
    {
        so = (AnimationDataSO)target;
        SceneView.duringSceneGui += DuringSceneGUI;
        EditorApplication.update += EditorUpdate;
        lastEditorUpdateTime = (float)EditorApplication.timeSinceStartup;

        EnsureGhostMaterial();
    }

    void OnDisable()
    {
        SceneView.duringSceneGui -= DuringSceneGUI;
        EditorApplication.update -= EditorUpdate;

        if (s_GhostMaterial != null)
        {
            Object.DestroyImmediate(s_GhostMaterial);
            s_GhostMaterial = null;
        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MoveObjectsWithShip previewTarget = FindSceneTarget();
        if (previewTarget == null)
        {
            EditorGUILayout.HelpBox("No object in scene is using this AnimationDataSO.", MessageType.Info);
            return;
        }

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        if (!isPreviewing)
        {
            if (GUILayout.Button("Play Preview"))
                StartPreview();
        }
        else
        {
            if (GUILayout.Button("Stop Preview"))
                StopPreview();

            if (GUILayout.Button("Restart"))
            {
                previewTime = 0f;
                lastEditorUpdateTime = (float)EditorApplication.timeSinceStartup;
            }
        }

        if (GUILayout.Button("Reset"))
            SceneView.RepaintAll();

        EditorGUILayout.EndHorizontal();

        if (totalDuration <= 0f)
            ComputeTotalDuration();

        EditorGUILayout.BeginHorizontal();
        float newPreviewTime = EditorGUILayout.Slider(previewTime, 0f, Mathf.Max(0.0001f, totalDuration));

        if (!Mathf.Approximately(newPreviewTime, previewTime))
        {
            previewTime = newPreviewTime;
            SceneView.RepaintAll();
        }

        if (GUILayout.Button("0", GUILayout.Width(30))) 
        { 
            previewTime = 0f; 
            SceneView.RepaintAll();
        }

        if (GUILayout.Button("|<", GUILayout.Width(30))) 
        { 
            previewTime = Mathf.Max(0f, previewTime - 0.1f);
            SceneView.RepaintAll(); 
        }

        if (GUILayout.Button(">|", GUILayout.Width(30))) 
        { 
            previewTime = Mathf.Min(totalDuration, previewTime + 0.1f); 
            SceneView.RepaintAll(); 
        }

        EditorGUILayout.EndHorizontal();
    }

    void StartPreview()
    {
        if (so == null || so.AnimationData == null)
            return;

        isPreviewing = true;
        previewTime = 0f;

        ComputeTotalDuration();

        lastEditorUpdateTime = (float)EditorApplication.timeSinceStartup;
        SceneView.RepaintAll();
    }

    void StopPreview()
    {
        isPreviewing = false;
        previewTime = 0f;
        SceneView.RepaintAll();
    }

    void ComputeTotalDuration()
    {
        totalDuration = 0f;
        if (so.AnimationData == null)
            return;

        foreach (var anim in so.AnimationData)
            totalDuration += Mathf.Max(anim.MoveDuration, anim.RotateDuration);

        if (totalDuration <= 0f)
            totalDuration = 0.0001f;
    }

    void EditorUpdate()
    {
        if (!isPreviewing) return;

        float currentTime = (float)EditorApplication.timeSinceStartup;
        float delta = currentTime - lastEditorUpdateTime;
        lastEditorUpdateTime = currentTime;

        previewTime += delta;

        if (previewTime > totalDuration)
        {
            if (so.loopPreview)
                previewTime = 0f;
            else
                StopPreview();
        }

        SceneView.RepaintAll();
    }

    void DuringSceneGUI(SceneView sceneView)
    {
        if ((isPreviewing == false && Event.current.type != EventType.Repaint) && Event.current.type != EventType.Layout)
            return;

        if (so == null || so.AnimationData == null)
            return;

        MoveObjectsWithShip targetComponent = FindSceneTarget();

        if (targetComponent == null)
            return;

        Transform t = targetComponent.transform;

        Vector3 currentWorldPos = t.position;
        Quaternion currentWorldRot = t.rotation;

        float elapsed = 0f;

        foreach (var anim in so.AnimationData)
        {
            float segDur = Mathf.Max(0f, Mathf.Max(anim.MoveDuration, anim.RotateDuration));

            Vector3 segStartPos = currentWorldPos;
            Quaternion segStartRot = currentWorldRot;

            Vector3 segTargetPosWorld = segStartPos; 
            Quaternion segTargetRotWorld = segStartRot; 

            if (anim.MoveDuration > 0f)
            {
                if (anim.UseWorldSpace)
                {
                    segTargetPosWorld = anim.MoveToPosition;
                }
                else
                {
                    Transform parent = t.parent;

                    if (parent != null)
                        segTargetPosWorld = parent.TransformPoint(anim.MoveToPosition);
                    else
                        segTargetPosWorld = anim.MoveToPosition;
                }
            }

            if (anim.RotateDuration > 0f)
            {
                if (anim.UseWorldSpace)
                {
                    segTargetRotWorld = anim.RotateToRotation;
                }
                else
                {
                    Transform parent = t.parent;

                    if (parent != null)
                        segTargetRotWorld = parent.rotation * anim.RotateToRotation;
                    else
                        segTargetRotWorld = anim.RotateToRotation;
                }
            }

            float moveT = anim.MoveDuration > Mathf.Epsilon ? Mathf.Clamp01((previewTime - elapsed) / anim.MoveDuration) : (previewTime >= elapsed ? 1f : 0f);
            float rotT = anim.RotateDuration > Mathf.Epsilon ? Mathf.Clamp01((previewTime - elapsed) / anim.RotateDuration) : (previewTime >= elapsed ? 1f : 0f);

            float easedMoveT = anim.MoveDuration > Mathf.Epsilon ? DOVirtual.EasedValue(0f, 1f, moveT, anim.MoveEase) : (moveT >= 1f ? 1f : 0f);
            float easedRotT = anim.RotateDuration > Mathf.Epsilon ? DOVirtual.EasedValue(0f, 1f, rotT, anim.RotateEase) : (rotT >= 1f ? 1f : 0f);

            Vector3 nextPos = Vector3.Lerp(segStartPos, segTargetPosWorld, easedMoveT);
            Quaternion nextRot = Quaternion.Slerp(segStartRot, segTargetRotWorld, easedRotT);

            currentWorldPos = nextPos;
            currentWorldRot = nextRot;

            elapsed += segDur;

            if (previewTime < elapsed)
                break;
        }

        DrawGhost(t, currentWorldPos, currentWorldRot);
    }

    void DrawGhost(Transform target, Vector3 worldPos, Quaternion worldRot)
    {
        if (s_GhostMaterial == null) EnsureGhostMaterial();

        MeshFilter[] meshes = target.GetComponentsInChildren<MeshFilter>(true);

        foreach (var mf in meshes)
        {
            if (mf.sharedMesh == null) 
                continue;

            Transform meshTransform = mf.transform;

            Matrix4x4 rootToMesh = Matrix4x4.TRS(meshTransform.localPosition, meshTransform.localRotation, meshTransform.localScale);
            Matrix4x4 rootWorld = Matrix4x4.TRS(worldPos, worldRot, target.lossyScale);
            Matrix4x4 matrix = rootWorld * Matrix4x4.TRS(meshTransform.localPosition, meshTransform.localRotation, meshTransform.lossyScale);

            s_GhostMaterial.SetPass(0);

            if (s_GhostMaterial.HasProperty("_Color"))
                s_GhostMaterial.SetColor("_Color", new Color(0f, 1f, 1f, 0.25f));

            Graphics.DrawMeshNow(mf.sharedMesh, matrix);
        }
    }

    void EnsureGhostMaterial()
    {
        if (s_GhostMaterial != null) return;

        Shader shader = Shader.Find("Hidden/Internal-Colored");
        if (shader == null)
            shader = Shader.Find("Sprites/Default"); 

        s_GhostMaterial = new Material(shader);
        s_GhostMaterial.hideFlags = HideFlags.HideAndDontSave;
        s_GhostMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        s_GhostMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        s_GhostMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
        s_GhostMaterial.SetInt("_ZWrite", 0);
    }

    MoveObjectsWithShip FindSceneTarget()
    {
        MoveObjectsWithShip[] all = GameObject.FindObjectsOfType<MoveObjectsWithShip>();
        foreach (var obj in all)
        {
            if (obj.AnimationDataSO == so)
                return obj;
        }
        return null;
    }
}
