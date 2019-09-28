using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour {
	public bool Spin;
	public bool Oscillate;
	public float Speed;
	public float PathLength;
	private Vector2 op; 
	private int dir = 1;
	// Use this for initialization
	void Start () {
		Spin = true;
		Oscillate = true;
		op = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if(Spin)
			transform.Rotate(Vector2.right * Speed * Time.deltaTime);

		if (Oscillate) {
			if (dir == 0) {
				Vector2 temp = new Vector2 (op.x, transform.position.y + 0.1f * Time.deltaTime);
				transform.position = temp;
				if (transform.position.y >= op.y + PathLength)
					dir = 1; 
			} else if (dir == 1) {
				Vector2 temp = new Vector2 (op.x, transform.position.y - 0.1f * Time.deltaTime);
				transform.position = temp;
				if (transform.position.y <= op.y - PathLength)
					dir = 0; 
			}
		}

	}
}
