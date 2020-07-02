using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	public static GameController instance;

	public enum GameState {
		PLAYING,
		RESPAWNING,
		GAME_OVER
	}

	public int score = 0;

	float stateStartTime = 0;

	public Rect bounds;

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
	public GameObject jumpStartGravityEffect;
	public GameObject jumpEndGravityEffect;

	public GameObject gameOver;

	public CameraFollowPlayer followCamera;

	public EnemySpawner enemySpawner;

	public GraphicRaycaster raycaster;
	
	float playerRespawnTime;

	public int lives = 3;

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

		followCamera.SetTarget(player.gameObject);
		followCamera.Update();
		enemySpawner.player = player;
		gameOver.SetActive(false);

		enemySpawner.Reset();
	}

	public void StartRespawning()
	{
		currentState = GameState.RESPAWNING;
		stateStartTime = Time.time;
		playerRespawnTime = Time.time + 3;
	}

	public void StartGameOver()
	{
		gameOver.SetActive(true);
		currentState = GameState.GAME_OVER;
		stateStartTime = Time.time;
		enemySpawner.Reset();
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
			ResetTouchCenter();
			touchStartTime = Time.time;
		}

		if (Input.GetMouseButtonUp(0))
		{
			if (Time.time - touchStartTime < 0.15f)
			{
				GameObject.Destroy(GameObject.Instantiate(jumpStartEffect, player.transform.position, Quaternion.identity),1);
				GameObject.Instantiate(jumpStartGravityEffect, player.transform.position, Quaternion.identity);

				player.transform.position = (Vector2)player.transform.position + player.moveVector.normalized * 75;

				EnforcePlayerBounds();

				GameObject.Destroy(GameObject.Instantiate(jumpEndEffect, player.transform.position, Quaternion.identity), 1);
				GameObject.Instantiate(jumpEndGravityEffect, player.transform.position, Quaternion.identity);

				if (Random.Range(0f, 1f) < 0.25f)
				{
					DistortionController.instance.AddDistortion();
				}
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
		if (currentState == GameState.PLAYING)
		{
			CheckPlayingInput();
		}
		else if (currentState == GameState.GAME_OVER)
		{
			if (Input.GetMouseButtonDown(0) && Time.time - stateStartTime > 0.5)
			{
				lives = 3;
				score = 0;
				StartPlaying();
			}
		}
	}

	public void AddScore(int scoreAdded)
	{
		score += scoreAdded;
	}

	void FixedUpdate()
	{
		if (currentState == GameState.PLAYING)
		{
			EnforcePlayerBounds();
			foreach (Entity entity in Entity.entities)
			{
				EnforceEntityBounds(entity);
			}
		}
		
		if (currentState == GameState.RESPAWNING)
		{
			if (Time.time > playerRespawnTime)
			{
				StartPlaying();
			}
		}
	}

	void ResetTouchCenter()
	{
		touchCenter = (Vector2)Input.mousePosition - player.targetVector * MAX_TOUCH_DIST_IN_INCHES * Screen.dpi * player.targetSpeed / player.maxSpeed;
	}

	void EnforceEntityBounds(Entity entity)
	{
		if (entity.transform.position.x < bounds.x)
		{
			entity.transform.position = new Vector3(bounds.x + (bounds.x - entity.transform.position.x), entity.transform.position.y, entity.transform.position.z);
			entity.moveVector.x = Mathf.Abs(entity.moveVector.x);
		}
		if (entity.transform.position.x > bounds.x + bounds.width)
		{
			entity.transform.position = new Vector3(bounds.x + bounds.width + (bounds.x + bounds.width - entity.transform.position.x), entity.transform.position.y, entity.transform.position.z);
			entity.moveVector.x = -Mathf.Abs(entity.moveVector.x);
		}

		if (entity.transform.position.y < bounds.y)
		{
			entity.transform.position = new Vector3(entity.transform.position.x, bounds.y + (bounds.y - entity.transform.position.y), entity.transform.position.z);
			entity.moveVector.y = Mathf.Abs(entity.moveVector.y);
		}
		if (entity.transform.position.y > bounds.y + bounds.height)
		{
			entity.transform.position = new Vector3(entity.transform.position.x, bounds.y + bounds.height + (bounds.y + bounds.height - entity.transform.position.y), entity.transform.position.z);
			entity.moveVector.y = -Mathf.Abs(entity.moveVector.y);
		}
	}

	void EnforcePlayerBounds()
	{
		if (player.transform.position.x < bounds.x)
		{
			player.transform.position = new Vector3(bounds.x + (bounds.x - player.transform.position.x)/2, player.transform.position.y, player.transform.position.z);
			player.moveVector.x = Mathf.Abs(player.moveVector.x);
			player.targetVector.x = Mathf.Abs(player.targetVector.x);
			if (Input.GetMouseButton(0)) ResetTouchCenter();
		}
		if (player.transform.position.x > bounds.x + bounds.width)
		{
			player.transform.position = new Vector3(bounds.x + bounds.width + (bounds.x + bounds.width - player.transform.position.x)/2, player.transform.position.y, player.transform.position.z);
			player.moveVector.x = -Mathf.Abs(player.moveVector.x);
			player.targetVector.x = -Mathf.Abs(player.targetVector.x);
			if (Input.GetMouseButton(0)) ResetTouchCenter();
		}

		if (player.transform.position.y < bounds.y)
		{
			player.transform.position = new Vector3(player.transform.position.x, bounds.y + (bounds.y - player.transform.position.y)/2, player.transform.position.z);
			player.moveVector.y = Mathf.Abs(player.moveVector.y);
			player.targetVector.y = Mathf.Abs(player.targetVector.y);
			if (Input.GetMouseButton(0)) ResetTouchCenter();
		}
		if (player.transform.position.y > bounds.y + bounds.height)
		{
			player.transform.position = new Vector3(player.transform.position.x, bounds.y + bounds.height + (bounds.y + bounds.height - player.transform.position.y)/2, player.transform.position.z);
			player.moveVector.y = -Mathf.Abs(player.moveVector.y);
			player.targetVector.y = -Mathf.Abs(player.targetVector.y);
			if (Input.GetMouseButton(0)) ResetTouchCenter();
		}
	}

	public void OnPlayerKill()
	{
		lives -= 1;

		foreach (Entity entity in Entity.entities)
		{
			if (entity.team == Entity.Team.ENEMY)
			{
				entity.Kill(false);
			}
		}

		if (lives==0)
		{
			StartGameOver();
		}
		else
		{
			StartRespawning();
		}
	}
}
