using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Interfaces {
    class Neighbours : MonoBehaviour, INeighbours {
        [SerializeField] private Boid.Boid[] _boids;

        private void Start() {
            _boids = FindObjectsByType<Boid.Boid>(FindObjectsSortMode.None);
        }

        public List<Boid.Boid> Get(Vector3 position, float perception) {
            return _boids.Where(i => Vector3.Distance(i.transform.position, position) < perception).ToList();
        }
    }
}