using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour {

	private SteamVR_TrackedObject trackedObj; 
	public GameObject laserPrefab;
	private GameObject laser; 
	private Transform laserTransform; 
	private Vector3 hitPoint; 
	public LayerMask laserPointerMask; 
	private bool shouldShowPanel;
	private string objectHit; 

	public GameObject factFileCanvas; 
	public GameObject wasp12Panel;
	public GameObject wasp12bPanel; 
	public GameObject earthPanel;
	public GameObject moonPanel;
	public GameObject earthMoonPanel;

	public GameObject player; 

	// Create containers for six-part warp tunnel effect: 
	public GameObject WarpTunnel; 
	public ParticleSystem BlueWarp;
	public ParticleSystem BlueWarpF; 
	public ParticleSystem BlueWarpB;
	public ParticleSystem YellowWarp;
	public ParticleSystem YellowWarpF;
	public ParticleSystem YellowWarpB; 
	private bool warp; 

	// Hide planets during warp: 
	public GameObject wasp12;
	public GameObject wasp12b; 
	public GameObject earth_planet; 
	public GameObject earth_moon; 
	public GameObject moon;  

	private int warpFrameCount;
	private int warpFrameRemat; 
	private int warpFrameTotal;
	private int warpInitialFrameRemat;
	private int warpInitialFrameTotal;

	// vars to help determine player motion based on predfined distances
	//private bool use_wasp_step; // default is to use wasp sized steps  
	//private float wasp_step = 1973.58f;
	// private float earthmoon_step = 220.93f;

	public GameObject radarMap; 
	public GameObject leftModel;
	// private bool showMap; 

	// Run mission 1? 
	private bool warpMission1; 
	private bool mission1; 

	/* time scale for orbit/rotate - this variable is referred to in RotateOrbitMovement.cs
	 using the notation: PlayerInteraction.scale_time. */
	public static double scale_time = 1.0; 	
	/* reset used to shift the planets back to origin positions for mission 1 */ 
	public static bool reset = false; 

	// Array for points: 
	private int mission1_max_steps = 42; // counting 0 
	private float[ , ] mission1_steps = new float[42,3];
	private int mission1_curr_step; 

	// Out of date with split between two controllers: 
	/*
	 * private SteamVR_Controller.Device Controller {
		get { return SteamVR_Controller.Input ((int)trackedObj.index); }S
	}
	*/
	// Isolate Right and Left Controller Devices: 
	private SteamVR_Controller.Device RightDevice
	{
		get { return SteamVR_Controller.Input((int)SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost)); }
	}
	private SteamVR_Controller.Device LeftDevice
	{ 
		get { return SteamVR_Controller.Input((int)SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost)); }
	}

	/* Method shows laser, positions it between controller and the point the raycast should hit, 
	 * points the laser at the position, and scales the laser so it fits between the two points.
	 * From "HTC Vive Tutorial for Unity". */

	private void ShowLaser(RaycastHit hit) {
		laser.SetActive (true); 
		laserTransform.position = Vector3.Lerp (trackedObj.transform.position, hit.point, 0.5f); 
		laserTransform.LookAt (hitPoint); 
		laserTransform.localScale = new Vector3 (laserTransform.localScale.x,laserTransform.localScale.y, hit.distance); 
		shouldShowPanel = true; 
	}

	private void showPanel(string objectHitName) {
		/* Object names: 
		 * Wasp-12: Wasp12Sphere
		 * Wasp-12b: Wasp12bSphere
		 * Earth: Earth_Planet
		 * EarthMoon: Earth_Moon
		 * Moon: Moon
		 * 
		 * Object collision exterior names: 
		 * Wasp-12: Wasp12_Collide
		 * Wasp-12b: Wasp12b_Collide
		 * Earth: Earth_Collide
		 * EarthMoon: EarthMoon_Collide
		 * Moon: Moon_Collide
		 */
		Vector3 movePanel = player.transform.position; 
		factFileCanvas.transform.position = new Vector3 (movePanel.x - 600.0f, movePanel.y + 600.0f, movePanel.z); 

		if (objectHitName == "Wasp12_Collide" || objectHitName == "Wasp12Sphere") {
			wasp12Panel.SetActive(true);
		}
		if (objectHitName == "Wasp12b_Collide" || objectHitName == "Wasph12bSphere") {
			wasp12bPanel.SetActive(true); 
		}
		if (objectHitName == "Earth_Collide" || objectHitName == "Earth_Planet") {
			earthPanel.SetActive(true);
		}
		if (objectHitName == "EarthMoon_Collide" || objectHitName == "Earth_Moon") {
			earthMoonPanel.SetActive(true);
		}
		if (objectHitName == "Moon_Collide" || objectHitName == "Moon") {
			moonPanel.SetActive(true);
		}
	}

	private void hidePanel(string objectHitName) {
		// reset panel parameters
		shouldShowPanel = false; 
		objectHit = ""; 

		/* Object names: 
		 * Wasp-12: Wasp12Sphere
		 * Wasp-12b: Wasp12bSphere
		 * Earth: Earth_Planet
		 * EarthMoon: Earth_Moon
		 * Moon: Moon
		 * 
		 * Object collision exterior names: 
		 * Wasp-12: Wasp12_Collide
		 * Wasp-12b: Wasp12b_Collide
		 * Earth: Earth_Collide
		 * EarthMoon: EarthMoon_Collide
		 * Moon: Moon_Collide
		 */
		if (objectHitName == "Wasp12_Collide" || objectHitName == "Wasp12Sphere") {
			wasp12Panel.SetActive(false);
		}
		if (objectHitName == "Wasp12b_Collide" || objectHitName == "Wasph12bSphere") {
			wasp12bPanel.SetActive(false); 
		}
		if (objectHitName == "Earth_Collide" || objectHitName == "Earth_Planet") {
			earthPanel.SetActive(false);
		}
		if (objectHitName == "EarthMoon_Collide" || objectHitName == "Earth_Moon") {
			earthMoonPanel.SetActive(false);
		}
		if (objectHitName == "Moon_Collide" || objectHitName == "Moon") {
			moonPanel.SetActive(false);
		}
	}
		
	private void startMission1() {
		// update 'reset' parameter to move planets in RotateOrbitMovement.cs
		reset = true; 

		/* Populate array with preprogrammed "steps" through the 
		first mission: Start slightly to the side the Earth/Moon from the 
		point in approximate wasp-steps just behind re: the 42th step. Move 
		laterally with each step, so that when the user has completed
		the steps and is at x = y = 0, then the z coordinate will be equal
		to one step in the -z direction (45 degree angle formed with
		wasp12b when x = 0). */ 
		float dx = (86070.0f / 41.0f); // an approximation of the wasp-12b orbital radius
		// dx = 2099.27f
		float dz = (1977.58f / 41.0f); // scaled so initializes one moon radii above moon
		float x_init = 86070.0f; // max of 41  steps
		float z_init = -4.0f; 

		for (int step = 0; step < mission1_max_steps; step++) { 
			// Get current step # and use to set x and z for steps
			// y is 0 always. 
			mission1_steps [step, 0] = x_init - (step * dx); // x-coord
			mission1_steps [step, 1] = 0.0f; // y-coord 
			mission1_steps [step, 2] = z_init + (step * dz); // z-coord
			Debug.Log ("(" + mission1_steps [step, 0] + ", " + mission1_steps [step, 1] + ", " + mission1_steps [step, 2] + ")"); 
		}
		mission1_curr_step = 0; // Start at furthest point
		// Initializes player to outer reaches of space: 
		Vector3 firstStep = new Vector3 (mission1_steps [mission1_curr_step, 0], 
			                    mission1_steps [mission1_curr_step, 1], 
			                    mission1_steps [mission1_curr_step, 2]); 
		player.transform.position = firstStep; 
	}

	void Start() {
		// Hide radar map initially: 
		// showMap = false; 
		radarMap.SetActive (false);

		// Setup Hidden Fact File Panels for each object: 
		factFileCanvas.SetActive(true);
		wasp12Panel.SetActive(false);
		wasp12bPanel.SetActive(false); 
		earthPanel.SetActive(false);
		moonPanel.SetActive(false);
		earthMoonPanel.SetActive(false);

		trackedObj = GetComponent<SteamVR_TrackedObject> (); 
		laser = Instantiate (laserPrefab); 
		laserTransform = laser.transform; 
		objectHit = ""; 

		// Set up warp boolean: 
		warp = false;
		warpMission1 = false; 
		mission1 = false; 

		float fps = 1.0f /  Time.fixedDeltaTime; 
		Debug.Log (fps + " FPS"); 
		// FPS = 90
		warpInitialFrameRemat = (int) fps * 9; 
		warpInitialFrameTotal = (int) fps * 10; 
		warpFrameTotal = (int) fps * 3; 
		warpFrameRemat = (int) fps * 2; 
		warpFrameCount = 0; 

		// Position Player one WASP-12b orbital distance in -Y direction and
		// two orbital distances in the -X direction 
		player.transform.position = new Vector3 (- (2.0f * 1973.58f), -1973.58f, 0.0f);  
	}

	void Update () { // Called once per frame 
		/* Use left and right triggers to move through the preprogrammed 
		Mission 1 spots - the leftmost trigger indicates a move closer to 
		(0, 0, 0)/Wasp-12, while the rightmost trigger cues motion away
		from the origin, out towards the Earth. Each step is the size of one
		wasp-12 to wasp-12b orbital radii, and the steps begin just above the
		moon. Code based on: www.youtube.com/watch?v=Awr52z9Y670 

		Paricle System code based on tutorial: 
		www.youtube.com/watch?v=4hlCOUoc6aQ  */

		/* Pressing down on the left touchpad adjusts the boolean warpMission1, which
		launches the long warp animation out to the final destination. */ 
		if (LeftDevice.GetPressDown (SteamVR_Controller.ButtonMask.Touchpad) && !(mission1)) {
			warpMission1 = true; 
			WarpTunnel.transform.rotation = Quaternion.Euler (new Vector3 (0, -90, 0)); 
			WarpTunnel.transform.localPosition = new Vector3 (75, 0, 0);
			YellowWarpB.Play ();
			BlueWarpB.Play ();
			YellowWarp.Play (); 
			BlueWarp.Play ();
			BlueWarpF.Play ();
			YellowWarpF.Play ();
			wasp12.SetActive (false); 
			wasp12b.SetActive (false);  
			earth_planet.SetActive (false); 
			earth_moon.SetActive (false); 
			moon.SetActive (false);
		}

		// For the initial, Mission 1 Warp: 
		/* Use the current warpFrameCount (reset to 0 at the end of each 
		warp-step to determine when the animation should terminate. When 
		the value of warpInitialFrameRemat(erialize) is reached, make the contents
		of the system visible again. */ 
		if (warpFrameCount == warpInitialFrameRemat && warpMission1) {
			wasp12.SetActive (true); 
			wasp12b.SetActive (true);  
			earth_planet.SetActive (true); 
			earth_moon.SetActive (true); 
			moon.SetActive (true);
		}

		/* when the warpInitialFrameTotal value is reached, clear the animation, 
		reset the warp boolean to false, and reset the warpFrameCount to 0. Begin
		mission 1 using the startMission1() method. Set the mission1 Boolean to true
		so that the right controller touchpad will be enabled. */
		if (warpFrameCount == warpInitialFrameTotal & warpMission1) {
			warpMission1 = false;
			warpFrameCount = 0;
			mission1 = true; 

			startMission1 (); 
			BlueWarp.Pause ();
			BlueWarpF.Pause ();
			BlueWarpB.Pause ();
			YellowWarp.Pause (); 
			YellowWarpF.Pause (); 
			YellowWarpB.Pause (); 
			BlueWarp.Clear ();
			BlueWarpF.Clear ();
			BlueWarpB.Clear ();
			YellowWarp.Clear (); 
			YellowWarpF.Clear (); 
			YellowWarpB.Clear (); 
		}

		/* When the warp animation is running (e.g. warp == true), 
		update the warpFrameCount by one. */ 
		if (warpMission1) {
			warpFrameCount++; 
		}


		/* Pressing down on the Touchpad of the Right Device cues the warp tunnel
		effect to begin running. The animation and movement will only run when the boolean
		warp is set to false (re: the warp animation is not currently running) and the 
		mission step being taken is within the range predefined with the mission1_steps array. */
		if (RightDevice.GetPressDown (SteamVR_Controller.ButtonMask.Touchpad) && mission1) {
			float axisY = RightDevice.GetAxis ().y; 

			if (warp == false && (mission1_curr_step < (mission1_max_steps - 1)) && (mission1_curr_step >= 0)) {
				if (axisY < 0) {
					WarpTunnel.transform.rotation = Quaternion.Euler (new Vector3 (0, -90, 0)); 
					WarpTunnel.transform.localPosition = new Vector3 (75, 0, 0); 
				}
				if (axisY > 0) {
					WarpTunnel.transform.rotation = Quaternion.Euler (new Vector3 (0, 90, 0)); 
					WarpTunnel.transform.localPosition = new Vector3 (-75, 0, 0); 
				}
				warp = true;
				YellowWarpB.Play ();
				BlueWarpB.Play ();
				YellowWarp.Play (); 
				BlueWarp.Play ();
				BlueWarpF.Play ();
				YellowWarpF.Play ();
				wasp12.SetActive (false); 
				wasp12b.SetActive (false);  
				earth_planet.SetActive (false); 
				earth_moon.SetActive (false); 
				moon.SetActive (false); 
			}
		}
	
		/* Pressing down on the Touchpad of the Right Device moves the Player in space
		to the next mission step. By checking the y-value corresponding to where on the 
		unit circle of the touchpad the user pressed, the direction of motion is determined; 
		a positive y value indicates a -X step (towards Wasp-12) and a negative y value indicates
		a +X step (towards Earth). The next location of the Player is accessed from the 
        Mission1_steps array using the value of the current step, which is updated after it is 
        confirmed to be in the valid ranges (not less than one or greater than one less 
        than the maximum step #). */
		if (RightDevice.GetPressUp (SteamVR_Controller.ButtonMask.Touchpad) && mission1) {
			if (RightDevice.GetAxis ().y > 0) {
				Debug.Log ("Step -X"); 
				if (mission1_curr_step < (mission1_max_steps - 1)) { 
					mission1_curr_step += 1; 
					Vector3 step_negative = new Vector3 (mission1_steps [mission1_curr_step, 0], 
                                                         mission1_steps [mission1_curr_step, 1], 
                                                         mission1_steps [mission1_curr_step, 2]); 
					Debug.Log ("At step number " + mission1_curr_step + "going to point: ("
                               + mission1_steps [mission1_curr_step, 0] + ", "
                               + mission1_steps [mission1_curr_step, 1] + ", "
                               + mission1_steps [mission1_curr_step, 2] + ")");
					player.transform.position = step_negative; 
				} else { 
					Debug.Log ("Step out of bounds: current step is " + mission1_curr_step); 
				}
			}

			// Step towards Earth (pos. x), (pos. y)
			// cued by pressing the lower half of the touchpad: 
			else if (RightDevice.GetAxis ().y < 0) {
				Debug.Log ("Step +X"); 
				if (mission1_curr_step > 0) {
					mission1_curr_step -= 1;
					Vector3 step_positive = new Vector3 (mission1_steps [mission1_curr_step, 0], 
                                                         mission1_steps [mission1_curr_step, 1], 
                                                         mission1_steps [mission1_curr_step, 2]); 
					Debug.Log ("At step number " + mission1_curr_step + "going to point: ("
					+ mission1_steps [mission1_curr_step, 0] + ", "
					+ mission1_steps [mission1_curr_step, 1] + ", "
					+ mission1_steps [mission1_curr_step, 2] + ")");
					player.transform.position = step_positive; 
				} else { 
					Debug.Log ("Step out of bounds: current step is " + mission1_curr_step); 
				}
			}	
		}
			
		/* Use the current warpFrameCount (reset to 0 at the end of each 
		warp-step to determine when the animation should terminate. When 
		the value of warpFrameRemat(erialize) is reached, make the contents
		of the system visible again. */ 
		if (warpFrameCount == warpFrameRemat && warp) {
			wasp12.SetActive (true); 
			wasp12b.SetActive (true);  
			earth_planet.SetActive (true); 
			earth_moon.SetActive (true); 
			moon.SetActive (true);
		}
					
		/* when the warpFrameTotal value is reached, clear the animation, 
		reset the warp boolean to false, and reset the warpFrameCount to 0. */
		if (warpFrameCount == warpFrameTotal && warp) {
			warp = false;
			warpFrameCount = 0;
			BlueWarp.Pause ();
			BlueWarpF.Pause ();
			BlueWarpB.Pause ();
			YellowWarp.Pause (); 
			YellowWarpF.Pause (); 
			YellowWarpB.Pause (); 
			BlueWarp.Clear ();
			BlueWarpF.Clear ();
			BlueWarpB.Clear ();
			YellowWarp.Clear (); 
			YellowWarpF.Clear (); 
			YellowWarpB.Clear (); 
		}

		/* When the warp animation is running (e.g. warp == true), 
		update the warpFrameCount by one. */ 
		if (warp) {
			warpFrameCount++; 
		}

		// WORK IN PROGRESS: TO BE COMPLETED --> issue with how the icons appear on map
		// Only show the radar/celestial map when the left trigger is being pressed
		/* Remove this block comment to enable: 
		if (LeftDevice.GetHairTriggerDown ()) {
			showMap = true; 
			leftModel.SetActive (false);
			radarMap.SetActive (true); 
		}

		if (LeftDevice.GetHairTriggerUp () && showMap) { 
			showMap = false; 
			radarMap.SetActive (false); 
			leftModel.SetActive (true); 
		}
		*/

		/* When the touchpad on the controller is depressed, shoot a ray from the controller. 
		 * If the ray "hits" something, store that point and release a laser pulse in that 
         * direction. Until the trigger is released, use the showPanel() method to show the  
         * relevant informational panel over and down the negative X-axis from the user. The user 
         * will look up at a 45 degree angle to read the panel. The hidePanel() method is called  
         * when the trigger is released. */ 
		if (RightDevice.GetHairTriggerDown()) {
			RaycastHit hit;

        // Show laser when user has found an object in the teleportation 
        // layer that they can navigate to - press while finding object/showing laser, 
        // then release to teleport. Releases a pulse of laser in the proper direction if 
        // target is found!
		
			if (Physics.Raycast (trackedObj.transform.position, transform.forward, out hit, Mathf.Infinity, laserPointerMask)) { 
				hitPoint = hit.point; 
				ShowLaser (hit); 
				shouldShowPanel = true; 
				Debug.Log (hit.collider.gameObject.name);
				objectHit = hit.collider.gameObject.name; 
				showPanel (objectHit); 
				Debug.Log ("showing panel for " + objectHit);  
			    }	
		    } 

		if (RightDevice.GetHairTriggerUp() && shouldShowPanel) { 
			Debug.Log ("hiding panel for " + objectHit); 
			hidePanel (objectHit);
			laser.SetActive (false); // show laser until trigger is released 
		}
	}
}

