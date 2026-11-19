using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    //cfg: move
    private const float MOVE_FORCE  = 26.0f;
    private const float MOVE_DIR_Y  = 0.78f;
    private const float MOVE_TORQUE = 0.5f;
    private const float MIN_Y       = -50.0f;

    //cfg: physics
    private const float JUMP_FORCE       = 50.0f;
    private const float GRAVITY_MODIFIER = 9.5f;

    //cfg: powerups
    private const float POWERUP_STRENGTH     = 50.0f;
    private const int   POWERUP_COOLDOWN_MAX = 200;
    private const float POWERUP_ROT_SPEED    = 93.0f;

    //cfg: sounds
    private const float SOUND_JUMP_VOLUME = 0.3f;

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

    //attr: game
    private bool gameOver        = false;
    private int  powerupCooldown = 0;
    private bool isOnGround      = true;



    //ini
    void Start() {
        rb          = GetComponent<Rigidbody>();
        audioPlayer = GetComponent<AudioSource>();

        //gravity
        Physics.gravity *= GRAVITY_MODIFIER;
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
            }

            //move
            moveInput = moveActionRef.action.ReadValue<Vector2>();
            if(isOnGround && (moveInput.x != 0 || moveInput.y != 0)){
                isOnGround = false;
                Vector3 moveDir = new Vector3(moveInput.x, MOVE_DIR_Y, moveInput.y);
                rb.AddForce( moveDir * MOVE_FORCE,  ForceMode.Impulse);
                rb.AddTorque(moveDir * MOVE_TORQUE, ForceMode.Impulse);
            }

            //jmp
            if(isOnGround && jumpActionRef.action.triggered) {
                rb.AddForce(Vector3.up * JUMP_FORCE, ForceMode.Impulse);
                audioPlayer.PlayOneShot(jumpSound, SOUND_JUMP_VOLUME);
                isOnGround = false;
            }

            //powerup always rotating
            powerupIndicator.transform.Rotate(Vector3.up * POWERUP_ROT_SPEED);

            //cooldowns
            if(powerupCooldown != 0) {
                powerupCooldown--;
                if(powerupCooldown == 0) { powerupIndicator.SetActive(false); }
            }
        }
    }



    //collisions
    void OnTriggerEnter(Collider other) {

        //catch powerup
        if(other.CompareTag("Powerup")){
            powerupCooldown = POWERUP_COOLDOWN_MAX;
            powerupIndicator.SetActive(true);
            Destroy(other);
        }
    }

    void OnCollisionEnter(Collision c) {
        GameObject other = c.gameObject;

        //ground
        if(other.CompareTag("Island")){ isOnGround = true; }

        //enemy: bump
        else if(other.CompareTag("Enemy") && powerupCooldown != 0){
            Rigidbody enemyRb = other.GetComponent<Rigidbody>();
            Vector3   toEnemy = other.transform.position - transform.position;

            //strong repulsive bump
            enemyRb.AddForce(toEnemy * POWERUP_STRENGTH, ForceMode.Impulse);
        }
    }
}
