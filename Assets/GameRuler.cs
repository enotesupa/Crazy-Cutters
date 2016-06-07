using UnityEngine;
using System.Collections;

public class GameRuler : MonoBehaviour {

    // vector of (plus, minus, sum)
    public Vector3 p1_points;
    public Vector3 p2_points;
    public Vector3 p3_points;
    public Vector3 p4_points;

    private bool is_called_once = false;
    private bool is_finished = false;

    public GameObject p1_obj;
    public GameObject p2_obj;
    public GameObject p3_obj;
    public GameObject p4_obj;

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update ()
    {
        if (is_finished && !is_called_once)
        {
            // calculating phase
            p1_points.x = p1_obj.GetComponent<MoveObject>().points;
            p2_points.x = p2_obj.GetComponent<MoveObject>().points;
            p3_points.x = p3_obj.GetComponent<MoveObject>().points;
            p4_points.x = p4_obj.GetComponent<MoveObject>().points;

            p1_obj.GetComponent<MoveObject>().Freeze();
            p2_obj.GetComponent<MoveObject>().Freeze();
            p3_obj.GetComponent<MoveObject>().Freeze();
            p4_obj.GetComponent<MoveObject>().Freeze();

            Debug.Log("Finished!");
            is_called_once = true;
        }
    }

    void OnGUI()
    {
        float fps = 1f / Time.deltaTime;
        GUILayout.BeginHorizontal("box");
        GUILayout.Box("FPS:" + fps.ToString() + "  ( " + 1.0f / fps * 1000.0f + " ms)", GUILayout.Width(200));
        GUILayout.EndHorizontal();
    }
}
