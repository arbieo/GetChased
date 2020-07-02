using UnityEngine;

public class DoubleSpawnPattern : SpawnPattern
{
    public GameObject prefab;

    float secondsPerSpawn = 2f;
    int spawnTotal = 2;

    int spawnCount = 0;

    float lastSpawnTime;

    public DoubleSpawnPattern(GameObject prefab, float secondsPerSpawn, int spawnTotal)
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