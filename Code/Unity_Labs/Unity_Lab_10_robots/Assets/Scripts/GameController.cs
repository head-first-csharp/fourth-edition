using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public bool GameOver { get; private set; } = true;
    public int Score { get; private set; }
    public Button PlayAgainButton;
    public GameObject RobotPrefab;
    private int totalRobots;
    private int liveRobots;
    public Text ScoreText;
    public Text RobotsLeftText;

    public void PlayerDied()
    {
        GameOver = true;
        PlayAgainButton.gameObject.SetActive(true);
    }

    void Start()
    {
        ResetLevel(5);
    }

    public void StartGame()
    {
        PlayAgainButton.gameObject.SetActive(false);
        Score = 0;
        GameOver = false;
        ResetLevel(20);
    }

    private void ResetLevel(int numberOfRobots)
    {
        foreach (GameObject robot in GameObject.FindGameObjectsWithTag("Robot"))
        {
            Destroy(robot);
        }

        liveRobots = numberOfRobots;
        totalRobots = numberOfRobots;

        PlayerBehaviour player = GameObject.FindGameObjectWithTag("Player")
            .GetComponent<PlayerBehaviour>();
        player.StopMoving();

        for (int i = 0; i < numberOfRobots; i++)
        {
            var robot = Instantiate(RobotPrefab);
            Vector3 position;
            do
            {
                position = RandomPointHelper
                     .RandomPointOnMesh(robot.transform.position, 100) + Vector3.up;
                robot.transform.position = position;
            } while (Vector3.Distance(position, player.transform.position) < 5);
        }
    }

    public void RobotDied()
    {
        Score += 10;
        liveRobots--;
        if (liveRobots == 0)
        {
            ResetLevel(totalRobots + 3);
        }
    }

    private void Update()
    {
        ScoreText.text = Score.ToString();
        RobotsLeftText.text = $"Robots left: {liveRobots} of {totalRobots}";

        foreach (Text text in GameObject.FindObjectsOfType<Text>())
        {
            if (text.CompareTag("Game Over Screen")) text.enabled = GameOver;
        }
    }


}
