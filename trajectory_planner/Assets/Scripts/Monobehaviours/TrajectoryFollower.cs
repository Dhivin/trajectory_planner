using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <<MonoBehaviour>>
/// TrajectoryFollower
/// ---------------------
/// + objectToMove : Transform
/// + startPoint : Vector3
/// + endPoint : Vector3
/// + maxVelocity : float
/// + acceleration : float
/// + deceleration : float
/// + samplingInterval : float
/// ---------------------
/// + Start() : void
/// + GenerateAndFollowPath() : void
/// - FollowPathCoroutine() : IEnumerator
/// - OnDrawGizmos() : void
/// </summary>
public class TrajectoryFollower : MonoBehaviour
{
    /// <summary>
    /// Reference to the object that will follow the path.
    /// </summary>
    [SerializeField] private Transform objectToMove;

    /// <summary>
    /// Path start position in world coordinates.
    /// </summary>
    [SerializeField] private Vector3 startPoint = new Vector3(-5, 0, 0);

    /// <summary>
    /// Path end position in world coordinates.
    /// </summary>
    [SerializeField] private Vector3 endPoint = new Vector3(5, 0, 0);

    /// <summary>
    /// Maximum cruising velocity.
    /// </summary>
    [SerializeField] private float maxVelocity = 5.0f;

    /// <summary>
    /// Rate of acceleration.
    /// </summary>
    [SerializeField] private float acceleration = 2.0f;

    /// <summary>
    /// Rate of deceleration.
    /// </summary>
    [SerializeField] private float deceleration = 2.0f;

    /// <summary>
    /// Time interval between sampled path points.
    /// </summary>
    [SerializeField] private float samplingInterval = 0.1f;

    /// <summary>
    /// Generated path consisting of sampled positions.
    /// </summary>
    private List<Vector3> _path;

    /// <summary>
    /// Unity lifecycle method. Initializes movement from startPoint.
    /// </summary>
    void Start()
    {
        if (objectToMove == null)
        {
            Debug.LogError("Object to Move is not assigned!");
            return;
        }

        objectToMove.position = startPoint;
        GenerateAndFollowPath();
    }

    /// <summary>
    /// Public method to regenerate and follow trajectory at runtime.
    /// </summary>
    public void GenerateAndFollowPath()
    {
        try
        {
            _path = TrajectoryGenerator.GeneratePath(startPoint, endPoint, maxVelocity, acceleration, deceleration, samplingInterval);

            StopAllCoroutines();
            StartCoroutine(FollowPathCoroutine());
        }
        catch (System.ArgumentException ex)
        {
            Debug.LogError($"Error generating path: {ex.Message}");
        }
    }

    /// <summary>
    /// Coroutine that iterates through path points and moves the object.
    /// </summary>
    private IEnumerator FollowPathCoroutine()
    {
        if (_path == null || _path.Count == 0)
        {
            yield break;
        }

        Debug.Log($"Path generated with {_path.Count} points. Starting animation.");

        foreach (var point in _path)
        {
            objectToMove.position = point;
            yield return new WaitForSeconds(samplingInterval);
        }
    }

    /// <summary>
    /// Draws gizmos in Unity editor for start and end visualization.
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(startPoint, 0.2f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(endPoint, 0.2f);
        Gizmos.color = Color.white;
        Gizmos.DrawLine(startPoint, endPoint);
    }
}
