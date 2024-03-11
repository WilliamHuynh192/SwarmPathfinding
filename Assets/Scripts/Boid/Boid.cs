using System;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;
using UnityEngine.Serialization;

namespace Boid {
    [Serializable]
    public struct Multipliers {
        [field: SerializeField] public float Alignment { get; set; }
        [field: SerializeField] public float Cohesion { get; set; }
        [field: SerializeField] public float Separation { get; set; }
    }

    public class Boid : MonoBehaviour {
        [field: SerializeField] public Multipliers Multipliers { private get; set; }
        [field: SerializeField] public float Speed { private get; set; }
        [SerializeField] private float perception;

        [SerializeField] private Rigidbody rb;

        private INeighbours _neighbours;

        private void Start() {
            rb.useGravity = false;
        }

        private void Update() {
            rb.velocity = GetVelocity(_neighbours.Get(transform.position, perception)) * Time.deltaTime;
        }

        private Vector3 GetVelocity(List<Boid> neighbours) {
            return Alignment(neighbours) * Multipliers.Alignment + Cohesion(neighbours) * Multipliers.Cohesion + Separation(neighbours) * Multipliers.Separation;
        }

        private Vector3 Alignment(List<Boid> neighbours) {
            return Vector3.zero;
        }

        private Vector3 Cohesion(List<Boid> neighbours) {
            return Vector3.zero;
        }

        private Vector3 Separation(List<Boid> neighbours) {
            return Vector3.zero;
        }

    }
}