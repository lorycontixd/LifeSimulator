using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
public sealed class GWorldOld {

    private static readonly GWorldOld instance = new GWorldOld();
    private static WorldStates world;
    private static Queue<GameObject> patients;
    private static Queue<GameObject> cubicles;




    static GWorldOld() {

        world = new WorldStates();
        patients = new Queue<GameObject>();
        cubicles = new Queue<GameObject>();

        GameObject[] cubes = GameObject.FindGameObjectsWithTag("Cubicle");

        foreach (GameObject c in cubes) cubicles.Enqueue(c);

        if (cubes.Length > 0) world.ModifyState("FreeCubicle", cubes.Length);

        Time.timeScale = 2.0f;
    }

    GWorldOld() {

    }

    public void AddPatient(GameObject p) {

        patients.Enqueue(p);
    }

    public GameObject RemovePatient() {

        if (patients.Count == 0) return null;
        return patients.Dequeue();
    }

    public void AddCubicle(GameObject p) {

        cubicles.Enqueue(p);
    }

    public GameObject RemoveCubicle() {

        if (cubicles.Count == 0) return null;
        return cubicles.Dequeue();
    }

    public static GWorldOld Instance { get { return instance; } }

    public WorldStates GetWorld() => world;
}*/