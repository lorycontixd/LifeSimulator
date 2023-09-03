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
        Debug.Log($"[Nurse->GetPatient] starting post perform");
        GWorld.Instance.GetWorld().ModifyState("PatientWaiting", -1);
        Debug.Log($"[Nurse->GetPatient] setting patient waiting");
        if (target != null)
        {
            GAgent agent = target.GetComponent<GAgent>();
            Debug.Log($"[Nurse->GetPatient] forcing agent stop");
            agent.ForceActionComplete();
            Debug.Log($"[Nurse->GetPatient] adding cubicle to inventory");
            agent.inventory.AddItem(resource); // Adding the cubicle to the patient's inventory

            Debug.Log($"[Nurse->GetPatient] setting gettreated goal");
            agent.AddGoal("GetTreated", 5, true);
        }
        return true;
    }

    public override bool PrePerform()
    {
        GAgent patient = HospitalManager.Instance.RemovePatient();
        Debug.Log($"Nurse => Getting patient");
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
        Debug.Log($"[Nurse->GetPatient] setting nurse speed");
        GetComponentInParent<NavMeshAgent>().speed = patient.GetNavmeshSpeed();
        Debug.Log($"[Nurse->GetPatient] ending pre perform");
        return true;
    }

    public override bool IsAchievable()
    {
        return HospitalManager.Instance.patientsCount > 0 && base.IsAchievable();
    }
}