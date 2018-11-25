using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridRenderer : MonoBehaviour {

	public float spaceBetweenLines = 5;
	public float thickness = 1;
	public Color color;

	public Rect gridRect;

	public Material material;

	 public void OnRenderObject()
	{
		GL.PushMatrix();
        material.SetPass(0);
        GL.LoadPixelMatrix();
		GL.Begin(GL.QUADS);
		GL.Color(color);
	
		//  Horizontal Lines
		for (float y = gridRect.y; y < gridRect.y + gridRect.height; y += spaceBetweenLines)
		{
			GL.Vertex3(gridRect.x, y+thickness/2, 1);
			GL.Vertex3(gridRect.x, y-thickness/2, 1);
			GL.Vertex3(gridRect.x + gridRect.height, y-thickness/2, 1);
			GL.Vertex3(gridRect.x + gridRect.height, y+thickness/2, 1);
		}
		//  Vertical Lines
		for (float x = gridRect.x; x < gridRect.x + gridRect.width; x += spaceBetweenLines)
		{
			GL.Vertex3(x+thickness/2, gridRect.y, 1);
			GL.Vertex3(x-thickness/2, gridRect.y, 1);
			GL.Vertex3(x-thickness/2, gridRect.y + gridRect.height, 1);
			GL.Vertex3(x+thickness/2, gridRect.y + gridRect.height, 1);
		}
		
		GL.End();
        GL.PopMatrix();
	}
}
