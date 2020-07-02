using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpEnemy : Entity {

	[HideInInspector]
	public Entity target { get; set; }

	public float timeToJump = 1;
    public float jumpDistance = 100;
    public float decideTime = 0.75f;
    bool decided = false;

    public bool destroyAfterJump = false;
    public bool signalJump = true;

    public LineRenderer lineRenderer;

    float lastJumpTime;
    bool jumping = false;


    public void Start()
    {
        lastJumpTime = Time.time;
    }

	public override void FixedUpdate () 
	{
		base.FixedUpdate();

        if (!decided && !jumping)
        {
            decided = true;
            targetVector = (target.transform.position - transform.position).normalized;
            if(signalJump)
            {
                if(lineRenderer.positionCount == 0)
                {
                    lineRenderer.positionCount = 2;
                    Vector2 startPosition = transform.position;
                    Vector2 endPosition = (Vector2)transform.position + targetVector * jumpDistance;

                    if (endPosition.x < GameController.instance.bounds.x)
                    {
                        endPosition.x = GameController.instance.bounds.x;
                    }
                    if (endPosition.x > GameController.instance.bounds.xMax)
                    {
                        endPosition.x = GameController.instance.bounds.xMax;
                    }
                    if (endPosition.y < GameController.instance.bounds.y)
                    {
                        endPosition.y = GameController.instance.bounds.y;
                    }
                    if (endPosition.y > GameController.instance.bounds.yMax)
                    {
                        endPosition.y = GameController.instance.bounds.yMax;
                    }

                    lineRenderer.SetPosition(0, startPosition);
                    lineRenderer.SetPosition(1, endPosition);
                }
            }
            
        }

        if (!jumping && Time.time - lastJumpTime > decideTime)
        {
            jumping = true;
            lastJumpTime = Time.time;
        }

        if (jumping)
        {
            if(signalJump && lineRenderer.positionCount == 2)
            {
                lineRenderer.SetPosition(0, transform.position);
            }
            transform.position = (Vector2)transform.position + targetVector * jumpDistance/timeToJump * Time.fixedDeltaTime;

            if (transform.position.x < GameController.instance.bounds.x)
            {
                targetVector.x = -targetVector.x;
                lineRenderer.positionCount = 0;
            }
            if (transform.position.x > GameController.instance.bounds.xMax)
            {
                targetVector.x = -targetVector.x;
                lineRenderer.positionCount = 0;
            }
            if (transform.position.y < GameController.instance.bounds.y)
            {
                targetVector.y = -targetVector.y;
                lineRenderer.positionCount = 0;
            }
            if (transform.position.y > GameController.instance.bounds.yMax)
            {
                targetVector.y = -targetVector.y;
                lineRenderer.positionCount = 0;
            }

            if (Time.time - lastJumpTime > timeToJump)
            {
                Kill();
            }
        }
	}
}
