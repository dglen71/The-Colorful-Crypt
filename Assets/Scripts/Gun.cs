using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour {

	//Class created specifically to handle all of the variables associated with the guns
	//What number is the gun?
	public int gun_number;
	//Has the gun been slected?
	public bool selected;

	//what is the value to be used for coloring purposes of the gun?
	public float red_value;
	public float green_value;
	public float blue_value;

	//Since colors are weird these values hold the values to be displayed on UI and when calculating damage. 
	//(Weird meaning I have to start at white and transition by decreasing the other two color values)
	public float red_UI_value;
	public float green_UI_value;
	public float blue_UI_value;

	//The total UI value. 
	public float total_value;

	//Renderer of gun
	public Renderer rend;

	//Button used in player's gun select menu
	public Button button;

	public GameObject player;
	private PlayerController player_control;

	// Use this for initialization
	void Start () {

		player = GameObject.Find ("Player");
		player_control = player.GetComponent<PlayerController> ();

		red_UI_value = 0;
		green_UI_value = 0;
		blue_UI_value = 0;
	}
	
	// Update is called once per frame
	void Update () {

		//This doesn't work for some reason. Have to update in shop Controller instead. Maybe because not all guns are active?
		total_value = red_UI_value + green_UI_value + blue_UI_value;
		
	}

	//Handles the switching of guns when the gun's button is pressed on the gun select menu
	public void OnClick()
	{

		selected = true;
		for (int i = 0; i < player_control.guns.Length; i++) {

			if (i + 1 != gun_number)
				player_control.guns [i].selected = false;

			//Code that is not in Player controller. Needs to be there to handle intial gun selection. Maybe changed later?
			/*
			if (player_control.guns [i].selected == true && player_control.guns[i].gameObject.activeSelf == false) {

				player_control.guns [i].gameObject.SetActive (true);
				player_control.gun_selected = player_control.guns [i].gun_number;

			}
			if (player_control.guns [i].selected == false && player_control.guns [i].gameObject.activeSelf != false) {

				player_control.guns [i].gameObject.SetActive (false);

			}
			*/

		}

	}
}
