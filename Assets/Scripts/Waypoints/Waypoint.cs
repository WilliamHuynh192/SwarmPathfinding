using System;
using Interfaces;
using UnityEngine;

namespace Waypoints {
    public class Waypoint : MonoBehaviour {
        [SerializeField] private float radius;
        [SerializeField] private int numBoids;
        public ITargetProvider TargetProvider { set; private get; }
        private Collider[] _boids;

        private void Start() {
            _boids = new Collider[numBoids];
        }

        private void FixedUpdate() {
            
            var currentNumBoids = Physics.OverlapSphereNonAlloc(transform.position, radius, _boids, LayerMask.GetMask("boid"));
            Debug.Log(currentNumBoids);
            if (currentNumBoids >= numBoids) {
                TargetProvider.OnTargetComplete();
                Destroy(gameObject);
            }
        }
    }
}