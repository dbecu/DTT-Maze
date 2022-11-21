using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMazeHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private GameObject objectivePrefab; //yellow ball
    [SerializeField]
    public GridsInitializer mazeGrid; 

    //Color ui presets
    [SerializeField]
    public Color[] WallColors;
    [SerializeField]
    public Color[] FloorColors;
    [SerializeField]
    public Color[] TouchedFloorColors;

    public void StartGame()
    {
        //Get random location for yellow ball
        Vector3 position = new Vector3(
            (float) (Random.Range(1f,  mazeGrid.WidthAmount) + 0.3), 
            -30,
            (float) (Random.Range(1f, mazeGrid.HeightAmount) + 0.3));

        //Instansiate player & yellow ball
        Instantiate(playerPrefab, new Vector3(0.1f, -30f, 0.3f), transform.rotation, transform);
        Instantiate(objectivePrefab, position, transform.rotation, transform);
    }
}
