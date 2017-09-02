using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ThumbnailInstantiation : MonoBehaviour
{
    public GameObject thumbnail;
    public GameObject startPosition;
    private Vector3 size;
    private List<Thumbnail> thumbnails;
    private int startIndex = 0;
    private FocusableButton upButton;
    private FocusableButton downButton;

    private void Start()
    {
        thumbnails = new List<Thumbnail>();
        BoxCollider coll = startPosition.GetComponent<BoxCollider>();
        size = coll.size;
        InitializeButtons();
        InstantiateThumbnails();
        RestManager.Instance.GET(InformationManager.Instance.FullBackendAddress + "/resources/model/overview", AvailableModelsLoaded);
    }

    private void InitializeButtons()
    {
        upButton = transform.Find("Up Button").gameObject.AddComponent<FocusableButton>();
        downButton = transform.Find("Down Button").gameObject.AddComponent<FocusableButton>();

        upButton.FocusHighlight = upButton.transform.Find("Up Frame").gameObject;
        downButton.FocusHighlight = downButton.transform.Find("Down Frame").gameObject;

        upButton.OnPressed = PageUp;
        downButton.OnPressed = PageDown;

        upButton.ButtonEnabled = false;

        // localization:
        upButton.transform.Find("Caption").GetComponent<TextMesh>().text = LocalizationManager.Instance.ResolveString("Page up");
        downButton.transform.Find("Caption").GetComponent<TextMesh>().text = LocalizationManager.Instance.ResolveString("Page down");
    }

    private void PageDown()
    {
        Debug.Log("Down");
        startIndex += thumbnails.Count;
        RestManager.Instance.GET(InformationManager.Instance.FullBackendAddress + "/resources/model/overview", AvailableModelsLoaded);
    }

    private void PageUp()
    {
        Debug.Log("Up");
        startIndex -= thumbnails.Count;
        RestManager.Instance.GET(InformationManager.Instance.FullBackendAddress + "/resources/model/overview", AvailableModelsLoaded);
    }

    private void AvailableModelsLoaded(UnityWebRequest res)
    {
        if (res.responseCode == 200)
        {
            JsonStringArray array = JsonUtility.FromJson<JsonStringArray>(res.downloadHandler.text);
            for(int i=0;i<thumbnails.Count;i++)
            {
                int iModel = i + startIndex;
                if (iModel < array.array.Count)
                {
                    thumbnails[i].LoadImage(array.array[iModel]);
                    thumbnails[i].Visible = true;
                }
                else
                {
                    thumbnails[i].Visible = false;
                }
            }

            if (array.array.Count > startIndex + thumbnails.Count)
            {
                downButton.ButtonEnabled = true;
            }
            else
            {
                downButton.ButtonEnabled = false;
            }

            if (startIndex > 0)
            {
                upButton.ButtonEnabled = true;
            }
            else
            {
                upButton.ButtonEnabled = false;
            }
        }
    }

    private void InstantiateThumbnails()
    {
        for (int i=0;i<2;i++)
        {
            for (int j=0;j<4;j++)
            {
                Vector3 instantiationPosition = new Vector3(
                    0,
                    size.y / 2f * -i - size.y/4f,
                    size.z / 4f * -j - size.z / 8f);
                GameObject thumbnailObj = Instantiate(thumbnail, startPosition.transform);

                thumbnailObj.transform.localPosition = instantiationPosition;

                thumbnailObj.transform.localScale = new Vector3(9, 9, 9);

                Thumbnail thumbnailScript = thumbnailObj.GetComponent<Thumbnail>();
                thumbnails.Add(thumbnailScript);
            }
        }
    }
}
