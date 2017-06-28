using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages important settings of the application
/// contains definitions for the IP address of the backend,
/// the port and a property which combines all into a http address which can be called
/// </summary>
public class InformationManager : MonoBehaviour {

    public string ipAddressBackend = "192.168.178.43";
    public int portBackend = 8080;

    /// <summary>
    /// http address which combines the ip address and the port
    /// </summary>
    public string BackendAddress { get { return "http://" + ipAddressBackend + ":" + portBackend.ToString(); } }
}
