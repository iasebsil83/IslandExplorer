using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    //attr: acs
    private GameObject player;

    //attr: world
    public  List<Vector3> islands;
    private Vector3       playerPos_atLastUpdate;



    //ini
    void Start() {
        player = GameObject.Find("Player");

        //ini last recorded player pos
        playerPos_atLastUpdate = player.transform.position;
    }



    //run
    void Update() {

        //
        Vector3 playerDist_sinceLastUpdate = player.transform.position - playerPos_atLastUpdate;
        //if(playerDist_sinceLastUpdate.x > )
    }
}
