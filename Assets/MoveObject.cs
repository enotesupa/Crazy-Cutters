using UnityEngine;
using System.Collections;

public class MoveObject : MonoBehaviour {

	private struct vecbool{
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
    public GameObject picture;
    private GameObject copied_picture;

	// Use this for initialization
	void Start () {
        cols = rows;
		vecmap = new vecbool[cols, rows];
		objects = new GameObject[cols, rows];
		for (int i = 0; i < cols; i++) {
			for (int j = 0; j < rows; j++) {
				vecmap[i,j].is_stepped = false;
				vecmap[i,j].pos = new Vector2(-(float)cols/cols/2f +(float)i/ expansion_rate,
                    rows/ expansion_rate - (float)j/ expansion_rate);
				objects[i,j] = Instantiate(origin) as GameObject;
				objects[i,j].transform.position = new Vector3(vecmap[i,j].pos.x, vecmap[i,j].pos.y, transform.position.z+1);
				objects[i,j].name = i+" "+j;
			}
		}
        copied_picture = Instantiate(picture) as GameObject;
        Debug.Log(copied_picture.transform.localScale);
        copied_picture.transform.position = new Vector3(vecmap[0, 0].pos.x, vecmap[0, 0].pos.y, transform.position.z + 1);
        Vector3 tmpvec = copied_picture.GetComponent<SpriteRenderer>().bounds.size;
        copied_picture.transform.localScale = new Vector3(
            Mathf.Abs(vecmap[cols - 1, rows - 1].pos.x - vecmap[0, 0].pos.x) / tmpvec.x,
            Mathf.Abs(vecmap[cols - 1, rows - 1].pos.y - vecmap[0, 0].pos.y) / tmpvec.y,
            transform.localScale.z);
    }

	void OnTriggerEnter2D(Collider2D other) {
		string[] strarg = other.name.Split(' ');
        if(vecmap[int.Parse(strarg[0]), int.Parse(strarg[1])].is_stepped == false)
        {
            points++;
            vecmap[int.Parse(strarg[0]), int.Parse(strarg[1])].is_stepped = true;
        }
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.UpArrow)) {
			transform.position = new Vector3(transform.position.x,transform.position.y+0.1f,transform.position.z);
		}
		if (Input.GetKey (KeyCode.DownArrow)) {
			transform.position = new Vector3(transform.position.x,transform.position.y-0.1f,transform.position.z);
		}
		if (Input.GetKey (KeyCode.RightArrow)) {
			transform.position = new Vector3(transform.position.x+0.1f,transform.position.y,transform.position.z);
		}
		if (Input.GetKey (KeyCode.LeftArrow)) {
			transform.position = new Vector3(transform.position.x-0.1f,transform.position.y,transform.position.z);
		}
	}
}
