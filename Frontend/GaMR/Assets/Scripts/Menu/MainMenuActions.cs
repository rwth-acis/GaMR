using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuActions : MonoBehaviour
{

    public GameObject carouselMenu;
    public GameObject carouselMenuStyle;
    Menu menu;
    InformationManager infoManager;
    RestManager restManager;
    ModelLoadManager modelLoadManager;
    static GameObject carouselInstance;

    public InformationManager InfoManager { get { return infoManager; } }

    public void Start()
    {
        infoManager = ComponentGetter.GetComponentOnGameobject<InformationManager>("InformationManager");
        restManager = ComponentGetter.GetComponentOnGameobject<RestManager>("RestManager");
        modelLoadManager = ComponentGetter.GetComponentOnGameobject<ModelLoadManager>("ModelLoadManager");
        if (carouselInstance != null)
        {
            Destroy(carouselInstance);
        }
    }

    public void EnterIPAddress()
    {
        Keyboard.Display(LocalizationManager.Instance.ResolveString("Enter the IP-Address"), SetIPAddress, false);
        gameObject.SetActive(false);
    }

    public void SetIPAddress(string address)
    {
        gameObject.SetActive(true);
        // if not null => input was accepted by user
        if (address != null)
        {
            Debug.Log("Set IP Address to " + address);
            infoManager.ipAddressBackend = address;
            TestAddress();
        }
    }

    private void TestAddress()
    {
        WaitCursor.Show();
        restManager.GET(infoManager.BackendAddress + "/resources/model/overview", RestResult);
    }

    private void RestResult(string result)
    {
        WaitCursor.Hide();
        if (result != null)
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

    public void EnterPort()
    {
        Keyboard.Display(LocalizationManager.Instance.ResolveString("Enter the port"), SetIPAddress, false);
        gameObject.SetActive(false);
    }

    public void SetIPPort(string port)
    {
        gameObject.SetActive(true);
        // if not null => input was accepted by user
        if (port != null)
        {
            int iPort;
            if (int.TryParse(port, out iPort))
            {
                Debug.Log("Set Port to " + port);
                infoManager.portBackend = iPort;
                TestAddress();
            }
            else
            {
                MessageBox.Show(LocalizationManager.Instance.ResolveString("Input was not a number") + Environment.NewLine + 
                    LocalizationManager.Instance.ResolveString("Could not set port"), MessageBoxType.ERROR);
            }
        }
    }

    public void ShowCarouselMenu()
    {
        WaitCursor.Show();
        restManager.GET(infoManager.BackendAddress + "/resources/model/overview", AvailableModelsLoaded);
    }

    private void AvailableModelsLoaded(string res)
    {
        WaitCursor.Hide();
        if (res == null)
        {
            MessageBox.Show(LocalizationManager.Instance.ResolveString("Server is not responding") + Environment.NewLine + 
                LocalizationManager.Instance.ResolveString("Could not list available 3D models"), MessageBoxType.ERROR);
            return;
        }

        if (carouselInstance == null)
        {
            carouselInstance = Instantiate(carouselMenu);
        }

        JsonStringArray array = JsonUtility.FromJson<JsonStringArray>(res);
        array.array.Sort();
        List<CustomMenuItem> items = new List<CustomMenuItem>();

        foreach(string modelName in array.array)
        {
            CustomMenuItem item = carouselInstance.AddComponent<CustomMenuItem>();
            item.Init(carouselMenuStyle, new List<CustomMenuItem>(), false);
            item.onClickEvent.AddListener(delegate { OnCarouselItemClicked(modelName); });
            item.Text = modelName;
            item.menuItemName = modelName;
            items.Add(item);
        }

        CarouselMenu carouselScript = carouselInstance.GetComponent<CarouselMenu>();

        if (carouselScript != null)
        {
            carouselScript.rootMenu = items;
        }

        Destroy(gameObject);
    }

    public void OnCarouselItemClicked(string name)
    {
        modelLoadManager.Load(name);
        Destroy(carouselInstance);
        carouselInstance = null;
    }
}
