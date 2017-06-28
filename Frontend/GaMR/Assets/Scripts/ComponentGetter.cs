using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentGetter {

    /// <summary>
    /// Method which abbreviates the often used code fragment to get a component from a gameobject if it exists
    /// </summary>
    /// <typeparam name="T">The type of the component on the gameobject</typeparam>
    /// <param name="gameObjectName">The name of the gameobject which has the component</param>
    /// <returns>the component or null if the component or the gameobject do not exist</returns>
	public static T GetComponentOnGameobject<T>(string gameObjectName)
    {
        // first find the gameobject
        // this check is necessary since the gameobject could not exist
        GameObject go = GameObject.Find(gameObjectName);
        if (go != null)
        {
            return go.GetComponent<T>();
        }
        else
        {
            // if it is null => the component will return null (i.e. default value)
            return default(T);
        }
    }
}
