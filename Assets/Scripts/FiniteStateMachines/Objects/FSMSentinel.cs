using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMSentinel : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string friendsTag = "Player";
    [SerializeField, Range(0f, 20f)] private float scanRange;
    [SerializeField, Range(0.1f, 5f)] private float reactionTime = 1f;
    [SerializeField] private float lightMaxIntensity = 2f;
    [SerializeField, Range(1f, 2f)] private float lightDimmingDuration = 1.5f;

    [Header("Objects")]
    [SerializeField] private Light sentinelLight;

    private FSM fsm;
    private float dimmingStart;

    private void Start()
    {
        if (!sentinelLight) return;

        // Off state
        FSMState offState = new FSMState();
        offState.enterActions.Add(TurnOffLight);
        // On state
        FSMState onState = new FSMState();
        onState.enterActions.Add (TurnOnLight);
        // Dimming State
        FSMState dimmingState = new FSMState();
        dimmingState.enterActions.Add(StartLightDimming);
        dimmingState.stayActions.Add(Dimmer);

        // Transitions
        FSMTransition t1 = new FSMTransition(FriendsInRange); // Forward transition
        FSMTransition t2 = new FSMTransition(NoFriendsInRange); // Backwards transition
        FSMTransition t3 = new FSMTransition(FriendsInRange); // Different transition from t1 -> goes from dimming to turnon
        FSMTransition t4 = new FSMTransition(LightIsOff); // From dimming to is off, condition is automatic
        // Link transitions with states
        offState.AddTransition(t1, onState);
        onState.AddTransition(t2, dimmingState);
        dimmingState.AddTransition(t3, onState);
        dimmingState.AddTransition(t4, offState);

        fsm = new FSM(offState);

        StartCoroutine(Patrol());

    }



    #region Conditions
    public bool FriendsInRange()
    {
        GameObject[] friends = GameObject.FindGameObjectsWithTag(friendsTag);
        foreach(GameObject go in friends)
        {
            if ((go.transform.position - transform.position).magnitude <= scanRange)
            {
                return true;
            }
        }
        return false;
    }
    public bool NoFriendsInRange()
    {
        return !FriendsInRange();
    }
    public bool LightIsOff()
    {
        return sentinelLight.intensity == 0f;
    }
    #endregion

    #region Actions
    public void TurnOnLight()
    {
        sentinelLight.intensity = lightMaxIntensity;
        Debug.Log($"Hello there!!");
    }
    public void TurnOffLight()
    {
        sentinelLight.intensity = 0f;
    }
    public void StartLightDimming()
    {
        dimmingStart = Time.realtimeSinceStartup;
    }
    public void Dimmer() {
        float newVal = lightMaxIntensity - Mathf.Clamp((Time.realtimeSinceStartup - dimmingStart) / lightDimmingDuration, 0f, lightMaxIntensity);
        Debug.Log($"Dimming light ==> New val: {newVal}", this);
        sentinelLight.intensity = newVal;
        
    }

    private IEnumerator Patrol()
    {
        while (true)
        {
            fsm.Update();
            yield return new WaitForSeconds(reactionTime);
        }
    }
    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, scanRange);
    }
}
