using UnityEngine;
using System.Collections;

public class LookAtMouse : MonoBehaviour
{
	//THIS CODE WAS TAKEN FROM UNITY WIKI PAGE BUT SOME SLIGHT ADJUSTMENTS WERE MADE TO HELP WITH SLOW DOWN EFFECT

	// speed is the rate at which the object will rotate
	public float speed;
	private float normal_speed;

	public GameObject player;
	private PlayerController player_control;

	void Start()
	{
		normal_speed = speed;
		player = GameObject.Find ("Player");
		player_control = player.GetComponent<PlayerController> ();
	}

	void FixedUpdate () 
	{
		//Reduce rotation speed if the Gun Select Menu is open.
		/*
		if (player_control.select_on == true)
			speed = normal_speed * player_control.reduce_fraction;
		if (player_control.select_on == false)
			speed = normal_speed;
		*/
		speed = player_control.SlowDown (normal_speed);


		// Generate a plane that intersects the transform's position with an upwards normal.
		Plane playerPlane = new Plane(Vector3.up, transform.position);

		// Generate a ray from the cursor position
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

		// Determine the point where the cursor ray intersects the plane.
		// This will be the point that the object must look towards to be looking at the mouse.
		// Raycasting to a Plane object only gives us a distance, so we'll have to take the distance,
		//   then find the point along that ray that meets that distance.  This will be the point
		//   to look at.
		float hitdist = 0.0f;
		// If the ray is parallel to the plane, Raycast will return false.
		if (playerPlane.Raycast (ray, out hitdist)) 
		{
			// Get the point along the ray that hits the calculated distance.
			Vector3 targetPoint = ray.GetPoint(hitdist);

			// Determine the target rotation.  This is the rotation if the transform looks at the target point.
			Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);

			// Smoothly rotate towards the target point.
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
		}
	}
}