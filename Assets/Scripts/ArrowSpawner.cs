using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowSpawner : MonoBehaviour {
	public GameObject as1, as2, as3; //Arrow spawner game objects
	public GameObject arrow;
	public bool ShouldFire;
	private float timer = 0f; 
	private float delay = 0.5f;
	private GameObject temp;
	List<GameObject> arrows = new List<GameObject>();
	// Use this for initialization
	void Start () {
		ShouldFire = false;
		temp = new GameObject();
	}

	// Update is called once per frame
	void Update () {
		if (ShouldFire) {
			if (timer > delay) {
				int spawner = (int)Random.Range (1.0f, 3.9f);
				Fire (spawner);
				timer = 0.0f;
			}
			if (arrows.Count > 0) {
				foreach (GameObject g in arrows) {
					if (g.transform.localRotation.z > 0) {
						g.transform.position -= transform.up * Time.deltaTime * 3.5f;
					} else {
						g.transform.position += transform.up * Time.deltaTime * 3.5f;
					}
				}
				for (int i = 0; i < arrows.Count; i++) {
					if (arrows [i].transform.localRotation.z < 0 && arrows [i].transform.position.y > 5f) {
						Destroy (arrows [i]);
						arrows.Remove (arrows [i]);
					} else if (arrows [i].transform.localRotation.z > 0 && arrows [i].transform.position.y < 1.5f) {
						Destroy (arrows [i]);
						arrows.Remove (arrows [i]);
					}
				}
			}
			timer += Time.deltaTime;
		}
		
	}

	void OnTriggerEnter2D(Collider2D col){
		if (col.tag == "Player")
			ShouldFire = true;
	}

	void Fire(int sp){
		switch (sp) {
		case 1: 
			temp = as1;
			break;
		case 2:
			temp = as2;
			break;
		case 3: 
			temp = as3;
			break;
		default: 
			Debug.Log ("ERROR: Unknown arrow spawner!");
			break;
		}
		GameObject t_Arrow = Instantiate (arrow, temp.transform.position, temp.transform.localRotation);
		arrows.Add (t_Arrow);
	}

	void Timer(){
		timer += Time.deltaTime;
	}
}
