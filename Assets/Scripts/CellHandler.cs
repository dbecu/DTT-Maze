using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class CellHandler : MonoBehaviour
{
    public HashSet<Wall> NeverOpenTheseWalls { get; private set; } = new HashSet<Wall>();
    public HashSet<Wall> OpenedWalls { get; private set; } = new HashSet<Wall>();
    public KeyValuePair<int, int> CellPosition { get; private set; }

    [SerializeField]
    private GameObject topWall;
    [SerializeField]
    private GameObject rightWall;
    [SerializeField]
    private GameObject bottomWall;
    [SerializeField]
    private GameObject leftWall;
    [SerializeField]
    private GameObject floor;

    private bool floorDarkened = false;

    private GridsInitializer parentGrid;

    // Start is called before the first frame update
    void Start()
    {
        if (parentGrid == null) parentGrid = transform.parent.GetComponent<GridsInitializer>();
        SetCellColors();
    }

    //UI
    public void DarkenFloor()
    {
        if (!floorDarkened)
        {
            var renderer = floor.GetComponent<Renderer>();
            renderer.material.SetColor("_Color", parentGrid.TouchedFloorColor);
        }
    }

    private void SetCellColors()
    {
        PutColorToCell(topWall);
        PutColorToCell(rightWall);
        PutColorToCell(bottomWall);
        PutColorToCell(leftWall);
        PutColorToCell(floor, false);
    }

    private void PutColorToCell(GameObject cellPart, bool isWall = true)
    {
        Color color = parentGrid.WallColor;
        if (!isWall)
        {
            color = parentGrid.FloorColor;
        }

        cellPart.GetComponent<Renderer>()?.material.SetColor("_Color", color);
    }

    //Ensures the out of bound walls will neer be opened
    public void SetPosition(KeyValuePair<int, int> position, int gridWidth, int gridHeight)
    {
        this.CellPosition = position;

        if (position.Key + 1 >= gridWidth) NeverOpenTheseWalls.Add(Wall.right);
        if (position.Key <= 0) NeverOpenTheseWalls.Add(Wall.left);
        if (position.Value + 1 >= gridHeight) NeverOpenTheseWalls.Add(Wall.top);
        if (position.Value <= 0) NeverOpenTheseWalls.Add(Wall.bottom);

    }

    //sets active the walls
    public void UpdateCell()
    {
        topWall.gameObject.SetActive(!OpenedWalls.Contains(Wall.top));
        rightWall.gameObject.SetActive(!OpenedWalls.Contains(Wall.right));
        bottomWall.gameObject.SetActive(!OpenedWalls.Contains(Wall.bottom));
        leftWall.gameObject.SetActive(!OpenedWalls.Contains(Wall.left));
    }

    private HashSet<Wall> GetOpenableDoors()
    {
        //get all wall possibilities
        HashSet<Wall> walls = ((Wall[]) Enum.GetValues(typeof(Wall))).ToHashSet();

        //remove the outofbound walls
        walls = walls.Except(NeverOpenTheseWalls).ToHashSet();

        //remove walls that were opened
        walls = walls.Except(OpenedWalls).ToHashSet();

        return walls;
    }

    //Gets random door than can be opened
    public Wall? GetOpenableDoor()
    {
        var walls = GetOpenableDoors();
        return walls.Count > 0 ? walls.ToList()[UnityEngine.Random.Range(0, walls.Count)] : null;
    }


    public void OpenWall(Wall wall)
    {
        GameObject wallObject = null;
        switch (wall)
        {
            case Wall.top:
                wallObject = topWall.gameObject;
                break;
            case Wall.right:
                wallObject = rightWall.gameObject;
                break;
            case Wall.bottom:
                wallObject = bottomWall.gameObject;
                break;
            case Wall.left:
                wallObject = leftWall.gameObject;
                break;
        }

        wallObject.SetActive(false);
        OpenedWalls.Add(wall);

    }
}

public enum Wall
{
    top, right, bottom, left
}