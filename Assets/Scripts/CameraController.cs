using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public GameObject player;
	public PlayerController player_control;

	//public Camera main_camera;

	//Where to move the camera to when the player is in the upgrade shop
	public GameObject upgrade_shop_camera_point;

	//private bool transition_shop;

	//Has the camera transitioned back to following the player?
	private bool transition_player;

	//How fast does it move and rotate?
	public float moveSpeed;
	public float rotateSpeed;

	//How far back from the player the camera is
	private Vector3 offset;

	// Use this for initialization
	void Start () {
		offset = transform.position - player.transform.position;

		player_control = player.GetComponent<PlayerController>();

		moveSpeed = 20f;
		rotateSpeed = 80f;

	}
	
	// LateUpdate is called once per frame but is for sure going to be called after all other proccesses are finished
	void LateUpdate () {

		//transistion back to player if the player leaves the upgrade shop
		if (player_control.upgrade_zone == false && transition_player == false) {
			transform.position = Vector3.MoveTowards (transform.position, player.transform.position + offset, moveSpeed * Time.deltaTime);//used to be 0.25f without Time.deltaTime
			transform.rotation = Quaternion.RotateTowards (transform.rotation, Quaternion.Euler (45f, 0f, 0f), rotateSpeed * Time.deltaTime);//1f

			//if it has made it to back to following the player then don't move the camera anymore.
			if (transform.position == player.transform.position + offset && transform.rotation == Quaternion.Euler (45f, 0f, 0f)) {
				transition_player = true;
			}
		}

		//follow the player once the camera has made it back to the player.
		if (transition_player == true)
			transform.position = player.transform.position + offset;

		//move to upgrade shop position if the player has enetered the upgrade shop zone.
		if (player_control.upgrade_zone == true) {
			transition_player = false;
			transform.position = Vector3.MoveTowards (transform.position, upgrade_shop_camera_point.transform.position, moveSpeed * Time.deltaTime);
			transform.rotation = Quaternion.RotateTowards (transform.rotation, upgrade_shop_camera_point.transform.rotation, rotateSpeed * Time.deltaTime);

		}	




	}
}
