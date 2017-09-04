using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

/// <summary>
/// collects all actions which can be performed by tapping on the main menu items
/// </summary>
public class MainMenuActions : MonoBehaviour
{

    public GameObject carouselMenu;
    public GameObject carouselMenuStyle;
    Menu menu;
    static GameObject carouselInstance;


    /// <summary>
    /// fetches all necessary components for the main menu actions
    /// </summary>
    public void Start()
    {
        menu = GetComponent<Menu>();
        if (carouselInstance != null)
        {
            Destroy(carouselInstance);
        }
    }

    /// <summary>
    /// Displays a keyboard in order to enter the ip address
    /// </summary>
    public void EnterServer()
    {
        Keyboard.Display(LocalizationManager.Instance.ResolveString("Enter the Server"), InformationManager.Instance.BackendServer, SetServer, true);
        gameObject.SetActive(false);
    }

    public void EnterGamificationServer()
    {
        Keyboard.Display(LocalizationManager.Instance.ResolveString("Enter Gamification Server"), InformationManager.Instance.GamificationServer, SetGamificationServer, true);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Sets the ip address which was entered before
    /// called by the keyboard which was created in EnterIPAddress
    /// </summary>
    /// <param name="address">The address which has been typed by the user (null if cancelled)</param>
    public void SetServer(string address)
    {
        gameObject.SetActive(true);
        // if not null => input was accepted by user
        if (address != null)
        {
            Debug.Log("Set Server to " + address);
            InformationManager.Instance.BackendServer = address;
            TestServer();
        }
    }

    public void SetGamificationServer(string address)
    {
        gameObject.SetActive(true);
        // if not null => input was accepted by user
        if (address != null)
        {
            Debug.Log("Set Gamification Server to " + address);
            InformationManager.Instance.GamificationServer = address;
        }

    }

    /// <summary>
    /// Test if the server is responding by requesting the model overview
    /// </summary>
    private void TestServer()
    {
        WaitCursor.Show();
        RestManager.Instance.GET(InformationManager.Instance.FullBackendAddress + "/resources/model/overview", RestResult, null);
    }

    /// <summary>
    /// Processes the result of the TestAddress web request
    /// </summary>
    /// <param name="result">The result of the request</param>
    private void RestResult(UnityWebRequest result, object[] args)
    {
        WaitCursor.Hide();
        if (result.responseCode == 200)
        {
            MessageBox.Show(LocalizationManager.Instance.ResolveString("Address successfully saved") + Environment.NewLine + 
                LocalizationManager.Instance.ResolveString("The server is responding"), MessageBoxType.SUCCESS);
        }
        else
        {
            MessageBox.Show(LocalizationManager.Instance.ResolveString("Address successfully saved") + Environment.NewLine + 
                LocalizationManager.Instance.ResolveString("However, the server does not respond"), MessageBoxType.WARNING);
        }
    }

    /// <summary>
    /// Triggers a web request to get the model overview and later display it in a carousel menu
    /// </summary>
    public void ShowCarouselMenu()
    {
        WaitCursor.Show();
        RestManager.Instance.GET(InformationManager.Instance.FullBackendAddress + "/resources/model/overview", AvailableModelsLoaded, null);
    }

    /// <summary>
    /// Displays a carousel menu and populates it with the available 3D models
    /// called when the web request finished in ShowCarouselMenu
    /// </summary>
    /// <param name="res">The result of the web request</param>
    private void AvailableModelsLoaded(UnityWebRequest res, object[] args)
    {
        WaitCursor.Hide();
        if (res.responseCode != 200)
        {
            MessageBox.Show(LocalizationManager.Instance.ResolveString("Server is not responding") + Environment.NewLine + 
                LocalizationManager.Instance.ResolveString("Could not list available 3D models"), MessageBoxType.ERROR);
            return;
        }

        if (carouselInstance == null)
        {
            carouselInstance = Instantiate(carouselMenu);
        }

        JsonStringArray array = JsonUtility.FromJson<JsonStringArray>(res.downloadHandler.text);

        if (array.array.Count == 0)
        {
            MessageBox.Show(LocalizationManager.Instance.ResolveString("There are no 3D models to show"), MessageBoxType.INFORMATION);
            return;
        }

        array.array.Sort();
        List<CustomMenuItem> items = new List<CustomMenuItem>();

        CarouselMenu carouselScript = carouselInstance.GetComponent<CarouselMenu>();

        foreach (string modelName in array.array)
        {
            CustomMenuItem item = carouselInstance.AddComponent<CustomMenuItem>();
            item.Init(carouselMenuStyle, new List<CustomMenuItem>(), false);
            item.onClickEvent.AddListener(delegate { OnCarouselItemClicked(modelName); });
            item.Text = modelName;
            item.MenuItemName = modelName;
            items.Add(item);
        }

        if (carouselScript != null)
        {
            carouselScript.rootMenu = items;
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// Loads the selected 3D model and closes the carousel menu
    /// Called if an item on the carousel menu is tapped
    /// </summary>
    /// <param name="name">The name of the menu item</param>
    public void OnCarouselItemClicked(string name)
    {
        ModelSynchronizer.Instance.LoadModelForAll(name);
        Destroy(carouselInstance);
        carouselInstance = null;
    }

    /// <summary>
    /// Sets the language to german
    /// </summary>
    public void SetLangaugeGerman()
    {
        SetLanguage(Language.GERMAN);
    }

    /// <summary>
    /// Sets the language to english
    /// </summary>
    public void SetLanguageEnglish()
    {
        SetLanguage(Language.ENGLISH);
    }

    /// <summary>
    /// Sets the language to the specified value
    /// </summary>
    /// <param name="language">The new language</param>
    private void SetLanguage(Language language)
    {
        InformationManager.Instance.Language = language;
        menu.UpdateTexts();
    }

    /// <summary>
    /// Loads the login scene and closes the current scene
    /// </summary>
    public void LogOut()
    {
        InformationManager.Instance.UserInfo = null;
        AuthorizationManager.Instance.Logout();
        SceneManager.LoadScene("Login", LoadSceneMode.Single);
    }

    public void ToggleCollision()
    {
        InformationManager.Instance.CollisionEnabled = !InformationManager.Instance.CollisionEnabled;
    }
}
