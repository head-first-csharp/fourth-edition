using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public float Height = 10F;
    public float RepeatRate = 1F;
    public GameObject Prefab;

    void Start()
    {
        InvokeRepeating("DropABall", 0F, RepeatRate);
    }

    private void DropABall()
    {
        GameObject ball = Instantiate(Prefab);
        ball.transform.position =
            new Vector3(10f - Random.value * 20f, 5f, 5f - Random.value * 10f);
    }
}

