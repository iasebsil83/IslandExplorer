using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //cfg: movements
    private const float ENEMY_SPEED = 25.0f;

    //cfg: explosion
    private const int   BEFORE_EXPLOSION_TIMER_MIN = 1000;
    private const int   BEFORE_EXPLOSION_TIMER_MAX = 2000;
    private const int   DURING_EXPLOSION_TIMER_MAX = 130;
    private const float EXPLOSION_GROWING_RATIO    = 1.01f;

    //cfg: sound
    private const float EXPLOSION_SOUND_DELAY  = 0.5f; //in s
    private const float SOUND_EXPLOSION_VOLUME = 0.6f;
    private const float SOUND_INFLATE_VOLUME   = 1.0f;

    //attr: acs
    private Rigidbody      rb;
    private GameObject     player;
    public  ParticleSystem explosionParticle;
    private AudioSource    audioPlayer;
    public  AudioClip      explosionSound;
    public  AudioClip      inflateSound;

    //attr: game
    private int beforeExplosion_timer;
    private int duringExplosion_timer;



    //ini
    void Awake() {
        rb          = GetComponent<Rigidbody>();
        player      = GameObject.Find("Player");
        audioPlayer = GetComponent<AudioSource>();

        //ini timer for explosion
        beforeExplosion_timer = Random.Range(BEFORE_EXPLOSION_TIMER_MIN, BEFORE_EXPLOSION_TIMER_MAX);
        duringExplosion_timer = 0;
    }



    //run
    void Update() {

        //move towards player
        Vector3 toPlayer = player.transform.position - transform.position;
        rb.AddForce(toPlayer.normalized * ENEMY_SPEED * Time.deltaTime);

        //state: waiting for explosion
        if(duringExplosion_timer == 0) {

            //time to start exploding
            if(beforeExplosion_timer == 0) {
                audioPlayer.PlayOneShot(inflateSound, SOUND_INFLATE_VOLUME);
                duringExplosion_timer = DURING_EXPLOSION_TIMER_MAX;
            }

            //dcr timer
            beforeExplosion_timer--;
        }

        //state: exploding
        else {

            //reached end-of-explosion => die
            if(duringExplosion_timer == 1){
                audioPlayer.PlayOneShot(explosionSound, SOUND_EXPLOSION_VOLUME);

                //turn invisible to get the sound playing, but make invisible
                GetComponent<MeshRenderer>().enabled = false;
                explosionParticle.Play();
                StartCoroutine(DestroyATinyBitLater());
            }

            //grow bigger
            transform.localScale *= EXPLOSION_GROWING_RATIO;

            //dcr timer
            duringExplosion_timer--;
        }
    }

    IEnumerator DestroyATinyBitLater() {
        yield return new WaitForSeconds(EXPLOSION_SOUND_DELAY);
        Destroy(gameObject);
    }
}
