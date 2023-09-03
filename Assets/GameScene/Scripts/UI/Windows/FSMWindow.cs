using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMWindow : MonoBehaviour
{
    [SerializeField] private Transform cloneHolder = null;
    private List<GameObject> stateClones = new List<GameObject>();

    [Header("Prefabs")]
    [SerializeField] private GameObject statePrefab;
    [SerializeField] private GameObject transitionPrefab;

    private FSM fsm;
    private bool IsActive = false;

    private void Start()
    {
        
    }

    public void SetFSM(FSM fsm)
    {
        this.fsm = fsm;
    }
    public void SetActive(bool IsActive)
    {
        this.IsActive = IsActive;
    }

}
