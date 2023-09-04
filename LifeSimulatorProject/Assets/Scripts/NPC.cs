using GOAP;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
public class NPC : GAgent
{
    private bool IsVisible;
    public Car myCar { get; private set; }

    protected override void Start()
    {
        base.Start();
        myCar = FindObjectsByType<Car>(FindObjectsSortMode.None).Where(car => car.playerID == id).FirstOrDefault();

        Goal s1 = new Goal("GotCar", 1, true);
        goals.Add(s1, 4);

        Goal s2 = new Goal("GetTreated", 1, true);
        goals.Add(s2, 5);
        //StartCoroutine(A());

    }

    private IEnumerator A()
    {
        yield return new WaitForSeconds(1f);
        
    }

    public void ToggleVisibility()
    {
        IsVisible = !IsVisible;
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in renderers)
        {
            renderer.enabled = IsVisible;
        }
    }
}
*/