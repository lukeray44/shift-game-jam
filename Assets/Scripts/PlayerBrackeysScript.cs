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
        Time.timeScale = 1;
    }

    void Update()
    {
        //Debug.Log(Mathf.Abs(rigidB.linearVelocityY));
        //can = false;
        Debug.Log("Moving " + move.action.ReadValue<float>() + "\nJumping " + jump.action.IsPressed());
         if (move.action.IsPressed() || jump.action.IsPressed() || (!controller.checkGrounded(ref nothing) && Mathf.Abs(rigidB.linearVelocityY) >= 0.1f)) {
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
        if (!wasAirbourne && justLanded)
        {
            //Debug.Log("Changed");
            justLanded = false;
        }
        //jumped = jumped == true? false : jumped;
        //Debug.Log("Grounded " + checkGrounded() + " Jumped " + jump.action.IsPressed() + " Jumps " + jumps + " Result " + (jump.action.IsPressed() && jumps > 0 && checkGrounded()));
        // check whether the player is moving, has jumped or is airbourne. Set rigidbody to dynamic if true, otherwise kinematic

        wasAirbourne = rigidB.linearVelocityY >= 0.1f;
        
        //Debug.Log("Jumps after " + jumps);
        // Moves the character. 1st argument is the horizontal movement, 3rd argument is whether the player has jumped and can jump
        controller.Move(moveSpeed * move.action.ReadValue<float>() * Time.deltaTime, false, jump.action.IsPressed() && !justLanded);  
        justLanded = !wasAirbourne;
        //Debug.Log(rigidB.linearVelocity);
        // if the player is not moving and not jumping
    }
    /*bool checkGrounded()
	{
		RaycastHit2D hit = Physics2D.Raycast(feet1.transform.position, Vector3.up, 0.15f);
        RaycastHit2D actuallyHit = Physics2D.Raycast(Vector3.zero, Vector3.zero);
        if (hit) actuallyHit = hit;
        RaycastHit2D hit2 = Physics2D.Raycast(feet2.transform.position, Vector3.down, 0.15f);
        if (hit2) actuallyHit = hit2;
        RaycastHit2D bigHit = Physics2D.Raycast(bigFeet.transform.position, Vector3.down,0.15f);
        if (bigHit) actuallyHit = bigHit;
        Debug.DrawRay(hit.point, hit.normal, Color.red);
        Debug.DrawRay(hit2.point, hit2.normal, Color.black);
		Debug.Log("Hit " + Convert.ToBoolean(Physics2D.Raycast(feet1.transform.position, Vector3.down, 0.15f)) + hit.distance + "\nHit2 " + Convert.ToBoolean(Physics2D.Raycast(feet2.transform.position, Vector3.down, 0.15f)) + hit2.distance + "\nBig hit " + Convert.ToBoolean(Physics2D.Raycast(bigFeet.transform.position, Vector3.down,0.15f)) + bigHit.distance);
		Debug.Log("zippy actually hit " + actuallyHit + "\ndistance " + actuallyHit.distance + "\nAngle " + Vector3.Angle(actuallyHit.normal, Vector3.up));

        //Debug.Log("");
        if (actuallyHit.distance == 0)
        {
            return false;
        }
		Debug.Log("actually hit " + actuallyHit + "\ndistance " + actuallyHit.distance + "\nAngle " + Vector3.Angle(actuallyHit.normal, Vector3.up));

		if (actuallyHit && Math.Round(Vector3.Angle(actuallyHit.normal, Vector3.up)) == 0 /*&& Mathf.Abs(GetComponent<Rigidbody2D>().linearVelocityY) < 0.1)
		{
			return true;
		}
		return false;
	}*/

    bool checkGrounded()
	{
		RaycastHit2D hit = Physics2D.Raycast(feet1.transform.position, Vector3.down);
        RaycastHit2D hit2 = Physics2D.Raycast(feet2.transform.position, Vector3.down);
        RaycastHit2D bigHit = Physics2D.Raycast(bigFeet.transform.position, Vector3.down);
        Debug.DrawRay(hit.point, hit.normal, Color.black);
        Debug.DrawRay(hit2.point, hit2.normal, Color.black);
        //Debug.Log("");
		if ((hit && hit.distance < 0.1f && Math.Round(Vector3.Angle(hit.normal, Vector3.up)) == 90) || (hit2 && hit2.distance < 0.1f && Math.Round(Vector3.Angle(hit2.normal, Vector3.up)) == 90) || (bigHit && bigHit.distance < 0.1f) /*&& Mathf.Abs(GetComponent<Rigidbody2D>().linearVelocityY) < 0.1*/)
		{
			return true;
		}
		return false;
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
