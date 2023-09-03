using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOAP;
using UnityEngine.AI;
using System.Linq;

public class GoToHospitalAction : GAction
{
    [SerializeField] private bool ReduceSpeed = true;


    public override bool PostPerform()
    {
        if (ReduceSpeed)
            this.GetComponentInParent<GAgent>().SetNavmeshSpeed(GetComponentInParent<NavMeshAgent>().speed / 2f);
        beliefs.ModifyState("LastAction", actionName);
        beliefs.AddState("IsAtHospital", true);
        return true;
    }

    public override bool PrePerform()
    {
        Debug.Log($"==> {preConditions.First().key} -- {this.beliefs.HasState(preconditions.First().Key)}");
        return true;
    }

    public override bool IsAchievableGiven(Dictionary<string, object> conditions)
    {
        bool match = base.IsAchievableGiven(conditions);
        return !conditions.ContainsKey("IsAtHospital") && conditions.ContainsKey("NeedsCures") && match;
    }
}
