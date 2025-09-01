using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <<utility>> TrajectoryGenerator
/// ---------------------
/// + GeneratePath(startPosition : Vector3,
///                endPosition : Vector3,
///                maxVelocity : float,
///                acceleration : float,
///                deceleration : float,
///                samplingInterval : float) : List&lt;Vector3&gt;
/// ---------------------
/// Static class that generates motion paths with
/// trapezoidal or triangular velocity profiles.
/// </summary>
public static class TrajectoryGenerator
{
    /// <summary>
    /// Generates a list of Vector3 points from start to end using a
    /// trapezoidal or triangular velocity profile.
    /// </summary>
    /// <param name="startPosition">The starting position vector.</param>
    /// <param name="endPosition">The ending position vector.</param>
    /// <param name="maxVelocity">Maximum cruise velocity.</param>
    /// <param name="acceleration">Acceleration rate.</param>
    /// <param name="deceleration">Deceleration rate.</param>
    /// <param name="samplingInterval">Time step between generated samples.</param>
    /// <returns>List of sampled Vector3 positions representing the path.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown if velocity, acceleration, deceleration, or sampling interval are not positive.
    /// </exception>
    public static List<Vector3> GeneratePath(
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

        var path = new List<Vector3> { startPosition };

        if (totalDistance < Mathf.Epsilon)
        {
            return path;
        }

        // --- Calculate Profile Timings ---
        float timeToMaxVel = maxVelocity / acceleration;
        float distToMaxVel = 0.5f * acceleration * timeToMaxVel * timeToMaxVel;

        float timeFromMaxVel = maxVelocity / deceleration;
        float distFromMaxVel = maxVelocity * timeFromMaxVel - 0.5f * deceleration * timeFromMaxVel * timeFromMaxVel;

        float accelerationTime, cruiseTime, decelerationTime, peakVelocity;

        // Decide between triangular and trapezoidal profiles
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

        // --- Generate Path Points ---
        for (float t = samplingInterval; t <= totalTime; t += samplingInterval)
        {
            float currentDistance;
            if (t <= accelerationTime)
            {
                // Acceleration phase
                currentDistance = 0.5f * acceleration * t * t;
            }
            else if (t <= accelerationTime + cruiseTime)
            {
                // Cruise phase
                float timeInCruise = t - accelerationTime;
                float cruiseStartDistance = 0.5f * acceleration * accelerationTime * accelerationTime;
                currentDistance = cruiseStartDistance + peakVelocity * timeInCruise;
            }
            else
            {
                // Deceleration phase
                float timeInDecel = t - (accelerationTime + cruiseTime);
                float cruiseStartDistance = 0.5f * acceleration * accelerationTime * accelerationTime;
                float cruiseEndDistance = cruiseStartDistance + peakVelocity * cruiseTime;
                currentDistance = cruiseEndDistance + (peakVelocity * timeInDecel - 0.5f * deceleration * timeInDecel * timeInDecel);
            }
            path.Add(startPosition + direction * currentDistance);
        }

        // --- Final Point Correction ---
        const float tolerance = 1e-4f;
        if (path.Count == 0 || Vector3.Distance(path[path.Count - 1], endPosition) > tolerance)
        {
            path.Add(endPosition);
        }
        else
        {
            path[path.Count - 1] = endPosition;
        }

        return path;
    }
}
