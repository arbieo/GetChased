using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POIController : MonoBehaviour {

	public static POIController instance;

	public enum PointType
	{
		ENEMY_BASE,
		FRIENDLY_BASE,
		MISSION
	}

	public class PointOfInterest
	{
		public PointType type;
		public Vector2 location;
		public Dictionary<Entity, GameObject> pointEntites = new Dictionary<Entity, GameObject>();
		public bool activated = false;
		public float activationRange;
	}

	public List<PointOfInterest> pointsOfInterest = new List<PointOfInterest>();
	Dictionary<PointOfInterest, GameObject> poiSymbols = new Dictionary<PointOfInterest, GameObject>();

	public GameObject POIPrefab;
	public GameObject turretPrefab;

	public Sprite baseSprite;
	public Sprite missionSprite;
	public Sprite friendlyBaseSprite;

	public float maxScreenDistance = 10;

	void Awake()
	{
		instance = this;
	}

	// Use this for initialization
	public void SpawnPoints (Vector2 center, float scatter) {
		for (int i =0; i<5; i++)
		{
			PointOfInterest newPoint = new PointOfInterest();
			newPoint.type = PointType.ENEMY_BASE;
			newPoint.activationRange = 500;
			newPoint.location = new Vector2(center.x + Random.Range(-scatter, scatter), center.y + Random.Range(-scatter, scatter));
			pointsOfInterest.Add(newPoint);
			poiSymbols[newPoint] = GameObject.Instantiate(POIPrefab);
			poiSymbols[newPoint].GetComponent<SpriteRenderer>().sprite = baseSprite;
		}

		/*for (int i =0; i<5; i++)
		{
			PointOfInterest newPoint = new PointOfInterest();
			newPoint.type = PointType.MISSION;
			newPoint.activationRange = 100;
			newPoint.location = new Vector2(center.x + Random.Range(-scatter, scatter), center.y + Random.Range(-scatter, scatter));
			pointsOfInterest.Add(newPoint);
			poiSymbols[newPoint] = GameObject.Instantiate(POIPrefab);
			poiSymbols[newPoint].GetComponent<SpriteRenderer>().sprite = missionSprite;
		}*/

		PointOfInterest friendlyBase = new PointOfInterest();
		friendlyBase.type = PointType.FRIENDLY_BASE;
		friendlyBase.activationRange = 500;
		friendlyBase.location = new Vector2(center.x , center.y);
		pointsOfInterest.Add(friendlyBase);
		poiSymbols[friendlyBase] = GameObject.Instantiate(POIPrefab);
		poiSymbols[friendlyBase].GetComponent<SpriteRenderer>().sprite = friendlyBaseSprite;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		float halfScreenHeightInUnits = Camera.main.orthographicSize; // basically height * screen aspect ratio
		float halfScreenWidthInUnits = halfScreenHeightInUnits * Screen.width/ Screen.height;
		float minX = Camera.main.transform.position.x - halfScreenWidthInUnits;
		float maxX = Camera.main.transform.position.x + halfScreenWidthInUnits;
		float minY = Camera.main.transform.position.y - halfScreenHeightInUnits;
		float maxY = Camera.main.transform.position.y + halfScreenHeightInUnits;
		foreach (KeyValuePair<PointOfInterest, GameObject> kvp in poiSymbols)
		{
			PointOfInterest poi = kvp.Key;
			GameObject symbol = kvp.Value;
			symbol.transform.localScale = Vector3.one * Camera.main.orthographicSize/10;

			if ((poi.location - (Vector2)Camera.main.transform.position).magnitude > maxScreenDistance * halfScreenHeightInUnits)
			{
				symbol.SetActive(false);
			}
			else
			{
				Vector2 symbolLocation = poi.location;
				symbol.SetActive(true);
				symbolLocation.x = Mathf.Clamp(symbolLocation.x, minX, maxX);
				symbolLocation.y = Mathf.Clamp(symbolLocation.y, minY, maxY);
				symbol.transform.position = symbolLocation;
			}

			foreach (KeyValuePair<Entity, GameObject> kvpEntity in poi.pointEntites)
			{
				Entity entity = kvpEntity.Key;
				if (entity == null)
				{
					continue;
				}
				GameObject entitySymbol = kvpEntity.Value;
				entitySymbol.transform.localScale = Vector3.one * Camera.main.orthographicSize/10;

				if (((Vector2)entity.transform.position - (Vector2)Camera.main.transform.position).magnitude > maxScreenDistance * halfScreenHeightInUnits)
				{
					entitySymbol.SetActive(false);
				}
				else
				{
					Vector2 symbolLocation = entity.transform.position;
					entitySymbol.SetActive(true);
					symbolLocation.x = Mathf.Clamp(symbolLocation.x, minX, maxX);
					symbolLocation.y = Mathf.Clamp(symbolLocation.y, minY, maxY);
					entitySymbol.transform.position = symbolLocation;
				}
			}
		}

		if (GameController.instance.currentState == GameController.GameState.PLAYING)
		{
			foreach (PointOfInterest poi in pointsOfInterest)
			{
				if ((poi.location - (Vector2)Camera.main.transform.position).magnitude < poi.activationRange && poi.activated == false)
				{
					if (poi.type == PointType.ENEMY_BASE)
					{
						for (int i = 0; i< 2; i++)
						{
							Vector2 relativeSpawnPosition = new Vector2(Random.Range(-300,300),Random.Range(-300,300));
							GameObject enemy = GameObject.Instantiate(turretPrefab, relativeSpawnPosition + poi.location, Quaternion.identity);
							enemy.GetComponent<FollowEnemy>().target = GameController.instance.player;
							poi.pointEntites[enemy.GetComponent<FollowEnemy>()] = GameObject.Instantiate(POIPrefab);
							poi.pointEntites[enemy.GetComponent<FollowEnemy>()].GetComponent<SpriteRenderer>().sprite = missionSprite;
							poi.activated = true;
						}
					}
				}

				List<Entity> entitiesToRemove = new List<Entity>();
				foreach (Entity entity in poi.pointEntites.Keys)
				{
					if (entity == null)
					{
						entitiesToRemove.Add(entity);
					}
				}
				foreach (Entity entity in entitiesToRemove)
				{
					GameObject.Destroy(poi.pointEntites[entity].gameObject);
					poi.pointEntites.Remove(entity);
				}

				if(poi.activated && poi.type == PointType.ENEMY_BASE && poi.pointEntites.Count == 0)
				{
					poi.type = PointType.FRIENDLY_BASE;
					poiSymbols[poi].GetComponent<SpriteRenderer>().sprite = friendlyBaseSprite;
				}
			}
		}
	}

	public void DeactiveControllers()
	{
		foreach (PointOfInterest poi in pointsOfInterest)
		{
			poi.activated = false;
			foreach (Entity entity in poi.pointEntites.Keys)
			{
				GameObject.Destroy(poi.pointEntites[entity].gameObject);
			}
			poi.pointEntites = new Dictionary<Entity, GameObject>();
		}
	}
}
