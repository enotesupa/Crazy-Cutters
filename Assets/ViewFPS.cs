using UnityEngine;
using System.Collections;

public class ViewFPS : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI(){
		float fps = 1f / Time.deltaTime;
		GUILayout.BeginHorizontal ("box");
		GUILayout.Box("FPS:" + fps.ToString() + "  ( " + 1.0f / fps * 1000.0f + " ms)", GUILayout.Width(200));
		GUILayout.EndHorizontal ();
	}
}
