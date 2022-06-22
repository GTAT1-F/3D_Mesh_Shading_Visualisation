using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mathf;

public enum MeshName
{
    TriaxialTeardrop,
    HyperbolicHelicoid,
    Folium
}

public interface IMesh
{
    Vector2 u { get; }
    Vector2 v { get; }
    Vector2Int Subdivisions { get; }
    Vector3 Vertex(float u, float v);
}

public class TriaxialTeardrop : IMesh
{   
    public Vector2Int Subdivisions => new Vector2Int(150, 150);
    public Vector2 u => new Vector2(0, 6 * PI);
    public Vector2 v => new Vector2(0, 2 * PI);
    private const int scale = 3;
    public Vector3 Vertex(float u, float v) => new Vector3(
        scale * (1 - Mathf.Cos(u)) * Mathf.Cos(u + 2 * Mathf.PI / 3) * Mathf.Cos(v + 2 * Mathf.PI / 3) / 2,
        scale * (1 - Mathf.Cos(u)) * Mathf.Cos(u + 2 * Mathf.PI / 3) * Mathf.Cos(v - 2 * Mathf.PI / 3) / 2,
        scale * (Mathf.Cos(u - 2 * Mathf.PI / 3))
    );
}

public class HyperbolicHelicoid : IMesh
{   
    public Vector2Int Subdivisions => new Vector2Int(150, 150);
    public Vector2 u => new Vector2(0, 6 * PI);
    public Vector2 v => new Vector2(0, 2 * PI);
    private const int scale = 3;
    public Vector3 Vertex(float u, float v) => new Vector3(
        scale * (Mathf.Sin(v) * Mathf.Cos(3 * u) / (1 + Mathf.Cos(u) * Mathf.Cos(v))),
        scale * (Mathf.Sin(v) * Mathf.Cos(3 * u) / (1 + Mathf.Cos(u) * Mathf.Cos(v))),
        scale * (Mathf.Cos(v) * Mathf.Sin(u) / (1 + Mathf.Cos(u) * Mathf.Cos(v)))
    );
}

public class Folium : IMesh
{   
    public Vector2Int Subdivisions => new Vector2Int(150, 150);
    public Vector2 u => new Vector2(0, 6 * PI);
    public Vector2 v => new Vector2(0, 2 * PI);
    private const int scale = 3;
    public Vector3 Vertex(float u, float v) => new Vector3(
        scale * (Mathf.Cos(u) * (2 * v / Mathf.PI - Mathf.Tan(v))),
        scale * (Mathf.Cos(u + 2 * Mathf.PI / 3) / Mathf.Cos(v)),
        scale * (Mathf.Cos(u - 2 * Mathf.PI / 3) / Mathf.Cos(v))
    );
}

public class MeshShapeGenerator : MonoBehaviour
{
    [SerializeField] private Mesh generatedMesh;
    [SerializeField] private MeshName meshName;
    [SerializeField, Range(1, 5)] private float generationTime;

    // Start is called before the first frame update
    void Start()
    {
        Generate();
    }

    // Update is called once per frame
    void Generate()
    {
        StartCoroutine(Generate(SelectMesh()));
    }
    private IMesh SelectMesh() => meshName switch
    {
        MeshName.TriaxialTeardrop => new TriaxialTeardrop(),
        MeshName.HyperbolicHelicoid => new HyperbolicHelicoid(),
        MeshName.Folium => new Folium(),
        _ => throw new System.ArgumentOutOfRangeException(nameof(MeshName))

    };
    private IEnumerator Generate(IMesh mesh)
    {
        generatedMesh = new Mesh();

        var subdivisions = mesh.Subdivisions;
        var vertexSize = subdivisions + new Vector2Int(1, 1);
        
        var vertices = new Vector3[vertexSize.x * vertexSize.y];
        var uvs = new Vector2[vertices.Length];
        
        var uDelta = mesh.u.y - mesh.u.x;
        var vDelta = mesh.v.y - mesh.v.x;

        for (var y = 0; y < vertexSize.y; ++y)
        {
            var v = (1f / subdivisions.y) * y;

            for (var x = 0; x < vertexSize.x; ++x)
            {
                var u = (1f / subdivisions.x) * x;
                var scaledUV = new Vector2(u * uDelta - mesh.u.x, v * vDelta - mesh.v.y);
                var vertex = mesh.Vertex(scaledUV.x, scaledUV.y);
                
                var arrayIndex = x + y * vertexSize.x;
                vertices[arrayIndex] = vertex;
                uvs[arrayIndex] = new Vector2(u, v);
            }
        }

        var triangles = new int[subdivisions.x * subdivisions.y * 6];
        for (var i = 0; i < subdivisions.x * subdivisions.y; ++i)
        {
            var triangleIndex = (i % subdivisions.x) + (i / subdivisions.x) * vertexSize.x;
            var indexer = i * 6;

            triangles[indexer + 0] = triangleIndex;
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

        yield return new WaitForSeconds(0.0f);
    }
}
