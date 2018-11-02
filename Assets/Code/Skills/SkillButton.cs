using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour {
	public Text text;
	public Image cooldownImage;
	public Button button;

	Skill skill;

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

	public void SetSkill(Skill skill)
	{
		this.skill = skill;
		text.text = skill.name;
		button.onClick.AddListener(() => GameController.instance.UseSkill(skill));
		UpdateCooldown(0);
	}

	public void FixedUpdate()
	{
		if (skill.onCooldown && Time.time - skill.lastTimeUsed > skill.cooldown)
		{
			skill.onCooldown = false;
		}

		if (skill.onCooldown)
		{
			button.enabled = false;
			UpdateCooldown((Time.time - skill.lastTimeUsed)/skill.cooldown);
		}
		else
		{
			button.enabled = true;
			UpdateCooldown(0);
		}
	}
}
