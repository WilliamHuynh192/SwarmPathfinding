using UnityEngine;
using Waypoints;

namespace Interfaces {
    public interface ITargetProvider {
        public Vector3 Target { get; }

        public void OnTargetComplete();
    }
}