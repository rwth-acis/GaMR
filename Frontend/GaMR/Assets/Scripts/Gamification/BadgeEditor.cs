using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BadgeEditor : MonoBehaviour
{
    private static GameObject stextMenuItem;
    private static GameObject sbadgeMenuItem;
    private static CarouselMenu carouselMenuInstance;

    public GameObject textMenuItem;
    public GameObject badgeMenuItem;


    private void Awake()
    {
        stextMenuItem = textMenuItem;
        sbadgeMenuItem = badgeMenuItem;
    }

    private void Start()
    {
        ShowBadges();
    }

    public static void ShowBadges()
    {
        RestManager.Instance.GET(InformationManager.Instance.BackendAddress + "/resources/badges/overview", BadgeOverviewLoaded);
    }

    private static void BadgeOverviewLoaded(UnityWebRequest req)
    {
        if (req.responseCode == 200)
        {
            JsonStringArray array = JsonUtility.FromJson<JsonStringArray>(req.downloadHandler.text);

            carouselMenuInstance = CarouselMenu.Show();

            List<CustomMenuItem> items = new List<CustomMenuItem>();

            for (int i=0;i<array.array.Count;i++)
            {
                CustomMenuItem item = carouselMenuInstance.gameObject.AddComponent<CustomMenuItem>();
                item.Init(stextMenuItem, new List<CustomMenuItem>(), false);
                //item.onClickEvent.AddListener(delegate { OnCarouselItemClicked(modelName); });
                item.Text = array.array[i];
                item.MenuItemName = array.array[i];
                items.Add(item);
            }

            carouselMenuInstance.rootMenu = items;

            ReplaceWithImages(carouselMenuInstance);
        }
    }

    private static void ReplaceWithImages(CarouselMenu menu)
    {
        for (int i =0;i < menu.rootMenu.Count;i++)
        {
            RestManager.Instance.GetTexture(InformationManager.Instance.BackendAddress + "/resources/badges/" + menu.rootMenu[i].MenuItemName, Replace
                );
        }
    }

    private static void Replace(UnityWebRequest req, Texture tex)
    {
        if (req.responseCode == 200)
        {
            carouselMenuInstance.rootMenu[0].menuStyle = sbadgeMenuItem;
            carouselMenuInstance.rootMenu[0].Icon = tex;
            carouselMenuInstance.ResetMenu();
        }
    }
}
