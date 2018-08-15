using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;

public class GridManager : Singleton<GridManager>
{
    [SerializeField]
    private GameObject gridPrefab;

    private List<GridScale> grids;
    private int calls = 0;

    public void RegisterGrid(GridScale grid)
    {
        if (grids == null)
        {
            grids = new List<GridScale>();
        }
        grids.Add(grid);

        if (calls > 0) // grids are visible
        {
            grid.Show();
        }
    }

    public void UnRegisterGrid(GridScale grid)
    {
        if (grids == null)
        {
            grids = new List<GridScale>();
        }
        grids.Remove(grid);
    }

    public void ShowGrids()
    {
        // if no other call active => activate grids
        if (calls == 0)
        {
            foreach (GridScale grid in grids)
            {
                grid.Show();
            }
        }
        calls++;
    }

    public void HideGrids()
    {
        calls--;
        if (calls <= 0)
        {
            foreach (GridScale grid in grids)
            {
                grid.Hide();
            }
            calls = 0; // make sure that it can only go to 0
        }
    }

    public void CreateNewGrid(GameObject floorObject)
    {
        GameObject gridInstance = Instantiate(gridPrefab);
        GridScale gridScale = gridInstance.GetComponent<GridScale>();
        gridScale.referenceFloor = floorObject.transform;
    }
}
