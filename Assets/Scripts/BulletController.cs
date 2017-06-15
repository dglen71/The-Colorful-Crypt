using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {

	GameObject player;
	PlayerController player_control;

	Rigidbody rb;
	public Vector3 normal_vel;

	//Has the velocity of the bullet been stored in normal_vel?
	private bool got_vel;

	// Use this for initialization
	void Start () {

		player = GameObject.Find ("Player");
		player_control = player.GetComponent<PlayerController> ();

		rb = GetComponent<Rigidbody> ();
		//normal_vel = rb.velocity;

		got_vel = false;

	}

	void FixedUpdate()
	{

		//if the player has the gun select menu open get the vel of the bullet and slow it down
		if (player_control.select_on == true) {
			if (got_vel == false) {

				normal_vel = rb.velocity;
				got_vel = true;
			}
			var temp_vel = GetComponent<Rigidbody> ().velocity;
			temp_vel = normal_vel * player_control.reduce_fraction;
			rb.velocity = temp_vel;
		}

		//go back to normal if the player exits the gun select menu
		if (player_control.select_on == false) {
			got_vel = false;
			if (normal_vel != new Vector3 (0f, 0f, 0f)) {

				var temp_vel2 = GetComponent<Rigidbody> ().velocity;
				temp_vel2 = normal_vel;
				rb.velocity = temp_vel2;

			}


		}
			

	}
	
	// Update is called once per frame
	void Update () {

		
	}
}
