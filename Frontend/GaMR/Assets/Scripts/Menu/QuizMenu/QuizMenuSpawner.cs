using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizMenuSpawner : MonoBehaviour {

    [SerializeField]
    private GameObject spawnMenu;

    // Use this for initialization
    void Start()
    {
        GameObject menuInstance = Instantiate(spawnMenu);
        CirclePositioner positioner = menuInstance.GetComponentInChildren<CirclePositioner>();
        QuizMenu menu = menuInstance.GetComponent<QuizMenu>();
        menu.BoundingBox = transform.parent.gameObject;
        positioner.boundingBox = transform.parent;
    }
}
