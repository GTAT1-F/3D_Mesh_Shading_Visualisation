using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mathf;

public interface MeshFunction
{
    Vector2 uMinMax { get; }
    Vector2 vMinMax { get; }
    Vector2Int Subdivisions { get; }
    Vector3 Vertex(float u, float v);
}

public class Snail : MeshFunction
{
    public Vector2 uMinMax => new Vector2(0, 6 * PI);
    public Vector2 vMinMax => new Vector2(0, 2 * PI);
    public Vector2Int Subdivisions => new Vector2Int(150, 150);
    public Vector3 Vertex(float u, float v) => new Vector3(
        2 * (1 - Exp(u / (6 * PI)) * Cos(u) * Pow(Cos(v / 2), 2)),
        2 * (-1 + Exp(u / (6 * PI)) * Sin(u) * Pow(Cos(v / 2), 2)),
        1 - Exp(u / (3 * PI)) - Sin(v) + Exp(u / (6 * PI)) * Sin(v)
    );
}

public class Sandclock : MeshFunction
{
    public Vector2 uMinMax => new Vector2(-1, 1);
    public Vector2 vMinMax => new Vector2(0, 2);
    public Vector2Int Subdivisions => new Vector2Int(150, 50);
    public Vector3 Vertex(float u, float v) => new Vector3(
        Mathf.Cos(0.5f * Mathf.PI * v) * Mathf.Sin(Mathf.PI * u),
        v,
        Mathf.Cos(0.5f * Mathf.PI * v) * Mathf.Cos(Mathf.PI * u)
    );
}
public class MoebiusStrip : MeshFunction
{
    public Vector2 uMinMax => new Vector2(0, 2 * PI);
    public Vector2 vMinMax => new Vector2(0, 2 * PI);
    public Vector2Int Subdivisions => new Vector2Int(50, 50);
    public Vector3 Vertex(float u, float v) => new Vector3(
        (3 + Cos(u/2) * Sin(v) - Sin(u/2) * Sin(2 * v)) * Cos(u),
        (3 + Cos(u/2) * Sin(v) - Sin(u/2) * Sin(2 * v)) * Sin(u),
        Sin(u/2) * Sin(v) + Cos(u/2) * Sin(2 * v)
    );
}

public enum MeshType
{
    Snail,
    Sandclock,
    MoebiusStrip
}

public class ShapeMeshGenerator : MonoBehaviour
{
    [SerializeField] private Mesh generatedMesh;
    [SerializeField] private MeshType meshType;
    [SerializeField, Range(1, 5)] private float generationTime;

    // Start is called before the first frame update
    void Start()
    {
        Generate();
    }

    // Update is called once per frame
    void Generate()
    {
        StartCoroutine(Generate(SelectMeshFunction()));
    }
    private MeshFunction SelectMeshFunction() => meshType switch
    {
        MeshType.Snail => new Snail(),
        MeshType.Sandclock => new Sandclock(),
        MeshType.MoebiusStrip => new MoebiusStrip(),
        _ => throw new System.ArgumentOutOfRangeException(nameof(MeshType))

    };
    private IEnumerator Generate(MeshFunction meshFunction)
    {
        generatedMesh = new Mesh();

        var subdivisions = meshFunction.Subdivisions;
        var vertexSize = subdivisions + new Vector2Int(1, 1);
        
        var vertices = new Vector3[vertexSize.x * vertexSize.y];
        var uvs = new Vector2[vertices.Length];
        
        var uDelta = meshFunction.uMinMax.y - meshFunction.uMinMax.x;
        var vDelta = meshFunction.vMinMax.y - meshFunction.vMinMax.x;

        for (var y = 0; y < vertexSize.y; y++)
        {
            var v = (1f / subdivisions.y) * y;

            for (var x = 0; x < vertexSize.x; x++)
            {
                var u = (1f / subdivisions.x) * x;
                var scaledUV = new Vector2(u * uDelta - meshFunction.uMinMax.x, v * vDelta - meshFunction.vMinMax.y);
                var vertex = meshFunction.Vertex(scaledUV.x, scaledUV.y);
                
                var arrayIndex = x + y * vertexSize.x;
                vertices[arrayIndex] = vertex;
                uvs[arrayIndex] = new Vector2(u, v);
            }
        }

        var triangles = new int[subdivisions.x * subdivisions.y * 6];
        for (var i = 0; i < subdivisions.x * subdivisions.y; i += 1)
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
