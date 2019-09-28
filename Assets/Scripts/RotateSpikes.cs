using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSpikes : MonoBehaviour {
	private float Speed = 360f;
	public bool Clockwise;
	// Update is called once per frame
	void Update () {
		if (!Clockwise)
			Speed = -Speed;
		transform.Rotate (Vector3.forward * (Time.deltaTime * Speed));
	}
}
