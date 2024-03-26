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

        public Vector3 Acceleration { get; private set; }
        public Vector3 Velocity { get; private set; }
        private INeighbours _neighbours;

        private void Start() {
            Multipliers = new Multipliers { Alignment = 1, Separation = 1, Cohesion = 1 };
        }

        private void Update() {
            transform.position += GetVelocity(_neighbours.Get(transform.position, perception)) * Time.deltaTime;
        }

        private Vector3 GetVelocity(List<Boid> neighbours) {
            return Alignment(neighbours) * Multipliers.Alignment + Cohesion(neighbours) * Multipliers.Cohesion +
                   Separation(neighbours) * Multipliers.Separation;
        }

        private Vector3 Alignment(List<Boid> neighbours) {
            Vector3 alignment = Vector3.zero;

            foreach (var boid in neighbours) {
                if (this != boid) {
                    alignment += boid.Velocity;
                }
            }

            if (neighbours.Count > 0) {
                alignment /= neighbours.Count;
                alignment -= Velocity;
                alignment = Vector3.ClampMagnitude(alignment, Speed);
            }

            return alignment;
        }

        private Vector3 Cohesion(List<Boid> neighbours) {
            var cohesion = Vector3.zero;
            foreach (var boid in neighbours) {
                if (this != boid) {
                    cohesion  += boid.transform.position;
                }
            }
            if (neighbours.Count > 0) {
                cohesion /= neighbours.Count;
                cohesion -= transform.position;
                cohesion = Vector3.ClampMagnitude(cohesion, Speed);
            }
            return cohesion;
        }

        private Vector3 Separation(List<Boid> neighbours) {
            var separation = Vector3.zero;
            foreach (var boid in neighbours) {
                if (this != boid) {
                    var offset = transform.position - boid.transform.position;
                    separation += offset / offset.sqrMagnitude;
                }
            }

            if (neighbours.Count > 0) {
                separation /= neighbours.Count;
                separation = Vector3.ClampMagnitude(separation, Speed);
            }
            return separation;
        }
    }
}