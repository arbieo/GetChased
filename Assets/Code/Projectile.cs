using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : PropelledEntity {

	public override void FixedUpdate () 
	{
		base.FixedUpdate();
		DoPropelledStep();
	}

	protected override void CalculateTargets() {}
}
