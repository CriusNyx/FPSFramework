# FPSFramework

# [Demo](https://www.youtube.com/watch?v=dhxvxxY98KU)

Included is a small framework for working on FPS games. Below is a list of modules included in this framework.

## Dependencies
This framework depends on [UnityUtilities](https://github.com/CriusNyx/UnityUtilities)

IMPORTANT!!! You must enable Auto Sync Transforms in ProjecSettings/Physics for this to function as expected.

## Actor Module

Actors are MonoBehaviours that 
* manage dependencies between FPS modules
* provide a common interface for other classes to interact with actors
* automatically initialize other important FPS components

Actors have the following sub components
* MovementController
* ViewController
* Health
* PlatformController

## Movement Controller
A class that manages smooth movement in complex 3D spaces, such as shifting gravity and moving platforms.
Provides an interface for controlling the movement controller
Conforms to the EventSystem pattern provided by UnityUtilites 

## View Controller
A class that manages smooth camera movement in complex 3D spaces, such as shifting gravity and moving platforms.
Provides an interface for controlling the camera
Conforms to the EventSystem pattern provided by UnityUtilities

## Health System
Manages health for actors, and implements damage and death using functional programming and event handlers.
Also provides system for managing hitboxes/hurtboxes and hitscans.

## Platform Controller
Automatically manages collisions and movement in different coordinate spaces, and on platforms.
This component should require little interaction outside the framework, unless modification to platform detection is nessessariy.

## Spring System
A tree based spring system that allows heigharical based spring smothing over transforms.