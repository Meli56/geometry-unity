using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float Speed = 1.0f;
    public float JumpForce = 1.0f;
    public float JumpForceReverse = 0.5f;
    public float RotationSpeed = 1.0f;
    public Transform GroundCheck;
    public Transform TopCheck;
    public Transform CubeVisual;
    public float TimeBetweenParticles = 0.1f;
    public ParticleSystem DeadParticles;
    private int Obstacle;
    public Rigidbody2D Rigidbody2D;
    private bool Jump;
    private bool JumpReverse;
    private bool Top;
    public bool Dead;
    private float LastTimeParticle;
    private bool WasGround;

    private void Awake()
    {
        Obstacle = LayerMask.NameToLayer("Obstacle");
        Top = false;
    }

    private void Update()
    {
        if (Dead) return;

        bool ground = Physics2D.Raycast(GroundCheck.transform.position, Vector2.down, 0.05f);
        bool top = Physics2D.Raycast(TopCheck.transform.position, Vector2.up, 0.05f);

        if (ground || Top)
        {
            Quaternion rot = CubeVisual.rotation;
            rot.z = 0.0f;
            CubeVisual.rotation = rot;
        }
        else
        {
            CubeVisual.Rotate(Vector3.back * RotationSpeed * Time.deltaTime);
        }

        Jump = ground && (Jump || IsInput());
        JumpReverse = top && (JumpReverse || IsInput());
        WasGround = ground;
    }

    private void FixedUpdate()
    {
        if (Dead)
        {
            Rigidbody2D.velocity = Vector2.zero;
            return;
        }

        Vector2 velocity = Rigidbody2D.velocity;
        velocity.x = Speed * Time.fixedDeltaTime;
        Rigidbody2D.velocity = velocity;

        if (Jump)
        {
            Rigidbody2D.AddForce(Vector3.up * JumpForce * Rigidbody2D.mass, ForceMode2D.Impulse);
            Jump = false;
        }
        else if(JumpReverse){
            Rigidbody2D.AddForce(Vector3.down * JumpForceReverse * Rigidbody2D.mass, ForceMode2D.Impulse);
            JumpReverse = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == Obstacle)
        {
            Dead = true;
            GetComponentInChildren<SpriteRenderer>().enabled = false;
            DeadParticles.Play();
            GameManager.Instance.NotifyDead();
        }
        if (collision.CompareTag("portal"))
        {
            Debug.Log("reverse");
            Rigidbody2D.gravityScale *= -1;
            Top = true;
        }
        if (collision.CompareTag("portalback"))
        {
            Rigidbody2D.gravityScale *= -1;
            Top = false;
        }
    }

    private bool IsInput()
    {
        return Input.GetKey(KeyCode.Space) ||
               Input.GetKey(KeyCode.W) ||
               Input.GetKey(KeyCode.UpArrow) ||
               Input.touchCount > 0;
    }
}
