using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class n_MeshGenerator : MonoBehaviour
{
    [SerializeField] private Vector3[] vertices;
    [SerializeField] private Vector2[] uvs;
    [SerializeField] private int[] triangles;

    [SerializeField] private Mesh generatedMesh;
    [SerializeField] IMeshFunction meshFunction;
    [SerializeField] string meshName;

    private List<IMeshFunction> meshFunctions;
    [SerializeField] public static int meshFunctionsIndex;

    private void Awake()
    {
        meshFunctions = new List<IMeshFunction>();
        meshFunctions.Add(new MeshFunctions.BoySurfaceMeshFunction());
        meshFunctions.Add(new MeshFunctions.SineSurfaceMeshFunction());
        meshFunctions.Add(new MeshFunctions.MeshFunction());
        meshFunctions.Add(new MeshFunctions.TorusMeshFunction());
    }
    private void OnEnable()
    {
        meshFunction = meshFunctions[meshFunctionsIndex];
        meshName = meshFunction.Name;
        if (meshFunctionsIndex + 1 < meshFunctions.Count) meshFunctionsIndex++;

        generatedMesh = new Mesh();

        var subdivisions = meshFunction.Subdivisions;
        var vertexSize = subdivisions + new Vector2Int(1, 1);

        vertices = new Vector3[vertexSize.x * vertexSize.y];
        uvs = new Vector2[vertices.Length];

        var uDelta = meshFunction.UMinMax.y - meshFunction.UMinMax.x;
        var vDelta = meshFunction.VMinMax.y - meshFunction.VMinMax.x;


        for (var y = 0; y < vertexSize.y; y++)
        {
            var v = (1f / subdivisions.y) * y;

            for (var x = 0; x < vertexSize.x; x++)
            {
                var u = (1f / subdivisions.x) * x;

                var uv = new Vector2(u * uDelta - meshFunction.UMinMax.x, v * vDelta - meshFunction.VMinMax.y);
                var vertex = meshFunction.Vertex(uv.x, uv.y);

                var arrayIndex = x + y * vertexSize.x;

                vertices[arrayIndex] = vertex;
                uvs[arrayIndex] = uv;
            }
        }

        triangles = new int[subdivisions.x * subdivisions.y * 6];      
       
        for (var i = 0; i < subdivisions.x * subdivisions.y; i++)
        {
            var triangleIndex = (i % subdivisions.x) + (i / subdivisions.x) * vertexSize.x;
            var indexer = i * 6;

            triangles[indexer] = triangleIndex;
            triangles[indexer + 1] = triangleIndex + subdivisions.x + 1;
            triangles[indexer + 2] = triangleIndex + 1;

            triangles[indexer + 3] = triangleIndex + 1;
            triangles[indexer + 4] = triangleIndex + subdivisions.x + 1;
            triangles[indexer + 5] = triangleIndex + subdivisions.x + 2;
        }

        generatedMesh.vertices = vertices;
        generatedMesh.uv = uvs;
        generatedMesh.triangles = triangles;

        generatedMesh.RecalculateBounds();
        generatedMesh.RecalculateNormals();
        generatedMesh.RecalculateTangents();
        GetComponent<MeshFilter>().mesh = generatedMesh;

        if (meshFunction.Name == "Torus")
        {
            var meshCollider = gameObject.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = generatedMesh;
            meshCollider.convex = true;
            var rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = false;
        }

    }
}


