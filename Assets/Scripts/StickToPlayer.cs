using UnityEngine;
using UnityEngine.InputSystem;


public class StickToPlayer : MonoBehaviour
{
    //attr: acs
    private GameObject player;

    //attr: game
    private Vector3 pos_relativeToPlayer;



    //ini
    void Start() {
        player               = GameObject.Find("Player");
        pos_relativeToPlayer = player.transform.position - transform.position;
    }



    //run
    void Update() {

        //stick perfectly to player
        transform.position = player.transform.position - pos_relativeToPlayer;
    }
}
