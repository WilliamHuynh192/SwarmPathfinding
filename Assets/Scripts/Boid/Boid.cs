using System;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

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
        [field: SerializeField] public Transform Flock { private get; set; }
        
        private INeighbours _neighbours;
        [SerializeField] private float perception;

        [field: SerializeField] private Vector3 Velocity { get; set; }

        private void Start() {
            // Multipliers = new Multipliers { Alignment = 1, Separation = 1, Cohesion = 1 };
            Velocity = Random.insideUnitSphere;
            _neighbours = Flock.GetComponent<INeighbours>();
        }

        private void Update() {
            // Debug.Log(Velocity.magnitude);

            Velocity += GetAcceleration(_neighbours.Get(transform.position, perception));
            Velocity = Vector3.ClampMagnitude(Velocity, Speed );
            transform.position += Velocity * Time.deltaTime;
            Bounds();
        }

        private Vector3 GetAcceleration(List<Boid> neighbours) {
            return Separation(neighbours) * Multipliers.Separation + Alignment(neighbours) * Multipliers.Alignment + Cohesion(neighbours) * Multipliers.Cohesion;
        }

        private Vector3 Alignment(List<Boid> neighbours) {
            var alignment = Vector3.zero;

            foreach (var boid in neighbours) {
                if (this != boid) {
                    alignment += boid.Velocity;
                }
            }

            if (neighbours.Count - 1 > 0) {
                alignment /= neighbours.Count - 1;
                alignment = alignment.normalized * Speed;
                alignment -= Velocity;
                alignment = Vector3.ClampMagnitude(alignment, 0.33f);
            }

            return alignment;
        }

        private Vector3 Cohesion(List<Boid> neighbours) {
            var cohesion = Vector3.zero;
            foreach (var boid in neighbours) {
                if (this != boid) {
                    cohesion += boid.transform.position;
                }
            }
            if (neighbours.Count - 1 > 0) {
                cohesion /= neighbours.Count - 1;
                cohesion -= transform.position;
                cohesion = cohesion.normalized * Speed;
                cohesion -= Velocity;
                cohesion = Vector3.ClampMagnitude(cohesion, 0.33f);
            }
            return cohesion;
        }

        private Vector3 Separation(List<Boid> neighbours) {
            var separation = Vector3.zero;
            Debug.Log(neighbours.Count);
            foreach (var boid in neighbours) {
                if (this != boid) {
                    var offset = transform.position - boid.transform.position;
                    separation += offset / offset.sqrMagnitude;
                }
            }
            
            if (neighbours.Count - 1 > 0) {
                separation /= neighbours.Count - 1;
                separation = separation.normalized * Speed;
                separation -= Velocity;
                separation = Vector3.ClampMagnitude(separation, 0.33f);
            }
            
            return separation;
        }

        private void Bounds() {
            var max = 25;
            if (transform.position.x > max) transform.position = new Vector3(-max, transform.position.y, transform.position.z);
            if (transform.position.x < -max) transform.position = new Vector3(max, transform.position.y, transform.position.z);
            if (transform.position.y > max) transform.position = new Vector3(transform.position.x, -max, transform.position.z);
            if (transform.position.y < -max) transform.position = new Vector3(transform.position.x, max, transform.position.z);
            if (transform.position.z > max) transform.position = new Vector3(transform.position.x, transform.position.y, -max);
            if (transform.position.z < -max) transform.position = new Vector3(transform.position.x, transform.position.y, max);

        }
    }
}