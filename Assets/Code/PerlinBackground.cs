using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;

public class PerlinBackground : MonoBehaviour {

	Dictionary<int,Dictionary<int, Vector2>> perlinTextures = new Dictionary<int,Dictionary<int, Vector2>>();
	Dictionary<int,Dictionary<int, GameObject>> gameObjects = new Dictionary<int,Dictionary<int, GameObject>>();

	public GameObject textureSquarePrefab;

	public Color color1;
	public Color color2;
	public Color color3;
	public Color color4;

	public int textureSize = 16;

	public int desiredTilesHorizontal = 4;
	public int baseTileSize = 100;
	float tileScale = 1;

	public float continentFreq = 10000;
	public float hillFreq = 1000;
	public float detailFreq = 200;

	public float continentWeight = 1;
	public float hillWeight = 1;
	public float detailWeight = 1;

	public float parallax = 1;

	public float depth = 900;

	public Texture2D terrainTexture;

	HashSet<Vector2> openTerrainSlots = new HashSet<Vector2>();
	HashSet<Vector2> usedTerrainSlots = new HashSet<Vector2>();

	int numSlots;

	int FULL_TEXTURE_SIZE = 256;

	void Start ()
	{
		terrainTexture = new Texture2D(FULL_TEXTURE_SIZE, FULL_TEXTURE_SIZE, TextureFormat.RGBA32, false);
		terrainTexture.filterMode = FilterMode.Point;
		terrainTexture.anisoLevel = 0;
		terrainTexture.Apply();
		numSlots = FULL_TEXTURE_SIZE/(textureSize+2);

		for(int i = 0; i< numSlots; i++)
		{
			for(int j = 0; j< numSlots; j++)
			{
				openTerrainSlots.Add(new Vector2(i,j));
			}
		}
	}

	bool HasOpenSlot()
	{
		return openTerrainSlots.Count > 0;
	}

	Vector2 GetSlot()
	{
		HashSet<Vector2>.Enumerator enumerator = openTerrainSlots.GetEnumerator();
		enumerator.MoveNext();
		Vector2 openSlot = enumerator.Current;
		openTerrainSlots.Remove(openSlot);
		usedTerrainSlots.Add(openSlot);
		return openSlot;
	}

	void FreeSlot(Vector2 slot)
	{
		openTerrainSlots.Add(slot);
		usedTerrainSlots.Remove(slot);
	}

	void Update()
	{
		Vector2 focalPosition = new Vector2(Camera.main.transform.position.x, Camera.main.transform.position.y) * parallax;

   		float screenHeightInUnits = Camera.main.orthographicSize * 2; // basically height * screen aspect ratio
		float screenWidthInUnits = screenHeightInUnits * Screen.width/ Screen.height;
		float tilesInScreenHorizontal = screenWidthInUnits/baseTileSize;
		float newTileScale = (int)Mathf.Pow(2, Mathf.FloorToInt(Mathf.Log(tilesInScreenHorizontal/desiredTilesHorizontal, 2)));
		if(tileScale != newTileScale)
		{
			//destroy everything
			foreach (KeyValuePair<int,Dictionary<int, Vector2>> xPair in perlinTextures)
			{
				int x = xPair.Key;
				foreach (KeyValuePair<int, Vector2> yPair in xPair.Value)
				{
					int y = yPair.Key;
					GameObject.Destroy(gameObjects[x][y]);
					FreeSlot(yPair.Value);
				}
			}
			
			gameObjects = new Dictionary<int,Dictionary<int, GameObject>>();
			perlinTextures = new Dictionary<int,Dictionary<int, Vector2>>();
		}
		tileScale = newTileScale;

		float tileSize = baseTileSize * tileScale;

		float screenWidthInScaledTiles = screenWidthInUnits/tileSize;
   		float screenHeightInScaledTiles = screenHeightInUnits/tileSize;

		Vector2 focalTextureCoord = focalPosition/tileSize;
		Vector2 focalTextureCoordFloor = new Vector2(Mathf.Floor(focalTextureCoord.x), Mathf.Floor(focalTextureCoord.y));
		
		int minTextureXOffset = Mathf.FloorToInt(focalTextureCoordFloor.x - screenWidthInScaledTiles/2);
		int maxTextureXOffset = Mathf.CeilToInt(focalTextureCoordFloor.x + screenWidthInScaledTiles/2);
		int minTextureYOffset = Mathf.FloorToInt(focalTextureCoordFloor.y - screenHeightInScaledTiles/2);
		int maxTextureYOffset = Mathf.CeilToInt(focalTextureCoordFloor.y + screenHeightInScaledTiles/2);

		List<Vector2> keysToRemove = new List<Vector2>();
		foreach (KeyValuePair<int,Dictionary<int, Vector2>> xPair in perlinTextures)
		{
			int x = xPair.Key;
			foreach (KeyValuePair<int, Vector2> yPair in xPair.Value)
			{
				int y = yPair.Key;
				if (x < minTextureXOffset || x > maxTextureXOffset || y > maxTextureYOffset || y < minTextureYOffset)
				{
					keysToRemove.Add(new Vector2(x,y));
				}
			}
		}
		foreach(Vector2 pointToRemove in keysToRemove)
		{
			int x = (int)pointToRemove.x;
			int y = (int)pointToRemove.y;
			GameObject.Destroy(gameObjects[x][y]);
			FreeSlot(perlinTextures[x][y]);
			perlinTextures[x].Remove(y);
			gameObjects[x].Remove(y);
		}

		bool textureChanged = false;
		for (int x = minTextureXOffset; x<= maxTextureXOffset; x++)
		{
			for (int y = minTextureYOffset; y<= maxTextureYOffset; y++)
			{
				if (!perlinTextures.ContainsKey(x))
				{
					perlinTextures.Add(x, new Dictionary<int, Vector2>());
					gameObjects.Add(x, new Dictionary<int, GameObject>());
				}
				if (!perlinTextures[x].ContainsKey(y))
				{
					Vector2 textureRef = GetSlot();
					Color32[] colors = new Color32[(textureSize+2)*(textureSize+2)];
					for(int i = 0; i< textureSize; i++)
					{
						for(int j = 0; j< textureSize; j++)
						{
							float perlinX = (x*textureSize + i) * tileScale;
							float perlinY = (y*textureSize + j) * tileScale;
							float continentComponent = Mathf.PerlinNoise(perlinX*Mathf.PI/continentFreq,perlinY*Mathf.PI/continentFreq);
							float hillComponent = Mathf.PerlinNoise(perlinX*Mathf.PI/hillFreq,perlinY*Mathf.PI/hillFreq);
							float detailComponent = Mathf.PerlinNoise(perlinX*Mathf.PI/detailFreq,perlinY*Mathf.PI/detailFreq);
							float totalWeight = continentWeight + hillWeight + detailWeight;
							float strength = (continentComponent + hillComponent + detailComponent) / totalWeight;
							int selection = (int)(strength * 4);
							Color32 color;
							if(selection == 0)
							{
								color = color1;
							}
							else if(selection == 1)
							{
								color = color2;
							}
							else if(selection == 2)
							{
								color = color3;
							}
							else if(selection == 3)
							{
								color = color4;
							}
							else
							{
								color = Color.white;
							}

							colors[(j+1)*(textureSize+2)+i+1] = color;
						}
					}
					for(int i = 0; i< textureSize+2; i++)
					{
						colors[i*(textureSize+2)] = colors[i*(textureSize+2)+1];
						colors[i*(textureSize+2) + textureSize+1] = colors[i*(textureSize+2) + textureSize];
						colors[i] = colors[i + textureSize+2];
						colors[i + (textureSize+2)*(textureSize+1)] = colors[i+(textureSize+2)*(textureSize)];
					}
					terrainTexture.SetPixels32((int)textureRef.x*(textureSize+2), (int)textureRef.y*(textureSize+2), textureSize+2, textureSize+2, colors);
					textureChanged = true;

					GameObject textureSquare = GameObject.Instantiate(textureSquarePrefab);
					textureSquare.transform.parent = transform;
					textureSquare.GetComponent<SpriteRenderer>().sprite = Sprite.Create(terrainTexture, new Rect(textureRef.x*(textureSize+2)+1, textureRef.y*(textureSize+2)+1, textureSize, textureSize), new Vector2(0f, 0f), textureSize);

					perlinTextures[x][y] = textureRef;
					gameObjects[x][y] = textureSquare;
					gameObjects[x][y].transform.localPosition = new Vector3(x*tileSize,y*tileSize);
					gameObjects[x][y].transform.localScale = Vector3.one * tileSize;
				}
			}
		}
		if (textureChanged)
		{
			terrainTexture.Apply();
		}
		Vector3 position = transform.position;
		position.z = depth;
		transform.position = position;
	}
}
