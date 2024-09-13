# Cosmere Based Game

This project tries to create a game using the mechanics that exists in the universe of the "Cosmere"

# Introduction
## 1. What is the Cosmere?

I you are interested in this project you surely know what the Cosmere is. 
But if you do not know it's okay, I will explain briefly. 

The Cosmere is a universe ideated by Brandon Sanderson. 
This websites compiles a lot of detailed information about it and is a great source of inspiration to help with the integration of the diferent mechanics. 

- [Coppermind - The Cosmere](https://coppermind.net/wiki/Cosmere)

The Cosmere is composed of multiple worlds. There are 2 worlds that are the most popular ones:
Roshar and Scadrial where the adventures of The Stormlight Archive and The Mistborn Series occur. 
## 2. Goal of this project

The goal is to try and build an environment where a player can get immersed in the epic adventures of some of the characters of the Cosmere such as Kaladin. 


# Documentation

## 1. Engine
This project is being build using Unity 2022.3 with the Universal Render Pipeline (URP).

It also uses the Unity Input Actions system to help with the development. 

## 2. Folder Structure
Inside the Assets folder the structure is as follows:
0. Scripts
1. Animations
2. Prefabs
3. Materials
4. Meshes
5. Scenes
6. Shaders
7. Textures
8. Icons
9. Packages

Each name is self explanatory.

## 3. Player Behaviour and Interactions

For now the main mechanic being implemented is all from Kaladin's powers. This includes, Basic Lash, Full Lash and Inverse Lash with the use of Stormlight and a bit of fighting with a spear. 

### PlayerStateMachine

To implement the character movement and abilities we use a Hierarchical Finite State Machine.
If you do not know what it is I recommend reading [this](https://gamedevbeginner.com/state-machines-in-unity-how-and-when-to-use-them/) article or whatching [this](https://www.youtube.com/watch?v=Vt8aZDPzRjI) youtube series.

This image shows a basic representation of all the existing states. 


### Interactions

The interactions work with the Interactable Interface. 

There are these types of Interactables objects:
- **Pickable**: Basically spheres containing stormlight that are automatically picked when approached.
- **Infusable**: Can be infused with Stormlight for different effects.

