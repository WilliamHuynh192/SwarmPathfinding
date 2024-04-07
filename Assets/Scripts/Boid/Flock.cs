using System.Collections.Generic;
using System.Linq;
using Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Boid {
    public class Flock : MonoBehaviour, INeighbours {
        [SerializeField] private int count;
        [SerializeField] private GameObject boid;

        [SerializeField] private float avgDistance;

        [SerializeField] private List<Boid> boids;
        [SerializeField] private Transform target;
        [SerializeField] private Transform test;
        public Bounds Bounds => GetComponentInChildren<MeshCollider>().bounds;

        private void Start() {
            Instantiate(test, Bounds.min, Quaternion.identity);
            Instantiate(test, Bounds.max, Quaternion.identity);
            foreach (var i in Enumerable.Range(0, count)) {
                var instance = Instantiate(boid, transform.position + Random.insideUnitSphere * 30, Quaternion.identity);
                instance.GetComponent<Boid>().Flock = transform;
                instance.GetComponent<Boid>().Target = target;
                instance.GetComponent<Boid>().ID = i + 1;
                boids.Add(instance.GetComponent<Boid>());
            }
        }

        public List<Boid> Get(Vector3 position, float perception) {
            return boids.Where(i => Vector3.Distance(i.transform.position, position) < perception).ToList();
        }

        public List<Boid> Get() {
            return boids;
        }

        public void Update() {
            avgDistance = boids.Average(i => Vector3.Distance(i.transform.position, target.position));
        }
    }
}