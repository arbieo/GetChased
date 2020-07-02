using UnityEngine;

public class SpawnEffect : MonoBehaviour
{
    public GameObject spawnObject;

    public void OnDestroy()
    {
        spawnObject.SetActive(true);
    }
}