using UnityEngine;

public class SpiralSpawnPattern : SpawnPattern
{
    GameObject prefab;

    float distance = 75;

    int spawnCount = 0;
    int spawnTotal = 20;

    float timePerSpawn = 0.2f;

    float lastSpawnAngle = 0;

    float lastSpawnTime = 0;

    Vector2 midPosition;

    public SpiralSpawnPattern(GameObject prefab)
    {
        this.prefab = prefab;
    }

    public override void Tick()
    {
        if (spawnCount == 0)
        {
            midPosition = GameController.instance.player.transform.position;
        }

        Rect bounds = GameController.instance.bounds;
        if (lastSpawnTime + timePerSpawn <= Time.time)
        {
            float angle = lastSpawnAngle;

            Vector2 spawnPosition = new Vector2(midPosition.x + Mathf.Cos(Mathf.Deg2Rad * angle) * distance, midPosition.y + Mathf.Sin(Mathf.Deg2Rad * angle) * distance);
            if (spawnPosition.x > bounds.x && spawnPosition.x < bounds.x + bounds.width && spawnPosition.y > bounds.y && spawnPosition.y < bounds.y + bounds.height)
            {
                SpawnEnemy(prefab, spawnPosition);
            }

            lastSpawnAngle += 30;
            lastSpawnTime = Time.time;
            spawnCount ++;
        }
    }

    public override bool IsDoneSpawning()
    {
        return spawnCount >= spawnTotal;
    }
}