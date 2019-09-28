using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigOldSpike : MonoBehaviour {
	private float moveSpeed;
	public GameObject bos;
	private bool triggered, doubleCheck;
	void Start () {
		moveSpeed = 5f;
		triggered = false; 
		doubleCheck = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (triggered) {
			if (bos.transform.position.y < -3.35f) {
				bos.transform.position += bos.transform.up * moveSpeed * Time.deltaTime;
			} else {
				triggered = false;
			}
		}
		if (!triggered && bos.transform.position.y > -5.9f) {
			bos.transform.position += -bos.transform.up * (float)(moveSpeed/2.5f) * Time.deltaTime;
		} 
		else if (!triggered && bos.transform.position.y < -6f) {
			bos.transform.position = new Vector3 (9.274f, -6.07f, 0.1f);
		}
	}

	void OnTriggerEnter2D(Collider2D col){
		if (col.tag == "Player" && !doubleCheck) {
			triggered = true;
			doubleCheck = true;
		}
	}
}
