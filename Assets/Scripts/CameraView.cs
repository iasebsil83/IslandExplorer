using UnityEngine;
using UnityEngine.InputSystem;


public class CameraView : MonoBehaviour
{
    //cfg: view rotation speed
    private const float CAMERA_ROTX_SPEED = 0.1f;
    private const float CAMERA_ROTZ_SPEED = 0.1f;

    //attr: input
    public InputActionReference viewActionRef;
    public Vector2              viewInput;

    //attr: acs
    private GameObject player;

    //attr: view
    private float   rotX = 10.0f;
    private float   rotZ =  0.0f;
    private Vector3 lastPlayerPos;



    //ini
    void Start() {
        player = GameObject.Find("Player");

        //lock mouse movement to enable infinite view rotation
        Cursor.lockState = CursorLockMode.Locked;

        //ini rot
        transform.eulerAngles = new Vector3(rotX, 0.0f, rotZ);

        //ini pos
        lastPlayerPos = player.transform.position;
    }



    //run
    void Update() {
        Vector3 playerPos = player.transform.position;

        //camera view
        viewInput = viewActionRef.action.ReadValue<Vector2>();
        if(viewInput.x != 0 || viewInput.y != 0) {

            //compute X cur rot separately
            rotX += viewInput.y * CAMERA_ROTX_SPEED;

            //apply rotation: reset rot, pos horiz, rot horiz & vert, additional rot vert    !WARNING: SEQUENCE IS IMPORTANT!
            transform.eulerAngles = Vector3.zero;
            transform.RotateAround(playerPos, Vector3.up, viewInput.x * CAMERA_ROTZ_SPEED);
            transform.LookAt(playerPos, Vector3.up);
            transform.eulerAngles = new Vector3(rotX, transform.eulerAngles.y, transform.eulerAngles.z);
        }

        //keep same distance from player
        Vector3 deltaWithLastPlayerPos = playerPos - lastPlayerPos;
        transform.position += deltaWithLastPlayerPos;
        lastPlayerPos = playerPos;
    }
}
