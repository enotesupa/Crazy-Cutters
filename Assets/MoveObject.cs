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

	public int cols = 10;
	public int rows = 8;
	public int array_size;
	public int expansion_rate = 1;

    public int points = 0;
    public GameObject picture;
    private GameObject copied_picture;

	//private Rigidbody2D rbody;

	// Use this for initialization
	void Start () {
		//rbody = GetComponent<Rigidbody2D>();
		array_size = cols * rows ;
		vecmap = new vecbool[array_size,array_size];
		objects = new GameObject[cols, rows];
		for (int i = 0; i < cols; i++) {
			for (int j = 0; j < rows; j++) {
				vecmap[i,j].is_stepped = false;
				vecmap[i,j].pos = new Vector2(-(float)cols/20f +(float)i, rows/2f - (float)j );
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

//	void check_footprint(){
//		float min = (cols * cols) + (rows * rows);
//		for (int i = 0; i < array_size; i++) {
//			for (int j = 0; j < array_size; j++) {
//				float tmp_dis = (vecmap[i,j].pos.x) * (vecmap[i,j].pos.x)
//					+ (vecmap[i,j].pos.y) * (vecmap[i,j].pos.y);
//				if(tmp_dis < min){
//					min = tmp_dis;
//				}
//			}
//		}
//		for (int i = 0; i < array_size; i++) {
//			for (int j = 0; j < array_size; j++) {
//				float tmp_dis = (vecmap[i,j].pos.x) * (vecmap[i,j].pos.x)
//					+ (vecmap[i,j].pos.y) * (vecmap[i,j].pos.y);
//				if(tmp_dis == min){
//					if(!vecmap[i,j].is_stepped){
//						Debug.Log(i+", "+j+" ("+vecmap[i,j].pos.x+", "+vecmap[i,j].pos.y+") stepped!");
//					}
//					vecmap[i,j].is_stepped = true;
//					return;
//				}
//			}
//		}
//	}

	void OnTriggerEnter2D(Collider2D other) {
		//Debug.Log(other.transform.position.x+", "+other.transform.position.y+" :"+other.name);
		string[] strarg = other.name.Split(' ');
		//Debug.Log(vecmap[int.Parse(strarg[0]),int.Parse(strarg[1])].is_stepped+"");
        if(vecmap[int.Parse(strarg[0]), int.Parse(strarg[1])].is_stepped == false)
        {
            points++;
            vecmap[int.Parse(strarg[0]), int.Parse(strarg[1])].is_stepped = true;
        }
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.UpArrow)) {
			//rbody.MovePosition(new Vector3(transform.position.x,transform.position.y+0.1f,transform.position.z));
			transform.position = new Vector3(transform.position.x,transform.position.y+0.1f,transform.position.z);
			//check_footprint ();
		}
		if (Input.GetKey (KeyCode.DownArrow)) {
			//rbody.MovePosition(new Vector3(transform.position.x,transform.position.y-0.1f,transform.position.z));
			transform.position = new Vector3(transform.position.x,transform.position.y-0.1f,transform.position.z);
			//check_footprint ();			
		}
		if (Input.GetKey (KeyCode.RightArrow)) {
			//rbody.MovePosition(new Vector3(transform.position.x+0.1f,transform.position.y,transform.position.z));
			transform.position = new Vector3(transform.position.x+0.1f,transform.position.y,transform.position.z);
			//check_footprint ();			
		}
		if (Input.GetKey (KeyCode.LeftArrow)) {
			//rbody.MovePosition(new Vector3(transform.position.x-0.1f,transform.position.y,transform.position.z));
			transform.position = new Vector3(transform.position.x-0.1f,transform.position.y,transform.position.z);
			//check_footprint ();			
		}
	}
}
