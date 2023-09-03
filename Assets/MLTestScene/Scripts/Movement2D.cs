using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement2D : MonoBehaviour
{
    public float speed = 100f;
    public float JumpHeight;
    public bool InAir = false;
    public float xLimOffset = 0.5f;
    public Vector2 xLims;

    private Rigidbody2D rb2d;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        if (xLims[0] > xLims[1])
        {
            float temp = xLims[0];
            xLims[0] = xLims[1];
            xLims[1] = temp;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        InAir = false;
        Debug.Log("InAir false");
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        InAir = true;
        Debug.Log("InAir True");
    }

    void FixedUpdate()
    {
        Vector2 NoMovement = new Vector2(0f, 0f);

        float moveHorizontal = Input.GetAxis("Horizontal");
        if (moveHorizontal > 0)
        {
            transform.localPosition = new Vector3(Mathf.Clamp(transform.localPosition.x + speed * Time.deltaTime, xLims[0], xLims[1]), transform.localPosition.y, transform.localPosition.z);
        }
        if (moveHorizontal < 0)
        {
            transform.localPosition = new Vector3(Mathf.Clamp(transform.localPosition.x - speed * Time.deltaTime, xLims[0], xLims[1]), transform.localPosition.y, transform.localPosition.z);
        }
        if (Input.GetKeyDown(KeyCode.W) || (Input.GetKeyDown(KeyCode.UpArrow)))
        {
            if (InAir == false)
            {
                rb2d.AddForce(new Vector2(0, JumpHeight), ForceMode2D.Impulse);
            }
        }
    }

    private void CheckBoundaries()
    {
    }
}
