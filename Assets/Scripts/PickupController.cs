using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupController : MonoBehaviour {

	public GameObject player;
	private Transform player_tm;
	private PlayerController player_control;
	public float speed;
	private float normal_speed;
	public float timer;

	public float fall_fraction;

	//Maybe change from int later so combinations of colors can be collected
	public int color;//1 is red, 2 is green, and 3 is blue.

	private Vector3 spawn_position;

	private Rigidbody rb;
	private Transform tm;
	private Renderer rend;

	//For slowing down effect when falling
	//private bool got_vel;
	//private Vector3 normal_vel;
	//private Vector3 normal_ang_vel;

	//Time until the pickup is destroyed
	private float remove_time;
	private float timeleft_remove;

	// Use this for initialization
	void Start () {

		//How long before the pickup moves towards the player
		timer = 2.0f;

		speed = 7f;
		player = GameObject.Find ("Player");
		player_tm = player.GetComponent<Transform> ();
		player_control = player.GetComponent<PlayerController> ();

		rb = GetComponent<Rigidbody> ();
		tm = GetComponent<Transform> ();
		rend = GetComponent<Renderer> ();

		spawn_position = tm.position;

		normal_speed = speed;

		//got_vel = false;

		//Percent of speed that is decreased for slow fall
		fall_fraction = 0.05f;//0.1f default

		//Time until the pickup is destroyed
		remove_time = 10f;
		timeleft_remove = remove_time;
	}
	
	// Update is called once per frame
	void Update () {

		//timer -= Time.deltaTime;
		//Have a slowing down effect as they fall
	
		if (player_control.select_on == true) {

			/*
			if (got_vel == false) {

				normal_vel = rb.velocity;
				normal_ang_vel = rb.angularVelocity;
				got_vel = true;

			}

			var temp_vel = rb.velocity;
			temp_vel = normal_vel * player_control.reduce_fraction;
			rb.velocity = temp_vel;

			var temp_ang_vel = rb.angularVelocity;
			temp_ang_vel = normal_ang_vel * player_control.reduce_fraction;
			rb.angularVelocity = temp_ang_vel;
			*/
			//rb.velocity = (rb.velocity - (rb.velocity * fall_fraction)) * player_control.reduce_fraction;
			//rb.angularVelocity = (rb.angularVelocity - (rb.angularVelocity * fall_fraction)) * player_control.reduce_fraction;


			//Not super happy with this effect of slow mo falling but I don't wanna bother with it right now.
			rb.velocity -= (rb.velocity * fall_fraction);
			rb.velocity = rb.velocity * player_control.reduce_fraction;
			rb.angularVelocity -= rb.angularVelocity * fall_fraction;
			rb.angularVelocity = rb.angularVelocity * player_control.reduce_fraction;

			timer -= Time.deltaTime * player_control.reduce_fraction;

		}

		if (player_control.select_on == false) {

			/*
			got_vel = false;

			if (normal_vel != new Vector3 (0f, 0f, 0f)) {

				
				var temp_vel = rb.velocity;
				temp_vel = normal_vel;
				rb.velocity = temp_vel;


				var temp_ang_vel = rb.angularVelocity;
				temp_ang_vel = normal_ang_vel;
				rb.angularVelocity = temp_ang_vel;


	
		

			}
		*/

			rb.velocity -= (rb.velocity * fall_fraction);
			rb.angularVelocity -= rb.angularVelocity * fall_fraction;

			timer -= Time.deltaTime;



		}


		//rb.velocity -= (rb.velocity * fall_fraction);
		//rb.angularVelocity -= (rb.angularVelocity * fall_fraction);

		//Deals with removing pickups. Have to slowdown Time for them because they could idssapear while the player is switching weapons.
		//Not sure if most efficent way to remove?
		timeleft_remove -= player_control.SlowDown (Time.deltaTime);

		if (timeleft_remove <= 0f) {

			Destroy (gameObject);

		}


		//Move towards player now
		if (timer <= 0) {

			timer = 0;
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;

			//Slow down if player has gun select menu open
			if (player_control.select_on == true)
				speed = normal_speed * player_control.reduce_fraction;
			if (player_control.select_on == false)
				speed = normal_speed;
			
			float step = speed * Time.deltaTime;
			transform.position = Vector3.MoveTowards (transform.position, player_tm.position, step);
		}

		//Can't go below where they were spawned. Can't go through floor
		if (tm.position.y <= spawn_position.y - 0.2f) {

			tm.position = new Vector3 (tm.position.x, spawn_position.y - 0.2f, tm.position.z);
		}



		//Set the color of the pickup to be the color stated by the varaible color.
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
}
