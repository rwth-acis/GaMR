using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IViewEvents
{
    void UpdateView();

    void Close();

    void Show();

    void Destroy();
}
