using GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Talk : GAction
{
    private Player player;

    public override bool PostPerform()
    {
        this.player.StopTalk();
        this.player = null;
        return true;
    }

    public override bool PrePerform()
    {
        float duration = TalkManager.Instance.CalculateTalkingDuration(player.Personality, player.nearestNPC.Personality);
        this.duration = duration;
        this.target = player.gameObject;
        this.player.StartTalk();
        Debug.Log($"[Talk] Started conversation with {player.talkingToNPC.ID}");
        return true;
    }

    public override bool IsAchievable()
    {
        Player player = gameObject.GetComponentInParent<Player>();
        if (player == null)
        {
            Debug.LogWarning("[Talk] False: player is null");
            return false;
        }
        this.player = player;
        (bool, string) playerCanTalk = player.CanTalk();
        Debug.Log($"[Talk] Player can talk res: {playerCanTalk.Item2}");
        if (playerCanTalk.Item1)
        {
            float friendLevel = player.friendships.ContainsKey(player.nearestNPC) ? player.friendships[player.nearestNPC] : 0f;
            float talkProbability = TalkManager.Instance.CalculateTalkingProbability(player.Personality, player.nearestNPC.Personality, friendLevel);
            if (Random.Range(0f, 1f) < talkProbability)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            Debug.LogWarning("[Talk] False: player cannot talk");
            return false;
        }
    }
}
