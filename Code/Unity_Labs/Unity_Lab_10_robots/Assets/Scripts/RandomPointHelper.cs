using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

public static class RandomPointHelper
{

    public static Vector3 RandomPointOnMesh(
              Vector3 startingPoint, int numberOfSteps)
    {
        Vector3 randomPointOnMesh = startingPoint;
        for (int i = 0; i < numberOfSteps; i++)
        {

            randomPointOnMesh = SampleRandomMeshPosition(randomPointOnMesh);
        }

        return randomPointOnMesh;
    }

    private static Vector3 SampleRandomMeshPosition(Vector3 startingPoint)
    {

        Vector3 newPoint = startingPoint;

        Vector3 randomPoint = startingPoint + Random.insideUnitSphere * 6f;
        NavMeshHit hit;

        if (NavMesh.SamplePosition(randomPoint, out hit, 6f, NavMesh.AllAreas))
        {

            newPoint = hit.position;
        }

        return newPoint;

    }

}