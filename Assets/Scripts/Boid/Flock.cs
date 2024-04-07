using System;
using System.Collections.Generic;
using System.Linq;
using Interfaces;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Boid {
    public class Flock : MonoBehaviour, INeighbours {
        [SerializeField] private int count;
        [SerializeField] private GameObject boid;
        [field: SerializeField] public GameObject Bound { get; set; }

        [SerializeField] private float avgDistance;

        [SerializeField] private List<Boid> boids;
        [SerializeField] private Transform target;
        [SerializeField] private Waypoint waypoints;
        private Stack<Transform> _waypointList;
        private void Start()
        {
            _waypointList = waypoints.GetWaypoints();
            
            // Get an initial way point for the boids to follow
            
            foreach (var i in Enumerable.Range(0, count)) {
                var instance = Instantiate(boid, Random.insideUnitSphere * 50, Quaternion.identity);
                instance.GetComponent<Boid>().Flock = transform;
                instance.GetComponent<Boid>().Target = target;
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

        private void followWayPoints()
        {
            while (_waypointList.Count > 0)
            {
                var currentWaypoint = _waypointList.Pop();
                foreach (var boid in boids)
                {
                    boid.Target = currentWaypoint;
                }
            }
            
        }
    }
}