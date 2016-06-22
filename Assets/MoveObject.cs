using UnityEngine;
using System.Collections;
using GamepadInput;
using System;

public static class TrainRendererExtensions
{
	/// <summary>
	/// Reset the trail so it can be moved without streaking
	/// </summary>
	public static void Reset(this TrailRenderer trail, MonoBehaviour instance)
	{
		instance.StartCoroutine(ResetTrail(trail));   
	}
	
	/// <summary>
	/// Coroutine to reset a trail renderer trail
	/// </summary>
	/// <param name="trail"></param>
	/// <returns></returns>
	static IEnumerator ResetTrail(TrailRenderer trail)
	{
		var trailTime = trail.time;
		trail.time = 0;
		yield return 0;
		trail.time = trailTime;
	}        
}

public class MoveObject : MonoBehaviour
{

    private struct vecbool
    {
        public Vector2 pos;
        public bool is_stepped;
    };

    private vecbool[,] vecmap;

    public GameObject origin;
    private GameObject[,] objects;

    public int cols = 40;
    public int rows = 40;
    public float expansion_rate = 2f;

    public int points = 0;
    public int player_num = 0;

    public GameObject picture;
    private GameObject copied_picture;

    private bool is_finished = false;
    private bool is_finished2 = false;

	public int csv_mode = 0;
	private bool[,,] answers;

    public void Freeze()
    {
        is_finished = true;
    }

    public void forcedlyFreeze()
    {
        is_finished = true;
        is_finished2 = true;
    }

    // Use this for initialization
    void Start()
    {
        cols = rows;
        vecmap = new vecbool[cols, rows];
        objects = new GameObject[cols, rows];
        for (int i = 0; i < cols; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                vecmap[i, j].is_stepped = false;
                vecmap[i, j].pos = new Vector2(-(float)cols / cols / 2f + (float)i / expansion_rate,
                    rows / expansion_rate - (float)j / expansion_rate);
                objects[i, j] = Instantiate(origin) as GameObject;
                objects[i, j].transform.position = new Vector3(vecmap[i, j].pos.x, vecmap[i, j].pos.y, transform.position.z + 1);
                objects[i, j].name = i + " " + j + " " + player_num;
            }
        }
        copied_picture = Instantiate(picture) as GameObject;
        copied_picture.transform.position = new Vector3(vecmap[0, 0].pos.x, vecmap[0, 0].pos.y, transform.position.z + 1);
        Vector3 tmpvec = copied_picture.GetComponent<SpriteRenderer>().bounds.size;
        copied_picture.transform.localScale = new Vector3(
            Mathf.Abs(vecmap[cols - 1, rows - 1].pos.x - vecmap[0, 0].pos.x) / tmpvec.x,
            Mathf.Abs(vecmap[cols - 1, rows - 1].pos.y - vecmap[0, 0].pos.y) / tmpvec.y,
            transform.localScale.z);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        try
        {
            if (other.name == "initial " + player_num)
            {
                if (is_finished)
                {
					is_finished2 = true;
					if (csv_mode == 1) {// write mode
						System.IO.StreamWriter sw = new System.IO.StreamWriter (
							@"teacher.csv", false);
						string teacher_data = "";
						for (int i = 0; i < cols; i++) {
							for (int j = 0; j < rows; j++) {
								teacher_data += (vecmap [i, j].is_stepped) ? "1" : "0";
								teacher_data += (j == rows - 1) ? "" : ",";
							}
							sw.WriteLine (teacher_data);
							teacher_data = "";
						}
						sw.Close();
						Debug.Log("finished writing");
					} else if (csv_mode == 2) {// read mode
						answers = new bool[5,cols, rows];
						System.IO.StreamReader sr = new System.IO.StreamReader (@"teacher.csv");
						int line_num = 0;
						while(!sr.EndOfStream){
							string line = sr.ReadLine();
							string[] values = line.Split(',');
							int value_num = 0;
							foreach(string value in values){
								answers[0,line_num,value_num] = (value == "1");
								value_num++;
							}
							line_num++;
						}
						sr.Close();
						Debug.Log("finished reading");
						
						float matched = 0f;
						float unmatched = 0f;
						for (int i = 0; i < cols; i++) {
							for (int j = 0; j < rows; j++) {
								if(answers [0,i, j] == vecmap [i, j].is_stepped){
									matched += 1f;
								}else{
									unmatched += 1f;
								}
							}
						}
						Debug.Log("finished writing");
						Debug.Log("result "+(100f*(matched-unmatched)/matched));
					} else {// do nothing
						Debug.Log("mode none");
					}
					GetComponent<TrailRenderer>().Reset(this);
					is_finished2 = is_finished = false;
					GetComponent<MoveRestrictor>().MoveAgain();
                }
            }
            else
            {
                string[] strarg = other.name.Split(' ');
                if (vecmap[int.Parse(strarg[0]), int.Parse(strarg[1])].is_stepped == false)
                {
                    points++;
                    vecmap[int.Parse(strarg[0]), int.Parse(strarg[1])].is_stepped = true;
                }
            }
        }
        catch (FormatException)
        {
            // just in case
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!is_finished2)
        {
            switch (player_num)
            {
                case 1:
                    if (GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.One).y > 0)
                    {
                        transform.position = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);
                    }
                    if (GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.One).y < 0)
                    {
                        transform.position = new Vector3(transform.position.x, transform.position.y - 0.1f, transform.position.z);
                    }
                    if (GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.One).x > 0)
                    {
                        transform.position = new Vector3(transform.position.x + 0.1f, transform.position.y, transform.position.z);
                    }
                    if (GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.One).x < 0)
                    {
                        transform.position = new Vector3(transform.position.x - 0.1f, transform.position.y, transform.position.z);
                    }
                    break;
                case 2:
                    if (GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.Two).y > 0)
                    {
                        transform.position = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);
                    }
                    if (GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.Two).y < 0)
                    {
                        transform.position = new Vector3(transform.position.x, transform.position.y - 0.1f, transform.position.z);
                    }
                    if (GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.Two).x > 0)
                    {
                        transform.position = new Vector3(transform.position.x + 0.1f, transform.position.y, transform.position.z);
                    }
                    if (GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.Two).x < 0)
                    {
                        transform.position = new Vector3(transform.position.x - 0.1f, transform.position.y, transform.position.z);
                    }
                    break;
                case 3:
                    if (GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.Three).y > 0)
                    {
                        transform.position = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);
                    }
                    if (GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.Three).y < 0)
                    {
                        transform.position = new Vector3(transform.position.x, transform.position.y - 0.1f, transform.position.z);
                    }
                    if (GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.Three).x > 0)
                    {
                        transform.position = new Vector3(transform.position.x + 0.1f, transform.position.y, transform.position.z);
                    }
                    if (GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.Three).x < 0)
                    {
                        transform.position = new Vector3(transform.position.x - 0.1f, transform.position.y, transform.position.z);
                    }
                    break;
                case 4:
                    if (GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.Four).y > 0)
                    {
                        transform.position = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);
                    }
                    if (GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.Four).y < 0)
                    {
                        transform.position = new Vector3(transform.position.x, transform.position.y - 0.1f, transform.position.z);
                    }
                    if (GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.Four).x > 0)
                    {
                        transform.position = new Vector3(transform.position.x + 0.1f, transform.position.y, transform.position.z);
                    }
                    if (GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.Four).x < 0)
                    {
                        transform.position = new Vector3(transform.position.x - 0.1f, transform.position.y, transform.position.z);
                    }
                    break;
                default:
                    if (Input.GetKey(KeyCode.UpArrow))
                    {
                        transform.position = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);
                    }
                    if (Input.GetKey(KeyCode.DownArrow))
                    {
                        transform.position = new Vector3(transform.position.x, transform.position.y - 0.1f, transform.position.z);
                    }
                    if (Input.GetKey(KeyCode.RightArrow))
                    {
                        transform.position = new Vector3(transform.position.x + 0.1f, transform.position.y, transform.position.z);
                    }
                    if (Input.GetKey(KeyCode.LeftArrow))
                    {
                        transform.position = new Vector3(transform.position.x - 0.1f, transform.position.y, transform.position.z);
                    }
                    break;
            }
        }
    }
    
    void OnGUI()
    {
        if (is_finished2)
        {
            switch(player_num)
            {
                case 1:
                    GUI.Label(new Rect(Screen.width / 4,
                        Screen.height / 4,
                        Screen.width / 4,
                        Screen.height / 4),
                        "Finish!!");
                    break;
                case 2:
                    GUI.Label(new Rect(3*Screen.width / 4,
                        Screen.height / 4,
                        Screen.width / 4,
                        Screen.height / 4),
                        "Finish!!");
                    break;
                case 3:
                    GUI.Label(new Rect(Screen.width / 4,
                        3 * Screen.height / 4,
                        Screen.width / 4,
                        Screen.height / 4),
                        "Finish!!");
                    break;
                case 4:
                    GUI.Label(new Rect(3 * Screen.width / 4,
                        3 * Screen.height / 4,
                        Screen.width / 4,
                        Screen.height / 4),
                        "Finish!!");
                    break;
                default:
                    GUI.Label(new Rect(Screen.width / 4,
                        Screen.height / 4,
                        Screen.width / 4,
                        Screen.height / 4),
                        "Finish!!");
                    break;
            }
        }
    }
}