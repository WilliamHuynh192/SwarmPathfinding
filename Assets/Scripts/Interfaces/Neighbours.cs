using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Interfaces {
    class Neighbours : INeighbours {
        private List<Boid.Boid> _boids;

        public Neighbours(List<Boid.Boid> boids) {
            _boids = boids;
        }

        public List<Boid.Boid> Get(Vector3 position, float perception) {
            return _boids.Where(i => Vector3.Distance(i.transform.position, position) < perception).ToList();
        }
    }
}