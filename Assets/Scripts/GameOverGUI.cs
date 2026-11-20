using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverGUI : MonoBehaviour
{
    //attr: acs
    private Button restartButton;



    //ini
    void Start() {

        //restart button
        restartButton = GetComponent<Button>();
        restartButton.onClick.AddListener(RestartGame);
    }



    //restart game
    void RestartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }
}
