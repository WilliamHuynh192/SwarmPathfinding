using System.Collections.Generic;
using System.Linq;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Serialization;

namespace Interfaces {
    class Neighbours : MonoBehaviour {
        [SerializeField] private List<Boid.Boid> boids;

        private void Start() {
            Debug.Log(GameObject.FindGameObjectsWithTag("boid").Length);
            // foreach (var i in GameObject.FindGameObjectsWithTag("boid")) {
            //     boids.Add(i.GetComponent<Boid.Boid>());
            // }
        }

        public List<Boid.Boid> Get(Vector3 position, float perception) {
            return boids.Where(i => Vector3.Distance(i.transform.position, position) < perception).ToList();
        }
    }
}