using System;
using System.Collections.Generic;
using System.Linq;
using Interfaces;
using UnityEngine;
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
        [SerializeField] private PsopType technique = PsopType.Individual;
        private Vector3? Target => TargetProvider.Target;

        private Vector3 PersonalBest { get; set; }

        private Vector3 GlobalBest {
            get {
                var neighbors = technique switch {
                    PsopType.Global => Neighbours.Get(),
                    PsopType.Individual => Neighbours.Get(transform.position, perception),
                    _ => new()
                };
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

        private enum PsopType {
            Individual,
            Global
        }
        
        private void Start() {
            Physics.queriesHitBackfaces = true; // needed for raycasts to hit the inside of colliders
            name = $"Boid {ID}";
            Velocity = Random.insideUnitSphere * Speed;
            PersonalBest = transform.position;
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + Velocity.normalized * 2);
            // Gizmos.color = Color.white;
            // Gizmos.DrawWireSphere(transform.position, perception);
        }

        private void Update() {
            if (!Target.HasValue) return;
            transform.position += Velocity * Time.deltaTime;
            var acceleration = GetAcceleration(Neighbours.Get(transform.position, perception));
            // if (_collisionBound) {
                // Velocity = acceleration;
            // }
            // else {
                Velocity += Vector3.Lerp(Velocity, acceleration, .25f);
            // }

            Velocity = Velocity.normalized * Speed;
            
            if (Target.HasValue && Vector3.Distance(transform.position, Target.Value) < Vector3.Distance(PersonalBest, Target.Value)) {
                PersonalBest = transform.position;
            }
        }

        private Vector3 GetAcceleration(List<Boid> neighbours) {
            return Separation(neighbours) * Multipliers.Separation +
                   Alignment(neighbours) * Multipliers.Alignment +
                   Cohesion(neighbours) * Multipliers.Cohesion +
                   Pathfinding() * Multipliers.Pathfinding + _avoidance * Multipliers.Avoidance;
        }

        private void FixedUpdate() {
            _avoidance = Vector3.zero;
            var ray = new Ray(transform.position, Velocity.normalized);
            if (Physics.Raycast(ray, out var hit, perception, 1 << LayerMask.NameToLayer("Terrain"))) {
                _avoidance = Vector3.ProjectOnPlane(Velocity, hit.normal).normalized;
                // _avoidance = Vector3.Reflect(Velocity, hit.normal + (Random.insideUnitSphere / 3));

                _avoidance = _avoidance.normalized * Speed;
                _avoidance -= Velocity;
                // _avoidance = _avoidance.normalized;
                _collisionBound = true;
            }
            else {
                _collisionBound = false;
            }
            
        }

        private Vector3 Pathfinding() {
            if (!Target.HasValue) {
                return Vector3.zero;
            } 
            var global = Social * (GlobalBest - transform.position);
            var personal = Cognitive * (PersonalBest - transform.position);

            var pathfinding = personal + global;

            pathfinding = pathfinding.normalized * Speed;
            pathfinding -= Velocity;
            pathfinding = Vector3.ClampMagnitude(pathfinding, .25f);

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
                // alignment = alignment.normalized;
                alignment = Vector3.ClampMagnitude(alignment, .25f);
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
                // cohesion = cohesion.normalized;
                cohesion = Vector3.ClampMagnitude(cohesion, .25f);
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
                }
            }

            if (neighbours.Count - 1 > 0) {
                separation /= neighbours.Count - 1;
                // separation = separation.normalized * Speed;
                separation = separation.normalized * Speed;
                separation -= Velocity;
                separation = Vector3.ClampMagnitude(separation, .25f);
            }
            
            return separation;
        }
    }
}