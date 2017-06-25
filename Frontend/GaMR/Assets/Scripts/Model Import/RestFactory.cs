using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Handles the requests to the backend REST-service
/// </summary>
public class RestFactory : MonoBehaviour {

    /// <summary>
    /// creates a Coroutine which will query the url and return the result to the callback function
    /// </summary>
    /// <param name="url">The url to query</param>
    /// <param name="callback">The callback method which receives the downloaded data</param>
	public void GET(string url, System.Action<string> callback)
    {
        StartCoroutine(GetWWW(url, callback));
    }

    public void GetTexture(string url, System.Action<Texture> callback)
    {
        StartCoroutine(GetWWWTexture(url, callback));
    }

    /// <summary>
    /// Called as a Coroutine and queries the url. When data are downloaded, they are returned to the callback-method
    /// </summary>
    /// <param name="url">The url to query</param>
    /// <param name="callback">The callback method which receives the downloaded data</param>
    /// <returns></returns>
    IEnumerator GetWWW(string url, System.Action<string> callback)
    {
        UnityWebRequest req = UnityWebRequest.Get(url);
        yield return req.Send();

        if (callback != null)
        {
            callback(req.downloadHandler.text);
        }
    }

    IEnumerator GetWWWTexture(string url, System.Action<Texture> callback)
    {
        UnityWebRequest req = UnityWebRequest.GetTexture(url);
        yield return req.Send();

        if (callback != null)
        {
            callback(DownloadHandlerTexture.GetContent(req));
        }


    }
}
