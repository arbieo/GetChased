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

	public void DeployMines()
	{
		for (int i=0;i < 10; i++)
		{
			GameObject mine = GameObject.Instantiate(minePrefab, transform.position, Quaternion.identity);
			Mine mineEntity = mine.GetComponent<Mine>();
			mineEntity.moveVector = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * speed*1.5f;
		}
	}

	protected override void CalculateTargets()
	{
		targetSpeed = maxSpeed;
	}

	public override void Kill()
	{
		base.Kill();
	}
}
