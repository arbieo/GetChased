using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

	public GameObject chaserPrefab;
	public GameObject sprinterPrefab;
	public GameObject avoiderPrefab;
	public GameObject turretPrefab;
	public GameObject laserTurretPrefab;
	public GameObject jumpPrefab;

	public Rect bounds;
	public float padding;

	public Entity player;

	SpawnPattern currentSpawnPattern;

	LaserTurret laser1;
	LaserTurret laser2;

	float gapTime = 2;
	float gapStartTime;

	public void Reset()
	{
		currentSpawnPattern = null;
		gapStartTime = Time.time;
		gapTime = 2;

		if(laser1 != null)
		{
			laser1.target = GameController.instance.player;
			laser1.LaserOff();
		}

		if (laser2 != null)
		{
			laser2.target = GameController.instance.player;
			laser2.LaserOff();
		}
	}
	
	void SpawnLaser1()
	{
		if (laser1 == null)
		{
			Vector3 laserTurretLocation = new Vector3(GameController.instance.bounds.xMin, 0,0);
			GameObject laserTurret = GameObject.Instantiate(laserTurretPrefab, laserTurretLocation, Quaternion.identity);
			laser1 = laserTurret.GetComponent<LaserTurret>();
		}
		laser1.target = GameController.instance.player;
		laser1.LaserOff();
	}

	void SpawnLaser2()
	{
		if (laser2 == null)
		{
			Vector3 laserTurretLocation = new Vector3(GameController.instance.bounds.xMax, 0,0);
			GameObject laserTurret = GameObject.Instantiate(laserTurretPrefab, laserTurretLocation, Quaternion.identity);
			laser2 = laserTurret.GetComponent<LaserTurret>();
		}
		laser2.target = GameController.instance.player;
		laser2.LaserOff();
	}

	// Update is called once per frame
	void FixedUpdate () {

		if (GameController.instance.currentState == GameController.GameState.PLAYING)
		{
			bool noEnemies = true;
			foreach (Entity entity in Entity.entities)
			{
				if (entity.team == Entity.Team.ENEMY)
				{
					noEnemies = false;
					break;
				}
			}

			if (currentSpawnPattern == null && gapStartTime + gapTime < Time.time
				|| currentSpawnPattern == null && noEnemies)
			{
				if(GameController.instance.score < 1000)
				{
					switch (Random.Range(0, 2))
					{
						case 0:
							currentSpawnPattern = new DoubleSpawnPattern(chaserPrefab, 2f, 3);
						break;
						case 1:
							currentSpawnPattern = new SingleSpawnPattern(chaserPrefab, 1.5f, 6);
						break;
					}
				}
				else if (GameController.instance.score < 3000)
				{
					switch (Random.Range(0, 4))
					{
						case 0:
							currentSpawnPattern = new DoubleSpawnPattern(chaserPrefab, 2f, 3);
						break;
						case 1:
							currentSpawnPattern = new LineSpawnPattern(chaserPrefab);
						break;
						case 2:
							currentSpawnPattern = new CircleSpawnPattern(chaserPrefab);
						break;
					}
				}
				else if (GameController.instance.score < 7000)
				{
					switch (Random.Range(0, 5))
					{
						case 0:
							currentSpawnPattern = new DoubleSpawnPattern(chaserPrefab, 2f, 3);
						break;
						case 1:
							currentSpawnPattern = new LineSpawnPattern(chaserPrefab);
						break;
						case 2:
							currentSpawnPattern = new CircleSpawnPattern(chaserPrefab);
						break;
						case 3:
							currentSpawnPattern = new SingleSpawnPattern(jumpPrefab, 0.5f, 10);
						break;
						case 4:
							currentSpawnPattern = new LineSpawnPattern(jumpPrefab);
						break;
					}
				}
				else if (GameController.instance.score < 15000)
				{
					if (laser1 == null)
					{
						SpawnLaser1();
					}
					switch (Random.Range(0, 6))
					{
						case 0:
							currentSpawnPattern = new LineSpawnPattern(chaserPrefab);
						break;
						case 1:
							currentSpawnPattern = new CircleSpawnPattern(chaserPrefab);
						break;
						case 2:
							currentSpawnPattern = new SingleSpawnPattern(jumpPrefab, 0.5f, 10);
						break;
						case 3:
							currentSpawnPattern = new LineSpawnPattern(jumpPrefab);
						break;
						case 4:
							currentSpawnPattern = new LineSpawnPattern(avoiderPrefab);
						break;
						case 5:
							currentSpawnPattern = new CircleSpawnPattern(avoiderPrefab);
						break;
					}
				}
				else 
				{
					if (laser1 == null)
					{
						SpawnLaser1();
					}
					if (laser2 == null)
					{
						SpawnLaser2();
					}
					switch (Random.Range(0, 6))
					{
						case 0:
							currentSpawnPattern = new LineSpawnPattern(chaserPrefab);
						break;
						case 1:
							currentSpawnPattern = new CircleSpawnPattern(chaserPrefab);
						break;
						case 2:
							currentSpawnPattern = new SingleSpawnPattern(jumpPrefab, 0.5f, 10);
						break;
						case 3:
							currentSpawnPattern = new LineSpawnPattern(jumpPrefab);
						break;
						case 4:
							currentSpawnPattern = new LineSpawnPattern(avoiderPrefab);
						break;
						case 5:
							currentSpawnPattern = new CircleSpawnPattern(avoiderPrefab);
						break;
					}
				}
				
			}

			if (currentSpawnPattern != null)
			{
				if(!currentSpawnPattern.IsDoneSpawning())
				{
					currentSpawnPattern.Tick();
				}
				else
				{
					gapStartTime = Time.time;
					gapTime = currentSpawnPattern.GetGapTime();
					currentSpawnPattern = null;
				}
			}
		}
		/* if(isSpawning)
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
			if (Random.Range(0, 10f/Time.fixedDeltaTime) < 1)
			{
				SpawnEnemy(turretPrefab);
				SpawnEnemy(laserTurretPrefab);
			}
		}*/

	}
}
