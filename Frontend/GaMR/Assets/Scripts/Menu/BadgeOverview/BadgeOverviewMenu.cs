using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadgeOverviewMenu : BaseMenu
{

    public GameObject badge;
    public GameObject startPosition;

    private Vector3 size;

    private List<BadgeManager> instantiatedBadges;

    private List<KeyValuePair<string,Badge>> badges;

    private int startIndex = 0;
    private FocusableButton upButton;
    private FocusableButton downButton;
    private FocusableButton closeButton;

    private bool menuEnabled = true;

    public Action OnCloseAction { get; set; }

    public bool MenuEnabled
    {
        get { return menuEnabled; }
        set
        {
            menuEnabled = value;
            if (menuEnabled)
            {
                SetButtonStates();
            }
            else
            {
                upButton.ButtonEnabled = false;
                downButton.ButtonEnabled = false;
            }
            closeButton.ButtonEnabled = menuEnabled;
        }
    }

    protected override void Start()
    {
        base.Start();
        instantiatedBadges = new List<BadgeManager>();
        BoxCollider coll = startPosition.GetComponent<BoxCollider>();
        size = coll.size;
        InitializeButtons();
        InstantiateBadges();
        GamificationFramework.Instance.GetAllBadgesOfUser(BadgesLoaded);
    }

    private void InitializeButtons()
    {
        upButton = transform.Find("Up Button").gameObject.GetComponent<FocusableButton>();
        downButton = transform.Find("Down Button").gameObject.GetComponent<FocusableButton>();
        closeButton = transform.Find("Close Button").gameObject.GetComponent<FocusableButton>();

        upButton.OnPressed = PageUp;
        downButton.OnPressed = PageDown;
        closeButton.OnPressed = Close;

        OnUpdateLanguage();

        MenuEnabled = false;
    }

    private void Close()
    {
        if (OnCloseAction != null)
        {
            OnCloseAction();
            Destroy(gameObject);
        }
    }

    public override void OnUpdateLanguage()
    {
        // localization:
        upButton.Text = LocalizationManager.Instance.ResolveString("Page up");
        downButton.Text = LocalizationManager.Instance.ResolveString("Page down");
        closeButton.Text = LocalizationManager.Instance.ResolveString("Close");
    }

    private void PageDown()
    {
        Debug.Log("Down");
        startIndex += instantiatedBadges.Count;
        UpdateBadgeDisplay();
    }

    private void BadgesLoaded(List<KeyValuePair<string, Badge>> badgesOfAllGames, long resCode)
    {
        closeButton.ButtonEnabled = true;
        if (resCode == 200 && badgesOfAllGames != null)
        {
            badges = badgesOfAllGames;
            badges.Sort((badge1, badge2) =>
            {
                return string.Compare(badge1.Key, badge2.Key);
            });
            LoadImages();
        }
    }

    private void PageUp()
    {
        Debug.Log("Up");
        startIndex -= instantiatedBadges.Count;
        UpdateBadgeDisplay();
    }

    private void LoadImages()
    {
        int imagesLoaded = 0;
        for (int i=0;i<badges.Count;i++)
        {
            int indexForLambda = i;
            GamificationFramework.Instance.GetBadgeImage(badges[i].Key, badges[i].Value.ID,
                (tex, resCode) =>
                {
                    if (resCode == 200)
                    {
                        badges[indexForLambda].Value.Image = (Texture2D) tex;
                    }
                    imagesLoaded++;

                    if (imagesLoaded >= badges.Count)
                    {
                        MenuEnabled = true;
                        UpdateBadgeDisplay();
                    }
                }
                );
        }
    }

    private void UpdateBadgeDisplay()
    {
        for (int i = 0; i < instantiatedBadges.Count; i++)
        {
            int iBadge = i + startIndex;
            if (iBadge < badges.Count)
            {
                instantiatedBadges[i].gameObject.SetActive(true);
                instantiatedBadges[i].Badge = badges[iBadge].Value;
            }
            else
            {
                instantiatedBadges[i].gameObject.SetActive(false);
            }
        }

        SetButtonStates();
    }

    private void SetButtonStates()
    {
        if (badges.Count > startIndex + instantiatedBadges.Count)
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


    private void InstantiateBadges()
    {
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                Vector3 instantiationPosition = new Vector3(
                    0,
                    size.y / 2f * -i - size.y / 4f,
                    size.z / 4f * -j - size.z / 8f);
                GameObject badgeObj = Instantiate(badge, startPosition.transform);

                badgeObj.transform.localPosition = instantiationPosition;

                badgeObj.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);

                BadgeManager badgeManager = badgeObj.GetComponent<BadgeManager>();
                badgeManager.gameObject.SetActive(false);
                instantiatedBadges.Add(badgeManager);
            }
        }
    }
}
