using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour {

	GameObject target;

	// Use this for initialization
	public void SetTarget (GameObject target) {
		this.target = target;
		transform.position = target.transform.position + Vector3.back * 100;
	}
	
	// Update is called once per frame
	public void Update () {
		if (target != null)
		{
			transform.position = Vector3.MoveTowards(transform.position, target.transform.position + Vector3.back * 100, 1000*Time.fixedDeltaTime);
		}
	}
}
