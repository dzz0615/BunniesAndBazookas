﻿using System;
using UnityEngine;
using System.Collections;

public class PunchController : MonoBehaviour
{
	public enum PunchState
	{
		Ready,
		Charging,
		Punching,
		Retracting
	}

	public float punchSpeed;
    public float retractSpeed;
    public float pushTerrainSpeed;
    public float pushPlayerSpeed;
    public Vector3 spriteOffset;
    public float maxPunchLength;
    public float minRetractDistance;
	public PunchState punchState;

    private Transform playerTransform;
    private Vector2 impactForce;
	private Vector2 velocity;
    private GameObject player;
	private float chargeLevel;
    
	// Use this for initialization
	void Start ()
	{
        impactForce = Vector2.zero;
        player = transform.parent.gameObject;
	    playerTransform = player.transform;

		Reset();
	}
	
	// Update is called once per frame
	void Update () {
		if (punchState == PunchState.Punching)
		{
			if (Vector3.Distance(playerTransform.position, transform.position) > maxPunchLength)
			{
				punchState = PunchState.Retracting;
			}
		}
		else if (punchState == PunchState.Retracting)
		{
			if (Vector3.Distance(playerTransform.position, transform.position) < minRetractDistance)
			{
				Reset();
			}
			
			Vector2 retractAngle = (playerTransform.position - transform.position).normalized;
			velocity = retractAngle * retractSpeed;
			if (impactForce.magnitude > .1f)
			{
				velocity += impactForce * punchSpeed;
				impactForce *= .95f;
			}
		}

        Vector3 movement = velocity * Time.deltaTime;
        transform.position += movement;
	}

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.Equals(player) && punchState == PunchState.Retracting)
        {
			Reset();
        }
        else if (collider.gameObject.tag == "Terrain" && punchState == PunchState.Punching)
        {
            punchState = PunchState.Retracting;
            Vector2 pushAngle = player.transform.position - transform.position;
            player.rigidbody2D.velocity += pushAngle.normalized * pushTerrainSpeed * chargeLevel;
            chargeLevel = 1;
        }
        else if (collider.gameObject.tag == "Player" && punchState == PunchState.Punching)
        {
			punchState = PunchState.Retracting;
            collider.rigidbody2D.velocity += velocity.normalized * pushPlayerSpeed * chargeLevel;
			player.rigidbody2D.velocity += -velocity.normalized * pushPlayerSpeed/2;
        }
        else if (collider.gameObject.tag == "Puncher" && punchState == PunchState.Punching)
        {
            punchState = PunchState.Retracting;
            impactForce = collider.gameObject.GetComponent<PunchController>().velocity.normalized;
        }
		else {
			Reset();
		}
    }

    public void Fire(Vector2 aimAngle, float charge)
    {
        if (aimAngle.magnitude > 0)
        {
            punchState = PunchState.Punching;
            velocity = aimAngle * punchSpeed;
			chargeLevel = charge;
        }
    }

	private void Reset() {
		punchState = PunchState.Ready;
		transform.position = playerTransform.position + spriteOffset;
		velocity = Vector2.zero;
        GetComponentInChildren<String>().Reset();
	}

}
