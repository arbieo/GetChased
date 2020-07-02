using UnityEngine;

public abstract class SpawnPattern
{
    public float startTime;

    public abstract void Tick();

    protected SpawnPattern()
    {
        startTime = Time.time;
    }

    public abstract bool IsDoneSpawning();

    public virtual float GetGapTime()
    {
        return 5;
    }

    protected bool AreThereEnemies()
    {
        foreach (Entity entity in Entity.entities)
        {
            if (entity.team == Entity.Team.ENEMY)
            {
                return true;
            }
        }
        return false;
    }

    protected bool CheckSpawnPosition(Vector2 spawnPosition)
    {
        foreach(Entity entity in Entity.entities)
        {
            Vector2 differenceVector = (Vector2)entity.transform.position - spawnPosition;

            if( entity.team == Entity.Team.PLAYER)
            {
                if(differenceVector.magnitude < 100)
                {
                    return false;
                }
            }
            else
            {
                if(differenceVector.magnitude < 50)
                {
                    return false;
                }
            }
        }

        return true;
    }

    protected Vector2 GetEmptySpawnPosition()
    {
        Rect bounds = GameController.instance.bounds;
        int padding = 10;

        Vector2 spawnPosition;
        int i = 0;
        while(true)
		{
			float spawnX = Random.Range(bounds.x + padding, bounds.x+bounds.width - padding);
			float spawnY = Random.Range(bounds.y + padding, bounds.y+bounds.height - padding);

			spawnPosition = new Vector2(spawnX, spawnY);

			if (CheckSpawnPosition(spawnPosition))
            {
                break;
            }
			if (i>10)
			{
				break;
			}
			i++;
		}

        return spawnPosition;
    }

    protected void SpawnEnemy(GameObject enemyPrefab, Vector2 spawnPosition)
	{
		GameObject enemy = GameObject.Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
		if (enemy.GetComponent<FollowEnemy>() != null)
		{
			enemy.GetComponent<FollowEnemy>().target = GameController.instance.player;
			enemy.GetComponent<FollowEnemy>().moveVector = (GameController.instance.player.transform.position - enemy.transform.position).normalized;
		}
		else if (enemy.GetComponent<LaserTurret>() != null)
		{
			enemy.GetComponent<LaserTurret>().target = GameController.instance.player;
			enemy.GetComponent<LaserTurret>().moveVector = (GameController.instance.player.transform.position - enemy.transform.position).normalized;
		}
		else if (enemy.GetComponent<JumpEnemy>() != null)
		{
			enemy.GetComponent<JumpEnemy>().target = GameController.instance.player;
			enemy.GetComponent<JumpEnemy>().moveVector = (GameController.instance.player.transform.position - enemy.transform.position).normalized;
		}

        enemy.SetActive(false);

        GameObject spawnEffectPrefab = Resources.Load<GameObject>("Prefabs/SpawnEffect");
        GameObject spawnEffect = GameObject.Instantiate(spawnEffectPrefab, spawnPosition, Quaternion.identity);
        var main = spawnEffect.GetComponent<ParticleSystem>().main;
        main.startColor = enemy.GetComponent<Entity>().entityColor;
        spawnEffect.GetComponent<SpawnEffect>().spawnObject = enemy;
	}
}