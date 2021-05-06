using UnityEngine;
using UnityEngine.SceneManagement;

public class tryAgain : MonoBehaviour
{
    public uiManager ui;

    void Start()
    {
        ui = GameObject.FindWithTag("ui").GetComponent<uiManager>();
    }

    //This method creates a the user can press to try again if they lose
    void OnGUI()
    {
        GUI.contentColor = Color.white;

        if (GUI.Button(new Rect(Screen.width / 2 - 50, Screen.height / 2 + -250, 100, 40), "Try Again"))
        {
            SceneManager.LoadScene("Stage1");
            ui.Reset();
        }
    }
}
