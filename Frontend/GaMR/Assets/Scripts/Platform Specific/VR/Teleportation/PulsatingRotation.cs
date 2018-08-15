using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulsatingRotation : MonoBehaviour
{
    float time = 0f;
    float speed = 0.5f;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float degrees = (Mathf.Sin(time * speed) + 1) / 2f * 360f;
        transform.localEulerAngles = new Vector3(
            transform.localEulerAngles.x,
            transform.localEulerAngles.y,
            degrees
            );

        time += Time.deltaTime;
        if (time > 2 * Mathf.PI / speed)
        {
            time %= (2 * Mathf.PI / speed);
        }
    }
}
