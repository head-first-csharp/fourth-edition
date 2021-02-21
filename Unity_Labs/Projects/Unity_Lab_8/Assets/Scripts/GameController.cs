using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public float Height = 10F;
    public float RepeatRate = 1F;
    public GameObject Prefab;
    public int Score = 0;

    public bool GameOver = true;
    public int MaxScore = 10;
    public Text ScoreText;
    public Button PlayAgainButton;

    private float GameTimer = 0f;
    public Text TimerText;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        InvokeRepeating("DropABall", 0F, RepeatRate);
    }

    private void DropABall()
    {
        if (!GameOver)
        {
            GameObject ball = Instantiate(Prefab);
            ball.transform.position =
                new Vector3(10f - Random.value * 20f, 5f, 5f - Random.value * 10f);
        }
    }

    public void CollidedWithBall()
    {
        audioSource.Play();
        Score++;
        if (Score >= MaxScore)
        {
            GameOver = true;
            PlayAgainButton.gameObject.SetActive(true);
        }
    }

    void Update()
    {
        ScoreText.text = $"Score: {Score} of {MaxScore}";
        if (!GameOver)
        {
            GameTimer += Time.deltaTime;
        }
        TimerText.text = $"Time elapsed: {GameTimer:0.00}";
    }

    public void StartGame()
    {
        PlayAgainButton.gameObject.SetActive(false);
        Score = 0;
        GameOver = false;
        GameTimer = 0;
    }
}
