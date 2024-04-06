using System.Collections.Generic;
using UnityEngine;

namespace Interfaces {
    // inheritors will get neighbours using global data or per drone data
    internal interface INeighbours {
        public List<Boid.Boid> Get(Vector3 position, float perception);

        public List<Boid.Boid> Get();
    }
}