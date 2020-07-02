using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleController : MonoBehaviour {

	public GameObject grid;
	public GameObject mainScreen;
	public GameObject instructions;

	public bool showingInstructions;
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Mouse0))
		{
			if (!showingInstructions)
			{
				mainScreen.SetActive(false);
				grid.SetActive(false);
				instructions.SetActive(true);
				showingInstructions = true;
			}
			else
			{
				Debug.Log("load scene");
				SceneManager.LoadScene("BaseGame");
			}
		}		
	}
}
