# XML Serialization
**zero-hassle runtime loading and saving of entire scenes**

 This package provides a framework for saving entire scenes through the use of surrogates. 
Surrogate classes allow special-case code to be ran during serialization and de-serialization.
Surrogates can be written for any class. Associate a surrogate class with the class to be serialized by
tagging the surrogate class with an attribute.

## [Install through the Package Manager](https://docs.unity3d.com/Manual/upm-ui-giturl.html)

## Class overview
* **XMLSerialization** provides load and save methods for serializing any serializeable class to xml files.
* Component **XMLSerializeable** marks a GameObject as serializeable.
  * Upon **serialization**, it stores GameObject data within the serializeable class *GameObjectXML*, and finds surrogates
    for all attached components. Components without surrogates will not be serialized.
  * Upon **de-serialization**, the resource prefab at the path *resourceID* is 
    instantiated and the surrogate is de-serialized onto that.
  * Filling out **persistentID** marks this GameObject as persistent, meaning it will not be destroyed 
    before deserializing a currently loaded scene. Leaving it blank means the object is non-persistent
    and must be re-instantiated.
    * **Persistent gameObjects** are good for moving doors or GameObjects containing game logic, things that aren't
      intended to ever be destroyed. They do not requre a path to a resource for re-instantiation as it is expected
      deserializing onto the existing persistent gameobject will re-create its serialized state completely.
    * **non-persistent gameObjects** are good for NPCs, projectiles, or special effects. To save time, upon deserializing
      a currently loaded scene, all non-persistent gameobjects will be destroyed and re-instantiated since they can't
      be guaranteed to exist. These gameobject shouldn't rely heavily on external references to function as these are
      difficult to serialize.
* Component **PersistentParent** provide non-persistent gameObjects a parent to be instantiated onto. 
  * Despite being able do so in the editor, Unity provides abosulutely no way to obtain a reliable reference
    to an existing GameObject during serialization at runtime. InstanceIds change every time the scene is loaded. Using an 
    int[] to describe a GameObject's position in the heirarchy is unreliable because sibling order can change at random
    during runtime. There is no way to assign a persisten GUID as there is no *"OnInstantiate"* method and the closest
    method *"Awake"* is called every time it is loaded and the value will always be different. The only reliable way
    to ID a GameObject is to do it manually, and thus PersistentParent was born.
  
