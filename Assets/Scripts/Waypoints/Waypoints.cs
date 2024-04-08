using System.Collections.Generic;
using Interfaces;
using UnityEngine;

namespace Waypoints {
    public class Waypoints : MonoBehaviour, ITargetProvider {
        private readonly Stack<Waypoint> _targets = new();

        public Vector3? Target {
            get {
                if (_targets.TryPeek(out var target)) {
                    return target.transform.position;
                }
                return null;
            }
        } 
    
        private void Start() {
            foreach (var waypoint in GetComponentsInChildren<Waypoint>()) {
                waypoint.TargetProvider = this;
                _targets.Push(waypoint);
            }
        }

        public void OnTargetComplete() {
            _targets.Pop();
        }

    }
}