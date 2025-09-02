using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple MonoBehaviour that drives a  object.
/// </summary>
public class TrajectoryFollower : MonoBehaviour
{
    [Header("Object to Move")]
    public Transform objectToMove;

    [Header("Path Parameters")]
    public Vector3 startPoint = new Vector3(-5, 0, 0);
    public Vector3 endPoint = new Vector3(5, 0, 0);

  
    public float maxVelocity = 5.0f;
    public float acceleration = 2.0f;
    public float deceleration = 2.0f;
    public float samplingInterval = 0.1f;

    private TrajectoryState _trajectoryState;

    void Start()
    {
        if (objectToMove == null)
        {
            Debug.LogError("Object to Move is not assigned!");
            return;
        }

        GenerateAndFollowPath();
    }

    public void GenerateAndFollowPath()
    {
        var trajectory = TrajectoryGenerator.GenerateTrajectory(startPoint, endPoint, maxVelocity, acceleration, deceleration, samplingInterval);
        _trajectoryState = new TrajectoryState(trajectory);

        // Set initial position
        if (trajectory != null && trajectory.Count > 0)
        {
            objectToMove.position = trajectory[0].Position;
        }
    }

 
    void Update()
    {
        if (_trajectoryState != null && !_trajectoryState.IsFinished)
        {
            objectToMove.position = _trajectoryState.Update(Time.deltaTime);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(startPoint, 0.2f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(endPoint, 0.2f);
        Gizmos.color = Color.white;
        Gizmos.DrawLine(startPoint, endPoint);
    }
}