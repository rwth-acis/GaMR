using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowManager : Singleton<WindowManager>
{
    private static WindowManager instance;

    private List<Window> openWindows;


    // Use this for initialization
    void Start()
    {
        openWindows = new List<Window>();
    }

    public void Add(Window window)
    {
        if (window.stackable)
        {
            foreach(Window w in openWindows)
            {
                w.WindowDepth += 0.1f;
            }
        }
        else
        {
            List<Window> tempCopy = openWindows;
            foreach (Window w in tempCopy)
            {
                w.Close();
            }
        }

        openWindows.Add(window);
        window.WindowDepth = 2f;
    }

    public void Remove(Window window)
    {
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
