using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

	//I do not think any of the floor timers should be effected by the player's slow down. That seems silly to me. May change later.

	public int wave_number;
	public int max_wave_number;
	public GameObject follower;
	GameObject[] followers;
	public GameObject dasher;
	GameObject[] dashers;
	public Transform spawn1;
	public Transform spawn2;
	public Transform spawn3;
	public Transform spawn4;

	public GameObject player;
	public FireController fire_control;

	public GameObject pickup;

	Transform spawn_location;

	private bool start;
	private bool floor_running;

	private bool wave_over;

	private float wave_time;
	private float timeleft;

	private float spawn_time;
	private float timeleft_spawn;
	bool spawn_enemy;

	private float end_time;
	private float timeleft_end;

	private int number_enemies;
	private int number_followers;
	private int number_dashers;


	private int enemies_left;
	private int followers_left;
	private int dashers_left;

	private bool floor_over;

	private GameObject[] bullets;
	private GameObject[] pickups;
	//private GameObject[] follower_clones;
	private List<GameObject> follower_clones = new List<GameObject>();
	private List<GameObject> dasher_clones = new List<GameObject>();

	private float chance_follower;
	private float chance_dasher;

	// Use this for initialization
	void Start () {

		start = false;
		floor_running = false;

		//How any waves on the floor there are
		wave_number = 0;
		max_wave_number = 10;
		wave_over = true;

		//Time Between Waves
		wave_time = 4f;
		timeleft = wave_time;

		//Time between enemy spawns
		spawn_time = 1f;
		timeleft_spawn = spawn_time;
		spawn_enemy = false;

		//Time from when the floor ends to when the player is warped
		end_time = wave_time;
		timeleft_end = end_time;
		floor_over = false;

		//What is the chance a type of enemy will be set to spawn and spawn during a wave(same percentage)
		chance_follower = 0.6f;
		chance_dasher = 0.4f;

	}
	
	// Update is called once per frame
	void Update () {

		//bullets = GameObject.FindGameObjectsWithTag ("Bullet");
		//pickups = GameObject.FindGameObjectsWithTag ("Pick Up");
		//follower_clones = GameObject.FindGameObjectsWithTag ("Follower");

		//Where is the player? Change the option to start the floor if the player is on the floor
		if (player.GetComponent<PlayerController> ().CheckLocation () == "Floor1") {
			start = true;
		}

		if (player.GetComponent<PlayerController> ().CheckLocation () == "HubWorld") {
			start = false;
		}

		//Start the floor
		if (start == true && floor_running == false && floor_over == false) {
			floor_running = true;
			start = false;
		}
			
		//What occurs while the floor is running
		if (floor_running == true) {

			//Handles timer for spawning enemies
			if (spawn_enemy == true) {
				timeleft_spawn -= Time.deltaTime;
			}

			if (timeleft_spawn <= 0f) {

				timeleft_spawn = spawn_time;
				if (enemies_left > 0) {
					SpawnEnemy ();
				}
			}


			//Handles timer for chaging waves
			if (wave_over == true) {

				timeleft -= Time.deltaTime;
				spawn_enemy = false;

				if (wave_number >= max_wave_number) {

					//SceneManager.LoadScene ("HubWorld");
					floor_over = true;
					ResetFloor();
					//WarpPlayer ();
				}

			}
			if (timeleft <= 0f) {

				wave_over = false;
				timeleft = wave_time;
				if (wave_number <= max_wave_number)
					StartWave ();

			}

			//Check to see if all of the enemies have been killed. If they have then declare the wave to be over
			if (wave_over == false && wave_number != 0) {
				if (CheckEnemies () == true) {
						wave_over = true;
					}
				}

		}

		//Handles timing for the end of the floor
		if (floor_over == true) {

			timeleft_end -= Time.deltaTime;
		}
		if (timeleft_end < 0f) {
			timeleft_end = end_time;
			floor_over = false;
			WarpPlayer ();
		}



	}

	//What happens at the start of the wave
	void StartWave()
	{

		wave_number += 1;
		Debug.Log (wave_number);

		//Amount of enemies to be spawned during that wave
		number_enemies = wave_number;
		//Number of enemies left to be spawned. All of them at the start of a wave.
		enemies_left = number_enemies;

		//figure out how many of each enemy type will be spawning this wave
		if (number_enemies <= 4) {

			number_followers = number_enemies;
			number_dashers = 0;

		} else if (number_enemies > 4) {

			number_followers = 4;

			while (number_followers + number_dashers != number_enemies) {

				//will have to change when more then two enemies are added
				float rand = Random.Range (0.0f, 1.0f);
				if (rand <= chance_follower) {
					number_followers++;
				} else if (rand > 1 - chance_dasher) {
					number_dashers++;
				}

			}



		}

		followers_left = number_followers;
		dashers_left = number_dashers;

		followers = new GameObject[number_followers];
		dashers = new GameObject[number_dashers];

		/* No idea what this was gonna be used for? Maybe a more efficient spawning method?
		for(int i = 0; i < number_enemies; i++)
		{
			//followers [i].SetActive (false);
		}
		*/

		//Spawn the inital enemies.
		SpawnStartingEnemies ();	
	




	}

	//Spawn an enemy in one of the 4 random spawn locations
	void SpawnEnemy()
	{
		int spawn_number = Random.Range (1, 5);

		if (spawn_number == 1) {
			spawn_location = spawn1;
		}
		if (spawn_number == 2) {
			spawn_location = spawn2;
		}
		if (spawn_number == 3) {
			spawn_location = spawn3;
		}
		if (spawn_number == 4) {
			spawn_location = spawn4;
		}

		//only spawn followers at first
		if (wave_number <= 4) {
			SpawnFollower ();
		}
		if (wave_number > 4) {

			if (followers_left != 0) {

				if (dashers_left != 0) {

					float rand = Random.Range (0.0f, 1.0f);

					if (rand <= chance_follower) {

						SpawnFollower ();

					} else {
						SpawnDasher ();
					}

				} else if (dashers_left == 0) {
					SpawnFollower ();
				}

			} else if (followers_left == 0)
				SpawnDasher ();

		}



	}

	//Spawns a Follower
	void SpawnFollower()
	{
		GameObject clone = Instantiate (follower, spawn_location.position, spawn_location.rotation);
		CollectFollower (clone);
		followers_left--;
		enemies_left--;
		followers [number_followers - followers_left - 1] = clone;

	}

	void SpawnDasher()
	{
		GameObject clone = Instantiate (dasher, spawn_location.position, spawn_location.rotation);
		dasher_clones.Add (clone);
		dashers_left--;
		enemies_left--;
		dashers [number_dashers - dashers_left - 1] = clone;

	}


	//Spawns the inital amount of enemies. Max is 4 and then they start spawning on a timer.
	void SpawnStartingEnemies()
	{

		int number_to_spawn = 0;

		if (number_enemies < 5)
			number_to_spawn = number_enemies;
		else if (number_enemies >= 5)
			number_to_spawn = 4;

		for (int i = 0; i < number_to_spawn; i++) {

			SpawnEnemy ();

		}

		spawn_enemy = true;
			
	}
		

	bool CheckEnemies()//Checks to see if all of the enemies are dead. Returns true if they are.
	{
		int amount_dead = 0;

		if (enemies_left == 0) {
			
			for (int i = 0; i < number_followers; i++) {

				if (followers [i].activeSelf == false)
					amount_dead++;

			}
			if (wave_number > 4) {

				for (int i = 0; i < number_dashers; i++)
				{
					if (dashers [i].activeSelf == false)
						amount_dead++;

				}

			}


			if (amount_dead == number_enemies)
				return true;
			else
				return false;

		} else
			return false;


	}

	//Move player once floor is done
	void WarpPlayer()
	{
		//player.transform.position = new Vector3 (0f, 0.5f, 0f);
		player.GetComponent<PlayerController>().floor = 0;//Warp the player back to the Hub World by changing the floor variable.
	}

	/*Not used anymore
	void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.tag == "Player") {

			//start = true;
			//Debug.Log (start);

		}


	}
	*/

	//Adds followers to a list to be destroyed once the player is finished with the floor
	void CollectFollower(GameObject other)
	{
		follower_clones.Add (other);
	}

	void ResetFloor()//Sets all variables back to default values to ensure the floor can be played again
	{
		floor_running = false;
		wave_number = 0;
		wave_over = true;

		foreach (GameObject clone in follower_clones) {
			Destroy (clone);
		}

		fire_control.Clear ();
			

		/*Attempt at doing all cleaning here. Changed to be done in various scripts. Easiest to be done where they are created
		foreach (GameObject clone in bullets) {
			if (clone) {
				Destroy (clone);
			}
		}

		foreach (GameObject clone in pickups) {
			if (clone) {
				Destroy (clone);
			}
		}

		foreach (GameObject clone in follower_clones) {
			if (clone) {
				Destroy (clone);
			}
		}
		*/



	}

	//Drop Pickups for the player
	//More general version to be used by all enemies
	public void DropPickUps(int amount, Transform tm, int color)
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
}
