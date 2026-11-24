using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBrackeysScript : MonoBehaviour
{
    public CharacterController2D controller;
    public Rigidbody2D rigidB;
    public InputActionMap inputActionSet;
    public InputActionReference move;

    public InputActionReference jump;

    public Vector3 origin;

    public GameObject bigFeet;
    public GameObject feet1;
    public GameObject feet2;

    public float moveSpeed;
    public bool wasAirbourne;
    public float time = 0.25f;
    public bool justLanded; // whether the character had just landed a frame ago
    private string nothing = "";
    
    
    void Start()
    {
        rigidB.bodyType = RigidbodyType2D.Dynamic;
        Time.timeScale = time;
    }

    void Update()
    {
        double a = 0;
        // check whether the player is moving, has jumped or is airbourne. Set rigidbody to dynamic if true, otherwise kinematic
         if (move.action.IsPressed() || jump.action.IsPressed() || (!controller.checkGrounded(ref nothing, ref a))) {
            rigidB.bodyType = RigidbodyType2D.Dynamic;
            //rigidB.linearVelocityY = 0;
        } else
        {
            rigidB.bodyType = RigidbodyType2D.Kinematic;
            rigidB.useFullKinematicContacts = true;
            rigidB.linearVelocityY = 0;
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        double a = 0;
        if (!wasAirbourne && justLanded)
        {
            //Debug.Log("Changed");
            justLanded = false;
        }
        //jumped = jumped == true? false : jumped;
        //Debug.Log("Grounded " + checkGrounded() + " Jumped " + jump.action.IsPressed() + " Jumps " + jumps + " Result " + (jump.action.IsPressed() && jumps > 0 && checkGrounded()));

        wasAirbourne = rigidB.linearVelocityY >= 0.1f;
        
        //Debug.Log("Jumps after " + jumps);
        // Moves the character. 1st argument is the horizontal movement, 3rd argument is whether the player has jumped and can jump
        controller.Move(moveSpeed * move.action.ReadValue<float>(), false, jump.action.IsPressed() && !justLanded);  
        if (!controller.checkGrounded(ref nothing, ref a))
        {
            Debug.Log("movespeed " + moveSpeed + "\nMove " + move.action.ReadValue<float>());
        }
        justLanded = !wasAirbourne;
        //Debug.Log(rigidB.linearVelocity);
        // if the player is not moving and not jumping
    }

    bool checkSlope()
    {
        return true;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Money")
        {
            Destroy(collision.gameObject);
        }
    }
}
