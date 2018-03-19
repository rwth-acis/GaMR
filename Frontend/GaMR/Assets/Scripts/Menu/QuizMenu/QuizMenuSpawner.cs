using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizMenuSpawner : MonoBehaviour {

    // Use this for initialization
    void OnEnable()
    {
        GameObject menuInstance = Instantiate(WindowResources.Instance.QuizMenu);
        CirclePositioner positioner = menuInstance.GetComponentInChildren<CirclePositioner>();
        QuizMenu menu = menuInstance.GetComponent<QuizMenu>();
        menu.BoundingBox = transform.parent.gameObject;
        positioner.boundingBox = transform.parent;
    }
}
