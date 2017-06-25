using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InformationManager : MonoBehaviour {

    public string ipAddressBackend;
    public int portBackend = 8080;

    public string BackendAddress { get { return "http://" + ipAddressBackend + ":" + portBackend.ToString(); } }
}
