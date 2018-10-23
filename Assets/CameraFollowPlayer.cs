using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour {

	public GameObject target;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	public void Update () {
		if (target != null)
		{
			transform.position = new Vector3(target.transform.position.x, target.transform.position.y, - 100);
		}
	}
}
