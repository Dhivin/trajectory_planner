using System;
using System.Collections.Generic;
using UnityEngine;

public struct PathPoint
{
    public Vector3 Position { get; }
    public float Time { get; }

    public PathPoint(Vector3 position, float time)
    {
        Position = position;
        Time = time;
    }
}

public static class TrajectoryGenerator
{
    /// <summary>
    /// Generates a trajectory with position and time points using a trapezoidal velocity profile.
    /// </summary>
    /// <returns>A list of PathPoint objects representing the trajectory.</returns>
    public static List<PathPoint> GenerateTrajectory(
        Vector3 startPosition,
        Vector3 endPosition,
        float maxVelocity,
        float acceleration,
        float deceleration,
        float samplingInterval)
    {
        // --- Input Validation ---
        if (maxVelocity <= 0 || acceleration <= 0 || deceleration <= 0 || samplingInterval <= 0)
        {
            throw new ArgumentException("Velocity, acceleration, deceleration, and sampling interval must be positive.");
        }

        Vector3 displacement = endPosition - startPosition;
        float totalDistance = displacement.magnitude;
        Vector3 direction = displacement.normalized;

        var trajectory = new List<PathPoint> { new PathPoint(startPosition, 0f) };

        if (totalDistance < Mathf.Epsilon)
        {
            return trajectory;
        }

        // --- Calculate Profile Timings ---
        float timeToMaxVel = maxVelocity / acceleration;
        float distToMaxVel = 0.5f * acceleration * timeToMaxVel * timeToMaxVel;

        float timeFromMaxVel = maxVelocity / deceleration;
        float distFromMaxVel = maxVelocity * timeFromMaxVel - 0.5f * deceleration * timeFromMaxVel * timeFromMaxVel;

        float accelerationTime, cruiseTime, decelerationTime, peakVelocity;

        if (distToMaxVel + distFromMaxVel > totalDistance)
        {
            // --- Triangular Profile ---
            cruiseTime = 0;
            peakVelocity = Mathf.Sqrt(2 * totalDistance * acceleration * deceleration / (acceleration + deceleration));
            accelerationTime = peakVelocity / acceleration;
            decelerationTime = peakVelocity / deceleration;
        }
        else
        {
            // --- Trapezoidal Profile ---
            accelerationTime = timeToMaxVel;
            decelerationTime = timeFromMaxVel;
            peakVelocity = maxVelocity;
            float cruiseDistance = totalDistance - (distToMaxVel + distFromMaxVel);
            cruiseTime = cruiseDistance / maxVelocity;
        }

        float totalTime = accelerationTime + cruiseTime + decelerationTime;

        // --- Generate Trajectory Points ---
        float distanceAtEndOfAccel = 0.5f * acceleration * accelerationTime * accelerationTime;
        float distanceAtEndOfCruise = distanceAtEndOfAccel + peakVelocity * cruiseTime;

        for (float t = samplingInterval; t <= totalTime; t += samplingInterval)
        {
            float currentDistance;
            if (t <= accelerationTime)
            {
                currentDistance = 0.5f * acceleration * t * t;
            }
            else if (t <= accelerationTime + cruiseTime)
            {
                float timeInCruise = t - accelerationTime;
                currentDistance = distanceAtEndOfAccel + peakVelocity * timeInCruise;
            }
            else
            {
                float timeInDecel = t - (accelerationTime + cruiseTime);
                currentDistance = distanceAtEndOfCruise + (peakVelocity * timeInDecel - 0.5f * deceleration * timeInDecel * timeInDecel);
            }
            trajectory.Add(new PathPoint(startPosition + direction * currentDistance, t));
        }

        // --- Final goal ---
        trajectory.Add(new PathPoint(endPosition, totalTime));


        return trajectory;
    }
}