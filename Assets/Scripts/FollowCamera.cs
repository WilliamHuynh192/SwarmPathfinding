using System;
using System.Collections;
using System.Collections.Generic;
using Boid;
using UnityEngine;
using UnityEngine.Serialization;

public class FollowCamera : MonoBehaviour {
    [SerializeField] private Flock flock;
    [SerializeField] private Vector3 offset = new(5, 5, 5);
    private void Update() {
        var avg = flock.AvgPosition;
        transform.position = Vector3.Lerp(transform.position, avg + offset, 0.01f);
        transform.LookAt(avg);
    }
}
