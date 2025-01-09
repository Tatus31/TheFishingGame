using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class WaterManager : MonoBehaviour
{
    private MeshFilter meshFilter;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
    }

    private void Update()
    {
        Vector3[] vartices = meshFilter.mesh.vertices;
        for(int i = 0; i < vartices.Length; i++)
        {
            vartices[i].y = WaveManager.instance.GetWaveHeight(transform.position.x + vartices[i].x);
        }

        meshFilter.mesh.vertices = vartices;
        meshFilter.mesh.RecalculateNormals();
    }
}
