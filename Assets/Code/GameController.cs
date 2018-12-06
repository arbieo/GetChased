using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	public static GameController instance;

	public enum GameState {
		PLAYING,
		SHOP,
		MENU
	}

	float stateStartTime = 0;

	public GameState currentState = GameState.PLAYING;

	[HideInInspector]
	public Player player;
	GameObject targetObject;
	public GameObject directionArrow;

	int BASIC_CAMERA_HEIGHT = 150;

	float blinkCount = 1;

	public GameObject trail1;
	public GameObject trail2;
	public GameObject trail3;

	float timePerBlink = 3;

	public GameObject menuUI;
	public GameObject playUI;

	public GameObject playerPrefab;
	public GameObject bulletPrefab;

	public GameObject jumpStartEffect;
	public GameObject jumpEndEffect;

	public CameraFollowPlayer followCamera;

	public EnemySpawner enemySpawner;

	public GraphicRaycaster raycaster;
	
	void Awake()
	{
		instance = this;
	}

	public void StartPlaying()
	{

		player = GameObject.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity).GetComponent<Player>();
		currentState = GameState.PLAYING;
		stateStartTime = Time.time;
		
		ShowPlayUI();
		HideMenuUI();

		followCamera.SetTarget(player.gameObject);
		followCamera.Update();
		enemySpawner.isSpawning = true;
		enemySpawner.player = player;
	}

	public void StartMenu()
	{
		currentState = GameState.MENU;
		stateStartTime = Time.time;
		HidePlayUI();
		ShowMenuUI();
	}

	void ShowMenuUI()
	{
		menuUI.SetActive(true);
	}

	void HideMenuUI()
	{
		menuUI.SetActive(false);
	}

	void ShowPlayUI()
	{
		playUI.SetActive(true);
	}

	void HidePlayUI()
	{
		playUI.SetActive(false);
	}

	void Start() 
	{
		StartPlaying();
	}

	Vector3 lastMousePosition;
	Vector2 touchCenter;
	float MAX_TOUCH_DIST_IN_INCHES = 0.5f;
	float touchStartTime = 0;
	void CheckPlayingInput()
	{
		directionArrow.SetActive(false);

		if(Input.GetMouseButtonDown(0))
		{
			touchCenter = (Vector2)Input.mousePosition - player.targetVector * MAX_TOUCH_DIST_IN_INCHES * Screen.dpi * player.targetSpeed / player.maxSpeed;
			touchStartTime = Time.time;
		}

		if (Input.GetMouseButtonUp(0))
		{
			if (Time.time - touchStartTime < 0.15f)
			{
				GameObject.Destroy(GameObject.Instantiate(jumpStartEffect, player.transform.position, Quaternion.identity),1);
				player.transform.position = (Vector2)player.transform.position + player.moveVector.normalized * 75;
				GameObject.Destroy(GameObject.Instantiate(jumpEndEffect, player.transform.position, Quaternion.identity), 1);
			}
		}

		if (Input.GetMouseButton(0))
		{
			PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
			pointerEventData.position = Input.mousePosition;
			List<RaycastResult> results = new List<RaycastResult>();
			GameController.instance.raycaster.Raycast(pointerEventData, results);
			
			Vector2 mouseVector = (Vector2)Input.mousePosition - touchCenter;
			if (mouseVector.magnitude > MAX_TOUCH_DIST_IN_INCHES * Screen.dpi)
			{
				touchCenter = (Vector2)Input.mousePosition - mouseVector.normalized * MAX_TOUCH_DIST_IN_INCHES*Screen.dpi;
				mouseVector = (Vector2)Input.mousePosition - touchCenter;
			}

			player.targetVector = mouseVector;
			directionArrow.transform.position = Camera.main.ScreenToWorldPoint(Camera.main.WorldToScreenPoint(player.transform.position) + (Vector3)mouseVector);
			float arrowAngle = Mathf.Atan2(mouseVector.y, mouseVector.x) * Mathf.Rad2Deg;
			directionArrow.transform.rotation = Quaternion.AngleAxis(arrowAngle, Vector3.forward);
			directionArrow.SetActive(true);

			player.targetSpeed = player.maxSpeed * mouseVector.magnitude / (MAX_TOUCH_DIST_IN_INCHES * Screen.dpi);
		}
	}

	void Update()
	{
		CheckPlayingInput();
	}
}
