using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public bool GameOver = true;
    public int BallsPerGame = 5;
    private int ballsLeft = 0;
    private int score;
    public Text ScoreText;
    public Text BallsLeftText;
    public Button PlayAgainButton;

    public List<GameObject> TargetPrefabs;
    public int TargetCount = 20;
    private System.Random random = new System.Random();

    void Awake()
    {
        for (int i = 0; i < TargetCount; i++)
            Instantiate(TargetPrefabs[random.Next(0, 3)]);
    }

    private void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        ballsLeft = BallsPerGame;
        score = 0;
        GameOver = false;
        PlayAgainButton.gameObject.SetActive(false);
    }

    public void PlayerScored()
    {
        score++;
        Instantiate(TargetPrefabs[random.Next(0, 3)]);
    }

    public void BallLost()
    {
        ballsLeft--;
        if (ballsLeft <= 0)
        {
            GameOver = true;
            PlayAgainButton.gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        ScoreText.text = $"Score: {score}";
        BallsLeftText.text = $"Balls left: {ballsLeft}";
    }
}

