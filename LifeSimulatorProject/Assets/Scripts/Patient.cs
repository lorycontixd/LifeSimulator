using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOAP;

public class Patient : GAgent
{
    protected override void Start()
    {
        base.Start();

        StartCoroutine(A());

    }

    private IEnumerator A()
    {
        yield return new WaitForSeconds(4f);
        Goal s1 = new Goal("WaitForNurse", 1, true);
        goals.Add(s1, 3);

        Goal s2 = new Goal("GetTreated", 1, true);
        goals.Add(s2, 3);
    }
}
    