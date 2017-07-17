using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject menuObject;

    private static GameObject staticMenuObject;
    private static GameObject instance;

    public void Start()
    {
        // initialize for static call
        staticMenuObject = menuObject;
        Show();
    }

    public static void Show()
    {
        instance = GameObject.Instantiate(staticMenuObject);
        instance.transform.position = Camera.main.transform.position + new Vector3(-0.2f,0,0) + 2 * Camera.main.transform.forward;
    }

    public static void Close()
    {
        Destroy(instance);
        instance = null;
    }

    public static GameObject Instance
    {
        get { return instance; }
    }
}
