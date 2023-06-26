using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOAP;
using UnityEngine.Rendering;
using System.Linq;
using Lore.Game.Characters;

public class GoToWaitingRoom : GAction
{
    private Chair chair = null;

    public override bool PostPerform()
    {
        GWorld.Instance.GetWorld().ModifyState("PatientWaiting", 1);
        GAgent characterComponent = GetComponentInParent<GAgent>();
        if (characterComponent == null)
        {
            return false;
        }
        HospitalManager.Instance.AddPatient(characterComponent);
        beliefs.ModifyState("AtHospital", 1);
        return true;
    }

    public override bool PrePerform()
    {
        Chair[] chairs = FindObjectsByType<Chair>(FindObjectsSortMode.None);
        Chair[] freeChairs = chairs.Where(c => !c.isTaken).ToArray();
        if (freeChairs.Length <= 0)
        {
            return false;
        }
        this.chair = freeChairs[Random.Range(0, freeChairs.Length)];
        this.target = this.chair.gameObject;
        this.chair.isTaken = true;
        return true;
    }
}
