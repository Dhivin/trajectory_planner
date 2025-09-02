using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrajectoryGeneratorTests
{
    private const float Tolerance = 1e-4f;

    [Test]
    public void GenerateTrajectory_ZeroDistance_ReturnsSinglePointAtStartTime()
    {
        // Arrange
        var start = new Vector3(1, 2, 3);
        var end = new Vector3(1, 2, 3);

        // Act
        var trajectory = TrajectoryGenerator.GenerateTrajectory(start, end, 5f, 2f, 2f, 0.1f);

        // Assert
        Assert.AreEqual(1, trajectory.Count);
        Assert.AreEqual(start, trajectory[0].Position);
        Assert.AreEqual(0f, trajectory[0].Time);
    }

    [Test]
    public void GenerateTrajectory_InvalidArguments_ThrowsException()
    {
        // Arrange
        var start = Vector3.zero;
        var end = Vector3.one;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => TrajectoryGenerator.GenerateTrajectory(start, end, 0f, 1f, 1f, 0.1f));
        Assert.Throws<ArgumentException>(() => TrajectoryGenerator.GenerateTrajectory(start, end, 1f, -1f, 1f, 0.1f));
        Assert.Throws<ArgumentException>(() => TrajectoryGenerator.GenerateTrajectory(start, end, 1f, 1f, 1f, 0f));
    }

    [Test]
    public void GenerateTrajectory_TrapezoidalProfile_IsValid()
    {
        // Arrange: Use the "Standard Trapezoid" test case
        var start = Vector3.zero;
        var end = new Vector3(20, 0, 0);
        float maxVelocity = 5.0f;
        float acceleration = 2.0f;
        float expectedTotalTime = 6.5f; // Calculated manually: t_accel(2.5) + t_cruise(1.5) + t_decel(2.5)

        // Act
        var trajectory = TrajectoryGenerator.GenerateTrajectory(start, end, maxVelocity, acceleration, acceleration, 0.1f);

        // Assert
        // 1. Check start and end points
        Assert.AreEqual(start, trajectory[0].Position);
        Assert.AreEqual(0f, trajectory[0].Time, Tolerance);
        Assert.IsTrue(Vector3.Distance(end, trajectory.Last().Position) < Tolerance, "Path should end at the target position.");
        Assert.AreEqual(expectedTotalTime, trajectory.Last().Time, 0.05f, "Final time should be correct."); // Looser tolerance for time

        // 2. Check velocity profile
        float maxVelocityAchieved = 0f;
        for (int i = 1; i < trajectory.Count; i++)
        {
            float timeDelta = trajectory[i].Time - trajectory[i - 1].Time;
            float distanceDelta = Vector3.Distance(trajectory[i].Position, trajectory[i - 1].Position);
            float velocity = distanceDelta / timeDelta;

            Assert.LessOrEqual(velocity, maxVelocity + Tolerance, "Velocity should not exceed maxVelocity.");
            if (velocity > maxVelocityAchieved)
            {
                maxVelocityAchieved = velocity;
            }
        }

        Assert.AreEqual(maxVelocity, maxVelocityAchieved, 0.1f, "Peak velocity should reach the specified maxVelocity.");
    }

    [Test]
    public void GenerateTrajectory_TriangularProfile_IsValid()
    {
        // Arrange: Use the "Short Distance" test case
        var start = Vector3.zero;
        var end = new Vector3(5, 0, 0);
        float maxVelocity = 10.0f; // This velocity should NOT be reached
        float acceleration = 4.0f;

        // Act
        var trajectory = TrajectoryGenerator.GenerateTrajectory(start, end, maxVelocity, acceleration, acceleration, 0.1f);

        // Assert
        // 1. Check start and end points
        Assert.AreEqual(start, trajectory[0].Position);
        Assert.IsTrue(Vector3.Distance(end, trajectory.Last().Position) < Tolerance, "Path should end at the target position.");

        // 2. Check velocity profile
        float maxVelocityAchieved = 0f;
        int peakVelocityIndex = 0;

        for (int i = 1; i < trajectory.Count; i++)
        {
            float timeDelta = trajectory[i].Time - trajectory[i - 1].Time;
            float distanceDelta = Vector3.Distance(trajectory[i].Position, trajectory[i - 1].Position);
            float velocity = distanceDelta / timeDelta;

            if (velocity > maxVelocityAchieved)
            {
                maxVelocityAchieved = velocity;
                peakVelocityIndex = i;
            }
        }

        Assert.Less(maxVelocityAchieved, maxVelocity, "Peak velocity should be LESS than maxVelocity, proving it's a triangular profile.");
        Assert.Greater(maxVelocityAchieved, 0, "Should have moved.");

        // 3. Verify the triangular shape (velocity increases then decreases)
        float firstSegmentVelocity = Vector3.Distance(trajectory[1].Position, trajectory[0].Position) / (trajectory[1].Time - trajectory[0].Time);
        float lastSegmentVelocity = Vector3.Distance(trajectory.Last().Position, trajectory[trajectory.Count - 2].Position) / (trajectory.Last().Time - trajectory[trajectory.Count - 2].Time);

        Assert.Greater(maxVelocityAchieved, firstSegmentVelocity, "Peak velocity should be greater than the start velocity.");
        Assert.Greater(maxVelocityAchieved, lastSegmentVelocity, "Peak velocity should be greater than the end velocity.");
    }
}