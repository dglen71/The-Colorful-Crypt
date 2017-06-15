using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DasherController : MonoBehaviour {
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

	private Vector3 move_direction;
	private Vector3 look_direction;
	private Quaternion look_rotation;
	private bool got_target;
	public float turn_speed;
	private float normal_turn_speed;
	private bool have_turned;
	private bool reached_target;

	private float turn_pause_time;
	private float timeleft_turn;

	private float error;

	private float default_rotation_x;
	private float default_rotation_z;


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


		speed = 10f;
		normal_speed = speed;

		got_target = false;
		turn_speed = 150f;
		normal_turn_speed = turn_speed;
		have_turned = false;
		error = 1f;
		reached_target = false;

		turn_pause_time = 1f;
		timeleft_turn = turn_pause_time;

		default_rotation_x = transform.rotation.eulerAngles.x;
		default_rotation_z = transform.rotation.eulerAngles.z;

	}

	// Update is called once per frame
	void Update () {

		//Make sure the Dasher has the correct y-value
		if (player_control.floor == 1)
			transform.position = new Vector3(transform.position.x, 50.5f, transform.position.z);

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

		//Resets attack after getting to a location and taking a brief pause
		if (reached_target == true) {

			timeleft_turn -= Time.deltaTime;

		}
		if (timeleft_turn <= 0f) {

			reached_target = false;
			//got_target = false;
			timeleft_turn = turn_pause_time;

		}

		//Get the target location of the player if the enemy hasn't already
		if (got_target == false && reached_target == false && pause == false) {

			move_direction = player.transform.position;

			look_direction = (player.transform.position - transform.position).normalized;

			look_rotation = Quaternion.LookRotation (look_direction);

			//default_rotation_x = transform.rotation.eulerAngles.x;
			//default_rotation_z = transform.rotation.eulerAngles.z;

			got_target = true;
		}

		//Keep  x and z rotation stable
		transform.rotation = Quaternion.Euler(default_rotation_x,transform.rotation.eulerAngles.y,default_rotation_z);

		Debug.Log (have_turned);
		Debug.Log (transform.rotation.eulerAngles.y);
		Debug.Log (look_rotation.eulerAngles.y);
		//Rotate to the target

		turn_speed = player_control.SlowDown (normal_turn_speed);

		if (have_turned == false && got_target == true) {


			transform.rotation = Quaternion.RotateTowards (Quaternion.Euler(default_rotation_x, transform.rotation.eulerAngles.y, default_rotation_z), Quaternion.Euler(default_rotation_x, look_rotation.eulerAngles.y, default_rotation_z), turn_speed * Time.deltaTime);


			if (transform.rotation.eulerAngles.y > look_rotation.eulerAngles.y - error && transform.rotation.eulerAngles.y < look_rotation.eulerAngles.y + error) {

				have_turned = true;

			}

			/*
			if (transform.rotation.eulerAngles.y == look_rotation.eulerAngles.y) {

				have_turned = true;
			}
			*/
				

		}




	}

	void FixedUpdate(){

		//no rolling or any other normal physics movement! Cause Screw Physics!
		rb.velocity = new Vector3 (0f, 0f, 0f);
		rb.angularVelocity = new Vector3 (0f, 0f, 0f);

		//The hot new way to slow down the speed of things
		speed = player_control.SlowDown (normal_speed);

		/*
		//How the enemy move towards the player
		//Followers simply move towards the player with a set speed
		float step = speed * Time.deltaTime;
		if (pause == false) {
			transform.position = Vector3.MoveTowards (transform.position, player.transform.position, step);
		}
		*/

		//How the enemy moves towards the player
		//Dashers move quickly towards the target point after they have rotated.
		if (have_turned == true) {
			float step = speed * Time.deltaTime;
			//Keeps its y-value but moves towards the position(x and z) of the target
			transform.position = Vector3.MoveTowards (transform.position,new Vector3(move_direction.x, transform.position.y, move_direction.z), step);

			if (transform.position == move_direction) {

				have_turned = false;
				got_target = false;
				reached_target = true;

			}

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

		//Ignore Collisions with other enemies
		/*
		if (other.gameObject.tag != "Player" || other.gameObject.tag != "Wall") {

			Collider col = GetComponent<Collider> ();

			Physics.IgnoreCollision (col, other.collider);

		}
		*/
		if (other.gameObject.tag == "Follower" || other.gameObject.tag == "Dasher") {

			Collider col = GetComponent<Collider> ();

			Physics.IgnoreCollision (col, other.collider);

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
