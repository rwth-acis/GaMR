using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowManager : MonoBehaviour
{
    private static WindowManager instance;

    public static WindowManager Instance
    {
        get { return instance; }
    }

    private List<IWindow> openWindows;


    // Use this for initialization
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        openWindows = new List<IWindow>();
    }

    public void Add(IWindow window)
    {
        if (window.WindowStackable)
        {
            foreach(IWindow w in openWindows)
            {
                w.WindowDepth += 0.1f;
            }
        }
        else
        {
            List<IWindow> tempCopy = openWindows;
            foreach (IWindow w in tempCopy)
            {
                w.CloseWindow();
            }
        }

        openWindows.Add(window);
        window.WindowDepth = 2f;
    }

    public void Close(IWindow window)
    {
        window.CloseWindow();
        int toDelete = openWindows.IndexOf(window);
        for (int i = 0; i < openWindows.Count; i++)
        {
            if (i > toDelete)
            {
                openWindows[i].WindowDepth -= 0.1f;
            }
        }
        openWindows.RemoveAt(toDelete);
    }
}
