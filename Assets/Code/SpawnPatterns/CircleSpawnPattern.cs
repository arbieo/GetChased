using UnityEngine;

public class CircleSpawnPattern : SpawnPattern
{
    GameObject prefab;

    bool doneSpawn = false;

    float distance = 75;

    public CircleSpawnPattern(GameObject prefab)
    {
        this.prefab = prefab;
    }

    public override void Tick()
    {
        GameObject player = GameController.instance.player.gameObject;
        Rect bounds = GameController.instance.bounds;
        if (!doneSpawn)
        {
            for (int angle = 0; angle < 360; angle += 30)
            {
                Vector2 spawnPosition = new Vector2(player.transform.position.x + Mathf.Cos(Mathf.Deg2Rad * angle) * distance, player.transform.position.y + Mathf.Sin(Mathf.Deg2Rad * angle) * distance);
                if (spawnPosition.x > bounds.x && spawnPosition.x < bounds.x + bounds.width && spawnPosition.y > bounds.y && spawnPosition.y < bounds.y + bounds.height)
                {
                    SpawnEnemy(prefab, spawnPosition);
                }

            }

            doneSpawn = true;
        }
    }

    public override bool IsDoneSpawning()
    {
        return doneSpawn;
    }
}