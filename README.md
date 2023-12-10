# 4D Engine Unity
A 4D Raymarching Engine for Unity that helps render 4D objects and has the following features:
* 20+ 4D shapes to choose from.
* Control over shape's dimensions and color.
* Control over shape's 3D translation and rotation.
* Supports ambient occlusion, lighting information, hard and soft shadows.
* Control over both global and local 4D translation and rotation.

//add video here

## Usage

### Importing the Package
* Clone the repository or download the `.unitypackage` from the [Releases Section](https://github.com/aniketrajnish/4D-Engine-Unity/releases/tag/v001) and import it in your project. <br><br>
  ```
  git clone https://github.com/aniketrajnish/4D-Engine-Unity.git
  ```
* The package includes a pre-configured `Test` scene that serves as a reference. You can use this as a starting point for your project.
* If you want to setup a new scene by yourself, simply follow the steps outlined below.
 
### Setting up a scene
* Open a new scene and add the `Raymarcher` component to the Main Camera.
* The `Raymarcher` script has the following paramters in the inspector:
  
  | Category             | Variable         | Description                                                                  |
  |----------------------|------------------|------------------------------------------------------------------------------|
  | **General Settings** | Shader           | Shader used for raymarching.                                                 |
  |                      | Sun              | Directional light in the scene.                                              |
  |                      | Loop             | Repetition of structures in the raymarching shader.                          |
  | **4D Settings**      | W Pos            | Global W position in 4D space.                                               |
  |                      | W Rot            | Global W rotation in 4D space.                                               |
  | **Light Settings**   | Is Lit           | Toggle lighting calculations on/off.                                         |
  |                      | Is Shadow Hard   | Toggle between hard and soft shadows.                                        |
  |                      | Is AO            | Enable or disable Ambient Occlusion.                                         |
  |                      | Light Col        | Color of the light.                                                          |
  |                      | Light Intensity  | Intensity of the light.                                                      |
  |                      | Shadow Intensity | Intensity of shadows.                                                        |
  |                      | Shadow Min       | Minimum shadow distance.                                                     |
  |                      | Shadow Max       | Maximum shadow distance.                                                     |
  |                      | Shadow Smooth    | Smoothness of shadow edges.                                                  |
  |                      | AO Step          | Step size for AO calculation.                                                |
  |                      | AO Intensity     | Intensity of AO.                                                             |
  |                      | AO Iteration     | Number of iterations for AO calculation.                                     |
  | **Render Settings**  | Max Steps        | Maximum number of steps for raymarching.                                     |
  |                      | Max Dist         | Maximum distance to raymarch before considering a hit.                       |
  |                      | Surf Dist        | Threshold for considering a hit in raymarching.                              |

### Rendering a shape
* After adding the `Raymarcher` component to the Main Camera, create an empty gameobject and add the `RaymarchRenderer` component to it.
* Click the `Create New Dimensions` button to create a scriptable object containing all the data about the shape's dimension that you can control using the custom editor for each shape.
* The `RaymarchRenderer` script has the following paramters in the inspector:
  
  | Category              | Variable         | Description                                                               |
  |-----------------------|------------------|---------------------------------------------------------------------------|
  | **Default Inspector** | Shape            | 4D shape to be rendered.                                                  |
  |                       | Operation        | Operation to be performed (union, subtraction, intersection).             |
  |                       | Color            | Color of the shape.                                                       |
  |                       | Rot W            | Local W rotation in 4D space.                                             |
  |                       | Pos W            | Local W position in 4D space.                                             |
  |                       | Blend Factor     | Currently not implemented                                                 |
  |                       | Dimensions       | Scriptable Object holding the shape's dimension data.                     |
  | **Shape Dimensions**  | Dimension Props  | The dimension properties based on the chosen 4D shape.                    |

  
## Contributing
Contributions to the project are welcome. Things that need work:
* Currently operations triggers global operation on the last object. Need to change it to local operations with the objects in proximity/chosen objects.
* Make a better collision detection system.
* Implement a system to blend different shapes.
* Implementing [n-dimensional rigid body dynamics](https://marctenbosch.com/ndphysics/).
  
## License
MIT License
