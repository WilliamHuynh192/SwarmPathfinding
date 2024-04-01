using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Boid {
    public class Flock : MonoBehaviour {
        [SerializeField] private int count;
        [SerializeField] private GameObject boid;
        [SerializeField] private float bounds;
        private void Start() {
            foreach (var i in Enumerable.Range(0, count)) {
                var instance = Instantiate(boid, Random.insideUnitSphere * bounds, Quaternion.identity);
                instance.GetComponent<Boid>().Flock = transform;
            }
        }
    }
}