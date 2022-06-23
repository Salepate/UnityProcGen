# Unity Proc Gen Graph

Procedural Generative Graph Editor

| ![hosted by ImgBB](https://i.ibb.co/cFP24KB/Unity-y0-Op34-GHvj.png) | 
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