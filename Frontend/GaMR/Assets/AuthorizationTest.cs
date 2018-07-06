using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuthorizationTest : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        AuthorizationManager.Instance.Login(AuthorizationProvider.GOOGLE);
    }
}
