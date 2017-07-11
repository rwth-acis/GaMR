using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyScale : MonoBehaviour {

    public Transform target;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.localScale = target.localScale;
	}
}
