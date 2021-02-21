using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveToClick : MonoBehaviour
{
    private NavMeshAgent agent;
    private GameController gameController;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        gameController = GameObject.Find("Main Camera").GetComponent<GameController>();
    }

    void Update()
    {
        if (!gameController.GameOver && Input.GetMouseButtonDown(0))
        {
            Camera cameraComponent = GameObject.Find("Main Camera").GetComponent<Camera>();
            Ray ray = cameraComponent.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                if (hit.collider.gameObject.tag != "Obstacle")
                {
                    agent.SetDestination(hit.point);
                }
            }
        }
    }
}
