using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour {
	private float speed, diff;
	private Vector3 dir;
	private bool movingLeft, effectPlayer;
	Transform player;
	void Start () {
		dir = transform.right;
		speed = 0.75f;
		movingLeft = true;
		effectPlayer = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (movingLeft) {
			transform.position -= dir * speed * Time.deltaTime;
			if ( transform.position.x < 8.1f && movingLeft)
				movingLeft = false;
			if (effectPlayer) {
				diff = Mathf.Abs(player.transform.position.x - transform.position.x);
				player.transform.position = new Vector3 (transform.position.x - diff, player.transform.position.y);
			}
		} else {
			transform.position += dir * speed * Time.deltaTime;
			if (transform.position.x > 10f && !movingLeft)
				movingLeft = true;
			if (effectPlayer) {
				diff = Mathf.Abs(player.transform.position.x - transform.position.x);
				player.transform.position = new Vector3 (transform.position.x + diff, player.transform.position.y);
			}
		}
	}

	void OnCollisionEnter2D(Collision2D col){
		if (col.transform.tag == "Player") {
			player = col.transform;
			effectPlayer = true;
		}
	}

	void OnCollisionExit2D(Collision2D col){
		if (col.transform.tag == "Player") {
			effectPlayer = false;
		}
	}
}
