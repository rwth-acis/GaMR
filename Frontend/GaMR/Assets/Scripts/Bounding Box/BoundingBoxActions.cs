using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    /// <summary>
    /// toggles the visibility of the bounding boxx and whether or not its collider should be active
    /// </summary>
    public void ToggleBoundingBox()
    {
        if (boundingBoxVisible)
        {
            ToggleControls(false);
            boundingBoxVisible = false;
        }
        else
        {
            ToggleControls(true);
            boundingBoxVisible = true;
        }
    }

    /// <summary>
    /// toggles whether or not it is possible to add new annotations to the model by tapping on it
    /// </summary>
    public void ToogleEditMode()
    {
        attachementManager.EditMode = !attachementManager.EditMode;
    }

    /// <summary>
    /// Shows or hides all control handles which are attached to the bounding box
    /// </summary>
    /// <param name="active">The target visibility</param>
    private void ToggleControls(bool active)
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
        Destroy(gameObject);
    }

    public void SelectQuiz()
    {
        RestManager.instance.GET(InformationManager.instance.BackendAddress + "/resources/quiz/overview/" + objectInfo.ModelName, AvailableQuizzesLoaded);
    }

    public void LoadAnnotations()
    {
        attachementManager.SetAnnotationManager();
    }

    private void AvailableQuizzesLoaded(string res)
    {
        if (res == null)
        {
            MessageBox.Show(LocalizationManager.Instance.ResolveString("Server is not responding")
                + Environment.NewLine + LocalizationManager.Instance.ResolveString("Could not list available quizzes"), MessageBoxType.ERROR);
            return;
        }
        else
        {
            JsonStringArray array = JsonUtility.FromJson<JsonStringArray>(res);
            array.array.Sort();
            List<CustomMenuItem> items = new List<CustomMenuItem>();

            carouselInstance = CarouselMenu.Show();

            foreach(string quiz in array.array)
            {
                CustomMenuItem item = carouselInstance.gameObject.AddComponent<CustomMenuItem>();
                item.Init(carouselMenuStyle, new List<CustomMenuItem>(), false);
                item.onClickEvent.AddListener(delegate { OnCarouselItemClicked(quiz); });
                item.Text = quiz;
                items.Add(item);
            }

            carouselInstance.rootMenu = items;
        }
    }

    private void OnCarouselItemClicked(string quizName)
    {
        attachementManager.SetQuizManager(quizName);
        Destroy(carouselInstance.gameObject);
    }

    public void CreateNewQuiz()
    {
        Keyboard.Display(LocalizationManager.Instance.ResolveString("Enter the name of the quiz"), CreateQuiz, true);
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
