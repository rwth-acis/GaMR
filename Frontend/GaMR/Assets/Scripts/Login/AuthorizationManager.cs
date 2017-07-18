using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AuthorizationManager : MonoBehaviour {

	public static void Authorize(string name, string password)
    {
        SceneManager.LoadScene("Scene", LoadSceneMode.Single);
    }
}
