using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridsInitializer : MonoBehaviour
{
    [SerializeField]
    private GameObject myCellPrefab;

    public int HeightAmount { get; private set; }
    public int WidthAmount { get; private set; }

    private CellHandler[,] maze;
    private List<KeyValuePair<int, int>> visitedCells;

    //UI preset
    [SerializeField]
    private UiHandler UiHandler;
    public Color WallColor { get; private set; }
    public Color FloorColor { get; private set; }
    public Color TouchedFloorColor { get; private set; }

    private bool isRemoving = false;

    // Start is called before the first frame update
    void Start()
    {
        SetUpGame();
    }

    //To cleanup previously made mazes
    void RemovePreviousSetup()
    {
        isRemoving = true;
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        maze = null;
        visitedCells = new List<KeyValuePair<int, int>>();
        isRemoving = false;
    }

    
    public void SetUpGame(int heightAmount = 10, int widthAmount = 10)
    {
        this.HeightAmount = heightAmount;
        this.WidthAmount = widthAmount;

        //Adjust camera so it focuses on the whole maze
        SetupCamera();

        var playMaze = transform.parent.GetComponent<PlayMazeHandler>();

        //Set color
        var index = Random.Range(0, playMaze.WallColors.Length);
        WallColor = playMaze.WallColors[index];
        FloorColor = playMaze.FloorColors[index];
        TouchedFloorColor = playMaze.TouchedFloorColors[index];

        RemovePreviousSetup();
        SetupMaze();
    }

    private void SetupCamera()
    {
        Camera.main.transform.position = new Vector3(WidthAmount / 2, 0, HeightAmount / 2);
        int highest = HeightAmount;
        if (WidthAmount > highest) highest = WidthAmount;
        Camera.main.orthographicSize = (float)(highest / 1.2);
    }

    private void SetupMaze()
    {
        //UiHandler.ActivateButton();
        maze = new CellHandler[WidthAmount, HeightAmount];

        //Instansiates all the cells
        for (int i = 0; i < WidthAmount; i++)
        {
            for (int j = 0; j < HeightAmount; j++)
            {
                var myGameObject = Instantiate(myCellPrefab, new Vector3(i, 0, j), Quaternion.Euler(new Vector3(0, 90, 180)), transform);
                myGameObject.transform.parent = transform;
                myGameObject.transform.localPosition = transform.position + new Vector3(i, 0, j);

                //Puts in maze for easy access
                maze[i, j] = myGameObject.GetComponent<CellHandler>();
                maze[i, j].SetPosition(new KeyValuePair<int, int>(i, j), WidthAmount, HeightAmount);
            }

        }

        StartCoroutine(CreateMaze());
    }

    private IEnumerator CreateMaze()
    {
        yield return StartCoroutine(GenerateMaze());
        UiHandler.ActivateButton(true);
    }

    private IEnumerator GenerateMaze()
    {
        UiHandler.ActivateButton(false);

        int currentCellX = Random.Range(0, WidthAmount);
        int currentCellY = Random.Range(0, HeightAmount);

        while (visitedCells.Count >= 0 && !isRemoving)
        {
            CellHandler currentCell = maze[currentCellX, currentCellY]; 

            // Gets all possible wall removals
            Wall? openThisDoor = currentCell.GetOpenableDoor();

            //If there is no possible wall removals, goes back in the visited list
            if (!openThisDoor.HasValue)
            {
                if (visitedCells.Count <= 0)
                {
                    //cause there's nothing to re-visit, so then it's done.
                    yield break;
                }
                else
                {
                    CloseNeighbourWalls(currentCellX, currentCellY);
                    currentCellX = visitedCells[visitedCells.Count - 1].Key;    //key == x
                    currentCellY = visitedCells[visitedCells.Count - 1].Value;  //value == y
                    visitedCells.RemoveAt(visitedCells.Count - 1); //removes last, which is the current currentCell
                }
            }
            else
            {
                //Doing the process of removing the wall
                visitedCells.Add(new KeyValuePair<int, int>(currentCellX, currentCellY)); 
                currentCell.OpenWall(openThisDoor.Value); //removes wall of current cell
                CloseNeighbourWalls(currentCellX, currentCellY);
                currentCell.UpdateCell();

                //remove wall on the other side
                switch (openThisDoor.Value)
                {
                    case Wall.top:
                        currentCellY += 1;
                        maze[currentCellX, currentCellY].OpenWall(Wall.bottom);
                        break;
                    case Wall.right:
                        currentCellX += 1;
                        maze[currentCellX, currentCellY].OpenWall(Wall.left);
                        break;
                    case Wall.bottom:
                        currentCellY -= 1;
                        maze[currentCellX, currentCellY].OpenWall(Wall.top);
                        break;
                    case Wall.left:
                        currentCellX -= 1;
                        maze[currentCellX, currentCellY].OpenWall(Wall.right);
                        break;
                }
            }

            yield return null;
        }

    }

    //Makes sure that neighbour's wall cannot be opened
    private void CloseNeighbourWalls(int xValue, int yValue)
    {
        //top neighbour
        if (yValue + 1 < HeightAmount) 
        {
            maze[xValue, yValue + 1].NeverOpenTheseWalls.Add(Wall.bottom);
        }

        //bottom neighbour
        if (yValue - 1 >= 0)
        {
            maze[xValue, yValue - 1].NeverOpenTheseWalls.Add(Wall.top); 
        }

        //right neighbour
        if (xValue + 1 < WidthAmount)
        {
            maze[xValue + 1, yValue].NeverOpenTheseWalls.Add(Wall.left); 
        }

        //left neighbour
        if (xValue - 1 >= 0)
        {
            maze[xValue - 1, yValue].NeverOpenTheseWalls.Add(Wall.right); 
        }
    }
}
