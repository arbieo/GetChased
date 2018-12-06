using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class GridMesh : MonoBehaviour
{
    public Rect gridRect;
    public Vector2 gridStep;

    public float gridWidth = 2;

    public Color color;

    void Awake()
    {
        if (gridRect.width % gridStep.x != 0 || gridRect.height % gridStep.y != 0)
        {
            Debug.LogWarning("Grid size does not divide by its grid step.");
        }

        MeshFilter filter = gameObject.GetComponent<MeshFilter>();        
        var mesh = new Mesh();
        var verticies = new List<Vector3>();

        var indicies = new List<int>();
        for (int i = 0; i < gridRect.width/gridStep.x; i++)
        {
            verticies.Add(new Vector3(gridRect.x + i * gridStep.x + gridWidth/2, gridRect.y, 0));
            verticies.Add(new Vector3(gridRect.x + i * gridStep.x - gridWidth/2, gridRect.y, 0));
            verticies.Add(new Vector3(gridRect.x + i * gridStep.x - gridWidth/2, gridRect.y + gridRect.height, 0));
            verticies.Add(new Vector3(gridRect.x + i * gridStep.x + gridWidth/2, gridRect.y + gridRect.height, 0));

            indicies.Add(4 * i + 0);
            indicies.Add(4 * i + 1);
            indicies.Add(4 * i + 2);
            indicies.Add(4 * i + 3);
        }

        int indexOffset = indicies.Count;

        for (int i = 0; i < gridRect.height/gridStep.y; i++)
        {
            verticies.Add(new Vector3(gridRect.x, gridRect.y + i * gridStep.y + gridWidth/2, 0));
            verticies.Add(new Vector3(gridRect.x, gridRect.y + i * gridStep.y - gridWidth/2, 0));
            verticies.Add(new Vector3(gridRect.x + gridRect.width, gridRect.y + i * gridStep.y - gridWidth/2, 0));
            verticies.Add(new Vector3(gridRect.x + gridRect.width, gridRect.y + i * gridStep.y + gridWidth/2, 0));

            indicies.Add(4 * i + indexOffset);
            indicies.Add(4 * i + indexOffset + 1);
            indicies.Add(4 * i + indexOffset + 2);
            indicies.Add(4 * i + indexOffset + 3);
        }

        mesh.vertices = verticies.ToArray(); 
        mesh.SetIndices(indicies.ToArray(), MeshTopology.Quads, 0);
        filter.mesh = mesh;

        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Sprites/Default"));
        meshRenderer.material.color = color;
    }
}