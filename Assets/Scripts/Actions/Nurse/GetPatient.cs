using GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GetPatient : GAction
{
    private GameObject resource;

    public override bool PostPerform()
    {
        GWorld.Instance.GetWorld().ModifyState("PatientWaiting", -1);
        if (target)
        {
            target.GetComponent<GAgent>().inventory.AddItem(resource); // Adding the cubicle to the patient's inventory
            target.GetComponent<GAgent>().AddGoal("GetTreated", 1, true);
        }
        return true;
    }

    public override bool PrePerform()
    {
        GAgent patient = HospitalManager.Instance.RemovePatient();
        if (patient == null)
        {
            return false;
        }
        target = patient.gameObject;

        resource = HospitalManager.Instance.RemoveCubicle().gameObject;
        if (resource == null)
        {
            HospitalManager.Instance.AddPatient(target.GetComponent<GAgent>());
            target = null;
            return false;
        }
        else
        {
            this.GetComponent<GAgent>().inventory.AddItem(resource); // Add's cubicle to nurse's inventory
        }
        patient.beliefs.AddState("NursePickedUp", true);
        GWorld.Instance.GetWorld().ModifyState("FreeCubicle", true);
        GetComponentInParent<NavMeshAgent>().speed = patient.GetNavmeshSpeed();
        return true;
    }

    public override bool IsAchievable()
    {
        return HospitalManager.Instance.patientsCount > 0;
    }
}
