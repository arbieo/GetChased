using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour {

	GameObject target;

	public Rect bounds;
	public float padding;

	// Use this for initialization
	public void SetTarget (GameObject target) {
		this.target = target;
		transform.position = target.transform.position + Vector3.back * 100;
	}
	
	// Update is called once per frame
	public void Update () {
		if (target != null)
		{
			float cameraHeight = Camera.main.orthographicSize;
			float cameraWidth = cameraHeight * Screen.width / Screen.height;
			Vector3 newPosition = Vector3.MoveTowards(transform.position, target.transform.position + Vector3.back * 100, 250*Time.fixedDeltaTime);
			newPosition.x = Mathf.Clamp(newPosition.x, bounds.x + cameraWidth - padding, bounds.x+bounds.width - cameraWidth + padding);
			newPosition.y = Mathf.Clamp(newPosition.y, bounds.y + cameraHeight - padding, bounds.y+bounds.height - cameraHeight + padding);
			transform.position = newPosition;
		}
	}
}
