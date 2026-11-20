using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    //cfg: move
    private const float MOVE_FORCE_ONLAND = 40.0f;
    private const float MOVE_FORCE_FLYING = 5.0f;
    private const float MOVE_DIR_Y        = 0.78f;
    private const float MOVE_TORQUE       = 0.08f;
    private const float MIN_Y             = -50.0f;

    //cfg: physics
    private const float JMP_FORCE         = 130.0f;
    private const int   JMP_COOLDOWN_MAX  = 400;
    private const int   MOVE_COOLDOWN_MAX = 35;
    private const float GRAVITY_BASE      = -9.81f;
    private const float GRAVITY_MODIFIER  = 8.5f;

    //cfg: powerups
    private const float POWERUP_STRENGTH     = 50.0f;
    private const int   POWERUP_COOLDOWN_MAX = 200;
    private const float POWERUP_ROT_SPEED    = 93.0f;

    //cfg: sounds
    private const float SOUND_JMP_VOLUME = 0.3f;

    //attr: input
    public InputActionReference jumpActionRef;
    public InputActionReference moveActionRef;
    public Vector2              moveInput;

    //attr: acs
    private Rigidbody   rb;
    public  GameObject  powerupIndicator;
    private AudioSource audioPlayer;
    public  AudioClip   jumpSound;
    public  GameObject  gameOverGUI;
    private GameObject  camera;

    //attr: game
    private bool gameOver        = false;
    private int  powerupCooldown = 0;
    private int  moveCoolDown    = 0;
    private int  jmpCoolDown     = 0;



    //ini
    void Start() {
        rb          = GetComponent<Rigidbody>();
        audioPlayer = GetComponent<AudioSource>();
        camera      = GameObject.Find("Camera");

        //gravity
        Physics.gravity = new Vector3(0, GRAVITY_BASE * GRAVITY_MODIFIER, 0);
    }



    //run
    void Update() {

        //game over
        if(gameOver) {
            transform.position = new Vector3(transform.position.x, MIN_Y, transform.position.z); //hold player altitude
        }

        //game playing
        else {

            //fall into the void
            if(transform.position.y <= MIN_Y){
                gameOver = true;
                gameOverGUI.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible   = true;
            }

            //move
            moveInput = moveActionRef.action.ReadValue<Vector2>();
            if(moveCoolDown == 0 && (moveInput.x != 0 || moveInput.y != 0)){
                moveCoolDown          = MOVE_COOLDOWN_MAX;
                Vector3 moveDir_front = camera.transform.forward.normalized * moveInput.y;
                Vector3 moveDir_side  = camera.transform.right.normalized   * moveInput.x;
                Vector3 moveDir       = moveDir_front + moveDir_side + new Vector3(0, MOVE_DIR_Y, 0);

                //moving on the ground
                if(jmpCoolDown == 0) {
                    rb.AddForce( moveDir * MOVE_FORCE_ONLAND,  ForceMode.Impulse);
                    rb.AddTorque(moveDir * MOVE_TORQUE,        ForceMode.Impulse);
                }

                //moving in the air
                else{
                    rb.AddForce( moveDir * MOVE_FORCE_FLYING,  ForceMode.Impulse);
                }
            }

            //jmp
            if(jmpCoolDown == 0 && jumpActionRef.action.triggered) {
                jmpCoolDown = JMP_COOLDOWN_MAX;
                rb.AddForce(Vector3.up * JMP_FORCE, ForceMode.Impulse);
                audioPlayer.PlayOneShot(jumpSound, SOUND_JMP_VOLUME);
            }

            //powerup always rotating
            powerupIndicator.transform.Rotate(Vector3.up * POWERUP_ROT_SPEED);

            //cooldowns
            if(powerupCooldown != 0) {
                powerupCooldown--;
                if(powerupCooldown == 0) { powerupIndicator.SetActive(false); }
            }
            if(jmpCoolDown  != 0) { jmpCoolDown--;  }
            if(moveCoolDown != 0) { moveCoolDown--; }
        }
    }



    //collisions
    void OnTriggerEnter(Collider other) {

        //catch powerup
        if(other.CompareTag("Powerup")){
            Debug.Log("POWERUP!");
            powerupCooldown = POWERUP_COOLDOWN_MAX;
            powerupIndicator.SetActive(true);
            Destroy(other);
        }
    }

    void OnCollisionEnter(Collision c) {
        GameObject other = c.gameObject;

        //ground
        if(other.CompareTag("Island")) { jmpCoolDown = 0; }

        //enemy: bump
        else if(other.CompareTag("Enemy") && powerupCooldown != 0){
            Rigidbody enemyRb = other.GetComponent<Rigidbody>();
            Vector3   toEnemy = other.transform.position - transform.position;

            //strong repulsive bump
            enemyRb.AddForce(toEnemy * POWERUP_STRENGTH, ForceMode.Impulse);
        }
    }
}
