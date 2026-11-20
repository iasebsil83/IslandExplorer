using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnManagerU4 : MonoBehaviour
{
    //cfg
    private const int   SPAWN_INTERVAL_MIN = 400; //nbr of update to be elapsed per spawn
    private const int   SPAWN_INTERVAL_MAX = 500;
    private const float SPAWN_X_RANGE      = 20.0f;
    private const float SPAWN_Y            = 40.0f;
    private const float SPAWN_Z_RANGE      = 20.0f;
    private const int   POWERUP_RARITY     = 10;

    //attr: acs
    public GameObject enemyPrefab;
    public GameObject powerupPrefab;

    //attr: spawn interval
    private int spawnIntervalCnt = 0;



    //ini
    void Start() {
    }



    //run
    void Update() {

        //reached end of interval
        if(spawnIntervalCnt == 0) {

            //prepare nxt spawn
            spawnIntervalCnt = Random.Range(SPAWN_INTERVAL_MIN, SPAWN_INTERVAL_MAX);

            //spawn pos
            Vector3 spawnPos = transform.position + new Vector3(
                Random.Range(-SPAWN_X_RANGE, SPAWN_X_RANGE),
                SPAWN_Y,
                Random.Range(-SPAWN_Z_RANGE, SPAWN_Z_RANGE)
            );

            //spawn: powerup
            if(Random.Range(0,POWERUP_RARITY) == 0) {
                Instantiate(powerupPrefab, spawnPos, powerupPrefab.transform.rotation);
            }

            //spawn: enemy
            else {
                Instantiate(enemyPrefab, spawnPos, enemyPrefab.transform.rotation);
            }
        }

        //waiting anyway
        spawnIntervalCnt--;
    }
}
