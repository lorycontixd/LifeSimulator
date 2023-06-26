using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;


[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Bot))]
public class NPC : MonoBehaviour
{
    public enum NPCBehaviour
    {
        WANDER,
        TALK,
        CROSSROAD
    }

    [Header("NPC traits")]
    public int ID;
    public Personality Personality;

    [Header("Settings")]
    public bool DebugMode = false;
    public bool IsActive = true;
    [SerializeField] private float crossRoadSpeedReduction = 3f;
    [SerializeField] private float fsmUpdateRateSeconds = 0.8f;
    [SerializeField] private float scanRateSeconds = 2f;
    [SerializeField] private float carScanRange = 6f;
    [SerializeField] private float arrivalDistance = 4f;

    [Header("Debug")]
    [SerializeField, Tooltip("Set the buildings that the npc should go to if you want a deterministic behaviour. Must be greater than 2")] private List<Building> buildings = new List<Building>();

    private NavMeshAgent agent;
    private Bot bot;
    private FSM fsm;
    private bool _playerWantsToTalk = false;
    private bool _carInRange = false;
    private Vector3 destination = Vector3.zero;
    private bool destinationReached = false;
    public NPCBehaviour behaviour; // Make private after finished debugging
    public Building targetBuilding = null;// Make private after finished debugging
    private int TargetBuildingDebugIndex = 0;
    private float _baseSpeed = -1f;


    [Header("FSM Settings")]
    [SerializeField] private float probabilityOfTalking;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => GameManager.Instance.GameSetup);
        agent = GetComponent<NavMeshAgent>();
        bot = GetComponent<Bot>();
        _baseSpeed = agent.speed;
        SetupFSM();

        StartCoroutine(ScanSurroundingsCo());
        StartCoroutine(Run());

    }
    private void Update()
    {
        if (targetBuilding != null)
        {
            if (Vector3.Distance(transform.position, targetBuilding.transform.position) < arrivalDistance)
            {
                StartCoroutine(ArriveAtBuilding());
            }
        }
    }
    public void SetupNPC(int id, Personality personality)
    {
        this.ID = id;
        this.Personality = personality;
    }
    public void SetupFSM()
    {
        FSMState wander = new FSMState();
        FSMState talk = new FSMState();
        FSMState crossRoad = new FSMState();

        wander.enterActions.Add(OnEnterWander);
        wander.exitActions.Add(OnExitWander);
        talk.enterActions.Add(OnEnterTalk);
        talk.exitActions.Add(OnExitTalk);
        crossRoad.enterActions.Add(OnEnterCrossroad);
        crossRoad.exitActions.Add(OnExitCrossroad);

        FSMTransition t1 = new FSMTransition(TalkToPlayer);
        FSMTransition t2 = new FSMTransition(StopTalkToPlayer); // Backward transition of t1
        FSMTransition t3 = new FSMTransition(IsCarInRange);
        FSMTransition t4 = new FSMTransition(IsNoCarInRange); // Backward transition of t3

        wander.AddTransition(t1, talk);
        wander.AddTransition(t3, crossRoad);
        talk.AddTransition(t2, wander);
        crossRoad.AddTransition(t4, wander);

        fsm = new FSM(wander);

    }
    private void SetRandomDestination()
    {
        Building building = GameManager.Instance.GetRandomBuilding();
        targetBuilding = building;
        agent.SetDestination(building.transform.position);
    }
    private void SetDebugDestination()
    {
        Building building = buildings[TargetBuildingDebugIndex];
        targetBuilding = building;
        agent.SetDestination(building.transform.position);
    }
    private void SetDestination(Vector3 destination)
    {
        agent.SetDestination(destination);
    }
    

    #region Conditions
    private bool TalkToPlayer()
    {
        return _playerWantsToTalk;
    }
    private bool StopTalkToPlayer()
    {
        return !_playerWantsToTalk;
    }
    private bool IsCarInRange()
    {
        return _carInRange;
    }
    private bool IsNoCarInRange()
    {
        return !_carInRange;
    }
    public void Talk()
    {
        _playerWantsToTalk = true;
    }
    public void EndTalk()
    {
        _playerWantsToTalk = false;
    }
    #endregion

    #region Actions
    public void OnEnterCrossroad()
    {
        //Debug.Log($"]NPC{ID}] Entered crossroad state!");
        agent.speed = agent.speed / crossRoadSpeedReduction;
    }
    public void OnExitCrossroad()
    {
        behaviour = NPCBehaviour.CROSSROAD;
        agent.speed = _baseSpeed;
    }
    public void OnEnterTalk()
    {
        //Debug.Log($"]NPC{ID}] Entered talk state!");
        behaviour = NPCBehaviour.TALK;
        Player player = GameObject.FindFirstObjectByType<Player>();
        if (player != null)
        {
            SetDestination(player.transform.position);
            transform.LookAt(player.transform);
        }
    }
    public void OnExitTalk()
    {

    }
    public void OnEnterWander()
    {
        //Debug.Log($"]NPC{ID}] Entered wander state!");
        behaviour = NPCBehaviour.WANDER;
        if (targetBuilding == null)
        {
            if (IsActive)
            {
                if (DebugMode)
                {
                    SetDebugDestination();
                }
                else
                {
                    SetRandomDestination();
                }
            }
        }
        else
        {
            SetDestination(targetBuilding.transform.position);
        }
    }
    public void OnExitWander()
    {

    }
    #endregion

    private bool CheckCarAround()
    {
        Car car = null;
        Collider[] colliders = Physics.OverlapSphere(transform.position, carScanRange);
        if (colliders.Length > 0)
        {
            Collider carCollider = colliders.FirstOrDefault(c => c.gameObject.GetComponent<Car>() != null);
            if (carCollider != null)
            {
                car = carCollider.gameObject.GetComponentInParent<Car>();
            }
        }
        _carInRange = car != null;
        return _carInRange;
    }
    public void StartTalk(Player player)
    {
        _playerWantsToTalk = true;
        transform.LookAt(player.transform);
    }
    public void StopTalk(Player player)
    {
        _playerWantsToTalk = false;
    }



    private IEnumerator ScanSurroundingsCo()
    {
        while (true)
        {
            CheckCarAround();
            yield return new WaitForSeconds(scanRateSeconds);
        }
    }
    private IEnumerator Run()
    {
        while (true)
        {
            fsm.Update();
            yield return new WaitForSeconds(fsmUpdateRateSeconds);
        }
    }
    private IEnumerator ArriveAtBuilding(float duration = 4f, float randomRange = 0f)
    {
        IsActive = false;
        targetBuilding = null;
        if (randomRange < 0f || randomRange > duration)
        {
            Debug.LogWarning($"[NPC] Arrived at building -> random range for arrival is invalid. Setting random range to zero.");
            randomRange = 0f;
        } 
        float noise = 0f;
        if (randomRange > 0)
        {
            noise = Random.Range(-randomRange, randomRange);
        }
        yield return new WaitForSeconds(duration + noise);
        if (DebugMode)
        {
            TargetBuildingDebugIndex = (TargetBuildingDebugIndex + 1) % buildings.Count;
        }
        IsActive = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = _carInRange ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, carScanRange);
    }
}
