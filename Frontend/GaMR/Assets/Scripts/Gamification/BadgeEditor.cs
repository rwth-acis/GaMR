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

    private static BadgeManager badgeManager;
    private static GamificationManager gamificationManager;


    private void Awake()
    {
        stextMenuItem = textMenuItem;
        sbadgeMenuItem = badgeMenuItem;
    }

    private void Start()
    {
        badgeManager = GetComponentInChildren<BadgeManager>();
        gamificationManager = GetComponentInChildren<GamificationManager>();
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

            for (int i = 0; i < array.array.Count; i++)
            {
                CustomMenuItem item = carouselMenuInstance.gameObject.AddComponent<CustomMenuItem>();
                item.Init(stextMenuItem, new List<CustomMenuItem>(), false);
                item.onClickEvent.AddListener(OnCarouselItemClicked);
                item.Text = array.array[i];
                item.MenuItemName = array.array[i];
                items.Add(item);
            }

            carouselMenuInstance.rootMenu = items;

            ReplaceWithImages();
        }
    }

    private static void OnCarouselItemClicked(string badgeId)
    {
        Badge selectedBadge = null;

        // try to find the badge
        // if the badge does not exist => create it
        GamificationFramework.Instance.GetBadgeWithId(gamificationManager.gameId, badgeId,
            (resBadge, resCode) =>
            {
                if (resCode == 200)
                {
                    selectedBadge = resBadge;
                    SetBadgeAndLoadImage(selectedBadge);
                }
                else
                {
                    // create the new badge
                    Badge newBadge = new Badge(badgeId, badgeId, badgeId);
                    // first: load the image of the badge
                    RestManager.Instance.GetTexture(InformationManager.Instance.BackendAddress + "/resources/badges/" + badgeId,
                        (reqRes, badgeTexture) =>
                        {
                            if (reqRes.responseCode == 200)
                            {
                                newBadge.Image = (Texture2D)badgeTexture;

                                GamificationFramework.Instance.CreateBadge(gamificationManager.gameId, newBadge,
                                    createCode =>
                                    {
                                        if (createCode == 200 || createCode == 201)
                                        {
                                            selectedBadge = newBadge;
                                            SetBadgeAndLoadImage(selectedBadge);
                                        }
                                    }
                                    );
                            }
                        }
                        );
                }
            });
    }

    private static void SetBadgeAndLoadImage(Badge selectedBadge)
    {
        // check if the badge was loaded and if it has an image
        if (selectedBadge != null)
        {
            // if image already loaded => just set the badge as current badge
            if (selectedBadge.Image != null)
            {
                gamificationManager.Badge = selectedBadge;
            }
            else // load the image and then set the badge as current badge
            {
                GamificationFramework.Instance.GetBadgeImage(gamificationManager.gameId, selectedBadge.ID,
                    (badgeTexture, imageCode) =>
                    {
                        if (imageCode == 200)
                        {
                            selectedBadge.Image = (Texture2D)badgeTexture;

                            gamificationManager.Badge = selectedBadge;
                        }
                        else
                        {
                            MessageBox.Show(LocalizationManager.Instance.ResolveString("Error while getting badge image.\nBadge was not saved"), MessageBoxType.ERROR);
                        }
                    }
                    );
            }
        }
        else // if selectedBadge is null, loading and creating of the badge failed
        {
            MessageBox.Show(LocalizationManager.Instance.ResolveString("Error while getting or creating badge.\nBadge was not saved"), MessageBoxType.ERROR);
        }
    }

    private static void ReplaceWithImages()
    {
        for (int i = 0; i < carouselMenuInstance.rootMenu.Count; i++)
        {
            object[] arg = { i };
            RestManager.Instance.GetTexture(InformationManager.Instance.BackendAddress + "/resources/badges/" + carouselMenuInstance.rootMenu[i].MenuItemName, Replace, arg);
        }
    }

    private static void Replace(UnityWebRequest req, Texture tex, object[] passOnArgs)
    {
        if (req.responseCode == 200)
        {
            int i = (int)passOnArgs[0];
            carouselMenuInstance.rootMenu[i].menuStyle = sbadgeMenuItem;
            carouselMenuInstance.rootMenu[i].Icon = tex;
            carouselMenuInstance.ResetMenu();
        }
    }
}
