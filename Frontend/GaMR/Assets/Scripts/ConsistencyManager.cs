using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsistencyManager : MonoBehaviour
{
    [Tooltip("Id of the object; There can only be one gameobject with this id in the game. All others are destroyed")]
    public string id;
    /// <summary>
    /// The dictionary which holds the instance for each id
    /// </summary>
    private static Dictionary<string, GameObject> instances;

    /// <summary>
    /// Initializes the dictionary if it has not been initialized
    /// tests if an instance with the id already exists
    /// if not: registers this gameobject as the instance and makes it consistent across scenes
    /// if it exists: destroys it
    /// </summary>
    protected void Awake()
    {
        if (instances == null)
        {
            instances = new Dictionary<string, GameObject>();
        }

        if (instances.ContainsKey(id) && instances[id] != null)
        {
            if (instances[id] != this)
            {
                // destroy immediate so that child scripts do not get the chance to run
                DestroyImmediate(gameObject);
                Debug.Log("Destroyed duplicate of " + id);
            }
        }
        else
        {
            if (instances.ContainsKey(id))
            {
                instances[id] = gameObject;
            }
            else
            {
                instances.Add(id, gameObject);
            }
            DontDestroyOnLoad(gameObject);
        }
    }
}
