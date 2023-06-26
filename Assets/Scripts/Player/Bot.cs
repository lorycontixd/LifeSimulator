using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bot : MonoBehaviour {

    public enum BotAction
    {
        NONE,
        SEEK,
        FLEE,
        PURSUE,
        EVADE,
        WANDER
    }

    public GameObject target;
    public GameObject sphere;
    [SerializeField] private BotAction startingAction = BotAction.NONE;  
    [SerializeField] private bool debugMode;

    GameObject jitter;

    NavMeshAgent agent;
    Drive ds;
    Vector3 wanderTarget = Vector3.zero;
    private BotAction currentAction;
    float q = 0.0f;


    void Start() {
        agent = GetComponent<NavMeshAgent>();
        if (target != null)
        {
            ds = target.GetComponent<Drive>();
        }
        if (sphere != null && debugMode)
            jitter = Instantiate(sphere);
        currentAction = startingAction;
    }

    public void SetBotAction(BotAction action, GameObject target = null)
    {
        currentAction = action;
        this.target = target;
    }


    void Seek(Vector3 location) {

        Debug.Log($"seeking");
        agent.SetDestination(location);
    }

    void Flee(Vector3 location) {

        Vector3 fleeVector = location - transform.position;
        agent.SetDestination(transform.position - fleeVector);
    }
        
    void Pursue() {
        if (ds == null)
        {
            return;
        }
        Vector3 targetDir = target.transform.position - transform.position;
        float relativeHeading = Vector3.Angle(transform.forward, transform.TransformVector(target.transform.forward));
        float toTarget = Vector3.Angle(transform.forward, transform.TransformVector(targetDir));


        if ((toTarget > 90.0f && relativeHeading < 20.0f) || ds.currentSpeed < 0.01f) {

            // Debug.Log("SEEKING");
            Seek(target.transform.position);
            return;
        }

        // Debug.Log("LOOKING AHEAD");
        float lookAhead = targetDir.magnitude / (agent.speed + ds.currentSpeed);
        Seek(target.transform.position + target.transform.forward * lookAhead);
    }

    void Evade() {

        Vector3 targetDir = target.transform.position - transform.position;
        float lookAhead = targetDir.magnitude / (agent.speed + ds.currentSpeed);
        Flee(target.transform.position + target.transform.forward * lookAhead);
    }

    void Wander() {

        float wanderRadius = 10.0f;
        float wanderDistance = 20.0f;
        float wanderJitter = 1.0f;

        wanderTarget += new Vector3(
            Random.Range(-1.0f, 1.0f) * wanderJitter,
            0.0f,
            Random.Range(-1.0f, 1.0f));
        wanderTarget.Normalize();
        wanderTarget *= wanderRadius;

        Vector3 targetLocal = wanderTarget + new Vector3(0.0f, 0.0f, wanderDistance);
        Vector3 targetWorld = gameObject.transform.InverseTransformVector(targetLocal);

        if (debugMode)
        {
            Debug.DrawLine(transform.position, targetWorld, Color.red);
            jitter.transform.position = targetWorld;
        }
        Seek(targetWorld);
    }

    private void ChooseAction()
    {
        switch (currentAction)
        {
            case BotAction.NONE:
                break;
            case BotAction.SEEK:
                Seek(target.transform.position);
                break;
            case BotAction.FLEE:
                Flee(target.transform.position);
                break;
            case BotAction.WANDER:
                Wander();
                break;
            case BotAction.PURSUE:
                Pursue();
                break;
            case BotAction.EVADE:
                Evade();
                break;
            default:
                Debug.LogWarning($"Invalid current bot action was selected: {currentAction}. Setting current action to none.");
                currentAction = BotAction.NONE;
                break;

        }
    }

    void Update() {
        // Seek(target.transform.position);
        // Flee(target.transform.position);
        // Pursue();
        // Evade();
        //Wander();
        ChooseAction();
    }

    void FixedUpdate() {
        // always draw a 5-unit colored line from the origin
        Color color = new Color(q, q, 1.0f);
        Debug.DrawLine(Vector3.zero, new Vector3(0, 5, 0), color);
        q = q + 0.01f;

        if (q > 1.0f) {
            q = 0.0f;
        }
    }
}
