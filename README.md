
# Trajectory Planning with Trapezoidal Velocity Profile
This project is a Unity C# implementation that generates a motion path between a start and end point using a trapezoidal velocity profile. It animates an object along the generated trajectory and is structured with a clean, decoupled architecture to ensure the core logic is highly testable.

## Features
- **Dynamic Path Generation:** Create trajectories from any start to end point.
- **Configurable Motion Profile:** Set Max Velocity, Acceleration, and Deceleration in the Inspector.
- **Smooth Animation:** Object movement is interpolated for smooth visual results matching trajectory timing.
- **Decoupled & Testable Architecture:** Core logic is separated from MonoBehaviour for robust unit testing.
- **Comprehensive Test Suite:** Includes Edit Mode tests for core logic.



## Architecture Overview
The project follows a decoupled, three-tier architecture to separate concerns. This makes the code cleaner, more maintainable, and significantly easier to test.

1. **Logic Layer (`TrajectoryGenerator.cs`)**
   - Static utility class for mathematical calculations.
   - Generates a `List<PathPoint>` (position + time) from motion parameters.
   - Independent of Unity's scene or game objects.

2. **State Management Layer (`TrajectoryState.cs`)**
   - Plain C# class acting as a state machine.
   - Consumes trajectories and holds current animation state (time, progress).

3. **Application Layer (`TrajectoryFollower.cs`)**
   - MonoBehaviour component that drives the animation.
   - Collect parameters from the Inspector.
   - Call `TrajectoryGenerator` to create paths.
   - Instantiate `TrajectoryState`.
   - Update object transform via `_trajectoryState.Update()` each frame.

## UML

![Alt text](uml.png?raw=true "uml")
## Getting Started

### Installation
- Clone this repository.
- Open Unity.
- Click the "Open" button.
- Navigate to the cloned repository folder and select it.
- Unity will open the project and import the assets.

## How to Run the Project
- In the Project window, navigate to the Assets/Scenes/ folder and open the MainScene.

- In the Hierarchy window, select the PathController GameObject.

- In the Inspector window, you can configure the Trajectory Follower component. Assign your target object (e.g., a Sphere) to the Object To Move field and adjust the Start Point, End Point, and other profile parameters as needed.

- Click the Play button at the top of the editor to run the scene. You will see the object move from the start to the end point.

## How to Run the Tests

- In the Unity Editor, go to Window > General > Test Runner.
- These tests cover the core logic (TrajectoryGenerator and TrajectoryState) and run instantly without starting the game.
- In the Test Runner window, select the EditMode tab.
- You will see TrajectoryGeneratorTests and TrajectoryStateTests.
- Click Run All to execute the tests.
