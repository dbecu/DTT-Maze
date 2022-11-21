using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private CharacterController controller;
    [SerializeField]
    private float speed = 4f;
    [SerializeField]
    private float gravity = -9.81f;
    [SerializeField]
    private Vector3 velocity;

    // Update is called once per frame
    void Update()
    {
        //If esc is pressed, end game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StopGame(GameObject.FindGameObjectWithTag("Finish")); //TODO: enter + touch to finish
        }

        MovePlayer();
    }

    private void MovePlayer()
    {
        //Player WASD movement 
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

    }

    //Triggers when in collision
    private void OnTriggerEnter(Collider other)
    {
        //If yellow ball gets collided with, end game
        if (other.tag == "Finish")
        {
            StopGame(other.gameObject);

        }
        else if(other.transform.parent.gameObject.TryGetComponent(out CellHandler cell)) {
            //if player just touched a new floor, darken it
            cell.DarkenFloor();
        }
    }

    //to stop game, destroy user which will put camera back to main screen
    private void StopGame(GameObject yellowBall)
    {
        Destroy(gameObject);
        Destroy(yellowBall);
        Cursor.lockState = CursorLockMode.None;
    }
}
