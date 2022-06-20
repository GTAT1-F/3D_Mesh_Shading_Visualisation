using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMeshFunction
{   
    Vector2 UMinMax { get; }
    Vector2 VMinMax { get; }
    Vector2Int Subdivisions { get; }
    Vector3 Vertex(float u, float v);
    string Name { get; }
}
