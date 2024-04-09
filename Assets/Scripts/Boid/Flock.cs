using System.Collections.Generic;
using System.Linq;
using Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Boid {
    public class Flock : MonoBehaviour, INeighbours {
        [SerializeField] private int count;
        [SerializeField] private GameObject boid;
        [SerializeField] private List<Boid> boids;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private float spawnRadius;
        [SerializeField] private Transform targetProvider;
        public Vector3 AvgPosition => boids.Aggregate(Vector3.zero, (avg, cur) => avg + cur.transform.position, avg => avg / boids.Count);
        public int Count => boids.Count;
        public Boid this[int i] => boids[i];

        private void Start() {
            foreach (var i in Enumerable.Range(0, count)) {
                var instance = Instantiate(boid, spawnPoint.transform.position + Random.insideUnitSphere * spawnRadius, Quaternion.identity);
                instance.GetComponent<Boid>().ID = i + 1;
                instance.GetComponent<Boid>().Neighbours = this;
                instance.GetComponent<Boid>().TargetProvider = targetProvider.GetComponent<ITargetProvider>();
                boids.Add(instance.GetComponent<Boid>());
            }
        }

        public List<Boid> Get(Vector3 position, float perception) {
            return boids.Where(i => Vector3.Distance(i.transform.position, position) < perception).ToList();
        }

        public List<Boid> Get() {
            return boids;
        }
        
    }
}