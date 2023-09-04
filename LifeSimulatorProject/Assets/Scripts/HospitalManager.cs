using GOAP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class HospitalManager : MonoBehaviour
{
    #region Singleton
    private static HospitalManager _instance;
    public static HospitalManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion

    private Queue<GAgent> patientQueue;
    private Queue<Cubicle> cubicles;

    public bool IsSetup { get; private set; } = false;
    public int patientsCount { get { return patientQueue.Count; } }
    public int cubicleCount { get { return cubicles.Count; } }

    private void Start()
    {
        cubicles = new Queue<Cubicle>();
        patientQueue = new Queue<GAgent>();
        IsSetup = true;

        SetupCubicles();
    }

    private void SetupCubicles()
    {
        List<Cubicle> cubicles = GameObject.FindObjectsOfType<Cubicle>().ToList();
        List<int> ids = cubicles.Select(c => c.id).ToList();
        if (cubicles.Count > 0)
        {
            foreach (Cubicle c in cubicles)
            {
                if (c != null)
                {
                    AddCubicle(c);
                }
            }
            GWorld.Instance.GetWorld().ModifyState("FreeCubicle", cubicles.Count );
        }
    }

    public void AddPatient(GAgent p)
    {
        patientQueue.Enqueue(p);
    }

    public GAgent RemovePatient()
    {
        if (patientQueue.Count > 0)
        {
            return patientQueue.Dequeue();
        }
        return null;
    }

    public void AddCubicle(Cubicle cubicle)
    {
        cubicles.Enqueue(cubicle);
    }
    public Cubicle RemoveCubicle()
    {
        return cubicles.Dequeue();
    }
}
