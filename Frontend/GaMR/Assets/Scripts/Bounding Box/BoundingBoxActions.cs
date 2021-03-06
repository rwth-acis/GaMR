﻿using HoloToolkit.Sharing.Tests;
using HoloToolkit.Unity.InputModule;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Collection of actions which are performed on the bounding box and by the attached menu
/// </summary>
public class BoundingBoxActions : MonoBehaviour
{

    public GameObject carouselMenuStyle;

    private bool boundingBoxVisible = true;
    public List<Transform> boundingBoxPieces;
    private BoxCollider coll;
    private AttachementManager attachementManager;
    private ObjectInfo objectInfo;
    private Transform x3dParent;
    private CarouselMenu carouselInstance;
    private bool nextQuizPositionToName;
    private CustomTapToPlace tapToPlace;
    private TransformationManager transformationManager;
    private BoundingBoxInfo info;

    /// <summary>
    /// Get the necessary components: the collider of the bounding box and its annotationManager
    /// </summary>
    public void Start()
    {
        coll = GetComponent<BoxCollider>();
        attachementManager = gameObject.GetComponentInChildren<AttachementManager>();
        x3dParent = transform.Find("Content/X3D Parent");
        if (x3dParent != null)
        {
            objectInfo = x3dParent.GetComponent<ObjectInfo>();
        }
        tapToPlace = GetComponent<CustomTapToPlace>();
        transformationManager = GetComponent<TransformationManager>();
        info = GetComponent<BoundingBoxInfo>();
    }

    /// <summary>
    /// toggles the visibility of the bounding boxx and whether or not its collider should be active
    /// </summary>
    public void EnableBoundingBox(bool enable)
    {
            EnableControls(enable);
            boundingBoxVisible = enable;
            tapToPlace.enabled = enable;
            transformationManager.enabled = enable;
    }

    /// <summary>
    /// toggles whether or not it is possible to add new annotations to the model by tapping on it
    /// </summary>
    public void EnableEditMode(bool enable)
    {
        attachementManager.EditMode = enable;
    }

    /// <summary>
    /// Shows or hides all control handles which are attached to the bounding box
    /// </summary>
    /// <param name="active">The target visibility</param>
    private void EnableControls(bool active)
    {
        coll.enabled = active;
        foreach(Transform boxPart in boundingBoxPieces)
        {
            boxPart.gameObject.SetActive(active);
        }
    }

    /// <summary>
    /// Destroys the bounding box and its content
    /// </summary>
    public void DeleteObject()
    {
        CustomMessages.Instance.SendModelDelete(info.Id);
        DeleteLocalObject();
    }

    public void DeleteLocalObject()
    {
        Debug.Log("Destroy object");
        info.Menu.Destroy();
        Destroy(gameObject);
    }

    public void SelectQuiz()
    {
        RestManager.Instance.GET(InformationManager.Instance.FullBackendAddress + "/resources/quiz/overview/" + objectInfo.ModelName, AvailableQuizzesLoaded, null);
    }

    public void LoadAnnotations()
    {
        attachementManager.SetAnnotationManager();
    }

    private void AvailableQuizzesLoaded(UnityWebRequest res, object[] args)
    {
        if (res.responseCode != 200)
        {
            MessageBox.Show(LocalizationManager.Instance.ResolveString("Server is not responding")
                + Environment.NewLine + LocalizationManager.Instance.ResolveString("Could not list available quizzes"), MessageBoxType.ERROR);
            return;
        }
        else
        {
            JsonStringArray array = JsonUtility.FromJson<JsonStringArray>(res.downloadHandler.text);
            if (array.array.Count == 0)
            {
                MessageBox.Show(LocalizationManager.Instance.ResolveString("There are no quizzes to show for this 3D model"), MessageBoxType.INFORMATION);
                return;
            }
            array.array.Sort();

            if (info.QuizSelectionMenu == null)
            {
                GameObject quizSelectionMenuInstance = Instantiate(WindowResources.Instance.QuizSelectionMenu);
                info.QuizSelectionMenu = quizSelectionMenuInstance.GetComponent<QuizSelectionMenu>();
            }

            if (info.QuizSelectionMenu != null)
            {
                info.QuizSelectionMenu.gameObject.SetActive(true); // show quiz selection menu
                info.QuizSelectionMenu.gameObject.GetComponent<CirclePositioner>().boundingBox = transform;
                info.QuizSelectionMenu.Items = array.array;
                info.QuizSelectionMenu.OnCloseAction = OnQuizSelected;
            }
            else
            {
                Debug.LogError("Expected QuizSelectionMenu but did not find one");
            }


            //carouselInstance = CarouselMenu.Show();

            //foreach(string quiz in array.array)
            //{
            //    CustomMenuItem item = carouselInstance.gameObject.AddComponent<CustomMenuItem>();
            //    item.Init(carouselMenuStyle, new List<CustomMenuItem>(), false);
            //    item.onClickEvent.AddListener(delegate { OnCarouselItemClicked(quiz); });
            //    item.Text = quiz;
            //    items.Add(item);
            //}

            //carouselInstance.rootMenu = items;
        }
    }

    private void OnQuizSelected(bool wasQuizSelected, string quizName)
    {
        if (wasQuizSelected)
        {
            attachementManager.SetQuizManager(quizName);
            ((QuizManager)attachementManager.Manager).PositionToName = nextQuizPositionToName;
        }
    }

    public void CreateNewQuiz()
    {
        Keyboard.Display(LocalizationManager.Instance.ResolveString("Enter the name of the quiz"), CreateQuiz, true);
    }

    public void SelectQuizPositionToName()
    {
        nextQuizPositionToName = true;
        SelectQuiz();
    }

    public void SelectQuizNameToPosition()
    {
        nextQuizPositionToName = false;
        SelectQuiz();
    }

    public void SelectQuizRandom()
    {
        int decision = UnityEngine.Random.Range(0, 2);
        if (decision == 0)
        {
            nextQuizPositionToName = false;
        }
        else
        {
            nextQuizPositionToName = true;
        }
        SelectQuiz();
    }

    public void AbortQuizSelection()
    {
        if (info.QuizSelectionMenu != null && info.QuizSelectionMenu.gameObject.activeSelf)
        {
            info.QuizSelectionMenu.Close();
        }
    }

    private void CreateQuiz(string text)
    {
        if (text != null)
        {
            attachementManager.SetQuizManager(text);
        }
    }

    public void ConvertAnnotations()
    {
        Keyboard.Display(LocalizationManager.Instance.ResolveString("Enter the name of the quiz"), ConvertToQuiz, true);
    }

    private void ConvertToQuiz(string res)
    {
        if (res != null)
        {
            attachementManager.ConvertToQuizManager(res);
        }
    }
}
