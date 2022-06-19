using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;
    Vector3[] verticies;
    Vector3[] uv;
    int[] triangles;
    public int xSize = 20;
    public int zSize = 20;
    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();
        UpdateMesh();
    }
    void CreateShape()
    {
        // verticies = new Vector3[]
        // {
        //     new Vector3 (0,0,0),
        //     new Vector3 (0,0,1),
        //     new Vector3 (1,0,0),
        //     new Vector3 (1,0,1)
        // };

        // uv = new Vector3[] 
        // {
        // new Vector3(0, 0),
        // new Vector3(0, 1),
        // new Vector3(1, 1)
        // };

        // triangles = new int[]
        // {
        //     0,1,2,
        //     1,3,2
        // };

        verticies = new Vector3[(xSize + 1) * (zSize + 1)];
        int i = 0;

        for (int z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = Mathf.PerlinNoise(x * .3f, z * .3f) * 2f;
                verticies[i] = new Vector3(x, y, z);
                i++;
            }
        }
        // fill the grid with triangles
        triangles = new int[xSize * zSize * 6];
        int vert = 0;
        int tris = 0;

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;
                vert++;
                tris += 6;
            }
            vert++;
        }
    }
    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = verticies;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
    // display verticies in the editor
    private void OnDrawGizmos()
    {
        if (verticies == null)
        {
            return;
        }
        for (int i = 0; i < verticies.Length; i++)
        {
            Gizmos.DrawSphere(verticies[i], .1f);
        }
    }
}
