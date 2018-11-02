using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect {

	public float duration;
	public float startTime;
	public Type type;

	public enum Type {
		INVULN,
		ETHERAL,
		EXCLUDE_COLLISONS,
		SHOOTING,
		DEATH,
	}

	public Effect(Type type, float duration)
	{
		this.type = type;
		this.duration = duration;
		this.startTime = Time.time;
	}

	public virtual void OnAddEffect(Entity entity)
	{

	}

	public virtual void OnRemoveEffect(Entity entity)
	{

	}

	public virtual void OnEffectTick(Entity entity)
	{
		
	}
}
