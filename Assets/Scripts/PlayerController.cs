using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {
    //Declare all required variables and such at the top of your class, this makes them essentially global but since they are within a class they are technically encapsulated. 
    // If you declare them as Public then they will be visible in the inspector in Unity, private means they are not. However, there is a way to have a variable that's private shown
    // in the inspector, one of them is listed below; can you tell which one? 
	static float MAX_VELOCITY = 3f;
	private Rigidbody2D body;
	private float horizontal, vertical;
	private float jumpVelocity = 290f;
	private float doubleJumpVelocity = 200f; 
	private float fallMultiplier = 1.25f;
	private float lowJumpMultiplier = 1.75f;
	public bool canDoubleJump, grounded;
	private bool Alive, GotKey, FallingSpikesActive, chasingSpikesActive, runTimer;
	private int Deaths;
	private float timeTaken;
    private Vector3 p_Velocities; //Here is a vector of the three velocities: Max Speed (x), Acceleration (y) and Deceleration (z)
	public Transform SpawnPoint;
    [SerializeField] private Vector2 vel;
	public GameObject gameOver, fallingSpikes01, key, keySprite, chasingSpikes01, doorLocked, doorUnlocked, Win;
	public Text deaths, timer, keyHint;
	private Sprite[] sprites;
	private SpriteRenderer spRen;

	// Use this for initialization of variables, whilst it's possible to initialise a variable upon definition (see above), it is generally considered better practice to do so in the Start() function. 
	void Start () {
		body = GetComponent<Rigidbody2D> ();
		grounded = true;
		canDoubleJump = true;
		Alive = true; 
		transform.position = SpawnPoint.position;
		GotKey = false;
		FallingSpikesActive = false;
		chasingSpikesActive = false;
		runTimer = true;
		Deaths = 0;
		deaths.text = "DEATHS: " + Deaths;
		sprites = Resources.LoadAll<Sprite> ("Sprites/pSprite");
		spRen = GetComponent<SpriteRenderer> ();
        p_Velocities = new Vector3(4f, 50f, 2.5f);
		timeTaken = 0f;
	}
	
	// Update is called once per frame and is effectively the main game loop, so, you would want to conduct all movements here that are not effective of Physics. If you are effecting the Physics directly you should 
    // instead use void FixedUpdate() 
	void Update () {
		//Check for any button presses first to respawn, reload or quit the application
		if (Input.GetKeyDown (KeyCode.Escape))
			Application.Quit ();
		else if (Input.GetKeyDown (KeyCode.R))
			Respawn ();
		else if (Input.GetKeyDown(KeyCode.T))
			SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);

		//set all win conditions (debug)
		if (Input.GetKey (KeyCode.P) && Input.GetKey (KeyCode.I) && Input.GetKey (KeyCode.E)) {
			GotKey = true;
			transform.position = new Vector3 (6.32f, -2f, 0f);
		}

		//Initially check if the player is alive, if not then wait until they respawn to continue with computation
		if (!Alive) {
			if (Input.GetKey (KeyCode.R))
				Respawn ();
		} else if (Win.activeSelf) {
			if (Input.GetKey (KeyCode.T))
				SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
			else if (Input.GetKey (KeyCode.Escape))
				Application.Quit ();
		}else {
			if (runTimer) {
				Timer ();
			}
			horizontal = Input.GetAxis ("Horizontal");
			vel = body.velocity;

            // Horizontal Movement 
            {
                //Check to see if the horizontal movement button has been pressed, if so increase the velocity on the x axis
                if (Input.GetButton("Horizontal"))
                {
                    // velocity of x + the right vector (1, 0, 0) * the horizontal input (0 or 1) * the max speed * the acceleration * delta time
                    body.velocity += Vector2.right * horizontal * p_Velocities.x * p_Velocities.y * Time.deltaTime;
                }

                //Check to see whether the player should be decelerating, or not. 
                if (body.velocity.x > 0f && !Input.GetButton("Horizontal"))
                {
                    body.velocity -= Vector2.right * p_Velocities.z * p_Velocities.x * Time.deltaTime;
                    if (body.velocity.x < 0f)
                        body.velocity = new Vector2(0f, body.velocity.y);
                }
                else if (body.velocity.x < 0f && !Input.GetButton("Horizontal"))
                {
                    body.velocity += Vector2.right * p_Velocities.z * p_Velocities.x * Time.deltaTime;
                    if (body.velocity.x > 0f)
                        body.velocity = new Vector2(0f, body.velocity.y);
                }

                //Check to ensure we cannot exceed the maximum stated velocity 
                if (body.velocity.x >= p_Velocities.x)
                    body.velocity = new Vector2(p_Velocities.x, body.velocity.y);
                else if (body.velocity.x <= -p_Velocities.x)
                    body.velocity = new Vector2(-p_Velocities.x, body.velocity.y);
            }

            //Jumping
            {
                if (Input.GetButtonDown("Jump") && grounded)
                {
                    grounded = false;
                    body.velocity += Vector2.up * jumpVelocity * Time.deltaTime;
                }
                else if (Input.GetButtonDown("Jump") && !grounded && canDoubleJump)
                {
                    body.velocity = new Vector2(body.velocity.x, 0f);
                    body.velocity += Vector2.up * doubleJumpVelocity * Time.deltaTime;
                    canDoubleJump = false;
                }
            }
            //falling behaviour
            {
                if (body.velocity.y < 0)
                {
                    body.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
                }
                else if (body.velocity.y < 0 && !Input.GetButton("Jump"))
                {
                    body.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
                }
            }
            //Sprite swapping depending on player state
            {
                if (!grounded)
                {
                    spRen.sprite = sprites[4];
                }
                else if (grounded && body.velocity.x > 0.0f)
                {
                    spRen.sprite = sprites[1];
                }
                else if (grounded && body.velocity.x < 0.0f)
                {
                    spRen.sprite = sprites[3];
                }
                else
                {
                    if (spRen.sprite == sprites[3])
                        spRen.sprite = sprites[2];
                    else if (spRen.sprite == sprites[1])
                        spRen.sprite = sprites[0];
                    else
                        spRen.sprite = sprites[0];
                }
            }



            //spike movement
            {
                if (FallingSpikesActive)
                {
                    Vector2 fsaTemp = new Vector2(fallingSpikes01.transform.position.x, fallingSpikes01.transform.position.y - 4f * Time.deltaTime);
                    fallingSpikes01.transform.position = fsaTemp;
                    if (fallingSpikes01.transform.position.y < -5f)
                    {
                        fallingSpikes01.SetActive(false);
                        fallingSpikes01.transform.position = Vector2.zero;
                        FallingSpikesActive = false;
                    }
                }

                if (chasingSpikesActive)
                {
                    Vector2 csaTemp = new Vector2(chasingSpikes01.transform.position.x + p_Velocities.x * 1.15f * Time.deltaTime, chasingSpikes01.transform.position.y);
                    chasingSpikes01.transform.position = csaTemp;
                    if (chasingSpikes01.transform.position.x > 3.25f)
                    {
                        chasingSpikes01.SetActive(false);
                        chasingSpikes01.transform.position = Vector2.zero;
                        chasingSpikesActive = false;
                    }
                }
            }	

		}
	}

	void Timer(){
		timeTaken += (float)Time.deltaTime;
		float m = Mathf.Floor (timeTaken / 60);
		float s = (timeTaken % 60);
		float mi = (timeTaken * 1000f) % 1000;
		timer.text = m.ToString ("00") + ":" + s.ToString ("00") + ":" + mi.ToString ("0"); 
	}

	void EndOfLevel(){
		Win.SetActive(true);
		runTimer = false;
	}

    //This method is triggered (lel EDP right?) when two colliders set as triggers intersect, this makes interaction between gameobject entities easy to control. Be wary that if you set a collider as a trigger, 
    //and the entity also has a rigidbody attached it will fall through the scene as gravity is activated and the collider is effectively not enabled so does not interact for a physical collision. 
	void OnTriggerEnter2D(Collider2D col){
		switch (col.tag) {
		case "Untagged":
			break;
		case "Finish":
			if (GotKey)
					EndOfLevel ();                   
                else
					keyHint.gameObject.SetActive (true);
			break;
		case "Ground":
			break;
		case "Boundaries":
			break;
		case "ArrowCollider":
			break;
		case "Collectible":
			GotKey = true;
			col.gameObject.SetActive (false);
			keySprite.SetActive (true);
			FallingSpikesActive = true;
			doorLocked.SetActive (false);
			doorUnlocked.SetActive (true);
			break;
		case "Death": 
			spRen.sprite = sprites [5];
			Alive = false;
			gameOver.SetActive (true);
			break;
		case "ChaseCollider":
			if (GotKey) {
				chasingSpikesActive = true;
			}
			break;
		default: 
			Debug.Log ("ERROR: Could not match OnTriggerEnter2D tag '"+col.tag+"' to case!");
			break;
		}
	}

	//Generally just used for displaying text when key has not been collected. 
	void OnTriggerExit2D(Collider2D col){
		switch (col.tag) {
		case "Finish":
			if (!GotKey)
				keyHint.gameObject.SetActive (false);
			break;
		default: 
			break;
		}
	}

    //Contrary to the above method, this one detects physical collisions between colliders. It is also useful for collision detections, that result in physical transformation, e.g. stopping a collider from passing through another!
	void OnCollisionEnter2D(Collision2D col){
		switch (col.transform.tag) {
		case "Untagged":
			break;
		case "ArrowCollider":
			break;
		case "Boundaries":
			break;
		case "Death": 
			spRen.sprite = sprites [5];
			Alive = false;
			gameOver.SetActive (true);
				break;
		case "Ground":
			if (!grounded)
				grounded = true;
			if (!canDoubleJump)
				canDoubleJump = true;
				break;
		default: 
			Debug.Log ("ERROR: Could not match OnCollisionEnter2D tag '"+col.transform.tag+"' to case!");
				break;
		}
	}

	//Function used upon death to respawn at the spawnpoint created for each level. (could be extended to move spawn points if a save point is incorporated later)
	void Respawn(){
		transform.position = SpawnPoint.position;
		Alive = true;
		gameOver.SetActive (false);
		Deaths++;
		deaths.text = "DEATHS: " + Deaths;
	}
}
