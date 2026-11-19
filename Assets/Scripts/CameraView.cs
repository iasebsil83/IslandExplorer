using UnityEngine;
using UnityEngine.InputSystem;


public class CameraView : MonoBehaviour
{
    //cfg
    private const float CAMERA_ROT_SPEED = 0.1f;

    //attr: input
    public InputActionReference viewActionRef;
    public Vector2              viewInput;

    //attr: acs
    private GameObject player;

    //attr: view
    private Vector3 curRot = new Vector3(20f, 0f, 0f);



    //ini
    void Start() {
        player = GameObject.Find("Player");

        //ini rot
        transform.eulerAngles = curRot;
    }



    //run
    void Update() {

        //rotate camera view
        viewInput = viewActionRef.action.ReadValue<Vector2>();
        if(viewInput.x != 0 || viewInput.y != 0) {

            //update cur angles
            curRot.x += viewInput.y * CAMERA_ROT_SPEED;
            curRot.y -= viewInput.x * CAMERA_ROT_SPEED;

            //apply rotation: reset rot, rot horiz, rot vert    !WARNING: SEQUENCE IS IMPORTANT!
            transform.eulerAngles = curRot;
        }
    }
}
