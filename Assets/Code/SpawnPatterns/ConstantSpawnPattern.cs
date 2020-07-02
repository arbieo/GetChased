using UnityEngine;

public class SingleSpawnPattern : SpawnPattern
{
    public GameObject prefab;

    float secondsPerSpawn = 0.5f;
    int spawnTotal = 10;

    int spawnCount = 0;

    float lastSpawnTime;

    public SingleSpawnPattern(GameObject prefab, float secondsPerSpawn, int spawnTotal)
    {
        lastSpawnTime = Time.time - secondsPerSpawn;
        this.secondsPerSpawn = secondsPerSpawn;
        this.prefab = prefab;
        this.spawnTotal = spawnTotal;
    }

    public override void Tick()
    {
        if (lastSpawnTime + secondsPerSpawn < Time.time)
        {
            SpawnEnemy(prefab, GetEmptySpawnPosition());
            lastSpawnTime = Time.time;
            spawnCount++;
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