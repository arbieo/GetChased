using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	public static GameController instance;

	public enum GameState {
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

	float stateStartTime = 0;
	float errorEndTime;

	public GameState currentState = GameState.MENU;

	public float timeSlowMax = 10;
	public float timeSlowRecovery = 5;
	public float timeslowMultiplier = 0.05f;
	float originalDeltaTime;

	[HideInInspector]
	public Player player;
	GameObject targetObject;

	int BASIC_CAMERA_HEIGHT = 150;

	public GameObject menuUI;
	public GameObject playUI;

	public GameObject playerPrefab;
	public GameObject targetPrefab;
	public CameraFollowPlayer followCamera;
	public GameObject errorPanel;
	public Image timeBar;
	public Image timeBarBack;
	public GameObject screenDarkener;
	public Text errorText;

	public GameObject skillButtonPrefab;
	public GameObject skillButtonContainer;

	public POIController poiController;
	public EnemySpawner enemySpawner;

	public GraphicRaycaster raycaster;

	float timeSlow;
	bool disableSlow = true;

	List<Skill> skills = new List<Skill>();

	public class Skill
	{
		public SkillType type;
		public string name;
		public float strength;
		public float cooldown;
		public Skill(SkillType type, string name, float strength, float cooldown)
		{
			this.type = type;
			this.name = name;
			this.strength = strength;
			this.cooldown = cooldown;
		}
	}

	public enum SkillType
	{
		BLNK,
		TIME_REVERSE,
		INVULN,
		TURRET,
	}
	
	void Awake()
	{
		Skill blinkSkill = new Skill(SkillType.BLNK, "Blink", 100, 10);
		skills.Add(blinkSkill);
		Skill reverseSkill = new Skill(SkillType.TIME_REVERSE, "Reverse Time", 2, 5);
		skills.Add(reverseSkill);
		Skill invulSkill = new Skill(SkillType.INVULN, "Roll", 1, 5);
		skills.Add(invulSkill);
		Skill turretSkill = new Skill(SkillType.TURRET, "Shoot", 1, 10);
		skills.Add(turretSkill);

		foreach (Skill skill in skills)
		{
			GameObject skillButton = GameObject.Instantiate(skillButtonPrefab);
			skillButton.transform.parent = skillButtonContainer.transform;
			skillButton.transform.Find("Text").GetComponent<Text>().text = skill.name;
			skillButton.GetComponent<Button>().onClick.AddListener(() => UseSkill(skill));
		}

		instance = this;
		originalDeltaTime = Time.fixedDeltaTime;
		timeSlow = timeSlowMax;
	}

	public void UseSkill(Skill skill)
	{
		
	}

	public void StartZoom(Vector2 launchPoint)
	{
		if (currentState != GameState.MENU)
		{
			Debug.LogError("Invalid time to start zoom");
		}
		this.launchPoint = launchPoint;
		stateStartTime = Time.time;
		currentState = GameState.ANIMATE_IN;

		targetObject = GameObject.Instantiate(targetPrefab, launchPoint, Quaternion.identity);
		targetObject.transform.localScale = Vector3.one * worldMapScale;
		HideMenuUI();
	}

	public void DisplayError(string error)
	{
		errorText.text = error;
		errorEndTime = Time.time + 2;
		errorPanel.GetComponent<CanvasGroup>().alpha = 1;
	}

	public void StartPlaying()
	{

		player = GameObject.Instantiate(playerPrefab, launchPoint, Quaternion.identity).GetComponent<Player>();
		currentState = GameState.PLAYING;
		stateStartTime = Time.time;
		
		ShowPlayUI();
		HideMenuUI();

		followCamera.target = player.gameObject;
		followCamera.Update();
		enemySpawner.isSpawning = true;
		enemySpawner.player = player;
	}

	public void StartZoomOut()
	{
		foreach (Entity entity in Entity.entities)
		{
			entity.Kill();
		}
		poiController.DeactiveControllers();
		currentState = GameState.ANIMATE_OUT;
		stateStartTime = Time.time;
		launchPoint = Camera.main.transform.position;
		followCamera.target = null;
		HidePlayUI();
		enemySpawner.isSpawning = false;
		enemySpawner.player = null;
	}

	public void StartMenu()
	{
		currentState = GameState.MENU;
		stateStartTime = Time.time;
		HidePlayUI();
		ShowMenuUI();
		Camera.main.orthographicSize = BASIC_CAMERA_HEIGHT * worldMapScale;
		Camera.main.transform.position = (Vector3)homePoint + new Vector3(0, 0, -100);
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
		poiController.SpawnPoints(homePoint, playingMapScale*BASIC_CAMERA_HEIGHT*30);
	}

	// Update is called once per frame
	void FixedUpdate () {
		if(errorEndTime < Time.time)
		{
			errorPanel.GetComponent<CanvasGroup>().alpha = 0;
		}

		switch (currentState)
		{
			case GameState.MENU:
				if (Input.GetMouseButtonDown(0))
				{

					Vector2 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					bool closeToFriendlyBase = false;
					bool closeToEnemyBase = false;
					foreach (POIController.PointOfInterest poi in poiController.pointsOfInterest)
					{
						if(poi.type == POIController.PointType.FRIENDLY_BASE && (mousePoint-poi.location).magnitude < playingMapScale*BASIC_CAMERA_HEIGHT*10)
						{
							closeToFriendlyBase = true;
						}
						if(poi.type == POIController.PointType.ENEMY_BASE && (mousePoint-poi.location).magnitude < playingMapScale*BASIC_CAMERA_HEIGHT*5)
						{
							closeToEnemyBase = true;
						}
					}
					if(!closeToFriendlyBase)
					{
						DisplayError("Launch close to a friendly base");
					}
					else if(closeToEnemyBase)
					{
						DisplayError("Cant launch on enemy base");
					}
					else
					{
						StartZoom(mousePoint);
					}
				}
			break;
			case GameState.PLAYING:
				if (player == null)
				{
					StartZoomOut();
				}
				/*if (Input.GetMouseButton(0))
				{
					PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
					pointerEventData.position = Input.mousePosition;
					List<RaycastResult> results = new List<RaycastResult>();
					GameController.instance.raycaster.Raycast(pointerEventData, results);

					if (results.Count == 0){
						Vector3 mouseViewportPosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
						if (mouseViewportPosition.x > 0.5f)
						{
							player.targetVector = new Vector2(player.moveVector.y, -player.moveVector.x);
						}
						else
						{
							player.targetVector = new Vector2(-player.moveVector.y, player.moveVector.x);
						}
					}
				} 
				else */
				bool shouldTimeSlow = false;
				if (Input.GetKey(KeyCode.Space))
				{
					shouldTimeSlow = true;
				}

				if (Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.RightArrow))
				{
					player.targetVector = player.moveVector;
				}
				else if (Input.GetKey(KeyCode.LeftArrow))
				{
					player.targetVector = new Vector2(-player.moveVector.y, player.moveVector.x);
				}
				else if (Input.GetKey(KeyCode.RightArrow))
				{
					player.targetVector = new Vector2(player.moveVector.y, -player.moveVector.x);
				}
				else
				{
					shouldTimeSlow = true;
					player.targetVector = player.moveVector;
				}

				if (shouldTimeSlow && timeSlow > 0 && !disableSlow)
				{
					timeSlow -= originalDeltaTime;
					Time.timeScale = timeslowMultiplier;
					Time.fixedDeltaTime = originalDeltaTime * timeslowMultiplier;
					screenDarkener.SetActive(true);
					if (timeSlow <= 0)
					{
						timeSlow = 0;
						disableSlow = true;
					}
				}
				else
				{
					if (!shouldTimeSlow)
					{
						disableSlow = false;
					}
					Time.timeScale = 1;
					Time.fixedDeltaTime = originalDeltaTime;
					timeSlow = Mathf.Clamp(timeSlow + Time.fixedDeltaTime * timeSlowRecovery, 0, timeSlowMax);
					screenDarkener.SetActive(false);
				}

				timeBar.fillAmount = timeSlow/timeSlowMax;
				if(timeSlow == timeSlowMax)
				{
					timeBar.enabled = false;
					timeBarBack.enabled = false;
				}
				else 
				{
					timeBar.enabled = true;
					timeBarBack.enabled = true;
				}
			break;
			case GameState.ANIMATE_IN:
				float currentZoomPercent = (Time.time - stateStartTime)/zoomTime;
				if (currentZoomPercent>=1)
				{
					currentZoomPercent = 1;
					StartPlaying();
				}
				float zoomLevel = BASIC_CAMERA_HEIGHT * Mathf.SmoothStep(worldMapScale, playingMapScale, Mathf.Clamp((currentZoomPercent-0.5f)*2, 0, 1));
				Camera.main.orthographicSize = zoomLevel;
				//spawn player
				Camera.main.transform.position = Vector3.Lerp(homePoint, player.transform.position, Mathf.SmoothStep(0,1,currentZoomPercent*2)) + new Vector3(0, 0, -100);
				if (currentZoomPercent > 0.75f && targetObject != null)
				{
					GameObject.Destroy(targetObject);
				}
			break;
			case GameState.ANIMATE_OUT:
				float currentZoomOutPercent = (Time.time - stateStartTime)/zoomTime;
				if (currentZoomOutPercent>=1)
				{
					currentZoomPercent = 1;
					StartMenu();
				}
				float zoomOutLevel = BASIC_CAMERA_HEIGHT * Mathf.SmoothStep(playingMapScale, worldMapScale, Mathf.SmoothStep(0,1,currentZoomOutPercent*2));
				Camera.main.orthographicSize = zoomOutLevel;
				Camera.main.transform.position = Vector3.Lerp(launchPoint, homePoint, Mathf.Clamp((currentZoomOutPercent-0.5f)*2, 0, 1)) + new Vector3(0, 0, -100);
			break;
		}
	}
}
