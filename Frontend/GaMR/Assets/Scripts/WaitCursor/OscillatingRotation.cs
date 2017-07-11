using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OscillatingRotation : MonoBehaviour {

    public float degreesPerSecond = 50;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, Mathf.Sin(Time.time) * degreesPerSecond * Time.deltaTime);
    }
}
