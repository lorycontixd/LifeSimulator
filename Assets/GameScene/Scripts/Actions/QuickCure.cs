using Lore.Game.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickCure : GAction
{
    public override bool PostPerform()
    {
        if (SicknessManager.Instance == null)
        {
            return false;
        }
        Debug.Log($"[QuickCure] Curing all diseases");
        SicknessManager.Instance.CureAll();
        this.beliefs.RemoveState("NeedsCures");
        StartCoroutine(LeaveHospitalCo());
        return true;
    }

    public override bool PrePerform()
    {
        if (target == null)
        {
            Cubicle[] cubicles = GameObject.FindObjectsByType<Cubicle>(FindObjectsSortMode.None);
            if (cubicles.Length > 0)
            {
                target = cubicles[Random.Range(0, cubicles.Length-1)].gameObject;
                return true;
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    private IEnumerator LeaveHospitalCo(float delay = 3f)
    {
        yield return new WaitForSeconds(delay);
        if (this.beliefs.HasState("IsAtHospital"))
        {
            this.beliefs.RemoveState("IsAtHospital");
        }
    }
}
