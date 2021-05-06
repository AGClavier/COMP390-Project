using UnityEngine;
using UnityEngine.SceneManagement;

public class playAgain : MonoBehaviour
{
    public uiManager ui;

    void Start()
    {
        ui = GameObject.FindWithTag("ui").GetComponent<uiManager>();
    }

    //This method creates a the user can press to play again if they win
    void OnGUI()
    {
        GUI.contentColor = Color.white;

        if (GUI.Button(new Rect(Screen.width / 2 - 70, Screen.height / 2 + -400, 100, 40), "Play Again"))
        {
            SceneManager.LoadScene("Stage1");
            ui.Reset();
        }
    }
}
