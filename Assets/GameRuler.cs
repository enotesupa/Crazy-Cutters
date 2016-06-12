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

    public float timer = 60f;

    private GUIStyle m_guiStyle;

    // Use this for initialization
    void Start ()
    {
        m_guiStyle = new GUIStyle();
        m_guiStyle.fontSize = 30;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(!is_finished)
            timer -= 0.01f;
        is_finished = (timer <= 0);
        if (is_finished && !is_called_once)
        {
            // calculating phase
            p1_points.x = p1_obj.GetComponent<MoveObject>().points;
            p2_points.x = p2_obj.GetComponent<MoveObject>().points;
            p3_points.x = p3_obj.GetComponent<MoveObject>().points;
            p4_points.x = p4_obj.GetComponent<MoveObject>().points;

            p1_obj.GetComponent<MoveObject>().forcedlyFreeze();
            p2_obj.GetComponent<MoveObject>().forcedlyFreeze();
            p3_obj.GetComponent<MoveObject>().forcedlyFreeze();
            p4_obj.GetComponent<MoveObject>().forcedlyFreeze();

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
        if(timer > 0)
        {
            GUI.Label(new Rect(Screen.width / 2,
                Screen.height / 2,
                Screen.width / 2,
                Screen.height / 2), "" + (int)(timer+1f), m_guiStyle);
        }
        else
        {
            GUI.Label(new Rect(Screen.width / 2,
                Screen.height / 2,
                Screen.width / 2,
                Screen.height / 2), "Finish!!", m_guiStyle);
        }
    }
}
