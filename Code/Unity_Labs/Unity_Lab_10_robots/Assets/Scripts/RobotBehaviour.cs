using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RobotBehaviour : BaseBehaviour
{
    private PlayerBehaviour playerBehaviour;
    private bool alive = true;

    protected override void Awake()
    {
        playerBehaviour = GameObject.FindGameObjectWithTag("Player")
            .GetComponent<PlayerBehaviour>();
        base.Awake();
    }

    void Start()
    {
        transform.position = RandomPointHelper.RandomPointOnMesh(
                                           transform.position, 100) + Vector3.up;
    }

    void Update()
    {
        if (alive && !gameController.GameOver && playerBehaviour.IsMoving)
            agent.SetDestination(playerBehaviour.transform.position);
        else agent.SetDestination(transform.position);
    }
        
    void OnTriggerEnter(Collider other)
    {
        if (alive && other.gameObject.CompareTag("Player"))
        {
            gameController.PlayerDied();
        }
        else if (alive && other.gameObject.CompareTag("Robot"))
        {
            alive = false;
            Renderer renderer = gameObject.GetComponent<Renderer>();
            renderer.material.color = Color.black;
            gameController.RobotDied();
        }
    }
}
