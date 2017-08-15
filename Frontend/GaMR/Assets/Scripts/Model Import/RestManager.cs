using HoloToolkit.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Handles the requests to the backend REST-service
/// </summary>
public class RestManager : Singleton<RestManager> {

    private Dictionary<string, string> standardHeader;

    public Dictionary<string, string> StandardHeader
    {
        get
        {
            if (standardHeader == null)
            {
                standardHeader = new Dictionary<string, string>();
            }

            return standardHeader;
        }
        set { standardHeader = value; }
    }

    private void Start()
    {
        StandardHeader.Add("Content-Type", "application/json");
    }

    /// <summary>
    /// creates a coroutine which will query the url and return the result to the callback function
    /// </summary>
    /// <param name="url">The url to query</param>
    /// <param name="callback">The callback method which receives the downloaded data</param>
	public void GET(string url, System.Action<string, object[]> callback, object[] passOnArgs)
    {
        StartCoroutine(GetWWW(url, callback, passOnArgs));
    }

    /// <summary>
    /// Creates a coroutine which will post the specified data to the url
    /// </summary>
    /// <param name="url">The url to post the data to</param>
    /// <param name="json">The body of the post</param>
    public void POST(string url, string json)
    {
        StartCoroutine(UploadWWW(url, "POST", Encoding.UTF8.GetBytes(json), null));
    }

    public void POST(string url, string json, System.Action<UnityWebRequest> callback)
    {
        StartCoroutine(UploadWWW(url, "POST", Encoding.UTF8.GetBytes(json), callback));
    }

    public void POST(string url, WWWForm formData, System.Action<UnityWebRequest> callback)
    {
        StartCoroutine(UploadWWW(url, "POST", formData.data, callback));
    }

    public void POST(string url, System.Action<UnityWebRequest> callback)
    {
        StartCoroutine(UploadWWW(url, "POST", null, callback));
    }

    public void PUT(string url, string json)
    {
        StartCoroutine(UploadWWW(url, "PUT", Encoding.UTF8.GetBytes(json), null));
    }

    public void PUT(string url, string json, System.Action<UnityWebRequest> callback)
    {
        StartCoroutine(UploadWWW(url, "PUT", Encoding.UTF8.GetBytes(json), callback));
    }

    public void PUT(string url, WWWForm formData, System.Action<UnityWebRequest> callback)
    {
        StartCoroutine(UploadWWW(url, "PUT", formData.data, callback));
    }

    public void PUT(string url, System.Action<UnityWebRequest> callback)
    {
        StartCoroutine(UploadWWW(url, "PUT", null, callback));
    }

    /// <summary>
    /// Called as a coroutine and posts the data to the url.
    /// </summary>
    /// <param name="url">The url to post the data to</param>
    /// <param name="callback">Called when the operation finished</param>
    /// <returns></returns>
    private IEnumerator UploadWWW(string url, string requestType, byte[] bodyRaw, System.Action<UnityWebRequest> callback)
    {
        UnityWebRequest req = new UnityWebRequest(url, requestType);
        foreach(KeyValuePair<string, string> header in StandardHeader)
        {
            req.SetRequestHeader(header.Key, header.Value);
        }
        req.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        yield return req.Send();

        if (callback != null)
        {
            callback(req);
        }
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
    IEnumerator GetWWW(string url, System.Action<string, object[]> callback, object[] passOnArgs)
    {
        UnityWebRequest req = UnityWebRequest.Get(url);
        yield return req.Send();

        if (callback != null)
        {
            if (req.responseCode == 200)
            {
                callback(req.downloadHandler.text, passOnArgs);
            }
            else
            {
                callback(null, passOnArgs);
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
                MessageBox.Show(LocalizationManager.Instance.ResolveString("Could not fetch texture"), MessageBoxType.ERROR);
            }
        }


    }
}
