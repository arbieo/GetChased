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
	}

	public Effect(Type type, float duration)
	{
		this.type = type;
		this.duration = duration;
		this.startTime = Time.time;
	}

	public virtual void OnAddEffect()
	{

	}

	public virtual void OnRemoveEffect()
	{

	}

	public virtual void OnEffectTick()
	{
		
	}
}
