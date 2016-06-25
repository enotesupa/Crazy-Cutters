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

    private int cols = 40;
    private int rows = 40;
    private float expansion_rate = 2f;

    public float points = 0;
	//public int current_img = -1;
    public int player_num = 0;

    public GameObject picture;
    private GameObject copied_picture;

    private bool is_finished = false;
    private bool is_finished2 = false;

	public int csv_mode = 0;
	
	private GUIStyle m_guiStyle;
	public GameObject gameruler;
	private GameRuler gr;
	private TrailRenderer tr;
	private MoveRestrictor mr;

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
		m_guiStyle = new GUIStyle();
		m_guiStyle.fontSize = 40;
		m_guiStyle.normal.textColor = new Color(255f/255f, 80f/255f, 0);

		gr = gameruler.GetComponent<GameRuler> ();
		tr = GetComponent<TrailRenderer>();
		mr = GetComponent<MoveRestrictor>();
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
					} else if (csv_mode == 2) {// compare mode	
						float matched = 0f;
						float unmatched = 0f;
						for (int i = 0; i < cols; i++) {
							for (int j = 0; j < rows; j++) {
								if(gr.getAnswer(0,i, j) == vecmap [i, j].is_stepped){
									matched += 1f;
								}else{
									unmatched += 1f;
								}
								vecmap [i, j].is_stepped = false;
							}
						}
						points += (100f*(matched-unmatched)/matched);
						Debug.Log("finished writing");
						Debug.Log("result "+points);
					} else {// do nothing
						Debug.Log("mode none");
					}
					//current_img = UnityEngine.Random.Range(0,19);
					//copied_picture.GetComponent<SpriteRenderer>().sprite = gr.getImage(current_img);
					tr.Reset(this);
					is_finished2 = is_finished = false;
					mr.MoveAgain();
                }
            }
            else
            {
                string[] strarg = other.name.Split(' ');
                if (vecmap[int.Parse(strarg[0]), int.Parse(strarg[1])].is_stepped == false)
                {
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
		switch (player_num) {
		case 1:
			GUI.Label(new Rect(Screen.width / 2.35f,
			                   Screen.height / 2.6f,
			                   Screen.width / 2,
			                   Screen.height / 2), ""+(int)points, m_guiStyle);
			break;
		case 2:
			GUI.Label(new Rect(Screen.width / 1.875f,
			                   Screen.height / 2.6f,
			                   Screen.width / 2,
			                   Screen.height / 2), ""+(int)points, m_guiStyle);
			break;
		case 3:
			GUI.Label(new Rect(Screen.width / 2.35f,
			                   Screen.height / 1.85f,
			                   Screen.width / 2,
			                   Screen.height / 2), ""+(int)points, m_guiStyle);
			break;
		case 4:
			GUI.Label(new Rect(Screen.width / 1.875f,
			                   Screen.height / 1.85f,
			                   Screen.width / 2,
			                   Screen.height / 2), ""+(int)points, m_guiStyle);
			break;
		default:
			break;
		}
    }
}