using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityEffect : MonoBehaviour {

	public float range;
	public float strength;
	public float time;
	float startTime;

	// Use this for initialization
	void Start () {
		startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if (startTime + time < Time.time)
		{
			GameObject.Destroy(gameObject);
		}

		float pullStrength = strength * Mathf.Sin(Mathf.PI * (Time.time - startTime)/time);

		foreach (Entity entity in Entity.entities)
		{
			Vector2 entityVector = transform.position - entity.transform.position;
			if ((entityVector).magnitude < range && entity.team == Entity.Team.ENEMY)
			{
				entity.transform.position += (Vector3)entityVector.normalized * pullStrength * Time.fixedDeltaTime;
			}
		}
	}
}
