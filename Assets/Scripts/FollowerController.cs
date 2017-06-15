using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerController : MonoBehaviour {

	public float speed;
	private float normal_speed;
	public float health;

	public float pause_time;
	public float timeleft;

	public GameObject pickup;
	public GameObject player;
	private PlayerController player_control;

	private GameController game_control;

	private bool pause;
	private Transform tm;
	private Rigidbody rb;
	private Renderer rend;

	private int color; //1 is red, 2 is green, 3 is blue

	//maybe change how pickups are destoryed later. Most likely done not in this class though
	//private List<GameObject> pickup_clones = new List<GameObject> ();
	//private float remove_time;
	//private  float timeleft_remove;

	// Use this for initialization
	void Start () {

		health = 30;
		pause = false;
		pause_time = 3f;
		timeleft = pause_time;
		normal_speed = speed;

		//remove_time = 10f;
		//timeleft_remove = remove_time;

		tm = GetComponent<Transform> ();
		rb = GetComponent<Rigidbody> ();
		rend = GetComponent<Renderer> ();

		player = GameObject.Find ("Player");
		player_control = player.GetComponent<PlayerController> ();

		game_control = GameObject.Find ("GameController").GetComponent<GameController> ();

		color = Random.Range (1, 4);// Gets a random number between 1 and 3 to set the color of the enemy.

		//Set the color of the enemy to be the color stated by the varaible color.
		if (color == 1) {

			rend.material.color = Color.red;
		}
		if (color == 2) {

			rend.material.color = Color.green;
		}
		if (color == 3) {

			rend.material.color = Color.blue;
		}

	}
	
	// Update is called once per frame
	void Update () {

		//handles how long the enemy stops after it has hit and done damage to the player.
		if (pause == true) {

			//timeleft -= Time.deltaTime;
			//Slow this down so the pause time an enemy takes is effected by the gun select menu's time slow down
			timeleft -= player_control.SlowDown(Time.deltaTime); 
		}
		if (timeleft <= 0f) {

			pause = false;
			timeleft = pause_time;
		}


	}

	void FixedUpdate(){

		//no rolling or any other normal physics movement! Cause Screw Physics!
		rb.velocity = new Vector3 (0f, 0f, 0f);

		//slow down the enemy if the player has the gun select menu open
		/*
		if (player_control.select_on == true)
			speed = normal_speed * player_control.reduce_fraction;
		if (player_control.select_on == false)
			speed = normal_speed;
		*/

		//The hot new way to slow down the speed  of things
		speed = player_control.SlowDown (normal_speed);

		//How the enemy move towards the player
		//Followers simply move towards the player with a set speed
		float step = speed * Time.deltaTime;
		if (pause == false) {
			transform.position = Vector3.MoveTowards (transform.position, player.transform.position, step);
		}
			

		//Followers really die if they are killed!
		if (health <= 0) {

			Death ();
		}

	}

	void OnTriggerEnter(Collider other){

		if (other.gameObject.CompareTag ("Bullet")) {

			//The default damage done by a bullet
			float damage = 10;
			//The damage changes based on the color of the enemy and  the color of the gun selected
			for (int i = 0; i < player_control.guns.Length; i++) {

				if (player_control.gun_selected == i + 1) {

					//Damage formula: Subject to change
					damage = 10 * ((rend.material.color.r * player_control.guns[i].red_UI_value / 100f) + 1) * ((rend.material.color.g * player_control.guns[i].green_UI_value/ 100f) + 1) * ((rend.material.color.b * player_control.guns[i].blue_UI_value/ 100f) + 1);

				}

			}

			Debug.Log (damage);
			other.gameObject.SetActive (false);
			health -= damage;

		}


	}

	void OnCollisionEnter(Collision other){

		//Stop moving for a short period of time if the enemy hits the player
		if (other.gameObject.tag == "Player") {

			pause = true;



		}

	}

	//What the enemy does when they die
	void Death(){

		//DropPickUps (3);
		game_control.DropPickUps(3, tm, color);

		gameObject.SetActive(false);
	}

	//Drop Pickups for the player
	void DropPickUps(int amount)
	{
		//Min and Max value for for applied to pickups when they are spawned
		float min_value = 300f;
		float max_value = 400f;


		for(int i = 0; i < amount; i++)
		{
			//random rotation is decided for each pickup spawned
			Quaternion random_rotation = Quaternion.Euler(new Vector3(Random.Range(0f,360f), Random.Range(0f,360f),Random.Range(0f,360f)));
			//Create the Pickup
			GameObject pickup_clone = Instantiate (pickup, tm.position, random_rotation);
			/* Removal of pickups is now handeled in Pickup controller.
			//pickup_clones.Add (pickup_clone);
			//Get rid of Pickup after a certain amount of time(Maybe change later to be more efficient?)
			//Destroy(pickup_clone,10f);
			//Fire Pickup!
			*/
			Vector3 force = (pickup_clone.GetComponent<Transform>().forward * Random.Range(min_value,max_value));
			pickup_clone.GetComponent<PickupController> ().color = color;
			pickup_clone .GetComponent<Rigidbody> ().AddForce (force);

		}


	}

	/*
	public void ClearPickUps()
	{
		foreach (GameObject clone in pickup_clones)
			Destroy (clone);

	}
	*/
		
}
