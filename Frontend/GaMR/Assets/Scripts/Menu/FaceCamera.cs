using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// objects with this component always face the player
/// </summary>
public class FaceCamera : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    public void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
    }
}
