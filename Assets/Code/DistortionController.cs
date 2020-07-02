using JetFistGames.RetroTVFX;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class DistortionController : MonoBehaviour
{
    public CRTEffect crtEffect;

    public static DistortionController instance;

    public float decayTime = 1;

    float ROffset = 0;
    float GOffset = 0;
    float BOffset = 0;
    public float colorOffsetStrength = -8;

    float brightnessOffset = 0;
    public float brightnessOffsetStrength = 1f;

    public float noiseBaseline = 0.15f;
    float noiseOffset = 0f;
    public float noiseOffsetStrength = 0.5f;

    float chromaOffsetX = 0;
    float chromaOffsetY = 0;
    public float chromaOffsetStrength = 1f;
    float lumaOffset = 0;
    public float lumaOffsetStrength = 4f;

    public void Start()
    {
        instance = this;
    }

    public void Update()
    {

        ReduceAndApply();
    }

    public void AddDistortion()
    {
        switch(Random.Range(0, 3))
        {
            case 0:
                brightnessOffset += brightnessOffsetStrength;
            break;
            case 1:
                noiseOffset += noiseOffsetStrength;
            break;
            case 2:
                lumaOffset += lumaOffsetStrength;
            break;
        }
    }

    public void ReduceAndApply()
    {
        ROffset = Mathf.Clamp(ROffset -= Time.deltaTime * colorOffsetStrength/decayTime, 2, 8);
        GOffset = Mathf.Clamp(GOffset -= Time.deltaTime * colorOffsetStrength/decayTime, 2, 8);
        BOffset = Mathf.Clamp(BOffset -= Time.deltaTime * colorOffsetStrength/decayTime, 2, 8);
        chromaOffsetX = Mathf.Clamp(chromaOffsetX -= Time.deltaTime * chromaOffsetStrength/decayTime, 0, 0.5f);
        chromaOffsetY = Mathf.Clamp(chromaOffsetY -= Time.deltaTime * chromaOffsetStrength/decayTime, 0, 0.5f);

        brightnessOffset = Mathf.Clamp(brightnessOffset -= Time.deltaTime * brightnessOffsetStrength/decayTime, 1, 2);
        noiseOffset = Mathf.Clamp(noiseOffset -= Time.deltaTime * noiseOffsetStrength/decayTime, noiseBaseline, 2f);
        lumaOffset = Mathf.Clamp(lumaOffset -= Time.deltaTime * lumaOffsetStrength/decayTime, 0, 2);

        crtEffect.RBits = (int)ROffset;
        crtEffect.GBits = (int)GOffset;
        crtEffect.BBits = (int)BOffset;
        crtEffect.IQOffset = new Vector2(chromaOffsetX, chromaOffsetY);
        crtEffect.PixelMaskBrightness = brightnessOffset;
        crtEffect.RFNoise = noiseOffset;
        crtEffect.LumaSharpen = lumaOffset;
    }
}