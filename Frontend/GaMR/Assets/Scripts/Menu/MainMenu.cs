using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{

    private static GameObject instance;

    public void Start()
    {
        Show();
    }

    public static void Show()
    {
        instance = (GameObject)GameObject.Instantiate(Resources.Load("MainMenu"));
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
