using GOAP;
using Lore.Game.Characters;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Interact : GAction
{
    [SerializeField, Range(0f, 1f)] private float InteractProbabilityWhenSad = 0.65f;

    GAgent gagent = null;
    private Citizen targetCitizen = null;
    private Friendship targetFriendship = null;
    private float defaultSpeed;
    private bool NPCreached = false;
    private Collider[] scannedObjs;

    private void Update()
    {
        if (!NPCreached && targetFriendship != null)
        {
            target = targetFriendship.gameObject;
            agent.SetDestination(target.transform.position);
            ScanTargetNPC();
        }
    }

    public override bool PostPerform()
    {
        gagent.OverrideNavmesh(false);
        targetCitizen.Interact(GetComponentInParent<GamePlayer>(), duration);
        targetFriendship = null;
        return true;
    }

    public override bool OnArrival()
    {
        return true;
    }

    public override bool PrePerform()
    {
        gagent = GetComponentInParent<GAgent>();
        gagent.OverrideNavmesh(true);
        agent = GetComponentInParent<NavMeshAgent>();
        Friendship[] friendships = FindObjectsByType<Friendship>(FindObjectsSortMode.None);
        targetFriendship = friendships.First(x => x.CurrentFriendship == friendships.Min(x => x.CurrentFriendship));
        targetCitizen = targetFriendship.gameObject.GetComponent<Citizen>();
        return true;
    }

    private void ScanTargetNPC()
    {
        float dist = Vector3.Distance(target.transform.position, transform.position);
        NPCreached = (dist < gagent.actionCompletionDistance);
    }

    public override bool IsAchievable()
    {
        GamePlayer player = GetComponentInParent<GamePlayer>();
        if (player.IsStealing)
        {
            return false;
        }
        if (player.beliefs.HasState("IsHungry"))
        {
            return false;
        }
        if (player.beliefs.HasState("IsSad"))
        {
            float r = UnityEngine.Random.Range(0f, 1f);
            Debug.Log($"[Interact->IsAch] r: {r}/ prob: {InteractProbabilityWhenSad}");
            if (r > InteractProbabilityWhenSad)
            {
                return false;
            }
        }
        if (player.beliefs.HasState("DogFollowing"))
        {
            return false;
        }
        if (GWorld.Instance.GetWorld().HasState("DogWantsWalk"))
        {
            return false;
        }
        return base.IsAchievable();
    }

    public override bool IsAchievableGiven(Dictionary<string, object> conditions)
    {
        if (conditions.ContainsKey("DogWantsWalk"))
        {
            return false;
        }
        return base.IsAchievableGiven(conditions);
    }
}
