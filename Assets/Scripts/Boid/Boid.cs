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
        [field: SerializeField] public Transform Flock { private get; set; }
        
        private INeighbours _neighbours;
        private Vector3 _avoidance;
        [field: SerializeField] public float Cognitive { get; set; } = .8f;
        [field: SerializeField] public float Social { get; set; } = .2f;
        [field: SerializeField] public Transform Target { private get; set; }

        private Vector3 PersonalBest { get; set; }
        private Vector3 GlobalBest {
            get {
                var neighbors = _neighbours.Get(transform.position, perception);
                return neighbors.Aggregate(neighbors.First(), (min, boid) =>
                        Vector3.Distance(min.PersonalBest, Target.position) <
                        Vector3.Distance(boid.PersonalBest, Target.position)
                    ? min
                    : boid,
                    boid => boid.PersonalBest);
            }
        }

        [SerializeField] private float perception;

        [field: SerializeField] public Vector3 Velocity { get; private set; }

        private Bounds _bounds;
        private void Start() {
            Physics.queriesHitBackfaces = true; // needed for raycasts to hit the inside of colliders
            // Multipliers = new Multipliers { Alignment = 1, Separation = 1, Cohesion = 1 };
            name = $"Boid {ID}";
            Velocity = Random.insideUnitSphere * Speed;
            
            _neighbours = Flock.GetComponent<INeighbours>();
            _bounds = Flock.GetComponent<Flock>().Bounds;
            PersonalBest = transform.position;
        }

        private void OnDrawGizmos() {
            // Gizmos.color = Color.black;
            // Gizmos.DrawWireSphere(transform.position, perception);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + Velocity.normalized * 2);
        }

        private void Update() {

            Velocity += GetAcceleration(_neighbours.Get(transform.position, perception));
            Velocity = Velocity.normalized * Speed;
            transform.position += Velocity * Time.deltaTime;

            if (Vector3.Distance(transform.position, Target.position) < Vector3.Distance(PersonalBest, Target.position)) {
                PersonalBest = transform.position;
            }
            
            Bounds();
        }

        private Vector3 GetAcceleration(List<Boid> neighbours) {
            return Separation(neighbours) * Multipliers.Separation +
                   Alignment(neighbours) * Multipliers.Alignment +
                   Cohesion(neighbours) * Multipliers.Cohesion +
                   Pathfinding() * 0;
            // _avoidance * Multipliers.Avoidance;
        }

        private void FixedUpdate() {
            _avoidance = Vector3.zero;
            if (Physics.Raycast(new Ray(transform.position, Velocity.normalized), out var hit, perception, LayerMask.NameToLayer("Terrain"))) {
                Debug.Log($"{name} boutta hit");
                _avoidance = ((hit.normal + Velocity) / 2).normalized * Speed;
            }
            
            _avoidance = Vector3.ClampMagnitude(_avoidance, .2f);
        }

        private Vector3 Pathfinding() {
            var global = Social * (GlobalBest - transform.position);
            var personal = Cognitive * (PersonalBest - transform.position);
            
            var pathfinding = personal + global;
            
            pathfinding -= Velocity;
            // pathfinding = pathfinding.normalized * Speed;
            // pathfinding = Vector3.ClampMagnitude(pathfinding, .33f);

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
                alignment = Vector3.ClampMagnitude(alignment, .33f);
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
                cohesion = cohesion - transform.position;
                cohesion = cohesion.normalized * Speed;
                cohesion -= Velocity;
                cohesion = Vector3.ClampMagnitude(cohesion, .33f);
            }
            return cohesion;
        }

        private Vector3 Separation(List<Boid> neighbours) {
            var separation = Vector3.zero;
            foreach (var boid in neighbours) {
                if (this != boid) {
                    // var offset = transform.position - boid.transform.position;
                    // separation += offset / offset.sqrMagnitude;
                    separation += boid.transform.position;
                }
            }
            
            if (neighbours.Count - 1 > 0) {
                separation /= neighbours.Count - 1;
                separation = transform.position - separation;
                separation = separation.normalized * Speed;
                separation -= Velocity;
                separation = Vector3.ClampMagnitude(separation, .33f);
            }
            return separation;
        }
        
        private void Bounds() {
            if (transform.position.x < _bounds.min.x) transform.position = new Vector3(_bounds.max.x, transform.position.y, transform.position.z);
            if (transform.position.x > _bounds.max.x) transform.position = new Vector3(_bounds.min.x, transform.position.y, transform.position.z);
            if (transform.position.y < _bounds.min.y) transform.position = new Vector3(transform.position.x, _bounds.max.y, transform.position.z);
            if (transform.position.y > _bounds.max.y) transform.position = new Vector3(transform.position.x, _bounds.min.y, transform.position.z);
            if (transform.position.z < _bounds.min.z) transform.position = new Vector3(transform.position.x, transform.position.y, _bounds.max.z);
            if (transform.position.z > _bounds.max.z) transform.position = new Vector3(transform.position.x, transform.position.y, _bounds.min.z);
        }
    }
}