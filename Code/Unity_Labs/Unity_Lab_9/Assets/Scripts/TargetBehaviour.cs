using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetBehaviour : MonoBehaviour
{
    private GameObject floor;
    public float MinimumHeight = 2f;
    public float MaximumHeight = 10f;
    private float tooFar = 20f;
    private GameController gameController;

    private void Awake()
    {
        floor = GameObject.FindGameObjectWithTag("Floor");
        transform.position = RandomPositionOverFloor();
        gameController = GameObject.FindGameObjectWithTag("GameController")
                                   .GetComponent<GameController>();
    }

    private Vector3 RandomPositionOverFloor()
    {
        var bounds = floor.GetComponent<Renderer>().bounds;
        var centerX = bounds.center.x;
        var centerZ = bounds.center.z;
        var sizeX = bounds.size.x;
        var sizeZ = bounds.size.z;
        var minX = centerX - (sizeX / 2) + 0.5f;
        var maxX = centerX + (sizeX / 2) - 0.5f;
        var minZ = centerZ - (sizeZ / 2) + 0.5f;
        var maxZ = centerZ + (sizeZ / 2) - 0.5f;
        var randomPosition = new Vector3(
            Random.Range(minX, maxX),
            Random.Range(MinimumHeight, MaximumHeight),
            Random.Range(minZ, maxZ));
        return randomPosition;
    }

    private void Update()
    {
        // Debug.DrawRay(RandomPositionOverFloor(), Vector3.up, Color.yellow, 1f);
        if (Mathf.Abs(transform.position.x) > tooFar || Mathf.Abs(transform.position.y) > tooFar
            || Mathf.Abs(transform.position.z) > tooFar)
        {
            gameController.PlayerScored();
            Destroy(gameObject);
        }
    }
}