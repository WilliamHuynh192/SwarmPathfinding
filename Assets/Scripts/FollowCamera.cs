using Boid;
using UnityEngine;
public class FollowCamera : MonoBehaviour {
    [SerializeField] private Flock flock;
    [SerializeField] private Vector3 offset = new(5, 5, 5);
    [SerializeField] private Vector3 target = new(0, 0, 0);
    [SerializeField] private FollowType currentTarget = FollowType.Random;
    private int _currentRandomPos;

    private enum FollowType {
        Random,
        Average
    }
    private void Update() {
        
        if (Input.GetKeyDown(KeyCode.Space)) {
            currentTarget = FollowType.Random;
            _currentRandomPos = Random.Range(0, flock.Count);
        }
        if (Input.GetKeyDown(KeyCode.A)) {
            currentTarget = FollowType.Average;
        }

        target = currentTarget switch {
            FollowType.Average => flock.AvgPosition,
            FollowType.Random => flock[_currentRandomPos].transform.position,
            _ => target
        };

        transform.LookAt(target);
        transform.position = Vector3.Lerp(transform.position, target + offset, .01f);
    }
}
