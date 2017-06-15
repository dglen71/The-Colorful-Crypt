using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour {

	//Gets the player and the guns array from the player.
	public PlayerController player;
	private Gun[] guns;

	//Guns that are displayed in the shop.
	public GameObject[] shop_guns;

	//UI sliders to change the with UI values of the guns.
	public Slider red_slider;
	public Slider green_slider;
	public Slider blue_slider;

	//UI Text number values to change with UI values of the guns.
	public Text red_text;
	public Text green_text;
	public Text blue_text;

	//Which gun has been selected to upgrade?
	private int gun_selected;


	// Use this for initialization
	void Start () {

		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController>();
		guns = player.guns;

		//Find the shop Guns
		shop_guns = new GameObject[guns.Length];
		shop_guns [0] = GameObject.Find ("Cylinder Gun Shop");
		shop_guns [1] = GameObject.Find ("Cube Gun Shop");
		shop_guns [1].gameObject.SetActive (false);
		shop_guns [2] = GameObject.Find ("Capsule Gun Shop");
		shop_guns [2].gameObject.SetActive (false);

		//red_slider = GameObject.Find ("Red Slider");
		//gun1_rend = GameObject.FindGameObjectWithTag ("player").GetComponentInChildren<Renderer> ();

		/*
		player.red1_UI = 0;
		player.green1_UI = 0;
		player.blue1_UI = 0;
		*/
		//gun1_shop_rend = gun1_shop.GetComponent<Renderer> ();
		//gun2_shop_rend = gun2_shop.GetComponent<Renderer> ();
		//gun3_shop_rend = gun3_shop.GetComponent<Renderer> ();

		//what is the starting gun selected to be upgraded?
		gun_selected = 1;
		
	}
	
	// Update is called once per frame
	void Update () {

		//red1 = player.red_gun1_value / 100f;
		//green1 = player.green_gun1_value / 100f;
		//blue1 = player.blue_gun1_value / 100f;

		/*
		total_gun1_value = player.red1_UI + player.green1_UI + player.blue1_UI;
		total_gun2_value = player.red2_UI + player.green2_UI + player.blue2_UI;
		total_gun3_value = player.red3_UI + player.green3_UI + player.blue3_UI;
		*/

		//Update slider values
		red_slider.value = guns[gun_selected - 1].red_UI_value / 100f;
		green_slider.value = guns[gun_selected - 1].green_UI_value / 100f;
		blue_slider.value = guns[gun_selected - 1].blue_UI_value / 100f;

		//Upgrade text number values
		red_text.text = guns[gun_selected - 1].red_UI_value.ToString ();
		green_text.text = guns[gun_selected - 1].green_UI_value.ToString ();
		blue_text.text = guns[gun_selected - 1].blue_UI_value.ToString ();

		//Change the total UI values of the guns. Must be done every frame. Must be done outside of gun class for some reason?
		for (int i = 0; i < guns.Length; i++) {

			guns [i].total_value = guns [i].red_UI_value + guns [i].green_UI_value + guns [i].blue_UI_value;

		}


	}

	//Handles the cycling through the guns when the next gun button is pressed.
	public void NextGun()
	{
		for (int i = 0; i < shop_guns.Length; i++) {

			if (gun_selected == i + 1) {
				if (gun_selected < shop_guns.Length) {
					gun_selected++;
					shop_guns [i].gameObject.SetActive (false);
					shop_guns [i + 1].gameObject.SetActive (true);
					break;
				} else if (gun_selected == shop_guns.Length) {
					gun_selected = 1;
					shop_guns [i].gameObject.SetActive (false);
					shop_guns [0].gameObject.SetActive (true);
					break;
				}
					
			}

		}

	}

	//Adds a red value to the current gun selected.
	public void AddRedGun()
	{

			for (int i = 0; i < guns.Length; i++) {

				if (player.red_count > 0 && gun_selected == i + 1 && guns [i].total_value < 100) {

				player.red_count--;
				guns [i].red_UI_value++;
				guns [i].green_value--;
				guns [i].blue_value--;

				guns [i].rend.material.color = new Color (guns [i].red_value / 100f, guns [i].green_value / 100f, guns [i].blue_value / 100f);
				shop_guns[i].GetComponent<Renderer>().material.color = guns [i].rend.material.color;

				}



			}
			
	}

	//Removes red value from the current gun selected. Gives you the value spent back. May change later?
	public void RemoveRedGun()
	{

		for (int i = 0; i < guns.Length; i++) {

			if (guns [i].red_UI_value > 0 && gun_selected == i + 1) {

				player.red_count++;
				guns [i].red_UI_value--;
				guns [i].green_value++;
				guns [i].blue_value++;

				guns [i].rend.material.color = new Color (guns [i].red_value / 100f, guns [i].green_value / 100f, guns [i].blue_value / 100f);
				shop_guns[i].GetComponent<Renderer>().material.color = guns [i].rend.material.color;


			}

		}

	}

	public void AddGreenGun()
	{


		for (int i = 0; i < guns.Length; i++) {

			if (player.green_count > 0 && gun_selected == i + 1 && guns [i].total_value < 100) {
				
				player.green_count--;
				guns [i].green_UI_value++;
				guns [i].red_value--;
				guns [i].blue_value--;

				guns [i].rend.material.color = new Color (guns [i].red_value / 100f, guns [i].green_value / 100f, guns [i].blue_value / 100f);
				shop_guns[i].GetComponent<Renderer>().material.color = guns [i].rend.material.color;


			}



		}

	}

	public void RemoveGreenGun()
	{


		for (int i = 0; i < guns.Length; i++) {

			if (guns [i].green_UI_value > 0 && gun_selected == i + 1) {

				player.green_count++;
				guns [i].green_UI_value--;
				guns [i].red_value++;
				guns [i].blue_value++;

				guns [i].rend.material.color = new Color (guns [i].red_value / 100f, guns [i].green_value / 100f, guns [i].blue_value / 100f);
				shop_guns[i].GetComponent<Renderer>().material.color = guns [i].rend.material.color;


			}

		}

	}

	public void AddBlueGun()
	{

		for (int i = 0; i < guns.Length; i++) {

			if (player.blue_count > 0 && gun_selected == i + 1 && guns [i].total_value < 100) {

				player.blue_count--;
				guns [i].blue_UI_value++;
				guns [i].red_value--;
				guns [i].green_value--;

				guns [i].rend.material.color = new Color (guns [i].red_value / 100f, guns [i].green_value / 100f, guns [i].blue_value / 100f);
				shop_guns[i].GetComponent<Renderer>().material.color = guns [i].rend.material.color;


			}



		}

	}

	public void RemoveBlueGun()
	{

		for (int i = 0; i < guns.Length; i++) {

			if (guns [i].blue_UI_value > 0 && gun_selected == i + 1) {

				player.blue_count++;
				guns [i].blue_UI_value--;
				guns [i].green_value++;
				guns [i].red_value++;

				guns [i].rend.material.color = new Color (guns [i].red_value / 100f, guns [i].green_value / 100f, guns [i].blue_value / 100f);
				shop_guns[i].GetComponent<Renderer>().material.color = guns [i].rend.material.color;


			}

		}

	}
}
