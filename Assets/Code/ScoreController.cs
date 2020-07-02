using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour {

	public Text text;

	public float time;
	float startTime;
	public Color startColor;
	public Color endColor;

	// Use this for initialization
	void Start () {
		startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		float phasePercent = (Time.time - startTime)/time;

		if (phasePercent >= 1)
		{
			GameObject.Destroy(gameObject);
			return;
		}

		text.color = Color.Lerp(startColor, endColor, phasePercent);
		transform.localScale = Vector3.one * (1-(phasePercent*phasePercent*phasePercent));
	}
}
