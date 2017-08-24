using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the logic in order to display and modify a carousel menu
/// </summary>
public class CarouselMenu : Menu
{

    private static CarouselMenu instance;

    /// <summary>
    /// the index of the element in the rootMenu which is shown in the middle
    /// </summary>
    private int currentIndex;
    /// <summary>
    /// The buttons which scroll left or right
    /// </summary>
    public GameObject buttonLeft, buttonRight;
    public Transform leftPosition, rightPosition, leftBackPosition, rightBackPosition;
    Vector3[] pos = new Vector3[5];
    Vector3[] rot = new Vector3[5];
    /// <summary>
    /// this is used to lock the object if there is currently movement
    /// without this lock the user can break the CarouselMenu by tapping on the scroll-button
    /// while movement is still happening since this will create new movement coroutines
    /// </summary>
    private int currentlyMoving = 0;

    [SerializeField]
    private float normalMoveTime = 1f;
    [SerializeField]
    private float fastMoveTime = 0.2f;


    /// <summary>
    /// Called if the Component is created
    /// Instantiates a new Carousel Menu
    /// </summary>
    public void Start()
    {
        InstantiateCarouselMenu(0);
    }

    public static CarouselMenu Show()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
        }
        GameObject carouselInstance = (GameObject)Instantiate(Resources.Load("Carousel Menu"));
        instance = carouselInstance.GetComponent<CarouselMenu>();
        return instance;
    }

    /// <summary>
    /// Overwrites the InstantiateMenu of the Menu-super class
    /// Redirects to InstantiateCarouselMenu for the Instantiation of the carousel menu
    /// the parameters are not used since the CarouselMenu behaves different to the menu
    /// </summary>
    /// <param name="instantiatePosition">not used</param>
    /// <param name="parentItemSize">not used</param>
    /// <param name="menu">not used</param>
    /// <param name="parent">not used</param>
    /// <param name="isSubMenu">not used</param>
    public void InstantiateMenu(Vector3 instantiatePosition, Vector3 parentItemSize, List<CustomMenuItem> menu, CustomMenuItem parent, bool isSubMenu)
    {
        InstantiateCarouselMenu(currentIndex);
    }

    /// <summary>
    /// Instantiates a CarouselMenu
    /// </summary>
    /// <param name="startIndex">The index of the element in the root-menu 
    /// which should initially be displayed in the middle</param>
    public void InstantiateCarouselMenu(int startIndex)
    {
        // initialize the index and the positions
        currentIndex = startIndex;
        pos[0] = leftBackPosition.localPosition;
        pos[1] = leftPosition.localPosition;
        pos[2] = Vector3.zero;
        pos[3] = rightPosition.localPosition;
        pos[4] = rightBackPosition.localPosition;
        rot[0] = new Vector3(0, 90, 0);
        rot[1] = new Vector3(0, 45, 0);
        rot[2] = Vector3.zero;
        rot[3] = new Vector3(0, -45, 0);
        rot[4] = new Vector3(0, -90, 0);


        for (int i = -1; i <= 1; i++)
        {
            if (startIndex + i >= 0 && startIndex + i < rootMenu.Count)
            {
                rootMenu[startIndex + i].Create(this, null);
                rootMenu[startIndex + i].Position = pos[i + 2];
                rootMenu[startIndex + i].GameObjectInstance.transform.localEulerAngles = rot[i + 2];
            }
        }

        // add the button component to the buttons
        LongPressButton btnLeft = buttonLeft.AddComponent<LongPressButton>();
        LongPressButton btnRight = buttonRight.AddComponent<LongPressButton>();
        btnLeft.OnPressed = ScrollLeftNormal;
        btnLeft.OnLongPressed = ScrollLeftFast;
        btnRight.OnPressed = ScrollRightNormal;
        btnRight.OnLongPressed = ScrollRightFast;
    }

    public override void ResetMenu()
    {
        DestroyAll();
        InstantiateCarouselMenu(currentIndex);
    }

    private void DestroyAll()
    {
        for (int i=0;i<rootMenu.Count;i++)
        {
            rootMenu[i].Destroy();
        }
    }

    public void ScrollLeftNormal()
    {
        ScrollLeft(normalMoveTime);
    }

    public void ScrollRightNormal()
    {
        ScrollRight(normalMoveTime);
    }

    public void ScrollLeftFast()
    {
        ScrollLeft(fastMoveTime);
    }

    public void ScrollRightFast()
    {
        ScrollRight(fastMoveTime);
    }

    /// <summary>
    /// called if the corresponding button is tapped
    /// initiates the movement
    /// creates and destroys menu entries which appear and disappear
    /// also handles special cases like reaching the limit of the rootMenu-array
    /// </summary>
    private void ScrollLeft(float moveTime)
    {
        Debug.Log("Clicked left");
        if (currentlyMoving == 0)
        {
            // if it is the most left element => indicate movement and then undo it
            if (currentIndex == 0)
            {
                // perform this for the current middle and the right element
                for (int i = 0; i < 2; i++)
                {
                    if (currentIndex + i < rootMenu.Count) // if there is only one element => don't try to move other elements
                    {
                        // only indicate the movement but don't perform it completely
                        Vector3 target = pos[i + 2] + 0.2f * (pos[i + 3] - pos[i + 2]);
                        float targetRot = 0.2f * rot[i + 3].y;
                        StartCoroutine(TryMove(rootMenu[currentIndex + i].GameObjectInstance.transform, target, targetRot, moveTime / 2f));
                    }
                }
            }
            // else: move everything to the right, create a new entry on the left and destroy the last one on the right
            else
            {
                // instantiate the next left menu item if it exists
                if (currentIndex > 1)
                {
                    rootMenu[currentIndex - 2].Create(this, null);
                    // position at the back
                    rootMenu[currentIndex - 2].Position = pos[0];
                    rootMenu[currentIndex - 2].GameObjectInstance.transform.localEulerAngles = rot[0];
                    // move it to the next position on the right
                    StartCoroutine(Move(rootMenu[currentIndex - 2].GameObjectInstance.transform, pos[1], rot[1].y, moveTime));
                }

                // move everything else
                for (int i = -1; i < 2; i++)
                {
                    if (currentIndex + i < rootMenu.Count) // if there is only one element => don't try to move other elements
                    {
                        // perform movement to next position
                        Vector3 target = pos[i + 3];
                        float targetRot = rot[i + 3].y;
                        if (i != 1)
                        {
                            StartCoroutine(Move(rootMenu[currentIndex + i].GameObjectInstance.transform, target, targetRot, moveTime));
                        }
                        else if (currentIndex + i < rootMenu.Count) // if it is the most right and exists => move and then destroy
                        {
                            StartCoroutine(MoveAndDestroy(rootMenu[currentIndex + i].GameObjectInstance.transform, target, targetRot, 1f, rootMenu[currentIndex + i]));
                        }
                    }
                }
                currentIndex--;
            }
        }
    }

    /// <summary>
    /// called if the corresponding button is tapped
    /// initiates the movement
    /// creates and destroys menu entries which appear and disappear
    /// also handles special cases like reaching the limit of the rootMenu-array
    /// </summary>
    private void ScrollRight(float moveTime)
    {
        Debug.Log("Clicked right");
        if (currentlyMoving == 0)
        {
            // if it is the most right element => indicate movement and then undo it
            if (currentIndex == rootMenu.Count - 1)
            {
                // perform this for the current middle and the right element
                for (int i = -1; i < 1; i++)
                {
                    if (currentIndex + i >= 0) // if there is only one element => don't try to move other elements
                    {
                        // only indicate the movement but don't perform it completely
                        Vector3 target = pos[i + 2] + 0.2f * (pos[i + 1] - pos[i + 2]);
                        float targetRot = 0.2f * rot[i + 1].y;
                        StartCoroutine(TryMove(rootMenu[currentIndex + i].GameObjectInstance.transform, target, targetRot, moveTime / 2f));
                    }
                }
            }
            // else: move everything to the right, create a new entry on the right and destroy the last one on the left
            else
            {
                // instantiate the next right menu item if it exists
                if (currentIndex < rootMenu.Count - 2)
                {
                    rootMenu[currentIndex + 2].Create(this, null);
                    // position at the back
                    rootMenu[currentIndex + 2].Position = pos[4];
                    rootMenu[currentIndex + 2].GameObjectInstance.transform.localEulerAngles = rot[4];
                    // move it to the next left position
                    StartCoroutine(Move(rootMenu[currentIndex + 2].GameObjectInstance.transform, pos[3], rot[3].y, moveTime));
                }


                for (int i = -1; i < 2; i++)
                {
                    if (currentIndex + i >= 0) // if there is only one element => don't try to move other elements
                    {
                        // perform movement to next position
                        Vector3 target = pos[i + 1];
                        float targetRot = rot[i + 1].y;
                        if (i != -1)
                        {
                            StartCoroutine(Move(rootMenu[currentIndex + i].GameObjectInstance.transform, target, targetRot, moveTime));
                        }
                        else if (currentIndex + i >= 0) // if it is the most left and exists => move and then destroy
                        {
                            StartCoroutine(MoveAndDestroy(rootMenu[currentIndex + i].GameObjectInstance.transform, target, targetRot, 1f, rootMenu[currentIndex + i]));
                        }
                    }
                }
                currentIndex++;
            }
        }
    }

    /// <summary>
    /// Manages the movement to indicate that the end of the menu-array is reached
    /// The movement to the next position is only indicated and then undone
    /// </summary>
    /// <param name="toMove">The transform which should be moved</param>
    /// <param name="newPos">The position where toMove should move to</param>
    /// <param name="newYAngle">The y-rotation toMove should have at newPos</param>
    /// <param name="duration">The duration of one half of the movement</param>
    /// <returns></returns>
    private IEnumerator TryMove(Transform toMove, Vector3 newPos, float newYAngle, float duration)
    {
        currentlyMoving++;
        //float elapsedTime = 0;
        Vector3 startingPos = toMove.localPosition;
        float startingYAngle = toMove.localEulerAngles.y;
        // move to target position
        yield return Move(toMove, newPos, newYAngle, duration);
        // then undo movement again
        yield return Move(toMove, startingPos, startingYAngle, duration);
        currentlyMoving--;
    }

    /// <summary>
    /// Manages the scrolling movement of one menu entry to its next position
    /// </summary>
    /// <param name="toMove">The transform of the menu entry which should be moved</param>
    /// <param name="newPos">The target position of the movement</param>
    /// <param name="newYAngle">The y-rotation that toMove should haved at newPos</param>
    /// <param name="duration">The duration of the movement</param>
    /// <returns></returns>
    private IEnumerator Move(Transform toMove, Vector3 newPos, float newYAngle, float duration)
    {
        currentlyMoving++;
        float elapsedTime = 0;
        Vector3 startingPos = toMove.localPosition;
        float startingYAngle = toMove.localEulerAngles.y;
        // move until time is up
        while (elapsedTime < duration)
        {
            toMove.localPosition = Vector3.Lerp(startingPos, newPos, elapsedTime / duration);
            toMove.localEulerAngles = new Vector3(0, Mathf.LerpAngle(startingYAngle, newYAngle, elapsedTime / duration), 0);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        toMove.localPosition = newPos;
        toMove.localEulerAngles = new Vector3(0, newYAngle, 0);
        currentlyMoving--;
    }

    /// <summary>
    /// Manages the movement of a menu entry which is scrolling out of the currently displayed carousel
    /// destroys the GameObject of the menu entry when the movement has finished
    /// </summary>
    /// <param name="toMove"></param>
    /// <param name="newPos"></param>
    /// <param name="newYAngle"></param>
    /// <param name="duration"></param>
    /// <param name="toDestroy"></param>
    /// <returns></returns>
    private IEnumerator MoveAndDestroy(Transform toMove, Vector3 newPos, float newYAngle, float duration, CustomMenuItem toDestroy)
    {
        currentlyMoving++;
        yield return Move(toMove, newPos, newYAngle, duration);
        toDestroy.Destroy();
        currentlyMoving--;
    }
}
