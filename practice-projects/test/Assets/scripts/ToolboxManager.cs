﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ToolboxManager : MonoBehaviour {

	public int checkinLeft_thread1;
	public int cutLeft_thread1;
	public int dryLeft_thread1;
	public int washLeft_thread1;

	public int whileLeft_thread1;
	public int ifLeft_thread1;

	public Text txt_checkinLeft_thread1;
	public Text txt_cutLeft_thread1;
	public Text txt_dryLeft_thread1;
	public Text txt_washLeft_thread1;

	public Text txt_whileLeft_thread1;
	public Text txt_ifLeft_thread1;

	public int checkinLeft_thread2;
	public int cutLeft_thread2;
	public int dryLeft_thread2;
	public int washLeft_thread2;

	public int whileLeft_thread2;
	public int ifLeft_thread2;

	public Text txt_checkinLeft_thread2;
	public Text txt_cutLeft_thread2;
	public Text txt_dryLeft_thread2;
	public Text txt_washLeft_thread2;

	public Text txt_whileLeft_thread2;
	public Text txt_ifLeft_thread2;

	public Text txtErrorMsg;

	// Use this for initialization
	void Start () {

		//Debug.Log ("Start in manager called");
		txtErrorMsg.enabled = false;

		updateValues ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void updateValues() {
		
		txt_checkinLeft_thread1.text = "x " + checkinLeft_thread1;
		txt_cutLeft_thread1.text = "x " + cutLeft_thread1;
		txt_washLeft_thread1.text = "x " + washLeft_thread1;
		txt_dryLeft_thread1.text = "x " + dryLeft_thread1;
		txt_whileLeft_thread1.text = "x " + whileLeft_thread1;
		txt_ifLeft_thread1.text = "x " + ifLeft_thread1;

		txt_checkinLeft_thread2.text = "x " + checkinLeft_thread2;
		txt_cutLeft_thread2.text = "x " + cutLeft_thread2;
		txt_washLeft_thread2.text = "x " + washLeft_thread2;
		txt_dryLeft_thread2.text = "x " + dryLeft_thread2;
		txt_whileLeft_thread2.text = "x " + whileLeft_thread2;
		txt_ifLeft_thread2.text = "x " + ifLeft_thread2;
	}

	public void showError(string msg) {

		StartCoroutine (ErrorMsg (msg));
		
	}

	IEnumerator ErrorMsg(string msg) {

		txtErrorMsg.enabled = true;
		txtErrorMsg.text = msg;
		yield return new WaitForSeconds(2.0f);
		txtErrorMsg.enabled = false;
	}
}
