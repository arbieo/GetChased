using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : Entity {
	
	public float aquireDistance = 40;
	public float maxSpeed;
	public float acceleration;
	public float timeToArm = 0.5f;

	LineRenderer lineRenderer;

	public Color disabledColor;
	public Color armedColor;

	float armTime;

	// Use this for initialization
	void Awake () {
		BecomeEtheral(timeToArm);
		armTime = Time.time + timeToArm * Random.Range(0.75f, 1.25f);
		lineRenderer = GetComponent<LineRenderer>();
		display.GetComponent<SpriteRenderer>().color = disabledColor;
	}
	
	// Update is called once per frame
	public override void FixedUpdate () {
		base.FixedUpdate();
		Entity targetEntity = null;
		if (Time.time > armTime)
		{
			display.GetComponent<SpriteRenderer>().color = armedColor;
			float minDistance = aquireDistance;
			foreach (Entity entity in entities)
			{
				if(entity == this) continue;
				if(entity.GetType() != typeof(Mine))
				{
					float distance = (entity.transform.position - transform.position).magnitude;
					if (distance < minDistance)
					{
						minDistance = distance;
						targetEntity = entity;
					}
				}
			}

		}

		if (targetEntity == null)
		{
			targetVector = Vector2.zero;

			if (moveVector.magnitude < acceleration * Time.fixedDeltaTime)
			{
				moveVector = Vector2.zero;
			}
			else
			{
				moveVector = moveVector.normalized * (moveVector.magnitude - acceleration * Time.fixedDeltaTime);
			}
			lineRenderer.enabled = false;
		}
		else
		{
			targetVector = targetEntity.transform.position - transform.position;

			moveVector += targetVector.normalized * acceleration * Time.fixedDeltaTime;

			if (moveVector.magnitude > maxSpeed)
			{
				moveVector = moveVector.normalized * maxSpeed;
			}
			lineRenderer.enabled = true;
			lineRenderer.SetPosition(0, transform.position);
			lineRenderer.SetPosition(1, targetEntity.transform.position);
		}

		transform.Translate(moveVector*Time.fixedDeltaTime);
	}

	protected override void OnImpact(Entity entity)
	{
		if(entity.GetType() == typeof(Mine))
		{
			return;
		}
		Kill();
	}
}
