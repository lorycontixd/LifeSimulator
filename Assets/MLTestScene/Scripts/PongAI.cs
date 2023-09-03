using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Linq;
using System;
using Random = UnityEngine.Random;

public class PongAI : Agent
{
    public float speed;
    private PongBall ball;



    private void Start()
    {
        ball = FindFirstObjectByType<PongBall>();
        ball.onGoalHit += OnGoalHit;
    }

    public override void OnEpisodeBegin()
    {
        //ball.transform.localPosition = new Vector3(0f, Random.Range(-3.5f, 3.5f), 0f);
        ball.transform.localPosition = Vector3.zero;

        float sx = Random.Range(0, 2) == 0 ? -1 : 1;
        float sy = Random.Range(0, 2) == 0 ? -1 : 1;
        float ballSpeed = 25f;
        ball.GetComponent<Rigidbody2D>().AddForce(new Vector2(ballSpeed * sx, ballSpeed * sy * 6f));
    }

    private void OnGoalHit(PongGoal goal)
    {
        SetReward(goal.IsPlayers ? 2f : -2f);
        EndEpisode();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition.x);
        sensor.AddObservation(ball.transform.localPosition.x);
        sensor.AddObservation(ball.transform.localPosition.y);
        sensor.AddObservation(ball.GetComponent<Rigidbody2D>().velocity.x);
        sensor.AddObservation(ball.GetComponent<Rigidbody2D>().velocity.y);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions.First();
        transform.localPosition += new Vector3(moveX, 0f, 0f) * Time.deltaTime * speed;
        transform.localPosition = new Vector3(Mathf.Clamp(transform.localPosition.x, -7f, 7f), transform.localPosition.y, transform.localPosition.z);
        

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.Equals(this.ball))
        {
            SetReward(1f);
            EndEpisode();
        }
    }

    
}
