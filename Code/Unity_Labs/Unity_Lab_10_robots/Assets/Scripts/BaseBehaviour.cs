using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseBehaviour : MonoBehaviour
{
    protected NavMeshAgent agent;
    protected GameController gameController;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        gameController = GameObject.FindGameObjectWithTag("GameController")
            .GetComponent<GameController>();
    }
}
