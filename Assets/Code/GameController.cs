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
	public float originalDeltaTime;

	[HideInInspector]
	public Player player;
	GameObject targetObject;
	public GameObject directionArrow;

	int BASIC_CAMERA_HEIGHT = 150;

	public GameObject menuUI;
	public GameObject playUI;

	public GameObject playerPrefab;
	public GameObject targetPrefab;
	public GameObject skillRangePrefab;
	public GameObject bulletPrefab;

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

	bool timeSlowActive = false;
	float timeSlow;
	bool disableSlow = true;

	List<Skill> skills = new List<Skill>();

	CastSkill castingSkill = null;
	GameObject castingSkillGameObject = null;
	
	void Awake()
	{
		for (int i=0; i<4; i++)
		{
			Skill skill = SkillBank.skillBank[i];
			GameObject skillButton = GameObject.Instantiate(skillButtonPrefab);
			skillButton.transform.SetParent(skillButtonContainer.transform);
			skillButton.GetComponent<SkillButton>().SetSkill(skill);
		}

		instance = this;
		originalDeltaTime = Time.fixedDeltaTime;
		timeSlow = timeSlowMax;
	}

	public void UpdateSkills()
	{
		if (castingSkill != null)
		{
			castingSkillGameObject.transform.position = player.transform.position;
		}
	}

	public void UseSkill(Skill skill)
	{
		disableSlow = false;
		skill.Use(player);

		UpdateSkills();
	}

	public void StartCasting(CastSkill skill)
	{
		if (castingSkill != null)
		{
			CancelCasting();
		}
		castingSkill = skill;
		castingSkillGameObject = GameObject.Instantiate(skillRangePrefab);
		castingSkillGameObject.transform.localScale = Vector3.one * skill.range;
	}

	public void DoCast(Vector2 position)
	{
		castingSkill.Cast(player, position);
		CancelCasting();
		UpdateSkills();
	}

	public void CancelCasting()
	{
		if (castingSkillGameObject != null)
		{
			GameObject.Destroy(castingSkillGameObject);
		}
		castingSkill = null;
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

		followCamera.SetTarget(player.gameObject);
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

	void CheckMenuInput()
	{
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
			
			StartZoom(mousePoint);
		}
	}

	Vector3 lastMousePosition;
	Vector2 touchCenter;
	float MAX_TOUCH_DIST_IN_INCHES = 0.75f;
	void CheckPlayingInput()
	{
		timeSlowActive = true;
		directionArrow.SetActive(false);

		if(Input.GetMouseButtonDown(0))
		{
			touchCenter = (Vector2)Input.mousePosition - player.targetVector * MAX_TOUCH_DIST_IN_INCHES * Screen.dpi * 0.75f;
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

			if (results.Count == 0){
				if (castingSkill != null)
				{
					DoCast(Camera.main.ScreenToWorldPoint(Input.mousePosition));
				}
				else {
					timeSlowActive = false;
					player.targetVector = mouseVector;
					directionArrow.transform.position = Camera.main.ScreenToWorldPoint(Camera.main.WorldToScreenPoint(player.transform.position) + (Vector3)mouseVector);
					float arrowAngle = Mathf.Atan2(mouseVector.y, mouseVector.x) * Mathf.Rad2Deg;
					directionArrow.transform.rotation = Quaternion.AngleAxis(arrowAngle, Vector3.forward);
					directionArrow.SetActive(true);
				}
			} else if(Input.GetMouseButtonDown(0) && castingSkill != null)
			{
				CancelCasting();
			}
		}
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
				CheckMenuInput();
			break;
			case GameState.PLAYING:
				if (player == null)
				{
					StartZoomOut();
					break;
				}

				CheckPlayingInput();
				
				if (timeSlowActive && timeSlow > 0 && !disableSlow)
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
					if (!timeSlowActive)
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

				UpdateSkills();
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
				Camera.main.transform.position = Vector3.Lerp(homePoint, launchPoint, Mathf.SmoothStep(0,1,currentZoomPercent*2)) + new Vector3(0, 0, -100);
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
				Camera.main.transform.position = Vector3.Lerp(launchPoint, homePoint, Mathf.Clamp((currentZoomOutPercent-0.75f)*4, 0, 1)) + new Vector3(0, 0, -100);
			break;
		}
	}
}
