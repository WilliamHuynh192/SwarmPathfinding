using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour {
    private void Start() {
        Physics.queriesHitBackfaces = true;
        // Vector3 test = new Vector3(5, 4, 2);
        transform.position = Vector3.Scale(transform.position, new Vector3(-1, 1, 1));
        Debug.Log(transform.position);
    }

    private void FixedUpdate() {
        // Debug.DrawLine(transform.position, transform.position + transform.up * 500);
        // if (Physics.Raycast(transform.position, transform.up * 500, float.PositiveInfinity)) {
        //     Debug.Log("hitting");
        // }
        // else {
        //     Debug.Log("not hittin");
        // }
    }
}