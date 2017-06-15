using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FireController : MonoBehaviour {

	public float time_to_fire;
	public float timeleft;
	public GameObject bullet;
	public float speed = 5f;
	private float normal_speed;

	public PlayerController player;
	public Renderer gun1_rend;

	//private Rigidbody rb; Not used 
	private Rigidbody rb_bullet;
	private GameObject bullet_clone;
	private Renderer bullet_rend;
	private Transform tm;

	private List<GameObject> bullet_clones = new List<GameObject> ();


	// Use this for initialization
	void Start () {

		//rb = GetComponent <Rigidbody> ();
		tm = GetComponent<Transform>();

		//Timer to see when the player can fire again. Maybe increase?
		time_to_fire = 0.1f;
		timeleft = time_to_fire;

		normal_speed = speed;

	}


	void Update(){

		//Handles timer.
		timeleft -= Time.deltaTime;
		if (timeleft <= 0) {

			timeleft = 0f;
		}

		//Reduces speed of bullets launched if the player has the gun select menu open
		/*
		if (player.select_on == true)
			speed = normal_speed * player.reduce_fraction;
		if (player.select_on == false)
			speed = normal_speed;
		*/
		speed = player.SlowDown (normal_speed);

		//Handles the firing of bullets. Can only fire at certain times
		if (Input.GetMouseButtonDown (0) == true && timeleft == 0 && player.upgrade_zone == false && player.select_on == false) {
			//Creates Bullet
			bullet_clone = Instantiate (bullet, tm.position, tm.rotation);
			//Destroy bullet after a certain amount of time if on Hub World because performance doesn't matter
			if (player.floor == 0) {
				Destroy (bullet_clone, 5f);
			}
			//If player is in game add them to a list to be deleted once the player is done on the floor. Hopes to increase performance.
			if (player.floor != 0) {
				bullet_clones.Add (bullet_clone);
			}

			//Fire the bullet
			bullet_rend = bullet_clone.GetComponent<Renderer> ();
			rb_bullet = bullet_clone.GetComponent<Rigidbody> ();
			rb_bullet.AddForce (bullet_clone.GetComponent<Transform> ().forward * speed);
			timeleft = time_to_fire;

			//Make the bullet the same color as the gun selected
			bullet_rend.material.color = player.guns [player.gun_selected - 1].rend.material.color;

		}

	}

	//Function to handle the clearing of the bullets collected when the player is on a floor.
	public void Clear()
	{
		foreach (GameObject clone in bullet_clones)
			Destroy (clone);
	}

}
