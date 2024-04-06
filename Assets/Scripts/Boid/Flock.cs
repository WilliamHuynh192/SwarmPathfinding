using System;
using System.Collections.Generic;
using System.Linq;
using Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Boid {
    public class Flock : MonoBehaviour, INeighbours {
        [SerializeField] private int count;
        [SerializeField] private GameObject boid;
        [field: SerializeField] public GameObject Bound { get; set; }

        [SerializeField] private List<Boid> boids;
        private void Start() {
            foreach (var i in Enumerable.Range(0, count)) {
                var instance = Instantiate(boid, transform.position, Quaternion.identity);
                instance.GetComponent<Boid>().Flock = transform;
                boids.Add(instance.GetComponent<Boid>());
            }
        }

        public List<Boid> Get(Vector3 position, float perception) {
            return boids.Where(i => Vector3.Distance(i.transform.position, position) < perception).ToList();
        }
    }
}