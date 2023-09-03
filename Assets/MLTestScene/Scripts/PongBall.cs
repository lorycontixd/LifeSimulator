using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D))]
public class PongBall : MonoBehaviour
{
    private Rigidbody2D rb2d;
    [SerializeField] private float playerBounceForce = 5f;
    [SerializeField] private string pongPlayerTag = "PongPlayer";
    [SerializeField] private string pongWallTag = "BoundaryCollider";
    public Action<PongGoal> onGoalHit;

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        if (pongPlayerTag == null || pongPlayerTag == string.Empty)
        {
            Debug.LogWarning($"[PongBall] Player tag is null");
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == pongPlayerTag)
        {
            Vector3 dir = this.transform.position - collision.transform.position;
            float y = dir.y;
            Vector2 force = new Vector3(Random.Range(-5f, 5f), y) * playerBounceForce;
            rb2d.AddForce(force, ForceMode2D.Impulse);
            Debug.Log($"Collided with player => Force: {force}");
        }
        if (collision.collider.tag == pongWallTag)
        {
            rb2d.AddForceY(Random.Range(0f, 1f) <= 0.5 ? 2f : -2f);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        PongGoal goal = collision.gameObject.GetComponent<PongGoal>();
        if (goal != null)
        {
            onGoalHit?.Invoke(goal);
        }
    }
}
