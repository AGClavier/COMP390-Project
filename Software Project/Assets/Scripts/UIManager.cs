using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    public int score = 0;
    public int lives = 3;
    public TMP_Text scoreText;
    public TMP_Text livesText;

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
}
