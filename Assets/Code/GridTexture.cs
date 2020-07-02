using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
public class GridTexture : MonoBehaviour
{
    public Rect gridRect;
    public Vector2 gridStep;

    public float gridWidth = 2;

    Texture2D gridtexture;

    void Awake()
    {
		Vector2 textureSize = Camera.main.WorldToScreenPoint(gridRect.size);
        textureSize.x = (int)textureSize.x;
        textureSize.y = (int)textureSize.y;

        Debug.Log("Texture size " + textureSize.ToString());

        if (gridRect.width % gridStep.x != 0 || gridRect.height % gridStep.y != 0)
        {
            Debug.LogWarning("Grid size does not divide by its grid step.");
        }

        Texture2D gridtexture = new Texture2D((int)textureSize.x, (int)textureSize.y, TextureFormat.RGBA32, false);

        Color32 white = new Color(1,1,1,1);
        Color32 none = new Color(1,1,1,0);

        Color32[] colors = new Color32[(int)textureSize.x * (int)textureSize.y];

        int count = 0;
        for (int x = 0; x < textureSize.x; x++)
        {
            for (int y = 0; y < textureSize.y; y++)
            {
                if (x%gridStep.x == 0 || y%gridStep.y == 0)
                {
                    colors[y * (int)textureSize.x  + x] = white;
                }
                else
                {
                    colors[y * (int)textureSize.x  + x] = none;
                }
            }
        }
        for (int x = 0; x < textureSize.x; x++)
        {
            for (int y = 0; y < textureSize.y; y++)
            {
                if (x%gridStep.x == 0 || y%gridStep.y == 0)
                {
                    if (!colors[y * (int)textureSize.x  + x].Equals(white))
                    {
                        Debug.Log("WTF");
                        Debug.Log(x);
                        Debug.Log(y);
                        return;
                    }
                }
                else
                {
                    if (!colors[y * (int)textureSize.x  + x].Equals(none))
                    {
                        Debug.Log("WTF");
                        Debug.Log(x);
                        Debug.Log(y);
                        return;
                    }
                }
            }
        }
        Debug.Log(count);
        Debug.Log(textureSize.x * textureSize.y);

        gridtexture.SetPixels32(colors);

        float pixelsPerUnit = Camera.main.WorldToScreenPoint(Camera.main.ScreenToWorldPoint(Vector3.zero) + Vector3.one).x;

        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = Sprite.Create(gridtexture, new Rect(0.0f, 0.0f, gridtexture.width, gridtexture.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
    }
}