using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class TrumpController : MonoBehaviour {

	private Rigidbody trump;
	private Animation trumpAnimation;
	private string toastString;
	private string defaultAnimation;

	void Start () {
		trump = GetComponent<Rigidbody> ();
		trumpAnimation = GetComponent<Animation> ();
		toastString = null;
		defaultAnimation = "idle";
	}
	
	void Update () {
		//Getting user input for moving Trump
		float x = CrossPlatformInputManager.GetAxis ("Horizontal");
		float y = CrossPlatformInputManager.GetAxis ("Vertical");

		//Change default animation on Click.
		if (CrossPlatformInputManager.GetButtonDown ("Jump")) {
			if (defaultAnimation == "idle")
				defaultAnimation = "china_walk";
			else
				defaultAnimation = "idle";
			
			showToast ("Animation changed to : " +defaultAnimation);	
		}

		//Add velocity to Trump
		Vector3 unitVelocity = new Vector3 (x,0,y);
		trump.velocity = unitVelocity * 0.2f;

		//Rotation for Trump
		if (x != 0 && y != 0) {
			transform.eulerAngles = new Vector3 (transform.eulerAngles.x, Mathf.Atan2(x,y) * Mathf.Rad2Deg, transform.eulerAngles.z);
		}

		//Play animation
		if (x != 0 || y != 0) {
			trumpAnimation.Play ("walk");
		} else {
			trumpAnimation.Play (defaultAnimation);
		}

		//Check for Back button
		OnBackPressed();
	}

	private void OnBackPressed(){
		if (Application.platform == RuntimePlatform.Android) {
			if (Input.GetKey (KeyCode.Escape)) {
				Application.Quit();
				return;
			}
		}
	}

	//Displays Toast on Android Platform.
	AndroidJavaObject currentActivity;
	AndroidJavaObject  context;

	private void showToast(string toastString){
		if (Application.platform == RuntimePlatform.Android && toastString != null)
		{
			AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
			context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");

			this.toastString = toastString;
			currentActivity.Call ("runOnUiThread", new AndroidJavaRunnable(showToastOnUI));
		}
	}
	private void showToastOnUI(){
		Debug.Log(this + " : Running on UI thread");

		AndroidJavaClass Toast = new AndroidJavaClass("android.widget.Toast");
		AndroidJavaObject javaString = new AndroidJavaObject("java.lang.String", toastString);
		AndroidJavaObject toast = Toast.CallStatic<AndroidJavaObject>("makeText", context, javaString, Toast.GetStatic<int>("LENGTH_SHORT"));
		toast.Call("show");
	}
}
