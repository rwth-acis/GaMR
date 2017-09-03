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
    private FocusableButton settingsButton;
    private FocusableButton logoutButton;
    private FocusableButton badgeButton;

    private List<string> models;
    private bool menuEnabled = true;


    public bool MenuEnabled
    {
        get { return menuEnabled; }
        set
        {
            menuEnabled = value;
            foreach (Thumbnail t in thumbnails)
            {
                t.ButtonEnabled = menuEnabled;
            }
            if (menuEnabled)
            {
                SetButtonStates();
            }
            else
            {
                upButton.ButtonEnabled = false;
                downButton.ButtonEnabled = false;
            }
        }
    }

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
        settingsButton = transform.Find("Settings Button").gameObject.AddComponent<FocusableButton>();
        logoutButton = transform.Find("Logout Button").gameObject.AddComponent<FocusableButton>();
        badgeButton = transform.Find("Badges Button").gameObject.AddComponent<FocusableButton>();

        upButton.OnPressed = PageUp;
        downButton.OnPressed = PageDown;
        settingsButton.OnPressed = ShowSettings;
        logoutButton.OnPressed = Logout;


        // localization:
        upButton.Text = LocalizationManager.Instance.ResolveString("Page up");
        downButton.Text = LocalizationManager.Instance.ResolveString("Page down");
        settingsButton.Text = LocalizationManager.Instance.ResolveString("Settings");
        logoutButton.Text = LocalizationManager.Instance.ResolveString("Logout");
        badgeButton.Text = LocalizationManager.Instance.ResolveString("Badges");


        // determine badge button visibility and fill the gap
        bool showBadgeButton = InformationManager.Instance.playerType != PlayerType.AUTHOR;

        badgeButton.gameObject.SetActive(showBadgeButton);
        GameObject topMenu = transform.Find("Top Menu").gameObject;
        topMenu.SetActive(!showBadgeButton);
        GameObject topMenuShort = transform.Find("Top Menu Short").gameObject;
        topMenuShort.SetActive(showBadgeButton);


        MenuEnabled = false;
    }

    private void Logout()
    {
        AuthorizationManager.Instance.Logout();
    }

    private void ShowSettings()
    {
        
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
            models = array.array;
            for(int i=0;i<thumbnails.Count;i++)
            {
                int iModel = i + startIndex;
                if (iModel < array.array.Count)
                {
                    thumbnails[i].Visible = true;
                    thumbnails[i].LoadImage(array.array[iModel]);
                }
                else
                {
                    thumbnails[i].Visible = false;
                }
            }

            SetButtonStates();
        }
    }

    private void SetButtonStates()
    {
        if (models.Count > startIndex + thumbnails.Count)
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
                thumbnailScript.InstantiationParent = this;
                thumbnailScript.Visible = false;
                thumbnails.Add(thumbnailScript);
            }
        }
    }

    public void OnThumbnailClicked(string modelName)
    {
        MenuEnabled = false;
        ModelLoadManager mln = new ModelLoadManager();
        mln.Load(modelName, OnModelLoadReady);
    }

    private void OnModelLoadReady()
    {
        MenuEnabled = true;
    }
}
