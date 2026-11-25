using System;
using Sirenix.OdinInspector.Editor.Internal;
using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
	[SerializeField] private float m_JumpForce = 400f;							// Amount of force added when the player jumps.
	[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;			// Amount of maxSpeed applied to crouching movement. 1 = 100%
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
	[SerializeField] private bool m_AirControl = false;							// Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;	
	[SerializeField] private Transform m_GroundCheck;						// A mask determining what is ground to the character
	[SerializeField] private Transform m_SlopeCheck;								// A position marking where to check if the player is grounded.
						// A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;							// A position marking where to check for ceilings
	[SerializeField] private Collider2D m_CrouchDisableCollider;				// A collider that will be disabled when crouching

	const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	[SerializeField]private bool m_Grounded;            // Whether or not the player is grounded.
	const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;
	[SerializeField] float conveyorSpeed = 100;
	[SerializeField] float coyoteTime = 0.25f;
    [SerializeField] bool airbourne;
    [SerializeField] float airTime;
	[SerializeField] float additionalForce;
	[SerializeField] float holdJumpMultiplier = 1;
	[SerializeField] float upperbound = 0.75f;
	[SerializeField] float holdJumpAdditionalForce = 0.001f;
	[SerializeField] bool canRise = true;
	[SerializeField] float speedIncreaseOnSlope = 0.3f;

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent;
	private bool m_wasCrouching = false;


	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();
	}

	private void FixedUpdate()
	{
		String nothing = "";
		bool wasGrounded = m_Grounded;
		m_Grounded = false;
		airbourne = !checkGrounded(ref nothing);
        if (airbourne)
        {
            airTime += 0.1f;
        } else
        {
            airTime = 0;
        }
		m_Grounded = checkGrounded(ref nothing) || (airTime < coyoteTime && m_Rigidbody2D.linearVelocity.y <= 0);
		//Debug.Log(m_Grounded);
		if (checkGrounded(ref nothing))
		{
			if (!wasGrounded)
			{
				OnLandEvent.Invoke();
			}
		}
	}

	public bool checkGrounded(ref string standingOn)
	{
		feetScript fScript = m_GroundCheck.GetComponent<feetScript>();
		if (fScript.isGrounded) standingOn = fScript.collidedWith.name;
		feetScript checkSlopeScript = m_SlopeCheck.GetComponent<feetScript>();
		if (checkSlopeScript.isGrounded) standingOn = checkSlopeScript.collidedWith.name;
		return fScript.isGrounded || checkSlopeScript.isGrounded;
	}


	public void Move(float move, bool crouch, bool jump, bool canJump)
	{
		String currentStandingOn = "";
		// If crouching, check to see if the character can stand up
		if (!crouch)
		{
			// If the character has a ceiling preventing them from standing up, keep them crouching
			if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
			{
				crouch = true;
			}
		}

		//only control the player if grounded or airControl is turned on
		if (m_Grounded || m_AirControl)
		{

			// If crouching
			if (crouch)
			{
				if (!m_wasCrouching)
				{
					m_wasCrouching = true;
					OnCrouchEvent.Invoke(true);
				}

				// Reduce the speed by the crouchSpeed multiplier
				move *= m_CrouchSpeed;

				// Disable one of the colliders when crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = false;
			} else
			{
				// Enable the collider when not crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = true;

				if (m_wasCrouching)
				{
					m_wasCrouching = false;
					OnCrouchEvent.Invoke(false);
				}
			}

			/*double angle = 0;
			checkGrounded(ref standingOn, ref angle);
			float finalAngle = (float)(90 - angle);
			Debug.Log("Ang " + finalAngle + "\nAdded velocity " + Mathf.Sin(Mathf.Rad2Deg * finalAngle) * 8);*/
			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.linearVelocity.y);
			// Check if standing on conveyor belt, increase velocity of player if so
			checkGrounded(ref currentStandingOn);
			if (m_Grounded && (currentStandingOn.Length >= 8) && (currentStandingOn.Substring(0,8) == "Conveyor"))
			{
				targetVelocity += Vector3.right * conveyorSpeed;
				m_Rigidbody2D.linearVelocityX += conveyorSpeed;
				Debug.Log("Target " + targetVelocity);
			}
			m_Rigidbody2D.linearVelocityX += checkSlope() ? speedIncreaseOnSlope : 0;
			
			// if airbourne then air speed = speed on the ground
			//targetVelocity.y = m_Rigidbody2D.bodyType == RigidbodyType2D.Kinematic? 0 : targetVelocity.y;
			m_Rigidbody2D.linearVelocityY = m_Rigidbody2D.bodyType == RigidbodyType2D.Kinematic? 0 : m_Rigidbody2D.linearVelocityY;
			// And then smoothing it out and applying it to the character
			m_Rigidbody2D.linearVelocity = Vector3.SmoothDamp(m_Rigidbody2D.linearVelocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
			//Debug.Log("target " + targetVelocity + "\nvelocity " + m_Rigidbody2D.linearVelocity);
			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
		}
//		Debug.Log("Jumped " + jump + "\ngrounded " + m_Grounded + "\nmult " + holdJumpMultiplier + "\nupperbound " + upperbound + "\nresult " + ((!jump || m_Grounded) && (holdJumpMultiplier >= upperbound)));
		if (!jump || m_Grounded)
		{
			holdJumpMultiplier = 0;
			canRise = m_Grounded ? true : false;
		} else if (jump && canRise)
		{
			holdJumpMultiplier += holdJumpAdditionalForce;
			if (holdJumpMultiplier >= upperbound)
			{
				holdJumpMultiplier = 0;
				canRise = false;
			}
		}
		
		// If the player should jump...
		if (m_Grounded && jump && canJump/* && m_Rigidbody2D.linearVelocityY <= 0*/)
		{
			// Add a vertical force to the player.
			m_Grounded = false;
			jump = false;
			if (!checkGrounded(ref currentStandingOn)) m_JumpForce += additionalForce;
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
		} else if (!m_Grounded && m_Rigidbody2D.linearVelocityY > 0 && jump)
		{
			// if holding jump while rising midair then add more force to jump
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce * holdJumpMultiplier));
		}
	}


	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
	// check whether the player is on a slope
	bool checkSlope()
	{
		//Debug.Log("Ground " + m_GroundCheck.GetComponent<feetScript>().isGrounded + "\nSlope " + m_SlopeCheck.GetComponent<feetScript>().isGrounded);
		if (!m_GroundCheck.GetComponent<feetScript>().isGrounded && m_SlopeCheck.GetComponent<feetScript>().isGrounded)
		{
			//Debug.Log("On slope");
			return true;
		} 
		return false;
	}
}
