using UnityEngine;
using System.Collections;

public class GameRuler : MonoBehaviour {
	public float p1_points;
	public float p2_points;
	public float p3_points;
	public float p4_points;
	
	private bool is_called_once = false;
	private bool is_finished = false;
	
	public GameObject p1_obj;
	public GameObject p2_obj;
	public GameObject p3_obj;
	public GameObject p4_obj;
	
	public float timer = 60f;
	
	private GUIStyle m_guiStyle;
	
	public int cols = 40;
	public int rows = 40;
	public Sprite[] image;
	private bool[,,] answers;
	
	public bool getAnswer(int arg1, int arg2, int arg3){
		return answers [arg1, arg2, arg3];
	}
	
	public Sprite getImage(int arg){
		return image [arg];
	}
	
	void readData(){
		int img_size = image.Length;
		answers = new bool[img_size,cols, rows];
		for (int i = 0; i < img_size; i++) {
			System.IO.StreamReader sr =
				new System.IO.StreamReader (Application.dataPath +"/teacher/teacher_" + i + ".csv");
			int line_num = 0;
			while (!sr.EndOfStream) {
				string line = sr.ReadLine ();
				string[] values = line.Split (',');
				int value_num = 0;
				foreach (string value in values) {
					answers [i, line_num, value_num] = (value == "1");
					value_num++;
				}
				line_num++;
			}
			sr.Close ();
			Debug.Log ("finished reading");
		}
	}
	
	// Use this for initialization
	void Start ()
	{
        //image = new Sprite[19];
		m_guiStyle = new GUIStyle();
		m_guiStyle.fontSize = 80;
		m_guiStyle.normal.textColor = Color.red;
        if(image != null)
            readData ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		timer -= 0.01f;
		is_finished = (timer <= 0);
		if (is_finished && !is_called_once)
		{
			// calculating phase
			p1_points = p1_obj.GetComponent<MoveObject>().points;
			p2_points = p2_obj.GetComponent<MoveObject>().points;
			p3_points = p3_obj.GetComponent<MoveObject>().points;
			p4_points = p4_obj.GetComponent<MoveObject>().points;
			
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
        if (timer > 0)
		{
			GUI.Label(new Rect(Screen.width / 2.15f,
			                   Screen.height / 2.35f,
			                   Screen.width / 2,
			                   Screen.height / 2), "" + (int)(timer+1f), m_guiStyle);
		}
		else
		{
			GUI.Label(new Rect(Screen.width / 2.50f,
			                   Screen.height / 2.35f,
			                   Screen.width / 2,
			                   Screen.height / 2), "Finish!!", m_guiStyle);
		}
	}
}
