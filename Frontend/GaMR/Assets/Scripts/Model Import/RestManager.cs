using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Handles the requests to the backend REST-service
/// </summary>
public class RestManager : MonoBehaviour {

    public static RestManager instance;

    public void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    /// <summary>
    /// creates a coroutine which will query the url and return the result to the callback function
    /// </summary>
    /// <param name="url">The url to query</param>
    /// <param name="callback">The callback method which receives the downloaded data</param>
	public void GET(string url, System.Action<string> callback)
    {
        StartCoroutine(GetWWW(url, callback));
    }

    /// <summary>
    /// Creates a coroutine which will post the specified data to the url
    /// </summary>
    /// <param name="url">The url to post the data to</param>
    /// <param name="json">The body of the post</param>
    public void POST(string url, string json)
    {
        StartCoroutine(PostWWW(url, json));
    }

    /// <summary>
    /// Called as a coroutine and posts the data to the url.
    /// </summary>
    /// <param name="url">The url to post the data to</param>
    /// <param name="json">The body of the post</param>
    /// <returns></returns>
    private IEnumerator PostWWW(string url, string json)
    {
        UnityWebRequest req = new UnityWebRequest(url, "POST");
        req.SetRequestHeader("Content-Type", "application/json");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        req.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        yield return req.Send();
        Debug.Log(req.responseCode);
    }

    public void GetTexture(string url, System.Action<Texture> callback)
    {
        StartCoroutine(GetWWWTexture(url, callback));
    }

    /// <summary>
    /// Called as a coroutine and queries the url. When data are downloaded, they are returned to the callback-method
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
            if (req.responseCode == 200)
            {
                callback(req.downloadHandler.text);
            }
            else
            {
                callback(null);
            }
        }
    }

    /// <summary>
    /// Called as a Coroutine and queries the url. Expects an image as the query-result
    /// When the image is downloaded, it is returned to the specified callback method
    /// </summary>
    /// <param name="url">The url to query</param>
    /// <param name="callback">The callback method which receives the downloaded image</param>
    /// <returns></returns>
    IEnumerator GetWWWTexture(string url, System.Action<Texture> callback)
    {
        UnityWebRequest req = UnityWebRequest.GetTexture(url);
        yield return req.Send();

        if (callback != null)
        {
            if (req.responseCode == 200)
            {
                callback(DownloadHandlerTexture.GetContent(req));
            }
            else
            {
                MessageBox.Show("Could not fetch texture", MessageBoxType.ERROR);
            }
        }


    }
}
