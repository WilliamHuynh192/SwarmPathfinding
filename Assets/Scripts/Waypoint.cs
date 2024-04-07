using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        foreach (Transform waypoint in transform)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(waypoint.position, 1f);
        }

        Gizmos.color = Color.blue;
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(i + 1).position);
        }
    }

    public Stack<Transform> GetWaypoints()
    {
        Stack<Transform> listOfWaypoints = new Stack<Transform>();
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            listOfWaypoints.Push(transform.GetChild(i));
        }

        return listOfWaypoints;
    }
}
