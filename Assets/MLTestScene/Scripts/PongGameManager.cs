using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PongGameManager : MonoBehaviour
{
    [SerializeField] private PongBall ball;
    [SerializeField] private float initialForce = 5f;
    public int maxScore = 5;

    private void Start()
    {
        /*
        float x = Random.Range(-1f, 1f);
        float y = Random.Range(-1f, 1f);
        Vector2 force = new Vector2(x, y).normalized * initialForce;
        ball.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
        */
    }
}
