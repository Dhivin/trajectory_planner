using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryStateTests
{
    [Test]
    public void Update_HalfwayThroughSegment_ReturnsInterpolatedPosition()
    {
        // ARRANGE
        var trajectory = new List<PathPoint>
        {
            new PathPoint(new Vector3(0, 0, 0), 0f),
            new PathPoint(new Vector3(10, 0, 0), 1f)
        };
        var state = new TrajectoryState(trajectory);

        // ACT
        Vector3 position = state.Update(0.5f); // Update halfway through the 1-second trajectory

        // ASSERT
        Assert.AreEqual(5f, position.x, 1e-5f);
        Assert.IsFalse(state.IsFinished);
    }

    [Test]
    public void Update_PastEndOfTrajectory_ReturnsFinalPositionAndIsFinished()
    {
        // ARRANGE
        var trajectory = new List<PathPoint>
        {
            new PathPoint(Vector3.zero, 0f),
            new PathPoint(new Vector3(10, 0, 0), 1f)
        };
        var state = new TrajectoryState(trajectory);

        // ACT
        Vector3 position = state.Update(1.5f); // Update past the end of the 1-second trajectory

        // ASSERT
        Assert.AreEqual(10f, position.x, 1e-5f);
        Assert.IsTrue(state.IsFinished);
    }

    [Test]
    public void IsFinished_WithShortTrajectory_IsImmediatelyTrue()
    {
        // ARRANGE
        var trajectory = new List<PathPoint> { new PathPoint(Vector3.zero, 0f) }; // Only one point

        // ACT
        var state = new TrajectoryState(trajectory);

        // ASSERT
        Assert.IsTrue(state.IsFinished);
    }
}