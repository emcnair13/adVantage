using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialMap : MonoBehaviour {

	// Get GameObjects for celestial objects (+ player), and for 
	// map counterparts: 
	public GameObject wasp12;
	public GameObject wasp12b; 
	public GameObject earth_planet; 
	public GameObject earth_moon; 
	public GameObject moon;
	public GameObject player;

	public GameObject wasp12_map;
	public GameObject wasp12b_map; 
	public GameObject earth_planet_map; 
	public GameObject earth_moon_map; 
	public GameObject moon_map;
	public GameObject player_map; 

	// Use this for initialization
	void Start () {
	}

	// helper method to handle system to map conversions: 
	private void convert_system2map(GameObject planet, GameObject mapped, float local_y) {
		// with the base assumption that each wasp step (see below) 
		// corresponds with 0.02 of the map: 
		float x_unscaled; 
		float z_unscaled;
		float x_convert; 
		float z_convert;
		// Basic conversion from celestial coords. to map coords. 
		float wasp_dist = (86070.0f / 41.0f);
		float base_scale = 0.02f;

		// convert based on system params: approx. 36 distances 
		// wasp12 to wasp12b from wasp12 out to moon - scaled: 2452 units
		// equal to 4261839.4 km. (see converstion excel doc. for details)
		Transform planet_transform = planet.transform; 
		Transform mapped_transform = mapped.transform;

		// Get x and z coords scaled from space --> map w/o scale
		// for ring conversion:
		x_unscaled = planet_transform.position.x; 
		z_unscaled = planet_transform.position.z; 
		x_convert = (x_unscaled / wasp_dist) * base_scale;
		z_convert = (z_unscaled / wasp_dist) * base_scale;

		// reset the x and z coordinates of the mapped param based on the 
		// current position of its planet counterpart; y stays the same;
		// params have been scaled to fit the map adjustments (for legibility) 
		// Same call made in the if cases below: 
		mapped_transform.localPosition = new Vector3 (x_convert, local_y, z_convert);
		Debug.Log (planet + " x: " + x_unscaled + " --> " + x_convert + " z: " + z_unscaled + " --> " + z_convert); 
	
	}

	// Update is called once per frame
	void Update () {
		// Call helper method to convert coordinates for all mapped step w/ each
		// movement of the objects in space + player: 
		//convert_system2map (wasp12, wasp12_map, 0.01f); 
		convert_system2map (wasp12b, wasp12b_map, 0.01f); 
		convert_system2map (earth_moon, earth_moon_map, 0.012f); 
		convert_system2map (earth_planet, earth_planet_map, 0.01f); 
		convert_system2map (moon, moon_map, 0.01f); 
		convert_system2map (player, player_map, 0.02f); 
	}
}

