using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TotalScoreController : MonoBehaviour {

	int displayedScore = 0;
	
	// Update is called once per frame
	void Update () {
		int score = GameController.instance.score;
		if (score > displayedScore)
		{
			displayedScore = Mathf.Min(score, displayedScore + Mathf.CeilToInt(Mathf.Max(100 * Time.deltaTime, (score-displayedScore)/2 * Time.deltaTime)));
		}

		GetComponent<Text>().text = "Score: " + displayedScore;
	}
}
