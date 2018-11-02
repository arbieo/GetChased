using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileLauncher : MonoBehaviour {

	public float timeBetweenLaunches = 10;
	public GameObject missilePrefab;
	public float timeToArm = 2;
	public float minLaunchDistance = 50;

	private float lastLaunchTime;
	Entity target;

	// Use this for initialization
	void Start () {
		lastLaunchTime = Time.time;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(target == null)
		{
			target = gameObject.GetComponent<FollowEnemy>().target;
		}

		if(target != null && Time.time - lastLaunchTime > timeBetweenLaunches && (target.transform.position - transform.position).magnitude < minLaunchDistance)
		{
			lastLaunchTime = Time.time;
			GameObject missile = GameObject.Instantiate(missilePrefab, transform.position, Quaternion.identity);
			FollowEnemy missileEntity = missile.GetComponent<FollowEnemy>();
			missileEntity.moveVector = gameObject.GetComponent<Entity>().moveVector;
			missileEntity.speed = missileEntity.maxSpeed;
			missileEntity.target = target;

			missileEntity.AvoidCollisions(gameObject.GetComponent<Entity>(), 2);
		}
	}
}
