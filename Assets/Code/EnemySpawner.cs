using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

	public static EnemySpawner instance;

	public bool isSpawning = false;

	public GameObject chaserPrefab;
	public GameObject sprinterPrefab;
	public GameObject avoiderPrefab;
	public GameObject turretPrefab;

	public Entity player;

	public bool spawned = false;

	void Awake()
	{
		instance = this;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(isSpawning && !spawned)
		{
			SpawnEnemy(sprinterPrefab);
			SpawnEnemy(sprinterPrefab);
			SpawnEnemy(sprinterPrefab);
			SpawnEnemy(sprinterPrefab);
			SpawnEnemy(sprinterPrefab);
			SpawnEnemy(turretPrefab);
			SpawnEnemy(turretPrefab);
			spawned = true;
		}
	}

	void SpawnEnemy(GameObject enemyPrefab)
	{
		Vector3 spawnPosition;
		int i = 0;
		while(true)
		{
			switch(Random.Range(0,4))
			{
				case 0:
				spawnPosition = new Vector3(-0.1f, Random.Range(0f,1f),10);
				break;
				case 1:
				spawnPosition = new Vector3(1.1f, Random.Range(0f,1f),10);
				break;
				case 2:
				spawnPosition = new Vector3(Random.Range(0f,1f), -0.1f,10);
				break;
				case 3:
				spawnPosition = new Vector3(Random.Range(0f,1f), 1.1f,10);
				break;
				default:
				spawnPosition = new Vector3(0,0,10);
				break;
			}

			bool tooClose = false;
			foreach(Entity entity in Entity.entities)
			{
				Vector2 differenceVector = entity.transform.position - Camera.main.ViewportToWorldPoint(spawnPosition);

				if(differenceVector.magnitude < 50)
				{
					tooClose = true;
					break;
				}
			}
			if(!tooClose)
			{
				break;
			}
			if (i>10)
			{
				return;
			}
			i++;
		}

		GameObject enemy = GameObject.Instantiate(enemyPrefab, (Vector2)Camera.main.ViewportToWorldPoint(spawnPosition), Quaternion.identity);
		enemy.GetComponent<FollowEnemy>().target = player;
		enemy.GetComponent<FollowEnemy>().moveVector = (player.transform.position - enemy.transform.position).normalized;
	}
}
