

# SCSU-Firefight

Microsoft HoloLens firefighting UI prototype developed with the Unity Game Engine. Developed for SE 345 at SCSU.

This application's main goal is to create a UI prototype for a firefighting application of the future. It runs on the assumption that if the 3D model for a building can be acquired ahead of time, a HoloLens World Anchor can be used to anchor the 3D model in the physical location of the actual building. This allows the application to take advantage of Unity's NavMesh-based pathfinding to visualize paths to different locations within the physical building. Users can also manipulate this feature by setting custom waypoints. When a waypoint is set, the pathfinder automatically finds the shortest path to the waypoint and informs the user of the distance remaining. In addition to waypoints, hazards can be marked, causing the pathfinder to find a path around the hazard. Users are also made aware of where fellow firefighters are by highlighting them through walls. The idea is that waypoints, hazards, and other firefighters would be visible to each user, allowing them to work in concert and improve situational awareness. User safety is a primary goal, so if a waypoint is not set, the pathfinder finds a path to the nearest exit.

**Uses Microsoft's Mixed Reality Toolkit:** 

https://github.com/Microsoft/MixedRealityToolkit-Unity
> Written with [StackEdit](https://stackedit.io/).
<!--stackedit_data:
eyJoaXN0b3J5IjpbMTIxNTIxNTIyNV19
-->