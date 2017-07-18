using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWindow {

    bool WindowStackable
    {
        get;
    }

    bool WindowSingleton
    {
        get;
    }

    float WindowDepth
    {
        get;set;
    }

    void CloseWindow();

}
