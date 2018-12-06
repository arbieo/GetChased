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
	public GameObject laserTurretPrefab;
	public GameObject jumpPrefab;

	public Rect bounds;
	public float padding;

	public Entity player;

	void Awake()
	{
		instance = this;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(isSpawning)
		{
			if (Random.Range(0, 2f/Time.fixedDeltaTime) < 1)
			{
				int enemy = Random.Range(0, 4);
				GameObject spawnPrefab = chaserPrefab;
				switch (enemy)
				{
					case 0:
						spawnPrefab = chaserPrefab;
						break;
					case 1:
						spawnPrefab = sprinterPrefab;
						break;
					case 2:
						spawnPrefab = avoiderPrefab;
						break;
					case 3:
						spawnPrefab = jumpPrefab;
						break;
				}
				SpawnEnemy(spawnPrefab);
			}
			/* if (Random.Range(0, 10f/Time.fixedDeltaTime) < 1)
			{
				SpawnEnemy(turretPrefab);
				SpawnEnemy(laserTurretPrefab);
			}*/
		}
	}

	void SpawnEnemy(GameObject enemyPrefab)
	{
		Vector3 spawnPosition;
		int i = 0;
		while(true)
		{
			float spawnX = Random.Range(bounds.x + padding, bounds.x+bounds.width - padding);
			float spawnY = Random.Range(bounds.y + padding, bounds.y+bounds.height - padding);

			spawnPosition = new Vector3(spawnX, spawnY, 0);

			bool tooClose = false;
			foreach(Entity entity in Entity.entities)
			{
				Vector2 differenceVector = entity.transform.position - spawnPosition;

				if( entity.team == Entity.Team.PLAYER)
				{
					if(differenceVector.magnitude < 100)
					{
						tooClose = true;
						break;
					}
				}
				else
				{
					if(differenceVector.magnitude < 50)
					{
						tooClose = true;
						break;
					}
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

		GameObject enemy = GameObject.Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
		if (enemy.GetComponent<FollowEnemy>() != null)
		{
			enemy.GetComponent<FollowEnemy>().target = player;
			enemy.GetComponent<FollowEnemy>().moveVector = (player.transform.position - enemy.transform.position).normalized;
		}
		else if (enemy.GetComponent<LaserTurret>() != null)
		{
			enemy.GetComponent<LaserTurret>().target = player;
			enemy.GetComponent<LaserTurret>().moveVector = (player.transform.position - enemy.transform.position).normalized;
		}
		else if (enemy.GetComponent<JumpEnemy>() != null)
		{
			enemy.GetComponent<JumpEnemy>().target = player;
			enemy.GetComponent<JumpEnemy>().moveVector = (player.transform.position - enemy.transform.position).normalized;
		}
	}
}
