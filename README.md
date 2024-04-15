# Swarm Pathfinding

## Setup
 - The project requires unity version 2022.3.20f1 which can be downloaded using [unity hub](https://unity.com/unity-hub).

## Running
 1. Open the project using unity version 2022.3.20f1.
 2. Open the Project tab in unity.
 3. Open the Scenes folder and double click `WaypointsNoObstacle` or `WaypointsWithObstacle`.
 4. Under the Hierarchy tab there will be two objects, `Flock` and `Waypoints`. The Flock object spawns all the boids and keeps track of them. It also has two children `Bounds` which controls the bounds of the simulation and `SpawnPoint` which can be used to control where the boids will spawn from.
 5. Clicking play will start the simulation and once all the waypoints have been reached the simulation will stop and the time taken will be logged to the console tab.
 
## Change Parameters
 1. Go to the Project tab.
 2. Under the Prefabs folder there will be a Boid prefab, click on it.
 3. In the inspector window, there will be a script component named `Boid`.
 4. Change these variables and restart the simulation by clicking play. Note that you must restart for the changes to take effect. For testing the social and cognitive variables were varied from 0 to 1. Ensuring they both add up to 1.
 5. The Technique variable can be used to switch between global and individual data for the PSOP algorithm.
