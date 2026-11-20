using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    //cfg: terrain loading
    private const float PLAYER_DISTANCE_TO_UPDATE = 100.0f;
    private const float ISLAND_UNLOADING_DISTANCE = 500.0f;

    //cfg: terrain generation
    private const float ISLAND_X_RANGE               = 40.0f;
    private const float ISLAND_Y_RANGE               = 20.0f;
    private const float ISLAND_Z_RANGE               = 40.0f;
    private const float GENERATION_GRID_HALFSIZE     = 500.0f;
    private const int   GENERATION_GRID_DIVISION     = 5;
    private const float GENERATION_GRID_CELLSIZE     = (2.0f * GENERATION_GRID_HALFSIZE) / GENERATION_GRID_DIVISION;
    private const float GENERATION_INTERDISTANCE_MIN = 2*ISLAND_X_RANGE;

    //attr: acs
    private GameObject player;
    public  GameObject islandPrefab;
    public  GameObject tmp;

    //attr: world
    public  List<Vector3>    islandsPos = new List<Vector3>();
    public  List<GameObject> islandsObj = new List<GameObject>();
    private Vector3          playerPos_atLastUpdate;



    //gnc tools
    private bool Vector3InXZBoundaries(Vector3 v, float minX, float maxX, float minZ, float maxZ) {
        return v.x >= minX && v.x <= maxX && v.z >= minZ && v.z <= maxZ;
    }



    //ini
    void Start() {
        player = GameObject.Find("Player");

        //ini last recorded player pos
        playerPos_atLastUpdate = player.transform.position;

        //generate islands around
        //Debug.Log("INITIAL GENERATION - BEGIN");
        GenerateNearbyIslands();
        //Debug.Log("INITIAL GENERATION - END");
    }



    //run
    void Update() {

        //update terrain if player moved a bit further away from last time
        Vector3 playerDist_sinceLastUpdate = player.transform.position - playerPos_atLastUpdate;
        if(
            Mathf.Abs(playerDist_sinceLastUpdate.x) > PLAYER_DISTANCE_TO_UPDATE ||
            Mathf.Abs(playerDist_sinceLastUpdate.z) > PLAYER_DISTANCE_TO_UPDATE
        ) {
            playerPos_atLastUpdate = player.transform.position;
            //Debug.Log("UPDATING TERRAIN");
            UpdateNearbyIslands();
        }
    }


    //terrain loading
    private bool TooFarFromPlayer(Vector3 tgt) {
        return Vector3.Distance(tgt, playerPos_atLastUpdate) > ISLAND_UNLOADING_DISTANCE;
    }

    private void UpdateNearbyIslands() {

        //among all existing islands
        //Debug.Log("START UpdateNearbyIslands");
        for(int islandIdx=0; islandIdx < islandsPos.Count; islandIdx++) {
            Vector3    islandPos = islandsPos[islandIdx];
			GameObject islandObj = islandsObj[islandIdx];
			bool       isLoaded  = islandObj is null;
			bool       tooFar    = TooFarFromPlayer(islandPos);
            //Debug.Log("TARGETING ISLAND (" + islandPos.x + "," + islandPos.y + "," + islandPos.z + ")");

            //too far and loaded => unload
            if(tooFar) {
            	if(isLoaded) {
                    //Debug.Log("UNLOADING ISLAND");
            		Destroy(islandObj);
            		islandsObj[islandIdx] = null;
            	}
            }

			//nearby and not-loaded => load
			else {
				if(!isLoaded) {
                    //Debug.Log("LOADING ISLAND");
            		islandsObj[islandIdx] = Instantiate<GameObject>(islandPrefab, islandPos, islandPrefab.transform.rotation);
            	}
			}
        }

		//generate islands around, if necessary
        GenerateNearbyIslands();
        //Debug.Log("STOP UpdateNearbyIslands");
    }



    //terrain generation
    private void GenerateIsland(Vector3 pos) {

        //last-check: cancel generation if too close to an existing island
        foreach(Vector3 islandPos in islandsPos) {
            Vector3 delta = islandPos - pos;
            if(Mathf.Abs(delta.x) < GENERATION_INTERDISTANCE_MIN && Mathf.Abs(delta.z) < GENERATION_INTERDISTANCE_MIN){ return; }
        }

        //Debug.Log("GENERATING ISLAND AT (" + pos.x + "," + pos.y + "," + pos.z + ")");
        islandsPos.Add(pos);
        islandsObj.Add(
            Instantiate<GameObject>(islandPrefab, pos, islandPrefab.transform.rotation)
        );
    }

    private void GenerateNearbyIslands() {
        //Debug.Log("START GenerateNearbyIslands");
        float minX = playerPos_atLastUpdate.x - GENERATION_GRID_HALFSIZE;
        float minZ = playerPos_atLastUpdate.z - GENERATION_GRID_HALFSIZE;
        float maxX = playerPos_atLastUpdate.x + GENERATION_GRID_HALFSIZE + GENERATION_GRID_CELLSIZE;
        float maxZ = playerPos_atLastUpdate.z + GENERATION_GRID_HALFSIZE + GENERATION_GRID_CELLSIZE;

        //go over each grid cell
        for(float x1=minX; x1 < maxX; x1 += GENERATION_GRID_CELLSIZE){
            for(float z1=minZ; z1 < maxZ; z1 += GENERATION_GRID_CELLSIZE){
                float x2 = x1 + GENERATION_GRID_CELLSIZE;
                float z2 = z1 + GENERATION_GRID_CELLSIZE;

                //cell boundaries
                //Debug.Log("CHECKING CELL (" + x1 + "," + x2 + "), (" + z1 + "," + z2 + ")");
                Instantiate(tmp, new Vector3(x1, 0.0f, z1), tmp.transform.rotation);

                //look for an existing island in that cell already
                bool noIslandFound = true;
                foreach(Vector3 islandPos in islandsPos) {
                    if(Vector3InXZBoundaries(islandPos, x1, x2, z1, z2)){
                        //Debug.Log("ISLAND (" + islandPos.x + "," + islandPos.y + "," + islandPos.z + ") IN CELL => DON'T GENERATE IN THAT CELL.");
                        noIslandFound = false; break;
                    }
                }

                //no island in that cell => generate one
                if(noIslandFound) {
                    float centerX = x1 + (x2 - x1)/2.0f;
                    float centerZ = z2 + (z2 - z1)/2.0f;
                    GenerateIsland(new Vector3(
                        centerX + Random.Range(-ISLAND_X_RANGE, ISLAND_X_RANGE),
                        Random.Range(-ISLAND_Y_RANGE, ISLAND_Y_RANGE),
                        centerZ + Random.Range(-ISLAND_X_RANGE, ISLAND_Z_RANGE)
                    ));
                }
            }
        }
        //Debug.Log("STOP GenerateNearbyIslands");
    }
}
