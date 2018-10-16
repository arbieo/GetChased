using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	enum GameState {
		PLAYING,
		MENU,
		ANIMATE_IN,
		ANIMATE_OUT
	}

	public float worldMapScale;
	public float playingMapScale;

	public Vector2 homePoint;
	public Vector2 launchPoint;

	public float zoomTime = 5;

	public float stateStartTime = 0;

	GameState currentState = GameState.MENU;

	public PerlinBackground terrain;
	Player player;

	public GameObject playerPrefab;
	
	void StartZoom(Vector2 launchPoint)
	{
		launchPoint *= worldMapScale;
		if (currentState != GameState.MENU)
		{
			Debug.LogError("Invalid time to start zoom");
		}
		this.launchPoint = launchPoint;
		stateStartTime = Time.time;
		currentState = GameState.ANIMATE_IN;
		player = GameObject.Instantiate(playerPrefab, launchPoint, Quaternion.identity).GetComponent<Player>();
		player.transform.localScale = Vector3.one / worldMapScale;
	}

	void Start() 
	{
		StartZoom(new Vector2(100,-50));
	}

	void StartPlaying()
	{

	}

	// Update is called once per frame
	void FixedUpdate () {
		switch (currentState)
		{
			case GameState.MENU:
			break;
			case GameState.PLAYING:
			break;
			case GameState.ANIMATE_IN:
				float currentZoomPercent = (Time.time - stateStartTime)/zoomTime;
				if (currentZoomPercent>=1)
				{
					currentZoomPercent = 1;
					StartPlaying();
				}
				terrain.scale = Mathf.Lerp(worldMapScale, playingMapScale, Mathf.Pow(currentZoomPercent, 1f/2f));
				//spawn player
				Camera.main.transform.position = Vector3.Lerp(homePoint, player.transform.position, Mathf.Pow(currentZoomPercent*1.1f, 1f/5f)) + new Vector3(0, 0, -10);
				player.transform.localScale = Vector3.one / terrain.scale;
			break;
			case GameState.ANIMATE_OUT:
			break;
		}
	}
}
