using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

	public float speed;
	private float normal_speed;
	public Text countText;
	public Text winText;
	public Text healthText;

	public float health;

	public float portal_time;
	public float timeleft;
	private bool on_portal;

	public int floor; //0 is hub world.

	private Rigidbody rb;
	private Transform tm;
	public int red_count;
	public int green_count;
	public int blue_count;
	private Scene current_scene;

	public GameObject upgrade_shop;
	public bool upgrade_zone;

	public Gun[] guns; //= new Gun[3];
	public int gun_selected;

	public Canvas gun_select;
	private Button[] gun_buttons;
	//private Button gun1_button;
	//private Button gun2_button;
	//private Button gun3_button;
	public bool select_on;

	public float reduce_fraction;

	void Start() //Occurs on the first frame of the game
	{
		rb = GetComponent <Rigidbody> ();
		tm = GetComponent<Transform>();
		//count = 0;
		//CheckScene();
		CheckLocation();
		SetCountText ();
		SetHealthText ();
		winText.text = "";

		floor = 0;

		//Haven't slowed this value down;
		portal_time = 3f;
		timeleft = portal_time;
		on_portal = false;

		upgrade_zone = false;


		//Set Up Guns
		guns = GetComponentsInChildren<Gun> ();
		Button[] guns_buttons = GetComponentsInChildren<Button> ();
		gun_selected = 1;
		guns [0].selected = true;
		guns [1].selected = false; 
		guns [2].selected = false;

		for (int i = 0; i < guns.Length; i++) {

			guns [i].gun_number = i + 1;

			guns [i].red_value = 100;
			guns [i].green_value = 100;
			guns [i].blue_value = 100;

			guns [i].red_UI_value = 0;
			guns [i].green_UI_value = 0;
			guns [i].blue_UI_value = 0;

			//Assuming no other buttons are on the player canvas besides gun buttons
			guns[i].button = guns_buttons[i];

			if (i == 0)
				guns [i].rend = GameObject.Find ("Cylinder Gun").GetComponent<Renderer> ();
			if (i == 1)
				guns [i].rend = GameObject.Find ("Cube Gun").GetComponent<Renderer> ();
			if (i == 2)
				guns [i].rend = GameObject.Find ("Capsule Gun").GetComponent<Renderer> ();
		}

		health = 100;

		/*Not in use because the buttons are now attached to the guns themselves
		gun_buttons = GetComponentsInChildren<Button>();
		foreach (Button button in gun_buttons) {

			if (button.name == "Gun1 Select") {
				gun1_button = button;
			}
			if (button.name == "Gun2 Select")
				gun2_button = button;
			if (button.name == "Gun3 Select")
				gun3_button = button;

		}
		*/

		//Setup Select Gun Menu
		select_on = false;
		gun_select.gameObject.SetActive (false);

		//How much everything is slowed down by when the gun select menu is open
		reduce_fraction = 0.1f;
		normal_speed = speed;


	}

	void Update() // Occurs before rendering a frame
	{
		//Developer Cheats
		if (Input.GetKeyDown (KeyCode.I) == true)
			red_count += 10;
		if (Input.GetKeyDown (KeyCode.O) == true)
			green_count += 10;
		if (Input.GetKeyDown (KeyCode.P) == true)
			blue_count += 10;


		SetHealthText ();
		//CheckScene ();
		CheckLocation();
		SetCountText ();

		//Turning on and off the Shop UI
		if (upgrade_zone == false) {
			upgrade_shop.SetActive (false);
		} else if (upgrade_zone == true) {
			upgrade_shop.SetActive (true);
		}

		//Handeling teleporting the player if on a portal. Gives them time to get off
		if (on_portal == true) {

			timeleft -= Time.deltaTime;
		}
		if (timeleft <= 0f) {

			on_portal = false;
			Warp ();

		}
		if (on_portal == false) {

			timeleft = portal_time;
		}

		//Gun Select Buttons
		if (Input.GetMouseButtonDown (1) == true && upgrade_zone == false) {
			select_on = true;
		}

		if (select_on == true) {
			
			gun_select.gameObject.SetActive (true);

		}

		//Getting Gun Select Buttons to match the color of the gun
		for (int i = 0; i < guns.Length; i++) {

			var temp_colors = guns [i].button.colors;
			temp_colors.normalColor = new Color (guns [i].red_value / 100f, guns [i].green_value / 100f, guns [i].blue_value / 100f);
			temp_colors.highlightedColor = new Color (temp_colors.normalColor.r * 0.9f, temp_colors.normalColor.g * 0.9f, temp_colors.normalColor.b * 0.9f);
			temp_colors.pressedColor = temp_colors.normalColor;
			temp_colors.disabledColor = temp_colors.normalColor;
			guns [i].button.colors = temp_colors;


		}

		//Handeling the input for the gun select menu round 2
		if (Input.GetMouseButtonUp (1) == true) {
			select_on = false;
		}

		if (select_on == false) {

			gun_select.gameObject.SetActive (false);

		}

		//Handles the switching of the guns. Turning off the other guns selected state is handled in the gun class.
		for (int i = 0; i < guns.Length; i++) {

			if (guns [i].selected == true && guns[i].gameObject.activeSelf == false) {

				guns [i].gameObject.SetActive (true);
				gun_selected = guns [i].gun_number;

			}
			if (guns [i].selected == false && guns [i].gameObject.activeSelf != false) {

				guns [i].gameObject.SetActive (false);

			}

		}



	}

	void FixedUpdate() //Occurs before any physics calculations
	{
		//Old Attempts to move and contain movement. Don't know if I should keep or not?

		//float temp = 0.5f;
		//Vector3 current_pos;
		//current_pos = tm.position;
		/*
		if (tm.position.y != tm.position.y) {

			tm.position = new Vector3 (current_pos.x, current_pos.y, current_pos.z);

		}
		*/
		//if (CheckLocation () == "HubWorld" && tm.position.y != 0.5f)
			//tm.position = new Vector3 (tm.position.x, 0.5f, tm.position.z);

		//if (CheckLocation() == "Floor1" && tm.position.y != 50.5f)
			//tm.position = new Vector3 (tm.position.x, 50.5f, tm.position.z);

		//Getting input from the player
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");

		//Movement allowed if not in the upgrade shop
		if (upgrade_zone == false) {

			//Slowing down time when gun select menu is open
			/*
			if (select_on == true)
				speed = normal_speed * reduce_fraction;
			if (select_on == false)
				speed = normal_speed;
			*/
			speed = SlowDown (normal_speed);

			//Normal Movement
			Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);

			tm.transform.Translate(movement * speed, Space.World);

			}

		//Restricting movement when in shop and giving the player a way to leave shop
		if (upgrade_zone == true) {
			if (Input.GetKeyDown(KeyCode.S) == true || Input.GetKeyDown(KeyCode.A) == true || Input.GetKeyDown(KeyCode.W) == true || Input.GetKeyDown(KeyCode.D) == true) {

				upgrade_zone = false;
			}

		}

		//Temporary death option
		if (health <= 0) {

			gameObject.SetActive (false);
		}


		//Making so no rolling happens
		rb.velocity = new Vector3 (0f, 0f, 0f);

	}


	void OnTriggerEnter(Collider other)
	{
		//Handles picking up pickups dropped by enemies
		if (other.gameObject.CompareTag ("Pick Up")) 
		{
			other.gameObject.SetActive (false);

			//Check if the pickup is red, green, or blue and change count accordingly
			int pickup_color = other.GetComponent<PickupController>().color;

			if (pickup_color == 1)
				red_count++;
			if (pickup_color == 2)
				green_count++;
			if (pickup_color == 3)
				blue_count++;

			//Update the count text
			SetCountText ();
		}

		//See if the player is standing on the portal
		if (other.gameObject.CompareTag("Portal"))
		{
			on_portal = true;
		}

		//See if the player is in the Upgrade Shop Zone
		if (other.gameObject.CompareTag ("Upgrade Shop")) {

			upgrade_zone = true;
		}



	}

	void OnTriggerExit(Collider other)
	{
		//See if the player left the portal area
		if (other.gameObject.CompareTag ("Portal")) {

			on_portal = false;

		}
			

	}

	void OnCollisionEnter(Collision other)
	{
		//Did the player hit a wall?
		if (other.gameObject.tag == "Wall") {

			rb.velocity = new Vector3 (0f, 0f, 0f);

		}

		//If the player hits an enemy follower reduce health by 10
		if (other.gameObject.tag == "Follower") {

			health -= 10;

		}

		if (other.gameObject.tag == "Dasher") {

			health -= 10;

		}

	}

	void SetCountText()
	{
		//countText.text = "Count: " + count.ToString ();
		countText.text = "Red Count:" + red_count.ToString() + "\n" + "Green Count:" + green_count.ToString() + "\n" + "Blue Count:" + blue_count.ToString();
	}

	void SetHealthText()
	{
		healthText.text = "Health: " + health.ToString ();

	}

	/*
	string CheckScene()
	{
		current_scene = SceneManager.GetActiveScene ();
		string scene_name = current_scene.name;

		if (scene_name == "HubWorld") {

			HubWorld ();

		}

		return scene_name;

	}
	*/

	//See where the player is and set variables accordingly
	public string CheckLocation()
	{
		if (floor == 0) {

			HubWorld ();
			tm.position = new Vector3 (tm.position.x, 0.5f, tm.position.z);
			return "HubWorld";

		}

		if (floor == 1) {

			tm.position = new Vector3 (tm.position.x, 50.5f, tm.position.z);
			return "Floor1";
		}
			
		return null;

	}

	//Set health to 100 when in Hub World. ay be used to set other variables later.
	void HubWorld()
	{
		health = 100;

	}

	//Warp the player from hub World to Floor 1.
	void Warp()
	{
		//if (CheckScene () == "HubWorld") {

			//SceneManager.LoadScene ("MiniGame");

		//}

		if (floor == 0) {
			transform.position = new Vector3 (0f, 50.5f, 0f);
			floor = 1;
		}



	}

	//Slows down a variable if the player has the gun select menu open
	public float SlowDown(float tobeslowded)
	{
		if (select_on == true)
			return tobeslowded * reduce_fraction;
		if (select_on == false)
			return tobeslowded;
		else
			return 0f;
	}




}


