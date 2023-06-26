using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOAP;
using UnityEngine.AI;

public class GoToHospitalAction : GAction
{
    [SerializeField] private bool ReduceSpeed = true;

    public override bool PostPerform()
    {
        if (ReduceSpeed)
            this.GetComponentInParent<GAgent>().SetNavmeshSpeed(GetComponentInParent<NavMeshAgent>().speed / 2f);
        return true;
    }

    public override bool PrePerform()
    {
        return true;
    }
}
