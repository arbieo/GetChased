using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TextColorLerp : MonoBehaviour {

	public Color color1 = Color.yellow;
	public Color color2 = Color.red;

	public float timeToLerp = 1;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		float lerpTime = Time.time % (timeToLerp * 2);
		if (lerpTime < timeToLerp)
		{
			GetComponent<Text>().color = Color.Lerp(color1, color2, lerpTime/timeToLerp);
		}
		else
		{
			GetComponent<Text>().color = Color.Lerp(color2, color1, (lerpTime-timeToLerp)/timeToLerp);
		}
	}
}
