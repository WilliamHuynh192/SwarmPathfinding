using System;
using System.Collections.Generic;
using System.Linq;
using Interfaces;
using UnityEngine;
using Waypoints;
using Random = UnityEngine.Random;

namespace Boid {
    [Serializable]
    public struct Multipliers {
        [field: SerializeField] public float Alignment { get; set; }
        [field: SerializeField] public float Cohesion { get; set; }
        [field: SerializeField] public float Separation { get; set; }
        [field: SerializeField] public float Pathfinding { get; set; }
        [field: SerializeField] public float Avoidance { get; set; }
    }

    public class Boid : MonoBehaviour {
        [field: SerializeField] public int ID { private get; set; }
        [field: SerializeField] public Multipliers Multipliers { private get; set; }
        [field: SerializeField] public float Speed { private get; set; }

        public INeighbours Neighbours { set; private get; }
        public ITargetProvider TargetProvider { set; private get; }
        
        private Vector3 _avoidance;
        [field: SerializeField] public float Cognitive { get; set; } = .8f;
        [field: SerializeField] public float Social { get; set; } = .2f;
        private Vector3? Target => TargetProvider.Target;

        private Vector3 PersonalBest { get; set; }

        private Vector3 GlobalBest {
            get {
                var neighbors = Neighbours.Get(transform.position, perception);
                return neighbors.Aggregate(neighbors.First(), (min, boid) =>
                        Vector3.Distance(min.PersonalBest, Target ?? min.PersonalBest) <
                        Vector3.Distance(boid.PersonalBest, Target ?? boid.PersonalBest)
                            ? min
                            : boid,
                    boid => boid.PersonalBest);
            }
        }

        [SerializeField] private float perception;

        [field: SerializeField] public Vector3 Velocity { get; private set; }
        
        private bool _collisionBound;

        private void Start() {
            Physics.queriesHitBackfaces = true; // needed for raycasts to hit the inside of colliders
            // Multipliers = new Multipliers { Alignment = 1, Separation = 1, Cohesion = 1 };
            name = $"Boid {ID}";
            Velocity = Random.insideUnitSphere * Speed;

        
            PersonalBest = transform.position;
        }

        private void OnDrawGizmos() {
            // Gizmos.color = Color.black;
            // Gizmos.DrawWireSphere(transform.position, perception);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + Velocity.normalized * 2);
        }

        private void Update() {
            transform.position += Velocity * Time.deltaTime;
            // Velocity = Quaternion.FromToRotation(Velocity, GetAcceleration(_neighbours.Get(transform.position, perception))) * Velocity;
            var acceleration = GetAcceleration(Neighbours.Get(transform.position, perception));
            if (_collisionBound) {
                Velocity = acceleration;
            }
            else {
                Velocity += Vector3.Lerp(Velocity, acceleration, .25f);
            }

            Velocity = Velocity.normalized * Speed;
            
            if (Target.HasValue && Vector3.Distance(transform.position, Target.Value) < Vector3.Distance(PersonalBest, Target.Value)) {
                PersonalBest = transform.position;
            }

            // Bounds();
        }

        private Vector3 GetAcceleration(List<Boid> neighbours) {
            if (!_collisionBound) {
                return Separation(neighbours) * Multipliers.Separation +
                       Alignment(neighbours) * Multipliers.Alignment +
                       Cohesion(neighbours) * Multipliers.Cohesion +
                       Pathfinding() * Multipliers.Pathfinding;
            }

            return _avoidance * Multipliers.Avoidance;
        }

        private void FixedUpdate() {
            _avoidance = Vector3.zero;
            var ray = new Ray(transform.position, Velocity.normalized);
            // Debug.DrawRay(transform.position, Velocity.normalized * perception);
            if (Physics.Raycast(ray, out var hit, perception, 1 << 6)) {
                _avoidance = Vector3.Reflect(Velocity, hit.normal).normalized * Speed;
                // _avoidance = ((hit.point + hit.normal) - transform.position).normalized * Speed;
                // _avoidance -= Velocity;
                _collisionBound = true;
                Debug.Log($"{hit.transform.name} hit");
            }
            else {
                _collisionBound = false;
            }

            _avoidance = Vector3.ClampMagnitude(_avoidance, .2f);
        }

        private Vector3 Pathfinding() {
            if (!Target.HasValue) {
                return Vector3.zero;
            } 
            var global = Social * (GlobalBest - transform.position);
            var personal = Cognitive * (PersonalBest - transform.position);

            var pathfinding = personal + global;

            pathfinding -= Velocity;
            pathfinding = pathfinding.normalized * Speed;
            pathfinding = Vector3.ClampMagnitude(pathfinding, .2f);

            return pathfinding;
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
                alignment = Vector3.ClampMagnitude(alignment, .2f);
            }

            return alignment.normalized;
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
                cohesion = cohesion - transform.position;
                cohesion = cohesion.normalized * Speed;
                cohesion -= Velocity;
                cohesion = Vector3.ClampMagnitude(cohesion, .2f);
            }

            return cohesion;
        }

        private Vector3 Separation(List<Boid> neighbours) {
            var separation = Vector3.zero;
            foreach (var boid in neighbours) {
                if (this != boid) {
                    var offset = transform.position - boid.transform.position;
                    offset /= offset.sqrMagnitude;
                    separation += offset;
                    // separation += boid.transform.position - separation;
                }
            }

            if (neighbours.Count - 1 > 0) {
                separation /= neighbours.Count - 1;
                separation = separation.normalized * Speed;
                separation -= Velocity;
                separation = Vector3.ClampMagnitude(separation, .2f);
                // separation = transform.position - separation;
            }

            return separation;
        }
    }
}