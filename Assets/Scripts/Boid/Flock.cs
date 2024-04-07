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

        [SerializeField] private float avgDistance;
        
        public Vector3 AvgPosition => boids.Aggregate(Vector3.zero, (avg, boid) => avg + boid.transform.position, avg => avg / boids.Count);

        [SerializeField] private List<Boid> boids;
        [SerializeField] private Transform target;
        [SerializeField] private Transform test;
        public Bounds Bounds => GetComponentInChildren<MeshCollider>().bounds;
        [SerializeField] private Waypoint waypoints;
        
        private Stack<Transform> _waypointList;
        

        private void Start() {
            Instantiate(test, Bounds.min, Quaternion.identity);
            Instantiate(test, Bounds.max, Quaternion.identity);
            _waypointList = waypoints.GetWaypoints();
            
            // Get an initial way point for the boids to follow
            var waypoint = _waypointList.Pop();
            
            foreach (var i in Enumerable.Range(0, count)) {
                var instance = Instantiate(boid, transform.position + Random.insideUnitSphere * 30, Quaternion.identity);
                instance.GetComponent<Boid>().Flock = transform;
                instance.GetComponent<Boid>().ID = i + 1;
                instance.GetComponent<Boid>().Target = waypoint;
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
            avgDistance = boids.Average(i => Vector3.Distance(i.transform.position, i.Target.position));
            
            // Iterate through waypoint when average distance is < 1.0, if no waypoint left, come to the final target
            AssignWayPoint();
            
        }

        private void AssignWayPoint()
        {
            if (avgDistance < 5.0f && _waypointList.Count > 0)
            {
                var nextWaypoint = _waypointList.Pop();
                foreach (var boid in boids)
                {
                    boid.Target = nextWaypoint;
                }
            } else if (avgDistance < 5.0f && _waypointList.Count == 0)
            {
                foreach (var boid in boids)
                {
                    boid.Target = target;
                }
            }
        }
    }
}