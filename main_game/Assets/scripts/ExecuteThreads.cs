﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class ExecuteThreads : MonoBehaviour {

	// --- IMAGE SIMULATION ---

	public GameObject simulationImagePrefab;
	public GameObject simulationErrorPrefab;
	public GameObject layoutPanel;
	public GameObject layoutPanel2;
	public Text stepsIndicator;

	public Sprite dogSprite;
	public Sprite dogSprite2;
	public Sprite workerSprite;
	public Sprite workerSprite2;
	public Sprite displayErrorSprite;
	public Sprite[] itemsSprites;
	public Sprite[] actionsSprites;

	// ------------------------

	ToolboxManager manager;
	GameObject disablePanel;
	ProgressBar bar;
	ScrollRect simulationScrollRect;

	public GameObject runButton;
	public GameObject stopButton;

	public bool level2;
	public bool level3;

	private Timer timer;
	private int numActions;
	private string toPrint;
	//get simulation space for printing
	public Text simulationTextArea;
	// public 
	//get instance of GenerateTasks
	public GenerateTasks genTasks;
	//Task t; //"playerTank"

//	Transform[] blocks;
	Transform[] blocks_t1;
	Transform[] blocks_t2;

	bool stop;
	bool err;
	bool paused;
	bool lost;

	// bool t1_has_dog;
	// bool t2_has_dog;

	bool t1_checkedin;
	bool t1_checkedout;
	bool t1_has_brush;
	bool t1_has_clippers;
	bool t1_has_conditioner;
	bool t1_has_dryer;
	bool t1_has_scissors;
	bool t1_has_shampoo;
	bool t1_has_station;
	bool t1_has_towel;

	bool t2_checkedin;
	bool t2_checkedout;
	bool t2_has_brush;
	bool t2_has_clippers;
	bool t2_has_conditioner;
	bool t2_has_dryer;
	bool t2_has_scissors;
	bool t2_has_shampoo;
	bool t2_has_station;
	bool t2_has_towel;

	bool t1_needs_cut;
	bool t1_needs_dry;
	bool t1_needs_wash;
	bool t1_needs_groom;
	bool t1_did_cut;
	bool t1_did_dry;
	bool t1_did_wash;
	bool t1_did_groom;

	bool t2_needs_cut;
	bool t2_needs_dry;
	bool t2_needs_wash;
	bool t2_needs_groom;
	bool t2_did_cut;
	bool t2_did_dry;
	bool t2_did_wash;
	bool t2_did_groom;

	string returnErrMsg = "\n> ERROR: You are trying to return a resource you don't have.";
	string acquireErrMsg = "\n> ERROR: You are trying to acquire a resource you already have.";

	void Start() {

		// t1_has_dog = false;
		// t2_has_dog = false;

		stop = false;
		err = false;
		paused = false;
		lost = false;

		t1_checkedin = false;
		t1_checkedout = false;
		t2_checkedin = false;
		t2_checkedout = false;

		t1_has_brush = false;
		t1_has_clippers = false;
		t1_has_conditioner = false;
		t1_has_dryer = false;
		t1_has_scissors = false;
		t1_has_shampoo = false;
		t1_has_station = false;
		t1_has_towel = false;

		t2_has_brush = false;
		t2_has_clippers = false;
		t2_has_conditioner = false;
		t2_has_dryer = false;
		t2_has_scissors = false;
		t2_has_shampoo = false;
		t2_has_station = false;
		t2_has_towel = false;

		manager = GameObject.Find ("_SCRIPTS_").GetComponent<ToolboxManager> ();
		timer = GameObject.FindObjectOfType<Timer> ();
		disablePanel = GameObject.Find ("DisablePanel");
		bar = GameObject.Find ("RadialProgressBar").GetComponent<ProgressBar>();
		simulationScrollRect = simulationTextArea.transform.parent.gameObject.GetComponent<ScrollRect>();

		try { 
			disablePanel.SetActive (false);
		} catch {
			Debug.Log ("Disable Panel can't be found.");
		}
	}


	// USED FOR LEVEL 1 ONLY 

	public void Execute_SingleThread() {

		t1_did_cut = false;
		t1_did_dry = false;
		t1_did_wash = false;
		t1_did_groom = false;

		// ----- SET UP FOR BRUCE, CUSTOMER FOR LEVEL 1 -----
		t1_needs_cut = true;
		t1_needs_dry = false;
		t1_needs_wash = false;
		t1_needs_groom = false;

		// ------ START EXECUTE THREADS -------
		simulationTextArea.text = "";
		clearVerticalLayout ();
		clearVerticalLayout2 ();

		try {
			GameObject.Find("InformationPanel").SetActive(false);
		} catch {}

		try {

			GameObject.Find("AgendaPanel").SetActive(false);
		} catch { }

		stop = false;
		err = false;
		paused = false;
		lost = false;

		t1_checkedin = false;
		t1_checkedout = false;
		t2_checkedin = false;
		t2_checkedout = false;

		t1_has_brush = false;
		t1_has_clippers = false;
		t1_has_conditioner = false;
		t1_has_dryer = false;
		t1_has_scissors = false;
		t1_has_shampoo = false;
		t1_has_station = false;
		t1_has_towel = false;


		// NOT USED 
		t2_has_brush = false;
		t2_has_clippers = false;
		t2_has_conditioner = false;
		t2_has_dryer = false;
		t2_has_scissors = false;
		t2_has_shampoo = false;
		t2_has_station = false;
		t2_has_towel = false;


		try {
			// disable all other functionalities
			disablePanel.SetActive (true);
		} catch {
			Debug.Log ("Cannot enable DisablePanel");
		}

		// switch to stop button
		runButton.transform.SetAsFirstSibling ();

	
		// ------------------------ READING TAB 1 ------------------------

		int thread1_whilesChildren = 0;

		// retrieving the objects (blocks) current in thread 1
		blocks_t1 = GetActionBlocks_MultiThreads ("1");
	
		// this structure will store the text lines to display
		List<string> blocks_names_t1 = new List<string> ();
		List<GameObject> simulationImagesToDisplay = new List<GameObject>();

		int i = 0;
		// bool isError = false;

		foreach (Transform child in blocks_t1) {

			if (child.GetComponent<Draggable> ().typeOfItem == Draggable.Type.ACTION) {

				//Debug.Log ("TYPE ACTION");


				if (blocks_t1 [i].transform.GetComponentInChildren<Text> ().text == "get") {

					string resource = blocks_t1 [i].transform.Find ("Dropdown").Find ("Label").GetComponent<Text> ().text;

					if (resource == "[null]") {
						terminateSimulation ();
						manager.showError ("Please select a resource to acquire.");
						return;

					} else {

						blocks_names_t1.Add ("acquire ( " + resource + " );\n");
						i++;

						// create new object from prefab
						GameObject newItem = Instantiate(simulationImagePrefab) as GameObject;
						newItem.transform.FindChild ("Icon").GetComponent<Image> ().sprite = workerSprite;
						newItem.transform.FindChild ("AcqRet").GetComponent<Image> ().sprite = actionsSprites [0];

						Sprite item;

						if (resource == "brush")
							item = itemsSprites [0];
						else if (resource == "clippers")
							item = itemsSprites [1];
						else if (resource == "cond.")
							item = itemsSprites [2];
						else if (resource == "dryer")
							item = itemsSprites [3];
						else if (resource == "scissors")
							item = itemsSprites [4];
						else if (resource == "shampoo")
							item = itemsSprites [5];
						else if (resource == "station")
							item = itemsSprites [6];
						else if (resource == "towel")
							item = itemsSprites [7];
						else
							item = displayErrorSprite;
						
						newItem.transform.FindChild ("ItemAction").GetComponent<Image> ().sprite = item;
						newItem.transform.FindChild ("ActionText").GetComponent<Text>().text = "get(" + resource + ");";
						simulationImagesToDisplay.Add (newItem);

					}

				} else if(blocks_t1 [i].transform.GetComponentInChildren<Text> ().text == "ret") {

					string resource = blocks_t1 [i].transform.Find ("Dropdown").Find ("Label").GetComponent<Text> ().text;

					if (resource == "[null]") {
						terminateSimulation ();
						manager.showError ("Please select a resource to return.");
						return;
					} else {

						blocks_names_t1.Add ("return ( " + resource + " );\n");
						i++;

						// create new object from prefab
						GameObject newItem = Instantiate(simulationImagePrefab) as GameObject;
						newItem.transform.FindChild ("Icon").GetComponent<Image> ().sprite = workerSprite;
						newItem.transform.FindChild ("AcqRet").GetComponent<Image> ().sprite = actionsSprites [1];

						Sprite item;

						if (resource == "brush")
							item = itemsSprites [0];
						else if (resource == "clippers")
							item = itemsSprites [1];
						else if (resource == "cond.")
							item = itemsSprites [2];
						else if (resource == "dryer")
							item = itemsSprites [3];
						else if (resource == "scissors")
							item = itemsSprites [4];
						else if (resource == "shampoo")
							item = itemsSprites [5];
						else if (resource == "station")
							item = itemsSprites [6];
						else if (resource == "towel")
							item = itemsSprites [7];
						else
							item = displayErrorSprite;

						newItem.transform.FindChild ("ItemAction").GetComponent<Image> ().sprite = item;
						newItem.transform.FindChild ("ActionText").GetComponent<Text>().text = "return(" + resource + ");";
						simulationImagesToDisplay.Add (newItem);

					}

				} else {

					String action = blocks_t1 [i].transform.GetComponentInChildren<Text> ().text;

					//blocks_names_t1 [i] = "[thread 1] " + blocks_t1 [i].transform.GetComponentInChildren<Text> ().text + ";";
					blocks_names_t1.Add (action + ";\n");
					i++;

					GameObject newItem = Instantiate (simulationImagePrefab) as GameObject;


					if (action == "checkin") {
						
						// Debug.Log ("CHECKING IN");

						newItem.transform.FindChild ("Icon").GetComponent<Image> ().sprite = workerSprite;
						newItem.transform.FindChild ("ItemAction").GetComponent<Image> ().sprite = dogSprite;
//						newItem.transform.FindChild ("ActionText").GetComponent<Text> ().text = action + ";";
						newItem.transform.FindChild ("AcqRet").GetComponent<Image> ().sprite = actionsSprites [0];

					} else if (action == "checkout") {

						//Debug.Log ("CHECKING OUT");

//						GameObject newItem = Instantiate (simulationImagePrefab) as GameObject;
						newItem.transform.FindChild ("Icon").GetComponent<Image> ().sprite = workerSprite;
						newItem.transform.FindChild ("ItemAction").GetComponent<Image> ().sprite = dogSprite;
//						newItem.transform.FindChild ("ActionText").GetComponent<Text> ().text = action + ";";
						newItem.transform.FindChild ("AcqRet").GetComponent<Image> ().sprite = actionsSprites [1];

					} else {

						// create new object from prefab (single action)
//						GameObject newItem = Instantiate (simulationImagePrefab) as GameObject;
						newItem.transform.FindChild ("Icon").GetComponent<Image> ().sprite = dogSprite;
						//					newItem.transform.FindChild ("AcqRet").GetComponent<Image> ().sprite = actionsSprites [0]; // leave empty

						Sprite item;

						if (action == "cut")
							item = actionsSprites [2];
						else if (action == "dry")
							item = actionsSprites [3];
						else if (action == "wash")
							item = actionsSprites [4];
						else if (action == "groom")
							item = actionsSprites [5];
						else
							item = displayErrorSprite;

						newItem.transform.FindChild ("ItemAction").GetComponent<Image> ().sprite = item;
//						newItem.transform.FindChild ("ActionText").GetComponent<Text> ().text = action + ";";
					}
					newItem.transform.FindChild ("ActionText").GetComponent<Text> ().text = action + ";";
					simulationImagesToDisplay.Add (newItem);

				}

			} else if (child.GetComponent<Draggable> ().typeOfItem == Draggable.Type.IFSTAT) {

				//Debug.Log ("TYPE IFSTAT");

				string condition, actionText, line;
				try {

					condition = blocks_t1 [i].GetComponentInChildren<Text> ().text;
					actionText = blocks_t1 [i].FindChild ("DropArea").GetComponentInChildren<Text> ().text;

					//line = "[thread 1] if ( " + condition + " ) {\n    " + actionText + "\n}";
					line = actionText + "; [ if ( " + condition + " ) ]\n";

				} catch (Exception e) {
					//manager.showError ("At least one if statement is empty.");
					//line = ">> ERROR: Empty if statement";
					simulationTextArea.text = "";
					manager.showError ("There is at least one empty if statement.");
					terminateSimulation ();
					return;
				}

				//blocks_names_t1 [i] = line;
				blocks_names_t1.Add (line);

				//blocks_names [i] = blocks[i].transform.GetComponentInChildren<Text> ().text;
				i++;

			} else if (child.GetComponent<Draggable> ().typeOfItem == Draggable.Type.WHILELOOP) {

				string condition, line;
				string actionText = "";

				int whileChildrenCount = child.Find ("DropArea").childCount;
				thread1_whilesChildren += whileChildrenCount;
				//Debug.Log ("child " + child.name + ", child count: " + whileChildrenCount);

				//Debug.Log ("Thread 1 whileChildrenCount: " + whileChildrenCount);
				if (whileChildrenCount < 1) {
					//Debug.Log(">>> ERROR: There is at least one empty while loop");
					//simulationTextArea.text = "There is at least one empty while loop in thread 2.";
					simulationTextArea.text = "";
					manager.showError ("There is at least one empty while loop.");
					terminateSimulation ();
					return;
				}

				Transform[] whileChildren = new Transform[whileChildrenCount];

				for (int k = 0; k < whileChildrenCount; k++) {
					//threadChildren [i] = this.transform.Find("DropAreaThread").GetChild (i).gameObject;

					whileChildren [k] = child.Find ("DropArea").GetChild (k);
					//threadChildren [i] = this.transform.Find ("DropAreaThread").GetChild (i).GetComponentInChildren<Text>().text;
					//Debug.Log ( timer.GetCurrentTime() + " -> " + threadChildren [i]);

					//Debug.Log ("actions: " + whileChildren [k]);
				}

				try {

					condition = blocks_t1 [i].Find ("Condition").GetComponent<Text> ().text;
					if (condition == "< 2") {

						if (whileChildrenCount > 1) {

							//Debug.Log("There are " + whileChildrenCount + " children.");

							/*
							for (int k = 0; k < whileChildrenCount; k++) 
								Debug.Log(whileChildren[k].GetComponentInChildren<Text>().text);
							*/

							for (int l = 0; l < 2; l++) {

								for (int m = 0; m < whileChildrenCount; m++) {

									blocks_names_t1.Add (whileChildren [m].GetComponentInChildren<Text> ().text + "; " +
										"[ while ( " + condition + " ), iter = " + (l + 1) + " ]\n");
								}
							}

						} else {
							//Debug.Log ("There is 1 child.");
							for (int k = 0; k < 2; k++) {
								blocks_names_t1.Add (whileChildren [0].GetComponentInChildren<Text> ().text + "; " +
									"[ while ( " + condition + " ), iter = " + (k + 1) + " ]\n");
							}
						}

					} else {
						Debug.Log ("Unidentified condition");
					}


					//line = "[thread 1] while ( " + condition + " ) {\n" + actionText + "}";

				} catch (Exception e) {
					manager.showError ("Exception caught.");
					line = ">>> Exception caught.";
				}

				//blocks_names_t1 [i] = line;

				i++;
			}
		}

		if (blocks_t1.Length < 1) {
			manager.showError ("There are no actions to run.");
			simulationTextArea.text = "";
			clearVerticalLayout ();
			clearVerticalLayout2 ();
			terminateSimulation ();
			return;
		}
	
		try {
			if ((blocks_names_t1 [0].Substring (0, 7) != "checkin" /*&& blocks_names_t1 [0].Substring (11, 17) != "pickup"*/)) {

				manager.showError ("Remember to always check-in your costumer first!");
				terminateSimulation ();
				return;
			}
		} catch {
			manager.showError ("Remember to always check-in your costumer first!");
			terminateSimulation ();
			return;
		}

		try {

			// Debug.Log(blocks_names_t1.Count);

				if ((blocks_names_t1 [blocks_names_t1.Count - 1].Substring (0, 8) != "checkout")) {

				manager.showError ("Remember to always check-out your costumer when you're done!");
				terminateSimulation ();
				return;
			}
		} catch{
			manager.showError ("Remember to always check-out your costumer when you're done!");
			terminateSimulation ();
			return;
		}

		if (!err)
			// StartCoroutine (printThreads_single (blocks_names_t1, 14)); // List<>, speed/step
			StartCoroutine (printThreads_single (blocks_names_t1, simulationImagesToDisplay, 14)); // List<>, speed/step

	}


	private Transform[] GetActionBlocks_MultiThreads(String tabNum) {

		//get children in drop area for thread
		//threadChildren = new GameObject[this.transform.Find("DropAreaThread").childCount];
	
		string path = "";

		if (tabNum == "1")
			path = "Tab1/ScrollRect/Holder/DropAreaThread1";
		else
			path = "Tab2/ScrollRect/Holder/DropAreaThread2";

		Debug.Log ("children thread " + tabNum + ": " + GameObject.Find (path).transform.childCount);
		int childCount = GameObject.Find (path).transform.childCount;

		Transform[] threadChildren = new Transform[childCount];

		//Debug.Log ("thread childCount: " + childCount);

		for (int i = 0; i < childCount; i++) {
			//threadChildren [i] = this.transform.Find("DropAreaThread").GetChild (i).gameObject;

			threadChildren [i] = GameObject.Find (path).transform.GetChild(i);
			//threadChildren [i] = this.transform.Find ("DropAreaThread").GetChild (i).GetComponentInChildren<Text>().text;
			//Debug.Log ( timer.GetCurrentTime() + " -> " + threadChildren [i]);

			//Debug.Log ("Child " + i + ": " + threadChildren [i].name);
		}
			
		return threadChildren;
	}


	// USED FOR LEVEL **3** ONLY
	public void Execute_MultiThreads_Level2() {

		// Debug.Log ("Execute_MultiThreads_Level2()");

		t1_did_cut = false;
		t1_did_dry = false;
		t1_did_wash = false;
		t1_did_groom = false;

		t2_did_cut = false;
		t2_did_dry = false;
		t2_did_wash = false;
		t2_did_groom = false;

		// ----- SET UP FOR LOLA AND ROCKY, CUSTOMERS FOR LEVEL 3 -----

		t1_needs_cut = true;
		t1_needs_dry = false;
		t1_needs_wash = true;
		t1_needs_groom = false;

		t2_needs_cut = true;
		t2_needs_dry = true;
		t2_needs_wash = false;
		t2_needs_groom = false;

		simulationTextArea.text = "";
		clearVerticalLayout ();
		clearVerticalLayout2 ();

		try {
			GameObject.Find("InformationPanel").SetActive(false);
		} catch {}

		try {

			GameObject.Find("AgendaPanel").SetActive(false);
		} catch { }

		stop = false;
		err = false;
		paused = false;
		lost = false;

		t1_checkedin = false;
		t1_checkedout = false;
		t2_checkedin = false;
		t2_checkedout = false;

		t1_has_brush = false;
		t1_has_clippers = false;
		t1_has_conditioner = false;
		t1_has_dryer = false;
		t1_has_scissors = false;
		t1_has_shampoo = false;
		t1_has_station = false;
		t1_has_towel = false;

		t2_has_brush = false;
		t2_has_clippers = false;
		t2_has_conditioner = false;
		t2_has_dryer = false;
		t2_has_scissors = false;
		t2_has_shampoo = false;
		t2_has_station = false;
		t2_has_towel = false;

		try {
			// disable all other functionalities
			disablePanel.SetActive (true);
		} catch {
			Debug.Log ("Cannot enable DisablePanel");
		}

		// switch to stop button
		runButton.transform.SetAsFirstSibling ();

		//simulationTextArea.text = "test";


		// ------------------------ READING TAB 1 ------------------------

		int thread1_whilesChildren = 0;

		blocks_t1 = GetActionBlocks_MultiThreads ("1");
		/*
		foreach (Transform action in blocks_t1)
			Debug.Log (action.GetComponentInChildren<Text>().text);
		*/

		List<string> blocks_names_t1 = new List<string> ();
		List<GameObject> simulationImagesToDisplay_T1 = new List<GameObject> ();

		int i = 0;
		bool isError = false; //unused, for now

		foreach (Transform child in blocks_t1) {

			if (child.GetComponent<Draggable> ().typeOfItem == Draggable.Type.ACTION) {

				//Debug.Log ("TYPE ACTION");


				if (blocks_t1 [i].transform.GetComponentInChildren<Text> ().text == "get") {

					string resource = blocks_t1 [i].transform.Find ("Dropdown").Find ("Label").GetComponent<Text> ().text;

					if (resource == "[null]") {
						terminateSimulation ();
						manager.showError ("Please select a resource to acquire in thread 1.");
						return;

					} else {
						
						// level 3
						blocks_names_t1.Add ("[thread 1] acquire ( " + resource + " );\n");
						i++;

						// create new object from prefab
						GameObject newItem = Instantiate(simulationImagePrefab) as GameObject;
						newItem.transform.FindChild ("Icon").GetComponent<Image> ().sprite = workerSprite;
						newItem.transform.FindChild ("AcqRet").GetComponent<Image> ().sprite = actionsSprites [0];

						Sprite item;

						if (resource == "brush")
							item = itemsSprites [0];
						else if (resource == "clippers")
							item = itemsSprites [1];
						else if (resource == "cond.")
							item = itemsSprites [2];
						else if (resource == "dryer")
							item = itemsSprites [3];
						else if (resource == "scissors")
							item = itemsSprites [4];
						else if (resource == "shampoo")
							item = itemsSprites [5];
						else if (resource == "station")
							item = itemsSprites [6];
						else if (resource == "towel")
							item = itemsSprites [7];
						else
							item = displayErrorSprite;

						newItem.transform.FindChild ("ItemAction").GetComponent<Image> ().sprite = item;
						newItem.transform.FindChild ("ActionText").GetComponent<Text>().text = "get(" + resource + ");";
						simulationImagesToDisplay_T1.Add (newItem);

					}

				} else if(blocks_t1 [i].transform.GetComponentInChildren<Text> ().text == "ret") {

					string resource = blocks_t1 [i].transform.Find ("Dropdown").Find ("Label").GetComponent<Text> ().text;

					if (resource == "[null]") {
						terminateSimulation ();
						manager.showError ("Please select a resource to return in thread 1.");
						return;
					} else {
						// level 3
						blocks_names_t1.Add ("[thread 1] return ( " + resource + " );\n");
						i++;

						// create new object from prefab
						GameObject newItem = Instantiate(simulationImagePrefab) as GameObject;
						newItem.transform.FindChild ("Icon").GetComponent<Image> ().sprite = workerSprite;
						newItem.transform.FindChild ("AcqRet").GetComponent<Image> ().sprite = actionsSprites [1];

						Sprite item;

						if (resource == "brush")
							item = itemsSprites [0];
						else if (resource == "clippers")
							item = itemsSprites [1];
						else if (resource == "cond.")
							item = itemsSprites [2];
						else if (resource == "dryer")
							item = itemsSprites [3];
						else if (resource == "scissors")
							item = itemsSprites [4];
						else if (resource == "shampoo")
							item = itemsSprites [5];
						else if (resource == "station")
							item = itemsSprites [6];
						else if (resource == "towel")
							item = itemsSprites [7];
						else
							item = displayErrorSprite;

						newItem.transform.FindChild ("ItemAction").GetComponent<Image> ().sprite = item;
						newItem.transform.FindChild ("ActionText").GetComponent<Text>().text = "return(" + resource + ");";
						simulationImagesToDisplay_T1.Add (newItem);
					}

				} else {
					
					String action = blocks_t1 [i].transform.GetComponentInChildren<Text> ().text;
					blocks_names_t1.Add ("[thread 1] " + action + ";\n");
					i++;

					GameObject newItem = Instantiate (simulationImagePrefab) as GameObject;

					if (action == "checkin") {

						// Debug.Log ("CHECKING IN");

						newItem.transform.FindChild ("Icon").GetComponent<Image> ().sprite = workerSprite;
						newItem.transform.FindChild ("ItemAction").GetComponent<Image> ().sprite = dogSprite;
						newItem.transform.FindChild ("AcqRet").GetComponent<Image> ().sprite = actionsSprites [0];

					} else if (action == "checkout") {

						//Debug.Log ("CHECKING OUT");

						newItem.transform.FindChild ("Icon").GetComponent<Image> ().sprite = workerSprite;
						newItem.transform.FindChild ("ItemAction").GetComponent<Image> ().sprite = dogSprite;
						newItem.transform.FindChild ("AcqRet").GetComponent<Image> ().sprite = actionsSprites [1];

					} else {

						newItem.transform.FindChild ("Icon").GetComponent<Image> ().sprite = dogSprite;

						Sprite item;

						if (action == "cut")
							item = actionsSprites [2];
						else if (action == "dry")
							item = actionsSprites [3];
						else if (action == "wash")
							item = actionsSprites [4];
						else if (action == "groom")
							item = actionsSprites [5];
						else
							item = displayErrorSprite;

						newItem.transform.FindChild ("ItemAction").GetComponent<Image> ().sprite = item;
					}
					newItem.transform.FindChild ("ActionText").GetComponent<Text> ().text = action + ";";
					simulationImagesToDisplay_T1.Add (newItem);
				}

			} else if (child.GetComponent<Draggable> ().typeOfItem == Draggable.Type.IFSTAT) {

				//Debug.Log ("TYPE IFSTAT");

				string condition, actionText, line;
				try {

					condition = blocks_t1 [i].GetComponentInChildren<Text> ().text;
					actionText = blocks_t1 [i].FindChild ("DropArea").GetComponentInChildren<Text> ().text;

					//line = "[thread 1] if ( " + condition + " ) {\n    " + actionText + "\n}";
					line = "[thread 1] " + actionText + "; [ if ( " + condition + " ) ]\n";

				} catch (Exception e) {
					//manager.showError ("At least one if statement is empty.");
					//line = ">> ERROR: Empty if statement";
					simulationTextArea.text = "";
					manager.showError ("There is at least one empty if statement in thread 1.");
					terminateSimulation ();
					return;
				}

				//blocks_names_t1 [i] = line;
				// level 3
				blocks_names_t1.Add (line);

				//blocks_names [i] = blocks[i].transform.GetComponentInChildren<Text> ().text;
				i++;

			} else if (child.GetComponent<Draggable> ().typeOfItem == Draggable.Type.WHILELOOP) {

				string condition, line;
				string actionText = "";

				int whileChildrenCount = child.Find ("DropArea").childCount;
				thread1_whilesChildren += whileChildrenCount;
				//Debug.Log ("child " + child.name + ", child count: " + whileChildrenCount);

				//Debug.Log ("Thread 1 whileChildrenCount: " + whileChildrenCount);
				if (whileChildrenCount < 1) {
					//Debug.Log(">>> ERROR: There is at least one empty while loop");
					//simulationTextArea.text = "There is at least one empty while loop in thread 2.";
					simulationTextArea.text = "";
					manager.showError ("There is at least one empty while loop in thread 1.");
					terminateSimulation ();
					return;
				}

				Transform[] whileChildren = new Transform[whileChildrenCount];

				for (int k = 0; k < whileChildrenCount; k++) {
					//threadChildren [i] = this.transform.Find("DropAreaThread").GetChild (i).gameObject;

					whileChildren [k] = child.Find ("DropArea").GetChild (k);
					//threadChildren [i] = this.transform.Find ("DropAreaThread").GetChild (i).GetComponentInChildren<Text>().text;
					//Debug.Log ( timer.GetCurrentTime() + " -> " + threadChildren [i]);

					//Debug.Log ("actions: " + whileChildren [k]);
				}

				try {

					condition = blocks_t1 [i].Find ("Condition").GetComponent<Text> ().text;
					if (condition == "< 2") {

						if (whileChildrenCount > 1) {

							//Debug.Log("There are " + whileChildrenCount + " children.");

							/*
							for (int k = 0; k < whileChildrenCount; k++) 
								Debug.Log(whileChildren[k].GetComponentInChildren<Text>().text);
							*/

							for (int l = 0; l < 2; l++) {

								for (int m = 0; m < whileChildrenCount; m++) {
									// level 3
									blocks_names_t1.Add ("[thread 1] " + whileChildren [m].GetComponentInChildren<Text> ().text + "; " +
										"[ while ( " + condition + " ), iter = " + (l + 1) + " ]\n");
								}
							}

						} else {
							//Debug.Log ("There is 1 child.");
							for (int k = 0; k < 2; k++) {
								// level 3
								blocks_names_t1.Add ("[thread 1] " + whileChildren [0].GetComponentInChildren<Text> ().text + "; " +
									"[ while ( " + condition + " ), iter = " + (k + 1) + " ]\n");
							}
						}

					} else {
						Debug.Log ("Unidentified condition");
					}


					//line = "[thread 1] while ( " + condition + " ) {\n" + actionText + "}";

				} catch (Exception e) {
					manager.showError ("Exception caught.");
					line = ">>> Exception caught.";
				}

				//blocks_names_t1 [i] = line;

				i++;
			}
		}

		// ------------------------ READING TAB 2 ------------------------


		blocks_t2 = GetActionBlocks_MultiThreads ("2");

		int thread2_whilesChildren = 0;

		//string[] blocks_names_t2 = new string[blocks_t2.Length];
		List<string> blocks_names_t2 = new List<string> ();
		List<GameObject> simulationImagesToDisplay_T2 = new List<GameObject> ();

		i = 0;

		foreach (Transform child in blocks_t2) {

			if (child.GetComponent<Draggable> ().typeOfItem == Draggable.Type.ACTION) {

				if (blocks_t2 [i].transform.GetComponentInChildren<Text> ().text == "get") {

					string resource = blocks_t2 [i].transform.Find ("Dropdown").Find("Label").GetComponent<Text> ().text;

					// Debug.Log ("... attempting resource: " + resource);


					if (resource == "[null]") {

						Debug.Log ("Please select a resource to acquire in thread 2.");

						terminateSimulation ();
						manager.showError ("Please select a resource to acquire in thread 2.");
						return;

					} else {

						blocks_names_t2.Add ("[thread 2] acquire ( " + resource + " );\n");
						i++;

						// create new object from prefab
						GameObject newItem = Instantiate(simulationImagePrefab) as GameObject;
						newItem.transform.FindChild ("Icon").GetComponent<Image> ().sprite = workerSprite2;
						newItem.transform.FindChild ("AcqRet").GetComponent<Image> ().sprite = actionsSprites [0];

						Sprite item;

						if (resource == "brush")
							item = itemsSprites [0];
						else if (resource == "clippers")
							item = itemsSprites [1];
						else if (resource == "cond.")
							item = itemsSprites [2];
						else if (resource == "dryer")
							item = itemsSprites [3];
						else if (resource == "scissors")
							item = itemsSprites [4];
						else if (resource == "shampoo")
							item = itemsSprites [5];
						else if (resource == "station")
							item = itemsSprites [6];
						else if (resource == "towel")
							item = itemsSprites [7];
						else
							item = displayErrorSprite;

						newItem.transform.FindChild ("ItemAction").GetComponent<Image> ().sprite = item;
						newItem.transform.FindChild ("ActionText").GetComponent<Text>().text = "get(" + resource + ");";
						simulationImagesToDisplay_T2.Add (newItem);
					}

				} else if(blocks_t2 [i].transform.GetComponentInChildren<Text> ().text == "ret") {

					string resource = blocks_t2 [i].transform.Find ("Dropdown").Find("Label").GetComponent<Text> ().text;

					if (resource == "[null]") {

						terminateSimulation ();
						manager.showError ("Please select a resource to return in thread 2.");
						return;

					} else {

						blocks_names_t2.Add ("[thread 2] return ( " + resource + " );\n");
						i++;

						// create new object from prefab
						GameObject newItem = Instantiate(simulationImagePrefab) as GameObject;
						newItem.transform.FindChild ("Icon").GetComponent<Image> ().sprite = workerSprite2;
						newItem.transform.FindChild ("AcqRet").GetComponent<Image> ().sprite = actionsSprites [1];

						Sprite item;

						if (resource == "brush")
							item = itemsSprites [0];
						else if (resource == "clippers")
							item = itemsSprites [1];
						else if (resource == "cond.")
							item = itemsSprites [2];
						else if (resource == "dryer")
							item = itemsSprites [3];
						else if (resource == "scissors")
							item = itemsSprites [4];
						else if (resource == "shampoo")
							item = itemsSprites [5];
						else if (resource == "station")
							item = itemsSprites [6];
						else if (resource == "towel")
							item = itemsSprites [7];
						else
							item = displayErrorSprite;

						newItem.transform.FindChild ("ItemAction").GetComponent<Image> ().sprite = item;
						newItem.transform.FindChild ("ActionText").GetComponent<Text>().text = "return(" + resource + ");";
						simulationImagesToDisplay_T2.Add (newItem);
					}

				} else {

					String action = blocks_t2 [i].transform.GetComponentInChildren<Text> ().text;
					blocks_names_t2.Add ("[thread 2] " + action + ";\n");
					i++;
				
					GameObject newItem = Instantiate (simulationImagePrefab) as GameObject;

					if (action == "checkin") {

						// Debug.Log ("CHECKING IN");

						newItem.transform.FindChild ("Icon").GetComponent<Image> ().sprite = workerSprite2;
						newItem.transform.FindChild ("ItemAction").GetComponent<Image> ().sprite = dogSprite2;
						newItem.transform.FindChild ("AcqRet").GetComponent<Image> ().sprite = actionsSprites [0];

					} else if (action == "checkout") {

						//Debug.Log ("CHECKING OUT");

						newItem.transform.FindChild ("Icon").GetComponent<Image> ().sprite = workerSprite2;
						newItem.transform.FindChild ("ItemAction").GetComponent<Image> ().sprite = dogSprite2;
						newItem.transform.FindChild ("AcqRet").GetComponent<Image> ().sprite = actionsSprites [1];

					} else {

						newItem.transform.FindChild ("Icon").GetComponent<Image> ().sprite = dogSprite2;

						Sprite item;

						if (action == "cut")
							item = actionsSprites [2];
						else if (action == "dry")
							item = actionsSprites [3];
						else if (action == "wash")
							item = actionsSprites [4];
						else if (action == "groom")
							item = actionsSprites [5];
						else
							item = displayErrorSprite;

						newItem.transform.FindChild ("ItemAction").GetComponent<Image> ().sprite = item;
					}
					newItem.transform.FindChild ("ActionText").GetComponent<Text> ().text = action + ";";
					simulationImagesToDisplay_T2.Add (newItem);

				}

			} else if (child.GetComponent<Draggable> ().typeOfItem == Draggable.Type.IFSTAT) {

				//Debug.Log ("TYPE IFSTAT");

				string condition, actionText, line;
				try {

					condition = blocks_t2 [i].GetComponentInChildren<Text> ().text;
					actionText = blocks_t2 [i].FindChild ("DropArea").GetComponentInChildren<Text> ().text;

					//line = "[thread 1] if ( " + condition + " ) {\n    " + actionText + "\n}";
					line = "[thread 2] " + actionText + "; [ if ( " + condition + " ) ]\n";

				} catch (Exception e) {
					//manager.showError ("At least one if statement is empty.");
					//line = ">> ERROR: Empty if statement";
					simulationTextArea.text = "";
					manager.showError ("There is at least one empty if statement in thread 2.");
					terminateSimulation ();
					return;
				}

				//blocks_names_t2 [i] = line;
				blocks_names_t2.Add (line);

				//blocks_names [i] = blocks[i].transform.GetComponentInChildren<Text> ().text;
				i++;

			} else if (child.GetComponent<Draggable> ().typeOfItem == Draggable.Type.WHILELOOP) {

				string condition, line;
				string actionText = "";

				int whileChildrenCount = child.Find ("DropArea").childCount;
				thread2_whilesChildren += whileChildrenCount;
				//Debug.Log ("child " + child.name + ", child count: " + whileChildrenCount);

				Debug.Log ("Thread 2 whileChildrenCount: " + whileChildrenCount);
				if (whileChildrenCount < 1) {
					//Debug.Log(">>> ERROR: There is at least one empty while loop");
					//simulationTextArea.text = "There is at least one empty while loop in thread 2.";
					simulationTextArea.text = "";
					manager.showError ("There is at least one empty while loop in thread 2.");
					terminateSimulation ();
					return;
				}

				Transform[] whileChildren = new Transform[whileChildrenCount];

				for (int k = 0; k < whileChildrenCount; k++) {
					//threadChildren [i] = this.transform.Find("DropAreaThread").GetChild (i).gameObject;

					whileChildren [k] = child.Find ("DropArea").GetChild (k);
					//threadChildren [i] = this.transform.Find ("DropAreaThread").GetChild (i).GetComponentInChildren<Text>().text;
					//Debug.Log ( timer.GetCurrentTime() + " -> " + threadChildren [i]);

					//Debug.Log ("actions: " + whileChildren [k]);
				}

				try {

					condition = blocks_t2 [i].Find ("Condition").GetComponent<Text> ().text;
					if (condition == "< 2") {

						if (whileChildrenCount > 1) {

							//Debug.Log ("There are " + whileChildrenCount + " children.");

							for (int l = 0; l < 2; l++) {

								for (int m = 0; m < whileChildrenCount; m++) {

									blocks_names_t2.Add ("[thread 2] " + whileChildren [m].GetComponentInChildren<Text> ().text + "; " +
										"[ while ( " + condition + " ), iter = " + (l + 1) + " ]\n");
								}
							}

						} else {
							//Debug.Log ("There is 1 child.");
							for (int k = 0; k < 2; k++)
								blocks_names_t2.Add ("[thread 2] " + whileChildren [0].GetComponentInChildren<Text> ().text + "; " +
									"[ while ( " + condition + " ), iter = " + (k + 1) + " ]\n");
						}

					} else {
						Debug.Log ("Unidentified condition");
					}

					//line = "[thread 1] while ( " + condition + " ) {\n" + actionText + "}";

				} catch (Exception e) {
					manager.showError ("Exception caught.");
					line = ">>> Exception caught.";
				}

				//blocks_names_t2 [i] = line;

				i++;
			}
		}

		if (blocks_t1.Length < 1) {
			manager.showError ("There are no actions to run in thread 1.");
			simulationTextArea.text = "";
			clearVerticalLayout ();
			clearVerticalLayout2 ();
			terminateSimulation ();
			return;
		}

		if (blocks_t2.Length < 1) {
			manager.showError ("There are no actions to run in thread 2.");
			simulationTextArea.text = "";
			clearVerticalLayout ();
			clearVerticalLayout2 ();
			terminateSimulation ();
			return;
		}

		try {
			
			if ((blocks_names_t1 [0].Substring (11, 7) != "checkin" /*&& blocks_names_t1 [0].Substring (11, 17) != "pickup"*/)
				|| (blocks_names_t2 [0].Substring (11, 7) != "checkin" /*&& blocks_names_t2 [0].Substring (11, 17) != "pickup"*/)) {

				manager.showError ("Remember to always check-in your costumer first!");
				terminateSimulation ();
				return;
			}
		} catch {
			manager.showError ("1217 EXCEPTION: Remember to always check-in your costumer first!");
			terminateSimulation ();
			return;
		}

		try {

			// Debug.Log(blocks_names_t1.Count);

			if ((blocks_names_t1 [blocks_names_t1.Count - 1].Substring (11, 8) != "checkout")
				|| (blocks_names_t2 [blocks_names_t2.Count - 1].Substring (11, 8) != "checkout")) {

				manager.showError ("Remember to always check-out your costumer when you're done!");
				terminateSimulation ();
				return;
			}
		} catch{
			manager.showError ("1234 EXCEPTION: Remember to always check-out your costumer when you're done!");
			terminateSimulation ();
			return;
		}

		if (!err)
			StartCoroutine (printThreads (blocks_names_t1, blocks_names_t2, simulationImagesToDisplay_T1, simulationImagesToDisplay_T2, 5));

	}

	public void terminateSimulation() {

		stepsIndicator.text = "0";

		err = true;
		lost = true;
		stop = true;
		// paused = true;

		try {
			disablePanel.SetActive (false);
		} catch {
			Debug.Log ("Cannot disable DisablePanel.");
		}
		simulationTextArea.text = "";
		//clearVerticalLayout ();
		//clearVerticalLayout2 ();

		runButton.transform.SetAsLastSibling ();
		bar.LoadingBar.GetComponent<Image> ().fillAmount = 0;

		simulationScrollRect.verticalNormalizedPosition = 1f;
	}

	// ------- FOR SINGLE THREAD (I.E. LEVEL 1)
//	IEnumerator printThreads_single(List<string> b1, int speed) {
	IEnumerator printThreads_single(List<string> b1, List<GameObject> b3, int speed) {

		// Debug.Log ("printThreads_single");
		
		bar.currentAmount = 0;

		// int speed = 15;

		int step_counter = 1;
		int t1_curr_index = 0;

		bool t1_canPrint = true;

		int limit = 0;
		int j = 0;

		while ((t1_curr_index < b1.Count)) {

			// Debug.Log ("b1.Count = " + b1.Count + ", t1_curr_index = " + t1_curr_index);

			if (bar.currentAmount < 100) {

				// Debug.Log ("bar.currentAmount < 100. Bar updated.");

				bar.currentAmount += speed;
				bar.LoadingBar.GetComponent<Image>().fillAmount = bar.currentAmount / 100;

			} else {

				manager.gameLost();
				stop = true;
				paused = true;
				lost = true;

				stopButton.transform.GetComponent<Button> ().interactable = false;
				// bar.LoadingBar.GetComponent<Image> ().fillAmount = 0;

				// break;
				// yield break;
				yield return 0;
			}

			if (stop) {

				if (!paused) {

					try {
						disablePanel.SetActive (false);
					} catch {
						Debug.Log ("Cannot disable DisablePanel");
					}
					//simulationTextArea.text = "";

					runButton.transform.SetAsLastSibling ();
					// bar.LoadingBar.GetComponent<Image> ().fillAmount = 0;

				}

				// Debug.Log ("Bar set to 0 in if(stop)");

				bar.LoadingBar.GetComponent<Image> ().fillAmount = 0;

				break;
				yield break;
				yield return 0;

			} else {

				simulationTextArea.text += "\nSTEP " + (j+1) + ": \n";
				stepsIndicator.text = ""+(j + 1);
//				Debug.Log("\nSTEP " + (j+1) + ": \n");

				// ------------------------------  THREAD 1 ------------------------------

				try {

					// {"[null]", "brush" ,"clippers" , "cond.", "dryer", "scissors", "shampoo", "station", "towel"};

					if (b1[t1_curr_index].Substring(0, 3) == "acq") {

						// Debug.Log("ACQUIRING " + b1[t1_curr_index].Substring(10, 5));

						// acquiring resource
						switch(b1[t1_curr_index].Substring(10, 5)) {

						case "brush":

							int output1 = acquire (ref t1_has_brush);
							t1_canPrint = true;
							// lost = false;

							if (output1 < 0) {
								// resError(b1[t1_curr_index]);
								resError(acquireErrMsg, 1); // ERROR: You are trying to acquire a resource you already have.";
							}
						
							break;

						case "clipp":

							int output2 = acquire (ref t1_has_clippers);
							t1_canPrint = true;
							// lost = false;

							if (output2 < 0) {
								// resError(b1[t1_curr_index]);
								resError(acquireErrMsg, 1);
							}

							break;

						case "cond.":

							int output3 = acquire (ref t1_has_conditioner);
							t1_canPrint = true;
							// lost = false;

							if (output3 < 0) {
								// resError(b1[t1_curr_index]);
								resError(acquireErrMsg, 1);
							}

							break;

						case "dryer":

							int output4 = acquire (ref t1_has_dryer);
							t1_canPrint = true;
							// lost = false;

							if (output4 < 0) {
								// resError(b1[t1_curr_index]);
								resError(acquireErrMsg, 1);
							}

				
							break;

						case "sciss":
							
							int output5 = acquire (ref t1_has_scissors);
							t1_canPrint = true;
							// lost = false;

							if (output5 < 0) {
								// resError(b1[t1_curr_index]);
								resError(acquireErrMsg, 1);
							}

							break;

						case "shamp":

							int output6 = acquire (ref t1_has_shampoo);
							t1_canPrint = true;
							// lost = false;

							if (output6 < 0) {
								// resError(b1[t1_curr_index]);
								resError(acquireErrMsg, 1);
							}

							break;

						case "stati":

							int output7 = acquire (ref t1_has_station);
							t1_canPrint = true;
							// lost = false;

							if (output7 < 0) {
								// resError(b1[t1_curr_index]);
								resError(acquireErrMsg, 1);
							}

							break;

						case "towel":

							int output8 = acquire (ref t1_has_towel);
							t1_canPrint = true;
							// lost = false;

							if (output8 < 0) {
								// resError(b1[t1_curr_index]);
								resError(acquireErrMsg, 1);
							}

							break;
						}

					} else if (b1[t1_curr_index].Substring(0, 3) == "ret") {

						// Debug.Log("RETURNING " + b1[t1_curr_index].Substring(0, 5));

						// returning resource
						switch(b1[t1_curr_index].Substring(9, 5)) {

						case "brush":
							int output1 = return_res (ref t1_has_brush);
						
							if (output1 < 0) {
//								resError(b1[t1_curr_index]);
								resError(returnErrMsg, 1);
							}

							break;

						case "clipp":
							int output2 = return_res (ref t1_has_clippers);

							if (output2 < 0) {
//								resError(b1[t1_curr_index]);
								resError(returnErrMsg, 1);
							}

							break;

						case "cond.":
							int output3 = return_res (ref t1_has_conditioner);

							if (output3 < 0) {
//								resError(b1[t1_curr_index]);
								resError(returnErrMsg, 1);
							}

							break;

						case "dryer":
							int output4 = return_res (ref t1_has_dryer);

							if (output4 < 0) {
//								resError(b1[t1_curr_index]);
								resError(returnErrMsg, 1);
							}

							break;

						case "sciss":
							int output5 = return_res (ref t1_has_scissors);

							if (output5 < 0) {
//								resError(b1[t1_curr_index]);
								resError(returnErrMsg, 1);
							}

							break;

						case "shamp":
							int output6 = return_res (ref t1_has_shampoo);

							if (output6 < 0) {
//								resError(b1[t1_curr_index]);
								resError(returnErrMsg, 1);
							}

							break;

						case "stati":
							int output7 = return_res (ref t1_has_station);

							if (output7 < 0) {
//								resError(b1[t1_curr_index]);
								resError(returnErrMsg, 1);
							}

							break;

						case "towel":
							int output8 = return_res (ref t1_has_towel);

							if (output8 < 0) {
//								resError(b1[t1_curr_index]);
								resError(returnErrMsg, 1);
							}

							break;
						}

					} else if (b1[t1_curr_index].Substring(0, 3) == "cut") {
						
						if (!t1_has_brush || !t1_has_scissors) {
							simulationTextArea.text += "<color=red>" + b1 [t1_curr_index] + "</color>";

							String actionText = b3[t1_curr_index].transform.Find("ActionText").GetComponent<Text>().text;
							b3[t1_curr_index].transform.Find("ActionText").GetComponent<Text>().text = "<color=red>" + actionText + "</color>";
							b3[t1_curr_index].transform.parent = layoutPanel.transform;
							Debug.Log("1");
							b3[t1_curr_index].transform.localScale = Vector3.one;

//							GameObject newItemError = Instantiate(simulationErrorPrefab) as GameObject;
//							newItemError.transform.Find("ActionText").GetComponent<Text>().text = "> ERROR: You can't cut without a brush and some scissors.";
//							newItemError.transform.parent = layoutPanel.transform;

							resError("\n> ERROR: You can't cut without a brush and some scissors.\n\n", 1);


						}

						// perform cut
						t1_did_cut = true;

					} else if (b1[t1_curr_index].Substring(0, 3) == "dry") {

						if (!t1_has_station || !t1_has_dryer || !t1_has_towel) {
							simulationTextArea.text += "<color=red>" + b1 [t1_curr_index] + "</color>";

							String actionText = b3[t1_curr_index].transform.Find("ActionText").GetComponent<Text>().text;
							b3[t1_curr_index].transform.Find("ActionText").GetComponent<Text>().text = "<color=red>" + actionText + "</color>";
							b3[t1_curr_index].transform.parent = layoutPanel.transform;
							Debug.Log("2");
							b3[t1_curr_index].transform.localScale = Vector3.one;

//							GameObject newItemError = Instantiate(simulationErrorPrefab) as GameObject;
//							newItemError.transform.Find("ActionText").GetComponent<Text>().text = "> ERROR: You can't dry without a station, a dryer and a towel.";
//							newItemError.transform.parent = layoutPanel.transform;

							resError("\n> ERROR: You can't dry without a station, a dryer and a towel.\n\n", 1);

						}

						// perform dry
						t1_did_dry = true;

					} else if (b1[t1_curr_index].Substring(0, 4) == "wash") {

						if (!t1_has_station || !t1_has_shampoo || !t1_has_towel || !t1_has_conditioner) {
							simulationTextArea.text += "<color=red>" + b1 [t1_curr_index] + "</color>";

							String actionText = b3[t1_curr_index].transform.Find("ActionText").GetComponent<Text>().text;
							b3[t1_curr_index].transform.Find("ActionText").GetComponent<Text>().text = "<color=red>" + actionText + "</color>";
							b3[t1_curr_index].transform.parent = layoutPanel.transform;
							Debug.Log("3");
							b3[t1_curr_index].transform.localScale = Vector3.one;

//							GameObject newItemError = Instantiate(simulationErrorPrefab) as GameObject;
//							newItemError.transform.Find("ActionText").GetComponent<Text>().text = "> ERROR: You can't wash without a station, shampoo, conditioner, and a towel.";
//							newItemError.transform.parent = layoutPanel.transform;

							resError("\n> ERROR: You can't wash without a station, shampoo, conditioner, and a towel.\n\n", 1);


						}

						// perform wash
						t1_did_wash = true;

					} else if (b1[t1_curr_index].Substring(0, 5) == "groom") {

						if (!t1_has_brush || !t1_has_clippers) {
							simulationTextArea.text += "<color=red>" + b1 [t1_curr_index] + "</color>";

							String actionText = b3[t1_curr_index].transform.Find("ActionText").GetComponent<Text>().text;
							b3[t1_curr_index].transform.Find("ActionText").GetComponent<Text>().text = "<color=red>" + actionText + "</color>";
							b3[t1_curr_index].transform.parent = layoutPanel.transform;
							Debug.Log("4");
							b3[t1_curr_index].transform.localScale = Vector3.one;

//							GameObject newItemError = Instantiate(simulationErrorPrefab) as GameObject;
//							newItemError.transform.Find("ActionText").GetComponent<Text>().text = "> ERROR: You can't groom without a brush and some nail clippers.";
//							newItemError.transform.parent = layoutPanel.transform;

							resError("\n> ERROR: You can't groom without a brush and some nail clippers.\n\n", 1);

						}

						// perform groom
						t1_did_groom = true;

					} else if (b1[t1_curr_index].Substring(0, 7) == "checkin") {

						if (t1_checkedin) {
							simulationTextArea.text += "<color=red>" + b1 [t1_curr_index] + "</color>";

							String actionText = b3[t1_curr_index].transform.Find("ActionText").GetComponent<Text>().text;
							b3[t1_curr_index].transform.Find("ActionText").GetComponent<Text>().text = "<color=red>" + actionText + "</color>";
							b3[t1_curr_index].transform.parent = layoutPanel.transform;
							Debug.Log("5");
							b3[t1_curr_index].transform.localScale = Vector3.one;

//							GameObject newItemError = Instantiate(simulationErrorPrefab) as GameObject;
//							newItemError.transform.Find("ActionText").GetComponent<Text>().text = "> ERROR: You are already checked in. You have to check out before attempting to check in a different customer.";
//							newItemError.transform.parent = layoutPanel.transform;

							resError("\n> ERROR: You are already checked in. You have to check out before attempting to check in a different customer.\n\n", 1);

						
						} else {
							t1_checkedin = true;
							t1_checkedout = false;
						}

					} else if (b1[t1_curr_index].Substring(0, 8) == "checkout") {
						
//						if (t1_has_brush || t1_has_clippers || t1_has_conditioner || t1_has_dryer || t1_has_scissors || t1_has_shampoo || t1_has_station || t1_has_towel) {
//
//							simulationTextArea.text += "<color=red>" + b1 [t1_curr_index] + "</color>";
//							resError("\n> ERROR: You need to return all the resources you acquired before checking out.\n\n");
//
//						} else 

						if ((t1_needs_cut && !t1_did_cut) || (t1_needs_dry && !t1_did_dry)
									|| (t1_needs_groom && !t1_did_groom) || (t1_needs_wash && !t1_did_wash)) {
						
							simulationTextArea.text += "<color=red>" + b1 [t1_curr_index] + "</color>";

							String actionText = b3[t1_curr_index].transform.Find("ActionText").GetComponent<Text>().text;
							b3[t1_curr_index].transform.Find("ActionText").GetComponent<Text>().text = "<color=red>" + actionText + "</color>";
							b3[t1_curr_index].transform.parent = layoutPanel.transform;
							Debug.Log("6");
							b3[t1_curr_index].transform.localScale = Vector3.one;

//							GameObject newItemError = Instantiate(simulationErrorPrefab) as GameObject;
//							newItemError.transform.Find("ActionText").GetComponent<Text>().text = "> ERROR: Seems like you didn't fulfill all of your customer's requests. Please try again.";
//							newItemError.transform.parent = layoutPanel.transform;

							resError("\n> ERROR: Seems like you didn't fulfill all of your customer's requests. Please try again.\n\n", 1);

						}
						else if (t1_checkedout) {
							simulationTextArea.text += "<color=red>" + b1 [t1_curr_index] + "</color>";

							String actionText = b3[t1_curr_index].transform.Find("ActionText").GetComponent<Text>().text;
							b3[t1_curr_index].transform.Find("ActionText").GetComponent<Text>().text = "<color=red>" + actionText + "</color>";
							b3[t1_curr_index].transform.parent = layoutPanel.transform;
							Debug.Log("7");
							b3[t1_curr_index].transform.localScale = Vector3.one;

							resError("\n> ERROR: You have to check in before attempting to check out a customer.\n\n", 1);

						} else {
							t1_checkedin = false;
							t1_checkedout = true;
						}
					}

				} catch { }

				try {

					if (t1_canPrint) {

						if (!err) {
							simulationTextArea.text += "" + b1 [t1_curr_index];
							b3[t1_curr_index].transform.parent = layoutPanel.transform;
							Debug.Log("8");
							b3[t1_curr_index].transform.localScale = Vector3.one;
						}
						t1_curr_index++;
					}

				} catch { }

				j++; // increment step

				Canvas.ForceUpdateCanvases();
				simulationScrollRect.verticalNormalizedPosition = 0f;
				Canvas.ForceUpdateCanvases ();

				yield return new WaitForSeconds (1);
			}
		}

		if (!lost) {
			manager.gameWon ();
			Debug.Log ("Finished in " + j + " steps.");
		}
	
	}

	IEnumerator printThreads(List<string> b1, List<string> b2, List<GameObject> s1, List<GameObject> s2, int speed) {

		// Debug.Log ("printThreads()");


		bar.currentAmount = 0;

		int step_counter = 1;
		int t1_curr_index = 0;
		int t2_curr_index = 0;

		bool t1_canPrint = true;
		bool t2_canPrint = true;

		int limit = 0;
		int j = 0;

		if (b1.Count > b2.Count)
			limit = b1.Count;
		else
			limit = b2.Count;

		// for (int j = 0; j < limit; j++) {
		// while (j < limit) {

		// while (j < 100) {

		while ((t1_curr_index < b1.Count) || (t2_curr_index < b2.Count)) {

			// Debug.Log ("b1.Count = " + b1.Count + ", t1_curr_index = " + t1_curr_index);

			if (bar.currentAmount < 100) {

				// Debug.Log ("bar.currentAmount < 100. Bar updated.");

				bar.currentAmount += speed;
				bar.LoadingBar.GetComponent<Image>().fillAmount = bar.currentAmount / 100;

			} else {

				manager.gameLost();
				stop = true;
				paused = true;
				lost = true;

				stopButton.transform.GetComponent<Button> ().interactable = false;
				// bar.LoadingBar.GetComponent<Image> ().fillAmount = 0;

				// break;
				// yield break;
				yield return 0;
			}

			if (stop) {

				if (!paused) {

					try {
						disablePanel.SetActive (false);
					} catch {
						Debug.Log ("Cannot disable DisablePanel");
					}
					//simulationTextArea.text = "";

					runButton.transform.SetAsLastSibling ();
					// bar.LoadingBar.GetComponent<Image> ().fillAmount = 0;

				}

				// Debug.Log ("Bar set to 0 in if(stop)");

				bar.LoadingBar.GetComponent<Image> ().fillAmount = 0;

				break;
				yield break;
				yield return 0;

			} else {

				simulationTextArea.text += "\nSTEP " + (j+1) + ": \n";
				stepsIndicator.text = "" + (j + 1);

				// ------------------------------  THREAD 1 ------------------------------

				try {
					
					// {"[null]", "brush" ,"clippers" , "cond.", "dryer", "scissors", "shampoo", "station", "towel"};

					if (b1[t1_curr_index].Substring(11, 3) == "acq") {

						// Debug.Log("ACQUIRING " + b1[t1_curr_index].Substring(21, 5));
							
						// acquiring resource
						switch(b1[t1_curr_index].Substring(21, 5)) {

						case "brush":

							if (!t1_has_brush && t2_has_brush) { // need to wait for resource

								// simDisplay("[thread 1] Waiting for a brush...\n");

								GameObject newItem = Instantiate(simulationImagePrefab) as GameObject;
								newItem.transform.FindChild ("Icon").GetComponent<Image>().sprite = actionsSprites[6]; // clock sprite
								newItem.transform.FindChild ("ItemAction").GetComponent<Image>().sprite = itemsSprites[0]; // brush sprite
								newItem.transform.FindChild ("ActionText").GetComponent<Text>().text = "<color=red>Waiting for a brush...</color>"; // clock sprite
								newItem.transform.parent = layoutPanel.transform;
								Debug.Log("9");
								newItem.transform.localScale = Vector3.one;

								t1_canPrint = false;

							} else {
								
								int output = acquire (ref t1_has_brush);
								t1_canPrint = true;
								// lost = false;

								if (output < 0) {
									
									// resError(b1[t1_curr_index]);
									resError(acquireErrMsg, 1);
								}
							}

							break;

						case "clipp":
							
							if (!t1_has_clippers && t2_has_clippers) { // need to wait for resource

								// simDisplay("[thread 1] Waiting for nail clippers...\n");

								GameObject newItem = Instantiate(simulationImagePrefab) as GameObject;
								newItem.transform.FindChild ("Icon").GetComponent<Image>().sprite = actionsSprites[6]; // clock sprite
								newItem.transform.FindChild ("ItemAction").GetComponent<Image>().sprite = itemsSprites[1]; // clippers sprite
								newItem.transform.FindChild ("ActionText").GetComponent<Text>().text = "<color=red>Waiting for nail clippers...</color>"; // clock sprite
								newItem.transform.parent = layoutPanel.transform;
								Debug.Log("10");
								newItem.transform.localScale = Vector3.one;

								t1_canPrint = false;

							} else {
								int output = acquire (ref t1_has_clippers);
								t1_canPrint = true;
								// lost = false;

								if (output < 0) {
									
									// resError(b1[t1_curr_index]);
									resError(acquireErrMsg, 1);
								}
							}

							break;

						case "cond.":

							if (!t1_has_conditioner && t2_has_conditioner) { // need to wait for resource

								// simDisplay("[thread 1] Waiting for condtioner...\n");
								t1_canPrint = false;

								GameObject newItem = Instantiate(simulationImagePrefab) as GameObject;
								newItem.transform.FindChild ("Icon").GetComponent<Image>().sprite = actionsSprites[6]; // clock sprite
								newItem.transform.FindChild ("ItemAction").GetComponent<Image>().sprite = itemsSprites[2]; // conditioner sprite
								newItem.transform.FindChild ("ActionText").GetComponent<Text>().text = "<color=red>Waiting for condtioner...</color>"; // clock sprite
								newItem.transform.parent = layoutPanel.transform;
								Debug.Log("11");
								newItem.transform.localScale = Vector3.one;

							} else {
								
								int output = acquire (ref t1_has_conditioner);
								t1_canPrint = true;
								// lost = false;

								if (output < 0) {
									// resError(b1[t1_curr_index]);
									resError(acquireErrMsg, 1);
								}
							}

							break;

						case "dryer":

							if (!t1_has_dryer && t2_has_dryer) { // need to wait for resource

								// simDisplay("[thread 1] Waiting for dryer...\n");
								t1_canPrint = false;

								GameObject newItem = Instantiate(simulationImagePrefab) as GameObject;
								newItem.transform.FindChild ("Icon").GetComponent<Image>().sprite = actionsSprites[6]; // clock sprite
								newItem.transform.FindChild ("ItemAction").GetComponent<Image>().sprite = itemsSprites[3]; // dryer sprite
								newItem.transform.FindChild ("ActionText").GetComponent<Text>().text = "<color=red>Waiting for dryer...</color>"; // clock sprite
								newItem.transform.parent = layoutPanel.transform;
								Debug.Log("12");
								newItem.transform.localScale = Vector3.one;

							} else {
								int output = acquire (ref t1_has_dryer);
								t1_canPrint = true;
								// lost = false;

								if (output < 0) {
									// resError(b1[t1_curr_index]);
									resError(acquireErrMsg, 1);
								}
							}

							break;

						case "sciss":
							
							if (!t1_has_scissors && t2_has_scissors) { // need to wait for resource

								// simDisplay("[thread 1] Waiting for scissors...\n");
								t1_canPrint = false;

								GameObject newItem = Instantiate(simulationImagePrefab) as GameObject;
								newItem.transform.FindChild ("Icon").GetComponent<Image>().sprite = actionsSprites[6]; // clock sprite
								newItem.transform.FindChild ("ItemAction").GetComponent<Image>().sprite = itemsSprites[4]; // scissors sprite
								newItem.transform.FindChild ("ActionText").GetComponent<Text>().text = "<color=red>Waiting for scissors...</color>"; // clock sprite
								newItem.transform.parent = layoutPanel.transform;
								Debug.Log("13");
								newItem.transform.localScale = Vector3.one;

							} else {
								int output = acquire (ref t1_has_scissors);
								t1_canPrint = true;
								// lost = false;

								if (output < 0) {
									// resError(b1[t1_curr_index]);
									resError(acquireErrMsg, 1);
								}
							}

							break;

						case "shamp":

							if (!t1_has_shampoo && t2_has_shampoo) { // need to wait for resource

								// simDisplay("[thread 1] Waiting for shampoo...\n");
								t1_canPrint = false;

								GameObject newItem = Instantiate(simulationImagePrefab) as GameObject;
								newItem.transform.FindChild ("Icon").GetComponent<Image>().sprite = actionsSprites[6]; // clock sprite
								newItem.transform.FindChild ("ItemAction").GetComponent<Image>().sprite = itemsSprites[5]; // shampoo sprite
								newItem.transform.FindChild ("ActionText").GetComponent<Text>().text = "<color=red>Waiting for shampoo...</color>"; // clock sprite
								newItem.transform.parent = layoutPanel.transform;
								Debug.Log("14");
								newItem.transform.localScale = Vector3.one;

							} else {
								int output = acquire (ref t1_has_shampoo);
								t1_canPrint = true;
								// lost = false;

								if (output < 0) {
									// resError(b1[t1_curr_index]);
									resError(acquireErrMsg, 1);
								}
							}

							break;

						case "stati":

							if (!t1_has_station && t2_has_station) { // need to wait for resource

								// simDisplay("[thread 1] Waiting for a station...\n");
								t1_canPrint = false;

								GameObject newItem = Instantiate(simulationImagePrefab) as GameObject;
								newItem.transform.FindChild ("Icon").GetComponent<Image>().sprite = actionsSprites[6]; // clock sprite
								newItem.transform.FindChild ("ItemAction").GetComponent<Image>().sprite = itemsSprites[6]; // station sprite
								newItem.transform.FindChild ("ActionText").GetComponent<Text>().text = "<color=red>Waiting for a station...</color>"; // clock sprite
								newItem.transform.parent = layoutPanel.transform;
								Debug.Log("15");
								newItem.transform.localScale = Vector3.one;

							} else {
								int output = acquire (ref t1_has_station);
								t1_canPrint = true;
								// lost = false;

								if (output < 0) {
									// resError(b1[t1_curr_index]);
									resError(acquireErrMsg, 1);
								}
							}

							break;

						case "towel":

							if (!t1_has_towel && t2_has_towel) { // need to wait for resource

								// simDisplay("[thread 1] Waiting for a towel...\n");
								t1_canPrint = false;

								GameObject newItem = Instantiate(simulationImagePrefab) as GameObject;
								newItem.transform.FindChild ("Icon").GetComponent<Image>().sprite = actionsSprites[6]; // clock sprite
								newItem.transform.FindChild ("ItemAction").GetComponent<Image>().sprite = itemsSprites[7]; // towel sprite
								newItem.transform.FindChild ("ActionText").GetComponent<Text>().text = "<color=red>Waiting for a towel...</color>"; // clock sprite
								newItem.transform.parent = layoutPanel.transform;
								Debug.Log("16");
								newItem.transform.localScale = Vector3.one;

							} else {
								int output = acquire (ref t1_has_towel);
								t1_canPrint = true;
								// lost = false;

								if (output < 0) {
									// resError(b1[t1_curr_index]);
									resError(acquireErrMsg, 1);
								}
							}

							break;
						}

					} else if (b1[t1_curr_index].Substring(11, 3) == "ret") {

						// Debug.Log("RETURNING " + b1[t1_curr_index].Substring(20, 5));

						// returning resource
						switch(b1[t1_curr_index].Substring(20, 5)) {

						case "brush":
							
							int output1 = return_res (ref t1_has_brush);

							if (output1 < 0) {

								// resError(b1[t1_curr_index]);
								resError(returnErrMsg, 1);
							}

							break;

						case "clipp":

							int output2 = return_res (ref t1_has_clippers);

							if (output2 < 0) {

								// resError(b1[t1_curr_index]);
								resError(returnErrMsg, 1);
							}

							break;

						case "cond.":

							int output3 = return_res (ref t1_has_conditioner);

							if (output3 < 0) {

								// resError(b1[t1_curr_index]);
								resError(returnErrMsg, 1);
							}

							break;

						case "dryer":
							int output4 = return_res (ref t1_has_dryer);

							if (output4 < 0) {

								// resError(b1[t1_curr_index]);
								resError(returnErrMsg, 1);
							}

							break;

						case "sciss":
							int output5 = return_res (ref t1_has_scissors);

							if (output5 < 0) {

								// resError(b1[t1_curr_index]);
								resError(returnErrMsg, 1);
							}

							break;

						case "shamp":

							int output6 = return_res (ref t1_has_shampoo);

							if (output6 < 0) {

								// resError(b1[t1_curr_index]);
								resError(returnErrMsg, 1);
							}

							break;

						case "stati":
							int output7 = return_res (ref t1_has_station);

							if (output7 < 0) {

								// resError(b1[t1_curr_index]);
								resError(returnErrMsg, 1);
							}

							break;

						case "towel":
							int output8 = return_res (ref t1_has_towel);

							if (output8 < 0) {

								// resError(b1[t1_curr_index]);
								resError(returnErrMsg, 1);
							}

							break;
						}

					} else if (b1[t1_curr_index].Substring(11, 3) == "cut") {

						if (!t1_has_brush || !t1_has_scissors) {

							simulationTextArea.text += "<color=red>" + b1 [t1_curr_index] + "</color>";

							String actionText = s1[t1_curr_index].transform.Find("ActionText").GetComponent<Text>().text;
							s1[t1_curr_index].transform.Find("ActionText").GetComponent<Text>().text = "<color=red>" + actionText + "</color>";
							s1[t1_curr_index].transform.parent = layoutPanel.transform;
							Debug.Log("17");
							s1[t1_curr_index].transform.localScale = Vector3.one;

							resError("\n> ERROR: You can't cut without a brush and some scissors.\n\n", 1);
						}

						t1_did_cut = true;
							
					} else if (b1[t1_curr_index].Substring(11, 3) == "dry") {

						if (!t1_has_station || !t1_has_dryer || !t1_has_towel) {
							
							simulationTextArea.text += "<color=red>" + b1 [t1_curr_index] + "</color>";

							String actionText = s1[t1_curr_index].transform.Find("ActionText").GetComponent<Text>().text;
							s1[t1_curr_index].transform.Find("ActionText").GetComponent<Text>().text = "<color=red>" + actionText + "</color>";
							s1[t1_curr_index].transform.parent = layoutPanel.transform;
							Debug.Log("18");
							s1[t1_curr_index].transform.localScale = Vector3.one;

							resError("\n> ERROR: You can't dry without a station, a dryer and a towel.\n\n", 1);
						}

						t1_did_dry = true;

					} else if (b1[t1_curr_index].Substring(11, 4) == "wash") {

						if (!t1_has_station || !t1_has_shampoo || !t1_has_towel || !t1_has_conditioner) {
							simulationTextArea.text += "<color=red>" + b1 [t1_curr_index] + "</color>";

							String actionText = s1[t1_curr_index].transform.Find("ActionText").GetComponent<Text>().text;
							s1[t1_curr_index].transform.Find("ActionText").GetComponent<Text>().text = "<color=red>" + actionText + "</color>";
							s1[t1_curr_index].transform.parent = layoutPanel.transform;
							Debug.Log("19");
							s1[t1_curr_index].transform.localScale = Vector3.one;

							resError("\n> ERROR: You can't wash without a station, shampoo, conditioner, and a towel.\n\n", 1);
						}

						t1_did_wash = true;

					} else if (b1[t1_curr_index].Substring(11, 5) == "groom") {

						if (!t1_has_brush || !t1_has_clippers) {
							simulationTextArea.text += "<color=red>" + b1 [t1_curr_index] + "</color>";

							String actionText = s1[t1_curr_index].transform.Find("ActionText").GetComponent<Text>().text;
							s1[t1_curr_index].transform.Find("ActionText").GetComponent<Text>().text = "<color=red>" + actionText + "</color>";
							s1[t1_curr_index].transform.parent = layoutPanel.transform;
							Debug.Log("20");
							s1[t1_curr_index].transform.localScale = Vector3.one;

							resError("\n> ERROR: You can't groom without a brush and some nail clippers.\n\n", 1);
						}

						t1_did_groom = true;

					} else if (b1[t1_curr_index].Substring(11, 7) == "checkin") {

						if (t1_checkedin) {
							simulationTextArea.text += "<color=red>" + b1 [t1_curr_index] + "</color>";

							String actionText = s1[t1_curr_index].transform.Find("ActionText").GetComponent<Text>().text;
							s1[t1_curr_index].transform.Find("ActionText").GetComponent<Text>().text = "<color=red>" + actionText + "</color>";
							s1[t1_curr_index].transform.parent = layoutPanel.transform;
							Debug.Log("21");
							s1[t1_curr_index].transform.localScale = Vector3.one;

							resError("\n> ERROR: You are already checked in. You have to check out before attempting to check in a different customer.\n\n", 1);
						} else {
							t1_checkedin = true;
							t1_checkedout = false;
						}
					
					} else if (b1[t1_curr_index].Substring(11, 8) == "checkout") {

						if ((t1_needs_cut && !t1_did_cut) || (t1_needs_dry && !t1_did_dry) || (t1_needs_wash && !t1_did_wash) || (t1_needs_groom && !t1_did_groom)) {

							// Debug.Log("worker 1 is missing actions. add them.");

							simulationTextArea.text += "<color=red>" + b1 [t1_curr_index] + "</color>";

							String actionText = s1[t1_curr_index].transform.Find("ActionText").GetComponent<Text>().text;
							s1[t1_curr_index].transform.Find("ActionText").GetComponent<Text>().text = "<color=red>" + actionText + "</color>";
							s1[t1_curr_index].transform.parent = layoutPanel.transform;
							Debug.Log("22");
							s1[t1_curr_index].transform.localScale = Vector3.one;

							resError("\n> ERROR: Seems like worker 1 didn't fulfill all of the customer's requests. Please try again.\n\n", 1);
						
						} else if (t1_has_brush || t1_has_clippers || t1_has_conditioner || t1_has_dryer || t1_has_scissors || t1_has_shampoo || t1_has_station || t1_has_towel) {

							// Debug.Log("worker 1: still have some resources.");

							simulationTextArea.text += "<color=red>" + b1 [t1_curr_index] + "</color>";

							String actionText = s1[t1_curr_index].transform.Find("ActionText").GetComponent<Text>().text;
							s1[t1_curr_index].transform.Find("ActionText").GetComponent<Text>().text = "<color=red>" + actionText + "</color>";
							s1[t1_curr_index].transform.parent = layoutPanel.transform;
							Debug.Log("23");
							s1[t1_curr_index].transform.localScale = Vector3.one;

							resError("\n> ERROR: You need to return all the resources you acquired before checking out.\n\n", 1);
						
						} else if (t1_checkedout) {
							
							// Debug.Log("check in before checking out.");

							simulationTextArea.text += "<color=red>" + b1 [t1_curr_index] + "</color>";

							String actionText = s1[t1_curr_index].transform.Find("ActionText").GetComponent<Text>().text;
							s1[t1_curr_index].transform.Find("ActionText").GetComponent<Text>().text = "<color=red>" + actionText + "</color>";
							s1[t1_curr_index].transform.parent = layoutPanel.transform;
							Debug.Log("24");
							s1[t1_curr_index].transform.localScale = Vector3.one;

							resError("\n> ERROR: You have to check in before attempting to check out a customer.\n\n", 1);

						} else {
							
							t1_checkedin = false;
							t1_checkedout = true;
						}
					}

				} catch { }

				try {

					if (t1_canPrint) {

						if (!err) {
							simulationTextArea.text += "" + b1 [t1_curr_index];
							s1[t1_curr_index].transform.parent = layoutPanel.transform;
							Debug.Log("25");
							s1[t1_curr_index].transform.localScale = Vector3.one;
						}
						t1_curr_index++;
					}

				} catch { }


				// ------------------------------  THREAD 2 ------------------------------

									
				try {

					// {"[null]", "brush" ,"clippers" , "cond.", "dryer", "scissors", "shampoo", "station", "towel"};

					if (b2[t2_curr_index].Substring(11, 3) == "acq") {

						// Debug.Log("ACQUIRING " + b2[t2_curr_index].Substring(21, 5));

						// acquiring resource
						switch(b2[t2_curr_index].Substring(21, 5)) {

						case "brush":

							if (!t2_has_brush && t1_has_brush) { // need to wait for resource

								// simDisplay("[thread 2] Waiting for a brush...\n");

								GameObject newItem = Instantiate(simulationImagePrefab) as GameObject;
								newItem.transform.FindChild ("Icon").GetComponent<Image>().sprite = actionsSprites[6]; // clock sprite
								newItem.transform.FindChild ("ItemAction").GetComponent<Image>().sprite = itemsSprites[0]; // brush sprite
								newItem.transform.FindChild ("ActionText").GetComponent<Text>().text = "<color=red>Waiting for a brush...</color>"; // clock sprite
								newItem.transform.parent = layoutPanel2.transform;
								Debug.Log("26");
								newItem.transform.localScale = Vector3.one;

								t2_canPrint = false;

							} else {
								
								int output1 = acquire (ref t2_has_brush);
								t2_canPrint = true;
								// lost = false;

								if (output1 < 0) {
									// resError(b2[t2_curr_index]);
									resError("\n> ERROR: You are trying to acquire a resource you already have.", 2);
								}
							}

							break;

						case "clipp":

							if (!t2_has_clippers && t1_has_clippers) { // need to wait for resource

								//simDisplay("[thread 2] Waiting for nail clippers...\n");

								GameObject newItem = Instantiate(simulationImagePrefab) as GameObject;
								newItem.transform.FindChild ("Icon").GetComponent<Image>().sprite = actionsSprites[6]; // clock sprite
								newItem.transform.FindChild ("ItemAction").GetComponent<Image>().sprite = itemsSprites[1]; // nail clippers sprite
								newItem.transform.FindChild ("ActionText").GetComponent<Text>().text = "<color=red>Waiting for nail clippers...</color>"; // clock sprite
								newItem.transform.parent = layoutPanel2.transform;
								Debug.Log("27");
								newItem.transform.localScale = Vector3.one;

								t2_canPrint = false;

							} else {

								int output2 = acquire (ref t2_has_clippers);
								t2_canPrint = true;
								// lost = false;

								if (output2 < 0) {
									// resError(b2[t2_curr_index]);
									resError("\n> ERROR: You are trying to acquire a resource you already have.", 2);
								}
							}

							break;

						case "cond.":

							if (!t2_has_conditioner && t1_has_conditioner) { // need to wait for resource

								// simDisplay("[thread 2] Waiting for conditioner...\n");

								GameObject newItem = Instantiate(simulationImagePrefab) as GameObject;
								newItem.transform.FindChild ("Icon").GetComponent<Image>().sprite = actionsSprites[6]; // clock sprite
								newItem.transform.FindChild ("ItemAction").GetComponent<Image>().sprite = itemsSprites[2]; // conditioner sprite
								newItem.transform.FindChild ("ActionText").GetComponent<Text>().text = "<color=red>Waiting for conditioner...</color>"; // clock sprite
								newItem.transform.parent = layoutPanel2.transform;
								Debug.Log("28");
								newItem.transform.localScale = Vector3.one;

								t2_canPrint = false;

							} else {
								int output3 = acquire (ref t2_has_conditioner);
								t2_canPrint = true;
								// lost = false;

								if (output3 < 0) {
									// resError(b2[t2_curr_index]);
									resError("\n> ERROR: You are trying to acquire a resource you already have.", 2);
								}
							}

							break;

						case "dryer":

							if (!t2_has_dryer && t1_has_dryer) { // need to wait for resource
								
								// simDisplay("[thread 2] Waiting for dryer...\n");

								GameObject newItem = Instantiate(simulationImagePrefab) as GameObject;
								newItem.transform.FindChild ("Icon").GetComponent<Image>().sprite = actionsSprites[6]; // clock sprite
								newItem.transform.FindChild ("ItemAction").GetComponent<Image>().sprite = itemsSprites[3]; // dryer sprite
								newItem.transform.FindChild ("ActionText").GetComponent<Text>().text = "<color=red>Waiting for dryer...</color>"; // clock sprite
								newItem.transform.parent = layoutPanel2.transform;
								Debug.Log("29");
								newItem.transform.localScale = Vector3.one;

								t2_canPrint = false;

							} else {
								
								int output4 = acquire (ref t2_has_dryer);
								t2_canPrint = true;
								// lost = false;

								if (output4 < 0) {
									// resError(b2[t2_curr_index]);
									resError("\n> ERROR: You are trying to acquire a resource you already have.", 2);
								}
							}

							break;

						case "sciss":

							if (!t2_has_scissors && t1_has_scissors) { // need to wait for resource

								// simDisplay("[thread 2] Waiting for scissors...\n");

								GameObject newItem = Instantiate(simulationImagePrefab) as GameObject;
								newItem.transform.FindChild ("Icon").GetComponent<Image>().sprite = actionsSprites[6]; // clock sprite
								newItem.transform.FindChild ("ItemAction").GetComponent<Image>().sprite = itemsSprites[4]; // scissors sprite
								newItem.transform.FindChild ("ActionText").GetComponent<Text>().text = "<color=red>Waiting for scissors...</color>"; // clock sprite
								newItem.transform.parent = layoutPanel2.transform;
								Debug.Log("30");
								newItem.transform.localScale = Vector3.one;

								t2_canPrint = false;

							} else {
								
								int output5 = acquire (ref t2_has_scissors);
								t2_canPrint = true;
								// lost = false;

								if (output5 < 0) {
									// resError(b2[t2_curr_index]);
									resError("\n> ERROR: You are trying to acquire a resource you already have.", 2);
								}
							}

							break;

						case "shamp":

							if (!t2_has_shampoo && t1_has_shampoo) { // need to wait for resource

								// simDisplay("[thread 2] Waiting for shampoo...\n");

								GameObject newItem = Instantiate(simulationImagePrefab) as GameObject;
								newItem.transform.FindChild ("Icon").GetComponent<Image>().sprite = actionsSprites[6]; // clock sprite
								newItem.transform.FindChild ("ItemAction").GetComponent<Image>().sprite = itemsSprites[5]; // shampoo sprite
								newItem.transform.FindChild ("ActionText").GetComponent<Text>().text = "<color=red>Waiting for shampoo...</color>"; // clock sprite
								newItem.transform.parent = layoutPanel2.transform;
								Debug.Log("31");
								newItem.transform.localScale = Vector3.one;

								t2_canPrint = false;

							} else {

								int output6 = acquire (ref t2_has_shampoo);
								t2_canPrint = true;
								// lost = false;

								if (output6 < 0) {
									// resError(b2[t2_curr_index]);
									resError("\n> ERROR: You are trying to acquire a resource you already have.", 2);
								}
							}

							break;

						case "stati":

							if (!t2_has_station && t1_has_station) { // need to wait for resource

								// simDisplay("[thread 2] Waiting for a station...\n");

								GameObject newItem = Instantiate(simulationImagePrefab) as GameObject;
								newItem.transform.FindChild ("Icon").GetComponent<Image>().sprite = actionsSprites[6]; // clock sprite
								newItem.transform.FindChild ("ItemAction").GetComponent<Image>().sprite = itemsSprites[6]; // station sprite
								newItem.transform.FindChild ("ActionText").GetComponent<Text>().text = "<color=red>Waiting for a station...</color>"; // clock sprite
								newItem.transform.parent = layoutPanel2.transform;
								Debug.Log("32");
								newItem.transform.localScale = Vector3.one;

								t2_canPrint = false;

							} else {

								int output7 = acquire (ref t2_has_station);
								t2_canPrint = true;
								// lost = false;

								if (output7 < 0) {
									// resError(b2[t2_curr_index]);
									resError("\n> ERROR: You are trying to acquire a resource you already have.", 2);
								}
							}

							break;

						case "towel":

							if (!t2_has_towel && t1_has_towel) { // need to wait for resource

								// simDisplay("[thread 2] Waiting for a towel...\n");

								GameObject newItem = Instantiate(simulationImagePrefab) as GameObject;
								newItem.transform.FindChild ("Icon").GetComponent<Image>().sprite = actionsSprites[6]; // clock sprite
								newItem.transform.FindChild ("ItemAction").GetComponent<Image>().sprite = itemsSprites[7]; // towel sprite
								newItem.transform.FindChild ("ActionText").GetComponent<Text>().text = "<color=red>Waiting for a towel...</color>"; // clock sprite
								newItem.transform.parent = layoutPanel2.transform;
								Debug.Log("33");
								newItem.transform.localScale = Vector3.one;

								t2_canPrint = false;

							} else {
								int output8 = acquire (ref t2_has_towel);
								t2_canPrint = true;
								// lost = false;

								if (output8 < 0) {
									// resError(b2[t2_curr_index]);
									resError("\n> ERROR: You are trying to acquire a resource you already have.", 2);
								}
							}

							break;
						}

					} else if (b2[t2_curr_index].Substring(11, 3) == "ret") {

						// Debug.Log("RETURNING " + b2[t2_curr_index].Substring(20, 5));

						// returning resource
						switch(b2[t2_curr_index].Substring(20, 5)) {

						case "brush":
							int output1 = return_res (ref t2_has_brush);
							if (output1 < 0) {
								// resError(b2[t2_curr_index]);
								resError(returnErrMsg, 2);
							}
							break;

						case "clipp":
							int output2 = return_res (ref t2_has_clippers);
							if (output2 < 0) {
								// resError(b2[t2_curr_index]);
								resError(returnErrMsg, 2);
							}
							break;

						case "cond.":
							int output3 = return_res (ref t2_has_conditioner);

							if (output3 < 0) {
								// resError(b2[t2_curr_index]);
								resError(returnErrMsg, 2);
							}

							break;

						case "dryer":
							int output4 = return_res (ref t2_has_dryer);

							if (output4 < 0) {
								// resError(b2[t2_curr_index]);
								resError(returnErrMsg, 2);
							}

							break;

						case "sciss":
							int output5 = return_res (ref t2_has_scissors);

							if (output5 < 0) {
								// resError(b2[t2_curr_index]);
								resError(returnErrMsg, 2);
							}

							break;

						case "shamp":
							int output6 = return_res (ref t2_has_shampoo);

							if (output6< 0) {
								// resError(b2[t2_curr_index]);
								resError(returnErrMsg, 2);
							}

							break;

						case "stati":
							int output7 = return_res (ref t2_has_station);
							if (output7 < 0) {
								// resError(b2[t2_curr_index]);
								resError(returnErrMsg, 2);
							}
							break;

						case "towel":
							int output8 = return_res (ref t2_has_towel);
							if (output8 < 0) {
								// resError(b2[t2_curr_index]);
								resError(returnErrMsg, 2);
							}
							break;
						}

					} else if (b2[t2_curr_index].Substring(11, 3) == "cut") {
						
						if (!t2_has_brush || !t2_has_scissors) {

							simulationTextArea.text += "<color=red>" + b2 [t2_curr_index] + "</color>";

							String actionText = s2[t2_curr_index].transform.Find("ActionText").GetComponent<Text>().text;
							s2[t2_curr_index].transform.Find("ActionText").GetComponent<Text>().text = "<color=red>" + actionText + "</color>";
							s2[t2_curr_index].transform.parent = layoutPanel2.transform;
							Debug.Log("34");
							s2[t2_curr_index].transform.localScale = Vector3.one;

							resError("\n> ERROR: You can't cut without a brush and some scissors.", 2);
						}

						t2_did_cut = true;

					} else if (b2[t2_curr_index].Substring(11, 3) == "dry") {

						if (!t2_has_station || !t2_has_dryer || !t2_has_towel) {
							simulationTextArea.text += "<color=red>" + b2 [t2_curr_index] + "</color>";

							String actionText = s2[t2_curr_index].transform.Find("ActionText").GetComponent<Text>().text;
							s2[t2_curr_index].transform.Find("ActionText").GetComponent<Text>().text = "<color=red>" + actionText + "</color>";
							s2[t2_curr_index].transform.parent = layoutPanel2.transform;
							Debug.Log("35");
							s2[t2_curr_index].transform.localScale = Vector3.one;

							resError("\n> ERROR: You can't dry without a station, a dryer and a towel.\n\n", 2);
						}

						t2_did_dry = true;

					} else if (b2[t2_curr_index].Substring(11, 4) == "wash") {

						if (!t2_has_station || !t2_has_shampoo || !t2_has_towel || !t2_has_conditioner) {
							simulationTextArea.text += "<color=red>" + b2 [t2_curr_index] + "</color>";

							String actionText = s2[t2_curr_index].transform.Find("ActionText").GetComponent<Text>().text;
							s2[t2_curr_index].transform.Find("ActionText").GetComponent<Text>().text = "<color=red>" + actionText + "</color>";
							s2[t2_curr_index].transform.parent = layoutPanel2.transform;
							Debug.Log("36");
							s2[t2_curr_index].transform.localScale = Vector3.one;

							resError("\n> ERROR: You can't wash without a station, shampoo, conditioner, and a towel.\n\n", 2);
						}

						t2_did_wash = true;
					
					} else if (b2[t2_curr_index].Substring(11, 5) == "groom") {

						if (!t2_has_brush || !t2_has_clippers) {
							simulationTextArea.text += "<color=red>" + b2 [t2_curr_index] + "</color>";

							String actionText = s2[t2_curr_index].transform.Find("ActionText").GetComponent<Text>().text;
							s2[t2_curr_index].transform.Find("ActionText").GetComponent<Text>().text = "<color=red>" + actionText + "</color>";
							s2[t2_curr_index].transform.parent = layoutPanel2.transform;
							Debug.Log("37");
							s2[t2_curr_index].transform.localScale = Vector3.one;

							resError("\n> ERROR: You can't groom without a brush and some nail clippers.\n\n", 2);
						}

						t2_did_groom = true;

					} else if (b2[t2_curr_index].Substring(11, 7) == "checkin") {

						if (t2_checkedin) {
							simulationTextArea.text += "<color=red>" + b2 [t2_curr_index] + "</color>";

							String actionText = s2[t2_curr_index].transform.Find("ActionText").GetComponent<Text>().text;
							s2[t2_curr_index].transform.Find("ActionText").GetComponent<Text>().text = "<color=red>" + actionText + "</color>";
							s2[t2_curr_index].transform.parent = layoutPanel2.transform;
							Debug.Log("38");
							s2[t2_curr_index].transform.localScale = Vector3.one;

							resError("\n> ERROR: You are already checked in. You have to check out before attempting to check in a different customer.\n\n", 2);
						} else {
							t2_checkedin = true;
							t2_checkedout = false;
						}
					
					} else if (b2[t2_curr_index].Substring(11, 8) == "checkout") {

						if ((t2_needs_cut && !t2_did_cut) || (t2_needs_dry && !t2_did_dry) || (t2_needs_wash && !t2_did_wash) || (t2_needs_groom && !t2_did_groom)) {

							// Debug.Log("worker 2 is missing actions. add them.");

							simulationTextArea.text += "<color=red>" + b2 [t2_curr_index] + "</color>";

							String actionText = s2[t2_curr_index].transform.Find("ActionText").GetComponent<Text>().text;
							s2[t2_curr_index].transform.Find("ActionText").GetComponent<Text>().text = "<color=red>" + actionText + "</color>";
							s2[t2_curr_index].transform.parent = layoutPanel2.transform;
							Debug.Log("39");
							s2[t2_curr_index].transform.localScale = Vector3.one;

							resError("\n> ERROR: Seems like worker 2 didn't fulfill all of the customer's requests. Please try again.\n\n", 2);

						} else if (t2_has_brush || t2_has_clippers || t2_has_conditioner || t2_has_dryer || t2_has_scissors || t2_has_shampoo || t2_has_station || t2_has_towel) {

							// Debug.Log("worker 2 hasnt returned all resources.");

							simulationTextArea.text += "<color=red>" + b2 [t2_curr_index] + "</color>";

							String actionText = s2[t2_curr_index].transform.Find("ActionText").GetComponent<Text>().text;
							s2[t2_curr_index].transform.Find("ActionText").GetComponent<Text>().text = "<color=red>" + actionText + "</color>";
							s2[t2_curr_index].transform.parent = layoutPanel2.transform;
							Debug.Log("40");
							s2[t2_curr_index].transform.localScale = Vector3.one;

							resError("\n> ERROR: You need to return all the resources you acquired before checking out.\n\n", 2);

						} else if (t2_checkedout) {

							// Debug.Log("worker 2 needs to check in before checking out.");

							simulationTextArea.text += "<color=red>" + b2 [t2_curr_index] + "</color>";

							String actionText = s2[t2_curr_index].transform.Find("ActionText").GetComponent<Text>().text;
							s2[t2_curr_index].transform.Find("ActionText").GetComponent<Text>().text = "<color=red>" + actionText + "</color>";
							s2[t2_curr_index].transform.parent = layoutPanel2.transform;
							Debug.Log("41");
							s2[t2_curr_index].transform.localScale = Vector3.one;

							resError("\n> ERROR: You have to check in before attempting to check out a customer.\n\n", 2);

						} else {
							
							t2_checkedin = false;
							t2_checkedout = true;
							lost = false;
						}
					}
						
				} catch { }

				try {

					if (t2_canPrint) {
						if (!err) {
							simulationTextArea.text += "" + b2 [t2_curr_index];
							s2[t2_curr_index].transform.parent = layoutPanel2.transform;
							Debug.Log("42");
							s2[t2_curr_index].transform.localScale = Vector3.one;
						}
						t2_curr_index++;
					}

				} catch { }

				j++; // increment step

				Canvas.ForceUpdateCanvases();
				simulationScrollRect.verticalNormalizedPosition = 0f;
				Canvas.ForceUpdateCanvases ();

				yield return new WaitForSeconds (1);
			}
		}

		// Debug.Log ("lost?: " + lost);

		if (!lost) {
			manager.gameWon ();
		}

		Debug.Log ("Finished in " + j + " steps.");
	}

	int acquire(ref bool resource) {

		if (resource) {

			err = true;
			lost = true;
			stop = true;
			paused = true;

			// resError("\n> ERROR: You are trying to acquire a resource you already have.");

			return -1;

		} else {
			
			resource = true;
			return 0;
		}

	}

	int return_res(ref bool resource) {

		if (!resource) {

			err = true;
			lost = true;
			stop = true;
			paused = true;

			// resError("\n> ERROR: You are trying to return a resource you don't have.");

			return -1;

		} else {
			resource = false;

			return 0;
		}

	}

	void resError(String msg, int thread_num) {

		err = true;
		lost = true;
		stop = true;
		paused = true;

		// simulationTextArea.text += "\n<color=red>" + msg + "</color>";
		simDisplay (msg, thread_num);

		// terminateSimulation ();

		try {
			disablePanel.SetActive (false);
		} catch {
			Debug.Log ("Cannot disable DisablePanel.");
		}

		runButton.transform.SetAsLastSibling ();
		bar.LoadingBar.GetComponent<Image> ().fillAmount = 0;

	}

	void simDisplay(String msg, int thread_num) {

		stepsIndicator.text = "0";

		Transform newItemParent;

		if (thread_num == 1)
			newItemParent = layoutPanel.transform;
		else // thread 2
			newItemParent = layoutPanel2.transform;

		lost = true;
		simulationTextArea.text += "<color=red>" + msg + "</color>";

		GameObject newItem = Instantiate(simulationErrorPrefab) as GameObject;
		newItem.transform.FindChild ("ActionText").GetComponent<Text>().text = "<color=red>" + msg + "</color>";
		newItem.transform.parent = newItemParent;
		Debug.Log("43");
		newItem.transform.localScale = Vector3.one;

	}

	void clearVerticalLayout() {

		stepsIndicator.text = "0";

		//layoutPanel
		foreach (Transform child in layoutPanel.transform) {
			GameObject.Destroy(child.gameObject);
		}
	}

	void clearVerticalLayout2() {

		stepsIndicator.text = "0";

		//layoutPanel
		foreach (Transform child in layoutPanel2.transform) {
			GameObject.Destroy(child.gameObject);
		}
	}
}
