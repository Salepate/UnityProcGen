# Unity Proc Gen Graph

Procedural Generative Graph Editor

| ![hosted by ImgBB](https://i.ibb.co/gSd8FwX/Unity-BEQ761qm-Lq.png) | 
|:--:| 
| *Unity ProcGen Graph Editor (image hosted by ImgBB)*  |


## Functions

* Generate data at runtime using a graph
* Graph can also be edited at runtime to quickly iterate

## Underlying Mechanics

* Use Unity.GraphView API (poorly)
* Nodes employ polymorphism
* Use Newtonsoft (partly) for serialization

## Performances

* probably poorly for now

## Samples
| ![hosted by ImgBB](https://i.ibb.co/WD2PPT2/Unity-1-Yb6-PWx-QJV.png) | 
|:--:| 
| *Use perlin noise to decide walkable tile*  |

| ![hosted by ImgBB](https://i.ibb.co/h2JkTx9/Unity-3-UDv1-Qujq-J.png) | 
|:--:| 
| *combine perlin to generate basic heightmap*  |


## Usage

[ProcGen.GenerativeGraphInstance](https://github.com/Salepate/UnityProcGen/blob/develop/Assets/ProcGen/Runtime/GenerativeGraphInstance.cs) is the main structure to use graphs during runtime.

see [DungeonTileSpawner.cs](https://github.com/Salepate/UnityProcGen/blob/develop/Assets/Samples/Behaviours/DungeonTileSpawner.cs) for a simple use case