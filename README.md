## OptiRingMesh
A Unity3D Utility Component to dynamically generate ring meshes with various parameters. Can be used for UI's and Visual Effects. Improves iteration speed and workflow compared to manual 3D remodeling.

<img width="357" height="190" alt="rings" src="https://github.com/user-attachments/assets/a97d7d09-1b8a-4098-8432-ad4c6ffa1e1b" />
<img width="371" height="190" alt="paveikslas" src="https://github.com/user-attachments/assets/be7afc18-3ab6-47f9-8f63-c569ff0f144c" />



### Installation

-   Download the package or clone the repository
-   Import the package in Unity or Drag and Drop the script file in the Unity project
-   Add the script component to a GameObject in the scene

Please make sure that the script component is added to a GameObject that has both MeshFilter and MeshRenderer component, otherwise it will fail to generate the mesh.


### Usage:

-   Attach this script to a GameObject in your scene
-   Adjust the parameters to generate the desired ring mesh
-   3D mesh updates on the fly

### Usage example 

```cs
  OptiRingMesh ring = gameObject.AddComponent<OptiRingMesh>();
  
  ring.radius = 10;
  ring.thickness = 2;
  ring.segments = 24;
  ring.angleRange = 90;
  ring.useXZPlane = false;
  
  ring.GenerateMesh()
```
you can find the example scene in the package or in the repository as well


### Features:

-   Generates a ring mesh on either the xz plane or the xy plane
-   Uses a single Mesh Filter and Mesh Renderer Component
-   Provides methods to update the mesh without allocating memory
-   Resizes the mesh if the radius or thickness changes
-   Hides obsolete mesh if the angle range changes
-   experimental uv circular mapping support

### Limitations and Todo's
Current implementation is slow when changing topology at runtime. Optimisations must be done.
Due to shared vertices UV mapping wraps wrong with naive algorithm. Either each quad must use different vertices or mirror UV wrapping should be applied.

### Licensing
This code is released under the MIT license.

### Authors
The original author of this script is Lukas Kornilovas, but you are welcome to submit pull requests and contribute to this project.
