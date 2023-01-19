## OptiRingMesh

This Unity3D script generates an optimised ring mesh with a given radius and thickness, with a given number of segments and angle range.

### Features:

-   Generates a ring mesh on either the xz plane or the xy plane
-   Uses a single Mesh Filter and Mesh Renderer
-   Provides methods to update the mesh without allocating memory
-   Resizes the mesh if the radius or thickness changes
-   Hides obsolete mesh if the angle range changes
-   experimental uv circular mapping support
-   works great in edit mode without warnings

### Usage:

-   Attach this script to a GameObject in your scene
-   Adjust the radius, thickness, segments, angle range, and useXZPlane parameters to generate the desired ring mesh
-   The OnValidate() function will automatically update the mesh when these parameters are changed


### Installation

-   Download the package or clone the repository
-   Import the package in Unity or Drag and Drop the script file in the Unity project
-   Add the script component to a GameObject in the scene

Please make sure that the script component is added to a GameObject that has both MeshFilter and MeshRenderer component, otherwise it will fail to generate the mesh.

### Example

Here is an example of how to use this script, you can find the example scene in the package or in the repository as well

  OptiRingMesh ring = gameObject.AddComponent<OptiRingMesh>();
  
  ring.radius = 10;
  ring.thickness = 2;
  ring.segments = 24;
  ring.angleRange = 90;
  ring.useXZPlane = false;
  
  ring.GenerateMesh()


### Limitations and Todo's
Current implementation is slow when changing topology at runtime. Optimisations must be done.
Due to shared vertices UV mapping wraps wrong with naive algorithm. Either each quad must use different vertices or mirror UV wrapping should be applied.

### Licensing
This code is released under the MIT license.

### Authors
The original author of this script is Viktor Kornilov, but you are welcome to submit pull requests and contribute to this project.
