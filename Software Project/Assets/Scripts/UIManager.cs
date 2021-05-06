using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class uiManager : MonoBehaviour
{
    public static int score = 0;
    public static int lives = 3;
    public TMP_Text scoreText;
    public TMP_Text livesText;

    void Start()
    {
        scoreText.text = "Score: " + score.ToString();
        livesText.text = "Lives: " + lives.ToString();
    }


    //This method holds and manipulates the users score
    public void IncrementScore()
    {
        score++;
        scoreText.text = "Score: " + score;
    }

    //This method holds and manipulates the users lives
    public void IncrementLives()
    {
        lives--;
        livesText.text = "Lives: " + lives;

        if (lives == 0)
        {
            SceneManager.LoadScene("GameOver");
        }
    }

    public void Reset()
    {
        score = 0;
        lives = 3;
    }
}
