using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarouselMenu : Menu
{

    private int currentIndex;
    public GameObject buttonLeft, buttonRight;
    public Transform leftPosition, rightPosition, leftBackPosition, rightBackPosition;
    Vector3[] pos = new Vector3[5];
    Vector3[] rot = new Vector3[5];
    private bool currentlyMoving = false;

    public void Start()
    {
        InstantiateCarouselMenu(0);
    }

    public new void InstantiateMenu(Vector3 instantiatePosition, Vector3 parentItemSize, List<CustomMenuItem> menu, CustomMenuItem parent, bool isSubMenu)
    {
        InstantiateCarouselMenu(currentIndex);
    }

    public void InstantiateCarouselMenu(int startIndex)
    {
        // init the index and the positions
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
        Button btnLeft = buttonLeft.AddComponent<Button>();
        Button btnRight = buttonRight.AddComponent<Button>();
        btnLeft.OnPressed = ScrollLeft;
        btnRight.OnPressed = ScrollRight;
    }

    public void ScrollLeft()
    {
        Debug.Log("Clicked left");
        if (!currentlyMoving)
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
                        StartCoroutine(TryMove(rootMenu[currentIndex + i].GameObjectInstance.transform, target, targetRot, 0.5f));
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
                    StartCoroutine(Move(rootMenu[currentIndex - 2].GameObjectInstance.transform, pos[1], rot[1].y, 1f));
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
                            StartCoroutine(Move(rootMenu[currentIndex + i].GameObjectInstance.transform, target, targetRot, 1f));
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

    public void ScrollRight()
    {
        Debug.Log("Clicked right");
        if (!currentlyMoving)
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
                        StartCoroutine(TryMove(rootMenu[currentIndex + i].GameObjectInstance.transform, target, targetRot, 0.5f));
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
                    StartCoroutine(Move(rootMenu[currentIndex + 2].GameObjectInstance.transform, pos[3], rot[3].y, 1f));
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
                            StartCoroutine(Move(rootMenu[currentIndex + i].GameObjectInstance.transform, target, targetRot, 1f));
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

    private IEnumerator TryMove(Transform toMove, Vector3 newPos, float newYAngle, float duration)
    {
        currentlyMoving = true;
        //float elapsedTime = 0;
        Vector3 startingPos = toMove.localPosition;
        float startingYAngle = toMove.localEulerAngles.y;
        // move to target position
        yield return Move(toMove, newPos, newYAngle, duration);
        // then undo movement again
        yield return Move(toMove, startingPos, startingYAngle, duration);

        currentlyMoving = false;
    }

    private IEnumerator Move(Transform toMove, Vector3 newPos, float newYAngle, float duration)
    {
        currentlyMoving = true;
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
        currentlyMoving = false;
    }

    private IEnumerator MoveAndDestroy(Transform toMove, Vector3 newPos, float newYAngle, float duration, CustomMenuItem toDestroy)
    {
        yield return Move(toMove, newPos, newYAngle, duration);
        toDestroy.Destroy();
    }
}
