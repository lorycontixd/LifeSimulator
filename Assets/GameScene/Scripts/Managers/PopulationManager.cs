using Lore.Game.Characters;
using Lore.Game.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lore.Game.Managers
{

}
public class PopulationManager : BaseManager
{
    #region Singleton
    private static PopulationManager _instance;
    public static PopulationManager Instance { get { return _instance; } }

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

    private List<Character> spawnedCharacters = new List<Character>();
    private List<Citizen> spawnedCitizens = new List<Citizen>();

    public override void Start()
    {
        base.Start();
    }

    public void AddCitizen(Citizen citizen)
    {
        spawnedCharacters.Add(citizen);
        spawnedCitizens.Add(citizen);
    }
}
