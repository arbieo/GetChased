using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Player : PropelledEntity {

	public enum CONTROL_MODE {
		TURNING,
		FOLLOW,
		JOYSTICK,
	}

	public CONTROL_MODE controlMode = CONTROL_MODE.TURNING;

	public GameObject minePrefab;

	public List<Vector2> positionHistory = new List<Vector2>();
	public List<Vector2> moveVectorHistory = new List<Vector2>();
	private const float HISTORY_TO_STORE = 5;
	private float lastHistoryTick = 0;
	
	// Update is called once per frame
	public override void FixedUpdate () 
	{
		base.FixedUpdate();
		
		/*if(HasEffect(Effect.Type.BOOST)) 
		{
			maxSpeed = originalMaxSpeed * 2;
		}*/
		DoPropelledStep();
		if (HasEffect(Effect.Type.ETHERAL))
		{
			Effect etheralEffect = effectsByType[Effect.Type.ETHERAL][0];
			float flipAngle = 360*(Time.time - etheralEffect.startTime)/etheralEffect.duration;
			display.transform.Rotate(new Vector3(flipAngle, 0,0 ));
		}
		else 
		{
			display.transform.Rotate(new Vector3(turningSpeed/maxTurnSpeed*maxBankAngle, 0,0 ));
		}
	}

	protected override void CalculateTargets()
	{
		//targetSpeed = maxSpeed;
	}

	public override void Kill()
	{
		base.Kill();
	}
}
