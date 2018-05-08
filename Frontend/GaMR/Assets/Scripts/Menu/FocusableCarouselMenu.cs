using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusableCarouselMenu : BaseMenu
{
    int currentPointer = 0;

    public List<string> Items { get; set; }

    private void Awake()
    {
        Items = new List<string>();
    }

    protected override void Start()
    {
        base.Start();
        Items.Add("hello");
        Items.Add("world");
        Items.Add("three");
        Items.Add("four");
        CreateView();
    }

    public void CreateView()
    {
        for (int i = 0; i < Items.Count; i++)
        {
            GameObject buttonObj = GameObject.Instantiate(WindowResources.Instance.CarouselMenuItem, Vector3.zero, Quaternion.identity, transform);
            FocusableButton btn = buttonObj.GetComponent<FocusableButton>();
            btn.Text = Items[i];
            btn.transform.localPosition = PositionOnCircle(Vector3.zero, 2, i * 90f);
        }
    }

    private Vector3 PositionOnCircle(Vector3 center, float radius, float angle)
    {
        Vector3 position = new Vector3(
            center.x + radius * Mathf.Sin(angle * Mathf.Deg2Rad),
            center.y,
            center.z + radius * Mathf.Sin(angle * Mathf.Deg2Rad)
            );
        return position;
    }
}
