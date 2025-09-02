using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A  C# class to manage the state and logic of following a trajectory.
/// </summary>
public class TrajectoryState
{
    private readonly List<PathPoint> _trajectory;
    private int _currentIndex;
    private float _currentTime;

    public bool IsFinished { get; private set; }

    public TrajectoryState(List<PathPoint> trajectory)
    {
        _trajectory = trajectory;
        _currentIndex = 0;
        _currentTime = 0f;

        IsFinished = (_trajectory == null || _trajectory.Count < 2);
    }

    /// <summary>
    /// Updates the position based on the time delta and returns the new interpolated position.
    /// </summary>
    /// <param name="deltaTime">The time that has passed since the last update.</param>
    /// <returns>The calculated position for the current time.</returns>
    public Vector3 Update(float deltaTime)
    {
        if (IsFinished)
        {
            return _trajectory.Last().Position;
        }

        _currentTime += deltaTime;

        // First, check if we have finished the entire trajectory.
        if (_currentTime >= _trajectory.Last().Time)
        {
            IsFinished = true;
            return _trajectory.Last().Position;
        }

        // Advance our current segment index as time passes.
        // The condition is corrected to - 1 to allow checking the final segment.
        while (_currentIndex < _trajectory.Count - 1 && _currentTime >= _trajectory[_currentIndex + 1].Time)
        {
            _currentIndex++;
        }


        // For smooth movement, we interpolate between the current point and the next one.
        PathPoint currentPoint = _trajectory[_currentIndex];
        PathPoint nextPoint = _trajectory[_currentIndex + 1];

        float segmentDuration = nextPoint.Time - currentPoint.Time;
        if (segmentDuration <= 0) return currentPoint.Position; // Avoid division by zero

        float timeIntoSegment = _currentTime - currentPoint.Time;

        // The interpolation factor 't' is how far we are through the current segment.
        float t = Mathf.Clamp01(timeIntoSegment / segmentDuration);

        return Vector3.Lerp(currentPoint.Position, nextPoint.Position, t);
    }
}