using UnityEngine;
using static UnityEngine.Mathf;

namespace MeshFunctions
{
    public class SphereFunction : IMeshFunction
    {
        public Vector2 UMinMax => new Vector2(0, 2 * Mathf.PI);

        public Vector2 VMinMax => new Vector2(0, Mathf.PI);

        public Vector2Int Subdivisions => new Vector2Int(32, 32);

        public string Name => "Sphere";

        public Vector3 Vertex(float u, float v)
        {
            Vector3 result = new Vector3();
            result.x = 1 * Mathf.Cos(u) * Mathf.Sin(v);
            result.y = 1 * Mathf.Sin(u) * Mathf.Sin(v);
            result.z = 1 * Mathf.Cos(v);
            return result;
        }
    }

    public class TubeFunction : IMeshFunction
    {
        public Vector2 UMinMax => new Vector2(-Mathf.PI, Mathf.PI);

        public Vector2 VMinMax => new Vector2(-Mathf.PI, Mathf.PI);

        public Vector2Int Subdivisions => new Vector2Int(16, 16);

        public string Name => "Tube";

        public Vector3 Vertex(float u, float v)
        {
            return new Vector3(u, Cos(v), Sin(v));
        }
    }

    // https://www.youtube.com/watch?v=345SnWfahhY
    public class TorusMeshFunction : IMeshFunction
    {
        public Vector2 UMinMax => new Vector2(0, 2 * PI);

        public Vector2 VMinMax => new Vector2(0, 2 * PI);

        public Vector2Int Subdivisions => new Vector2Int(16, 16);

        public string Name => "Torus";

        public Vector3 Vertex(float u, float v)
        {
            float r = 2;
            Vector3 result = new Vector3();
            result.x = r * Cos(u) + Cos(u) * Cos(v);
            result.y = r * Sin(u) + Sin(u) * Cos(v);
            result.z = Sin(v);
            return result;
        }
    }

    // https://mathworld.wolfram.com/SineSurface.html
    public class SineSurfaceMeshFunction : IMeshFunction
    {
        public Vector2 UMinMax => new Vector2(0, 2 * PI);

        public Vector2 VMinMax => new Vector2(0, 2 * PI);

        public Vector2Int Subdivisions => new Vector2Int(16 , 16);

        public string Name => "Sine Surface";

        public Vector3 Vertex(float u, float v)
        {
            Vector3 result = new Vector3();
            float a = 2f;

            result.x = a * Sin(u);
            result.y = a * Sin(v);
            result.z = a * Sin(u + v);
            return result;
        }
    }

    // https://www.geogebra.org/m/BjV7cNwb
    public class MeshFunction : IMeshFunction
    {
        public Vector2 UMinMax => new Vector2(0,2*PI);

        public Vector2 VMinMax => new Vector2(0,2*PI);

        public Vector2Int Subdivisions => new Vector2Int(32, 32);

        public string Name => "Mesh";

        public Vector3 Vertex(float u, float v)
        {
            Vector3 result = new Vector3();
            result.x = v * Cos(u);
            result.y = Sin(20 * v) * Cos(20 * v);
            result.z = v * Sin(u);
            return result;
        }
    }

/*    // Plane on the x/z plane
    public class PlaneMeshFunction : IMeshFunction
    {
        public Vector2 UMinMax => new Vector2(0, 1);

        public Vector2 VMinMax => new Vector2(0, 1);

        public Vector2Int Subdivisions => new Vector2Int(16, 16);

        public string Name => "Plane";

        public Vector3 Vertex(float u, float v)
        {
            Vector3 result = new Vector3();
            result.x = u;
            result.y = 0;
            result.z = v;
            return result;
        }
    }*/

}
