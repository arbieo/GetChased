using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillBank : MonoBehaviour {

    public static List<Skill> skillBank = new List<Skill>();

    public GameObject bulletPrefab;
    public GameObject missilePrefab;

    public void Awake()
    {
        ForwardBlink forwardBlink = new ForwardBlink();
        forwardBlink.name = "F Blink";
        forwardBlink.distance = 100;
        forwardBlink.cooldown = 2;
        skillBank.Add(forwardBlink);

        MissileSkill missileSkill = new MissileSkill();
        missileSkill.name = "Missiles";
        missileSkill.missilePrefab = missilePrefab;
        missileSkill.cooldown = 10;
        skillBank.Add(missileSkill);

        BlinkSkill blinkSkill = new BlinkSkill();
        blinkSkill.name = "Blink";
        blinkSkill.range = 200;
        blinkSkill.cooldown = 10;
        skillBank.Add(blinkSkill);

        ReverseTimeSkill reverseSkill = new ReverseTimeSkill();
        reverseSkill.name = "Reverse Time";
        reverseSkill.timeToReverse = 2;
        reverseSkill.cooldown = 3;
        skillBank.Add(reverseSkill);

        InvulnSkill invulnSkill = new InvulnSkill();
        invulnSkill.name = "Roll";
        invulnSkill.invulnTime = 1;
        invulnSkill.cooldown = 5;
        skillBank.Add(invulnSkill);

        ShootStraightSkill turretSkill = new ShootStraightSkill();
        turretSkill.name = "Shoot";
        turretSkill.bulletPrefab = bulletPrefab;
        turretSkill.shotRate = 0.1f;
        turretSkill.duration = 1;
        skillBank.Add(turretSkill);


    }
}