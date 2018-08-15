using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridScale : MonoBehaviour
{

    public float gridSize = 1f;
    public Transform referenceFloor;

    private SpriteRenderer spriteRenderer;
    private Coroutine runningCoroutine;

    private void OnDestroy()
    {
        GridManager.Instance.UnRegisterGrid(this);
    }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        GridManager.Instance.RegisterGrid(this);
        UpdateGrid();
        gameObject.SetActive(false);
    }

    public void Show()
    {
        if (referenceFloor != null)
        {
            gameObject.SetActive(true);
            UpdateGrid();
            if (runningCoroutine != null)
            {
                StopCoroutine(runningCoroutine);
            }
            runningCoroutine = StartCoroutine(Fade(new Color(1, 1, 1, 0), Color.white, 1));
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (referenceFloor == null)
        {
            Destroy(gameObject);
        }
        else
        {
            UpdateGrid();
        }
    }

    public void Hide()
    {
        gameObject.SetActive(true);
        if (runningCoroutine != null)
        {
            StopCoroutine(runningCoroutine);
        }
        runningCoroutine = StartCoroutine(FadeOut());
    }

    private IEnumerator Fade(Color start, Color end, float time)
    {
        float elapsedTime = 0f;
        while (elapsedTime < time)
        {
            spriteRenderer.color = Color.Lerp(start, end, elapsedTime / time);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator FadeOut()
    {
        yield return Fade(Color.white, new Color(1, 1, 1, 0), 1);
        gameObject.SetActive(false);
    }

    private void UpdateGrid()
    {
        transform.position = referenceFloor.position + new Vector3(0, referenceFloor.localScale.y / 2f + 0.01f, 0);

        float numCellsX = Mathf.Round(referenceFloor.localScale.x / gridSize);
        float numCellsZ = Mathf.Round(referenceFloor.localScale.z / gridSize);

        //transform.position = new Vector3(
        //    Mathf.Round(transform.position.x),
        //    transform.position.y,
        //    Mathf.Round(transform.position.z)
        //    );

        //if (numCellsX %2 == 1)
        //{
        //    transform.position += new Vector3(gridSize / 2f, 0, 0);
        //}

        //if (numCellsZ % 2 == 1)
        //{
        //    transform.position += new Vector3(0, 0, gridSize / 2f);
        //}

        transform.localScale = new Vector3(
            gridSize,
            gridSize,
            transform.localScale.z
            );

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        //spriteRenderer.size = new Vector2(
        //    RoundEven(referenceFloor.localScale.z / gridSize),
        //    RoundEven(referenceFloor.localScale.x / gridSize)
        //    );

        spriteRenderer.size = new Vector2(
            Mathf.Round(referenceFloor.localScale.x / gridSize),
            Mathf.Round(referenceFloor.localScale.z / gridSize)
            );
    }

    private int RoundEven(float value)
    {
        int factor = Mathf.RoundToInt(value / 2f);
        return 2 * factor;
    }
}
