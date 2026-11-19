using UnityEngine;

public class FallingDie : MonoBehaviour
{
    //cfg: void limit
    private const float MIN_Y = -50.0f;



    //run
    void Update() {
        if(transform.position.y < MIN_Y) { Destroy(gameObject); }
    }
}
