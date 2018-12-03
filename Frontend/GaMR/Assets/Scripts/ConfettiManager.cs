using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfettiManager : LocalSingleton<ConfettiManager>
{

    protected override void Awake()
    {
        base.Awake();
        gameObject.SetActive(false);
    }

    public void ShootConfetti(Vector3 position)
    {
        gameObject.SetActive(false);
        gameObject.transform.position = position;
        gameObject.SetActive(true);
    }
}
