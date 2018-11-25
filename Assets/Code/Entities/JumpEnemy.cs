using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpEnemy : Entity {

	[HideInInspector]
	public Entity target { get; set; }

	public float timeToJump = 1;
    public float jumpDistance = 100;
    public float timeBetweenJumps = 3;
    public float decideTime = 0.75f;
    public bool decided = false;

    float lastJumpTime;
    bool jumping = false;


	public override void FixedUpdate () 
	{
		base.FixedUpdate();

        if (jumping)
        {
            transform.position = (Vector2)transform.position + targetVector * jumpDistance/timeToJump * Time.fixedDeltaTime;
            if (Time.time - lastJumpTime > timeToJump)
            {
                jumping = false;
            }
        }
        if (Time.time - lastJumpTime > timeBetweenJumps)
        {
            jumping = true;
            lastJumpTime = Time.time;
            decided = false;
        }

        if (!decided && Time.time - lastJumpTime > timeBetweenJumps * decideTime)
        {
            decided = true;
            targetVector = (target.transform.position - transform.position).normalized;
        }
	}
}
