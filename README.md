# Unity Proc Gen Graph

Procedural Generative Graph Editor

| ![hosted by ImgBB](https://i.ibb.co/PrKhzKm/Unity-qu-R2-BCr-MO4.png) | 
|:--:| 
| *Unity ProcGen Graph Editor (image hosted by ImgBB)*  |


** Relies on Newtonsoft for Serialization

## Intention

* Control procedural generation through a graph that can be evaluated runtime.

## Underlying Mechanics

* Use Unity.GraphView API (poorly)
* Nodes are polymorphics.
* Evaluation is currently done by recursion (will be revamped using stacks)

## Performances

*Maybe someday*

## Samples
| ![hosted by ImgBB](https://i.ibb.co/WD2PPT2/Unity-1-Yb6-PWx-QJV.png) | 
|:--:| 
| *Use perlin noise to decide walkable tile*  |
