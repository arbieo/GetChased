using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour {

	public GameObject healthPrefab;

	List<GameObject> hpBlips = new List<GameObject>();

	public int HP_PER_BLIP = 1;

	// Use this for initialization
	public void SetHP(int health)
	{
		int blips = health/HP_PER_BLIP;
		while(hpBlips.Count < blips)
		{
			GameObject newBlip = GameObject.Instantiate(healthPrefab);
			newBlip.transform.SetParent(transform);
			hpBlips.Add(newBlip);
		}

		while(hpBlips.Count > blips)
		{
			GameObject.Destroy(hpBlips[hpBlips.Count-1]);
			hpBlips.RemoveAt(hpBlips.Count-1);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (GameController.instance != null && GameController.instance.player != null)
		{
			SetHP(GameController.instance.player.health);
		}
	}
}
