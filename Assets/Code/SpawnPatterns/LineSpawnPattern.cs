using UnityEngine;

public class LineSpawnPattern : SpawnPattern
{
    bool lineLeft;

    float timePerSpawn = 0.2f;

    float lastSpawnTime;

    int spawnCount = 0;
    int spawnTotal = 10;

    GameObject prefab;

    public override float GetGapTime()
    {
        return 7;
    }

    public LineSpawnPattern(GameObject prefab)
    {
        this.prefab = prefab;

        if (GameController.instance.player.transform.position.x > GameController.instance.bounds.x + GameController.instance.bounds.width/2)
        {
            lineLeft = true;
        }
        else
        {
            lineLeft = false;
        }

        lastSpawnTime = Time.time;
    }

    public override void Tick()
    {
        if (lastSpawnTime + timePerSpawn < Time.time)
        {
            Rect bounds = GameController.instance.bounds;
            float spawnSpacing = (bounds.height - 40) / spawnTotal;
            float spawnPositionX = lineLeft ? bounds.x + 20 : bounds.x + bounds.width - 20;
            float spawnPositionY = bounds.y + 20 + spawnSpacing * spawnCount;
            SpawnEnemy(prefab, new Vector2(spawnPositionX, spawnPositionY));

            spawnCount ++;
            lastSpawnTime = Time.time;
        }
    }

    public override bool IsDoneSpawning()
    {
        if (spawnCount >= spawnTotal)
        {
            return true;
        }
        return false;
    }
}