using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateOrbitMovement : MonoBehaviour {

	// Define objects used in animation: 
	public GameObject wasp12;
	public GameObject wasp12b; 
	public GameObject earth_planet; 
	public GameObject earth_moon; 
	public GameObject moon; 

	// Define movement parameters
	// Rotation params: 
	// private double wasp12_rotate_days = 24.0; 
	private double wasp12b_rotate_days = 1.0914203;
	private double earth_planet_rotate_days = (655.7/24);
	private double earth_moon_rotate_days = 27.3; 
	private double moon_rotate_days = 27.3; 

	// Orbit params: 
	private double wasp12b_orbit_days = 1.0914203; 
	private double earth_planet_orbit_days = 365.2; 
	private double earth_moon_orbit_days = 27.3;
	// true value --> 365.2; 
	private double moon_orbit_days = 27.3;
	// semi-major and semi-minor orbit distances for position calculations: 
	private double wasp12b_semimajor_dist = 1973.58; 
	private double wasp12b_semiminor_dist = 1973.58;
	private double earth_semimajor_dist = 86070.0; 
	private double earth_semiminor_dist = 86070.0;
	private double moon_semimajor_dist = 220.93; // error
	private double moon_semiminor_dist = 220.93; 
	private double earth_moon_semimajor_dist = 220.93;
	private double earth_moon_semiminor_dist = 220.93; 

	// set up counters for animation timesteps
	// use floats to reconcile fractions from PI
	// private double wasp12_rotate_time = 0.0; 
	private double wasp12b_rotate_time = 0.0; 
	private double earth_planet_rotate_time = 0.0; 
	private double earth_moon_rotate_time = 0.0; 
	private double moon_rotate_time = 0.0; 

	private double wasp12b_orbit_time = 0.0; 
	private double earth_planet_orbit_time = 0.0; 
	private double earth_moon_orbit_time = 0.0; 
	private double moon_orbit_time = 0.0; 
	 
	// Using scale_time: gets information from PlayerInteraction.cs... depends 
	// on whether mission 1 is running or not. 
	public double scale_time = PlayerInteraction.scale_time;  
	public bool reset = PlayerInteraction.reset; 

	// Define helper functions to find the x and z coordinates of orbiting objects
	// x = a * cos(theta); 
	public float Ellipse_x(double a, double theta) {
		//float rad = Mathf.Deg2Rad * (float)theta; 
		return (float)a * Mathf.Cos((float)theta); 
	}

	// z = - b * sin(theta);
	public float Ellipse_z(double b, double theta) {
		//float rad = Mathf.Deg2Rad * (float)theta; 
		return (float)b * Mathf.Sin((float)theta); 
	}

	void Start() {
		// Find GameObjects relevant for animation
	}

	// Define rotation and orbit motion w/ update method: 
	void Update() { 
		// Re-check params coming in from PlayerInteraction.cs: 
		scale_time = PlayerInteraction.scale_time; 
		reset = PlayerInteraction.reset; 

		if (reset) { 
			wasp12b.transform.position = new Vector3 ((float)wasp12b_semimajor_dist, 0.0f, 0.0f);
			earth_planet.transform.position = new Vector3 ((float)earth_semimajor_dist, 0.0f, 0.0f); 
			moon.transform.position = new Vector3 ((float)(moon_semimajor_dist + earth_semimajor_dist), 0.0f, 0.0f);
			earth_moon.transform.position = new Vector3 ((float)(earth_moon_semimajor_dist + wasp12b_semimajor_dist), 0.0f, 0.0f); 
		} else {
			// Orbit and Rotation: 
			// Each frame updated will step forward the solar system movements: 
			// set up counters for the current degree of rotation (in rad) of the objects
			// float delta_wasp12_rotate = 0; 
			float delta_wasp12b_rotate = 0; 
			float delta_earth_planet_rotate = 0;   
			float delta_earth_moon_rotate = 0;  
			float delta_moon_rotate = 0; 

			float delta_wasp12b_orbit = 0;  
			float delta_earth_planet_orbit = 0;  
			float delta_earth_moon_orbit = 0; 
			float delta_moon_orbit = 0; 

			// Rotates the appropriate system or planet by some angle defined in the params
			// Rotate and Orbit so each frame is equivalent to a second

			// WASP-12:	
			// Rotate: 
			// wasp12_rotate_time += (((Mathf.PI*2)/((24 * 60) * wasp12_rotate_days))/scale_time);
			// delta_wasp12_rotate = Mathf.Rad2Deg * (float)wasp12_rotate_time;   
			// wasp12.transform.Rotate(0, delta_wasp12_rotate, 0); 

			// WASP-12B:
			// Rotate:
			wasp12b_rotate_time = (((Mathf.PI * 2) / ((24 * 60) * wasp12b_rotate_days)) / scale_time);
			delta_wasp12b_rotate = -Mathf.Rad2Deg * (float)wasp12b_rotate_time;   
			wasp12b.transform.Rotate (0, delta_wasp12b_rotate, 0); 

			// Orbit: 
			wasp12b_orbit_time += (((Mathf.PI * 2) / ((24 * 60) * wasp12b_orbit_days)) / scale_time);
			delta_wasp12b_orbit = ((float)wasp12b_orbit_time % (2 * Mathf.PI)) / (2 * Mathf.PI); 
			float wasp12b_x = Ellipse_x (wasp12b_semimajor_dist, wasp12b_orbit_time); 
			float wasp12b_z = Ellipse_z (wasp12b_semiminor_dist, wasp12b_orbit_time); 
			wasp12b.transform.position = new Vector3 (wasp12b_x, 0, wasp12b_z);

			// EARTH_PLANET: 
			// Rotate
			earth_planet_rotate_time = (((Mathf.PI * 2) / ((24 * 60) * earth_planet_rotate_days)) / scale_time);
			delta_earth_planet_rotate = -Mathf.Rad2Deg * (float)earth_planet_rotate_time;   
			earth_planet.transform.Rotate (0, delta_earth_planet_rotate, 0); 

			// Orbit: 
			earth_planet_orbit_time += (((Mathf.PI * 2) / ((24 * 60) * earth_planet_orbit_days)) / scale_time);
			delta_earth_planet_orbit = ((float)earth_planet_orbit_time % (2 * Mathf.PI)) / (2 * Mathf.PI); 
			float earth_planet_x = Ellipse_x (earth_semimajor_dist, earth_planet_orbit_time); 
			float earth_planet_z = Ellipse_z (earth_semiminor_dist, earth_planet_orbit_time); 
			earth_planet.transform.position = new Vector3 (earth_planet_x, 0, earth_planet_z); 

			// MOON: 
			moon_rotate_time = (((Mathf.PI * 2) / ((24 * 60) * moon_rotate_days)) / scale_time); 
			delta_moon_rotate = -Mathf.Rad2Deg * (float)moon_rotate_time; 
			moon.transform.Rotate (0, delta_moon_rotate, 0); 

			// Orbit: 
			moon_orbit_time += (((Mathf.PI * 2) / ((24 * 60) * moon_orbit_days)) / scale_time);
			delta_moon_orbit = (float)(moon_orbit_time % (2 * Mathf.PI)) / (2 * Mathf.PI); 
			float moon_x = Ellipse_x (moon_semimajor_dist, moon_orbit_time); 
			float moon_z = Ellipse_z (moon_semiminor_dist, moon_orbit_time); 
			moon.transform.position = new Vector3 (moon_x + earth_planet_x, 0, moon_z + earth_planet_z);

			// EARTH_MOON: 
			// Rotate
			earth_moon_rotate_time = (((Mathf.PI * 2) / ((24 * 60) * earth_moon_rotate_days)) / scale_time);
			delta_earth_moon_rotate = -Mathf.Rad2Deg * (float)earth_moon_rotate_time;   
			earth_moon.transform.Rotate (0, delta_earth_moon_rotate, 0); 

			// Orbit: 
			earth_moon_orbit_time += (((Mathf.PI * 2) / ((24 * 60) * earth_moon_orbit_days)) / scale_time);
			delta_earth_moon_orbit = ((float)earth_moon_orbit_time % (2 * Mathf.PI)) / (2 * Mathf.PI); 

			float earth_moon_x = Ellipse_x (earth_moon_semimajor_dist, earth_moon_orbit_time); 
			float earth_moon_z = Ellipse_z (earth_moon_semiminor_dist, earth_moon_orbit_time); 
			earth_moon.transform.position = new Vector3 (earth_moon_x + wasp12b_x, 0, earth_moon_z + wasp12b_z); 
		}
	}
}


