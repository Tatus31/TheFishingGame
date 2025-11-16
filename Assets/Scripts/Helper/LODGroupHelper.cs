// using UnityEngine;
// using UnityEditor;
// using System.Collections.Generic;
//
// public class LODGroupHelper : Editor
// {
//     [MenuItem("Tools/LOD Group/Add All Renderers to Selected LOD Group")]
//     public static void AddAllRenderersToLODGroup()
//     {
//         if (Selection.activeGameObject == null)
//         {
//             Debug.LogWarning("Please select a GameObject with an LODGroup component.");
//             return;
//         }
//
//         LODGroup lodGroup = Selection.activeGameObject.GetComponent<LODGroup>();
//         if (lodGroup == null)
//         {
//             Debug.LogWarning("Selected object does not have an LODGroup component.");
//             return;
//         }
//
//         Renderer[] renderers = Selection.activeGameObject.GetComponentsInChildren<Renderer>();
//         if (renderers.Length == 0)
//         {
//             Debug.LogWarning("No renderers found under this object.");
//             return;
//         }
//
//         LOD[] lods = new LOD[1];
//         lods[0] = new LOD(0.5f, renderers);
//
//         lodGroup.SetLODs(lods);
//         lodGroup.RecalculateBounds();
//
//         Debug.Log($"Added {renderers.Length} renderers to LOD Group on '{lodGroup.name}'.");
//     }
// }
