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

    void OnEnable()
    {
        so = (AnimationDataSO)target;
        SceneView.duringSceneGui += DuringSceneGUI;
        EditorApplication.update += EditorUpdate;
        lastEditorUpdateTime = (float)EditorApplication.timeSinceStartup;
    }

    void OnDisable()
    {
        SceneView.duringSceneGui -= DuringSceneGUI;
        EditorApplication.update -= EditorUpdate;
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

        if (!isPreviewing)
        {
            if (GUILayout.Button("Play Preview"))
                StartPreview();
        }
        else
        {
            if (GUILayout.Button("Stop Preview"))
                StopPreview();
        }
    }

    void StartPreview()
    {
        isPreviewing = true;
        previewTime = 0f;

        totalDuration = 0f;
        foreach (var anim in so.AnimationData)
            totalDuration += Mathf.Max(anim.MoveDuration, anim.RotateDuration);

        lastEditorUpdateTime = (float)EditorApplication.timeSinceStartup;
        SceneView.RepaintAll();
    }

    void StopPreview()
    {
        isPreviewing = false;
        previewTime = 0f;
        SceneView.RepaintAll();
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
        if (!isPreviewing || so.AnimationData == null) 
            return;

        MoveObjectsWithShip targetComponent = FindSceneTarget();

        if (targetComponent == null)
            return;

        Transform target = targetComponent.transform;
        Vector3 currentPos = target.localPosition;
        Quaternion currentRot = target.localRotation;

        Vector3 startLocalPos = target.localPosition;
        Quaternion startLocalRot = target.localRotation;

        float elapsed = 0f;

        foreach (var anim in so.AnimationData)
        {
            Vector3 targetLocalPos = anim.MoveToPosition; 
            Quaternion targetLocalRot = anim.RotateToRotation; 

            float moveT = Mathf.Clamp01((previewTime - elapsed) / anim.MoveDuration);
            float rotT = Mathf.Clamp01((previewTime - elapsed) / anim.RotateDuration);

            float easedMoveT = DOVirtual.EasedValue(0f, 1f, moveT, anim.MoveEase);
            float easedRotT = DOVirtual.EasedValue(0f, 1f, rotT, anim.RotateEase);

            currentPos = Vector3.Lerp(currentPos, targetLocalPos, easedMoveT);
            currentRot = Quaternion.Slerp(currentRot, targetLocalRot, easedRotT);

            elapsed += Mathf.Max(anim.MoveDuration, anim.RotateDuration);
        }


        DrawGhost(target, currentPos, currentRot);
    }

    void DrawGhost(Transform target, Vector3 localPos, Quaternion localRot)
    {
        MeshFilter[] meshes = target.GetComponentsInChildren<MeshFilter>();

        foreach (var mf in meshes)
        {
            if (mf.sharedMesh == null)
                continue;

            Transform parent = mf.transform.parent;
            Vector3 worldPos = parent != null ? parent.TransformPoint(localPos) : localPos;
            Quaternion worldRot = parent != null ? parent.rotation * localRot : localRot;

            Vector3 worldScale = mf.transform.lossyScale;

            Matrix4x4 matrix = Matrix4x4.TRS(worldPos, worldRot, worldScale);

            Material ghostMat = new Material(Shader.Find("Hidden/Internal-Colored"));
            ghostMat.SetColor("_Color", new Color(0f, 1f, 1f, 0.25f));
            ghostMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            ghostMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            ghostMat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            ghostMat.SetInt("_ZWrite", 0);
            ghostMat.SetPass(0);

            Graphics.DrawMeshNow(mf.sharedMesh, matrix);
        }
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
