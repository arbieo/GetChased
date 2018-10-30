using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour {
	public Text text;
	public Image cooldownImage;
	public Button button;

	public void UpdateCooldown(float percentTime)
	{
		if (percentTime <= 0)
		{
			cooldownImage.enabled = false;
		}
		else
		{
			cooldownImage.enabled = true;
			cooldownImage.fillAmount = percentTime;
		}
	}
}
