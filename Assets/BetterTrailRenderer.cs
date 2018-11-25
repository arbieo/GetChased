using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetterTrailRenderer : MonoBehaviour {

	List<Vector3> points = new List<Vector3>();

	public int length;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		points.Add(transform.position);
		if(points.Count > length)
		{
			points.RemoveAt(0);
		}

		GetComponent<LineRenderer>().positionCount = points.Count;
		GetComponent<LineRenderer>().SetPositions(points.ToArray());
	}
}
