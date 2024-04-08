using Interfaces;
using UnityEngine;

namespace Waypoints {
    public class Waypoint : MonoBehaviour {
        [SerializeField] private float radius;
        [SerializeField] private int numBoids;
        public ITargetProvider TargetProvider { set; private get; }
        private Collider[] _boids;
        public bool IsActive { private get; set; }

        private void Start() {
            _boids = new Collider[numBoids];
        }

        private void FixedUpdate() {
            if (!IsActive) return;
            var currentNumBoids = Physics.OverlapSphereNonAlloc(transform.position, radius, _boids, LayerMask.GetMask("boid"));
            if (currentNumBoids >= numBoids) {
                TargetProvider.OnTargetComplete();
                Destroy(gameObject);
            }
        }
    }
}