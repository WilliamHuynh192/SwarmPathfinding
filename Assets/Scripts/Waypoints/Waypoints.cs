using System.Collections.Generic;
using System.Linq;
using Interfaces;
using UnityEngine;

namespace Waypoints {
    public class Waypoints : MonoBehaviour, ITargetProvider {
        private readonly Stack<Waypoint> _targets = new();

        private float _timeTaken;
        private float _timeStarted;
        public Vector3? Target {
            get {
                if (!_targets.TryPeek(out var target)) return null;
                target.IsActive = true;
                return target.transform.position;
            }
        } 
    
        private void Start() {
            _timeStarted = Time.time;
            foreach (var waypoint in GetComponentsInChildren<Waypoint>().Reverse()) {
                waypoint.TargetProvider = this;
                _targets.Push(waypoint);
            }
        }

        public void OnTargetComplete() {
            var waypoint = _targets.Pop();
            waypoint.IsActive = true;
            if (_targets.Count == 0) {
                _timeTaken = Time.time - _timeStarted;
                Debug.Log($"Time Taken: {_timeTaken}");
            }
        }

    }
}