using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetroFloat : MonoBehaviour {

	public int floatCount = 3;
	public float floatAmount = 200;

	public float floatTime = 1;

	Vector3 initialPosition;

	// Use this for initialization
	void Start () {
		initialPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		int totalFloatPositions = floatCount * 2 - 2;

		int floatPhase = (int)(Time.time /floatTime)%totalFloatPositions;

		int effectiveFloat;

		if (floatPhase < floatCount)
		{
			effectiveFloat = floatPhase;
		}
		else
		{
			effectiveFloat = floatCount * 2 - floatPhase - 2;
		}

		transform.position = initialPosition + new Vector3(0, floatAmount * effectiveFloat, 0);
	}
}
