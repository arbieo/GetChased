using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplacementController : MonoBehaviour {

	Texture2D displacementTextureX;
	Texture2D displacementTextureY;

	public Material displacementMaterial;

	Vector3 displacementTextureSize;

	public struct GravityEffect {
		public Vector2 position;
		public int size;

		public float startStrength;
		public float engageStrength;
		public float endStrength;

		public float startTime;
		public float engageTime;
		public float stayTime;
		public float disengageTime;
	}

	List<GravityEffect> gravityEffects = new List<GravityEffect>();

	byte[] pixelBufferX;
	byte[] pixelBufferY;

	// Use this for initialization
	void Start () {
		Vector3 planeSize = new Vector3(gameObject.transform.localScale.x * 10, gameObject.transform.localScale.z * 10, 1);
		displacementTextureSize = Camera.main.WorldToScreenPoint(planeSize);

		displacementTextureX = new Texture2D((int)displacementTextureSize.x, (int)displacementTextureSize.y, TextureFormat.Alpha8, false);
		displacementTextureY = new Texture2D((int)displacementTextureSize.x, (int)displacementTextureSize.y, TextureFormat.Alpha8, false);

		displacementMaterial.SetTexture("_DisplacementX", displacementTextureX);
		displacementMaterial.SetTexture("_DisplacementY", displacementTextureY);

		pixelBufferX = new byte[(int)(displacementTextureSize.x * displacementTextureSize.y)];
		pixelBufferY = new byte[(int)(displacementTextureSize.x * displacementTextureSize.y)];
	}
	
	// Update is called once per frame
	void Update () {

		if (Random.Range(0f, 100f) < 1)
		{
			Debug.Log("Add effect");
			GravityEffect gravityEffect = new GravityEffect();
			gravityEffect.position = new Vector2(displacementTextureSize.x/2,displacementTextureSize.y/2);
			gravityEffect.size = 128;
			gravityEffect.startStrength = 0;
			gravityEffect.engageStrength = 0.75f;
			gravityEffect.endStrength = 0;
			gravityEffect.startTime = Time.time;
			gravityEffect.engageTime = 0.5f;
			gravityEffect.stayTime = 0.5f;
			gravityEffect.disengageTime = 0.5f;
			gravityEffects.Add(gravityEffect);
		}

		for (int i= 0; i < pixelBufferX.Length; i++)
		{
			pixelBufferX[i] = 128;
			pixelBufferY[i] = 128;
		}

		List<GravityEffect> effectsToRemove = new List<GravityEffect>();
		foreach (GravityEffect gravityEffect in gravityEffects)
		{
			float phaseStartTime;
			float phaseEndTime;
			float phaseStartStrength;
			float phaseEndStrength;

			if (gravityEffect.startTime + gravityEffect.engageTime > Time.time)
			{
				phaseStartTime = gravityEffect.startTime;
				phaseEndTime = gravityEffect.startTime + gravityEffect.engageTime;
				phaseStartStrength = gravityEffect.startStrength;
				phaseEndStrength = gravityEffect.engageStrength;
			}
			else if(gravityEffect.startTime + gravityEffect.engageTime + gravityEffect.stayTime > Time.time)
			{
				phaseStartTime = gravityEffect.startTime + gravityEffect.engageTime;
				phaseEndTime = gravityEffect.startTime + gravityEffect.engageTime + gravityEffect.stayTime;
				phaseStartStrength = gravityEffect.engageStrength;
				phaseEndStrength = gravityEffect.engageStrength;
			}
			else if(gravityEffect.startTime + gravityEffect.engageTime + gravityEffect.stayTime + gravityEffect.disengageTime > Time.time)
			{
				phaseStartTime = gravityEffect.startTime + gravityEffect.engageTime + gravityEffect.stayTime;
				phaseEndTime = gravityEffect.startTime + gravityEffect.engageTime + gravityEffect.stayTime + gravityEffect.disengageTime;
				phaseStartStrength = gravityEffect.engageStrength;
				phaseEndStrength = gravityEffect.endStrength;
			}
			else
			{
				effectsToRemove.Add(gravityEffect);
				continue;
			}

			float phaseTime = (Time.time - phaseStartTime)/(phaseEndTime - phaseStartTime);
			float strength = phaseStartStrength * phaseTime + phaseEndStrength * (1-phaseTime);

			for (int x = -gravityEffect.size; x < gravityEffect.size; x++)
			{
				for (int y = -gravityEffect.size; y < gravityEffect.size; y++)
				{
					Vector2 pixelPosition = new Vector2(gravityEffect.position.x + x, gravityEffect.position.y + y);
					if(pixelPosition.x < 0 || pixelPosition.x > displacementTextureSize.x)
					{
						continue;
					}
					if (pixelPosition.y < 0 || pixelPosition.y > displacementTextureSize.y)
					{
						continue;
					}
					Vector2 dist = new Vector2(x, y);
					if (dist.magnitude > gravityEffect.size)
					{
						continue;
					}

					float offset = (1-dist.magnitude/gravityEffect.size) * strength;

					pixelBufferX[(int)(pixelPosition.x*displacementTextureSize.x + pixelPosition.y)] = 255;//(byte)(128 + 127 * offset * dist.normalized.x);
					pixelBufferY[(int)(pixelPosition.x*displacementTextureSize.x + pixelPosition.y)] = 255;//(byte)(128 + 127 * offset * dist.normalized.y);
				}
			}
		}

		foreach(GravityEffect gravityEffect in effectsToRemove)
		{
			gravityEffects.Remove(gravityEffect);
		}

		displacementTextureX.LoadRawTextureData(pixelBufferX);
 		displacementTextureX.Apply();

		displacementTextureY.LoadRawTextureData(pixelBufferY);
 		displacementTextureY.Apply();
	}
}
