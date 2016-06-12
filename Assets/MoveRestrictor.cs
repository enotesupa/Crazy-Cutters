using UnityEngine;
using System.Collections;

public class MoveRestrictor : MonoBehaviour {

    private Vector3 initial_pos;
    private float initial_sqrt;
    private float current_dis = 0f;
    public float restriction_thresh = 100f;
    private bool is_restricted = false;
    public GameObject initial_obj;
    private GameObject copied_initial_obj;

    // Use this for initialization
    void Start () {
        initial_pos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        initial_sqrt = initial_pos.x * initial_pos.x
            + initial_pos.y * initial_pos.y
            + initial_pos.z * initial_pos.z;
        copied_initial_obj = Instantiate(initial_obj) as GameObject;
        copied_initial_obj.transform.position = initial_pos;
        copied_initial_obj.name = "initial " + GetComponent<MoveObject>().player_num;
    }
	
	// Update is called once per frame
	void Update () {
        current_dis = Mathf.Abs((initial_pos.x - transform.position.x) * (initial_pos.x - transform.position.x)
            + (initial_pos.y - transform.position.y) * (initial_pos.y - transform.position.y));
        if (!is_restricted && current_dis > restriction_thresh)
        {
            is_restricted = true;
            GetComponent<MoveObject>().Freeze();
        }
	}
}
