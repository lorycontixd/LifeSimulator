using Lore.Game.Characters;
using Lore.Game.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceStation : MonoBehaviour
{
    #region Utility

    public enum CommunicationTarget
    {
        RECEIVERS,
        ALL // includes sender
    }
    public enum CommunicationMessage
    {
        NONE,
        ALERT,
        DISMISSED,
        DANGER
    }
    public abstract class CommunicationData
    {
        public Policeman sender;
        public GameObject Object;
        public DateTime Time;
        public CommunicationMessage message;

        protected CommunicationData(Policeman sender, GameObject @object, DateTime time)
        {
            this.sender = sender;
            Object = @object;
            Time = time;
        }
    }
    public class AlertData : CommunicationData
    {
        public AlertData(Policeman sender, GameObject @object, DateTime time) : base(sender, @object, time)
        {
            message = CommunicationMessage.ALERT;
        }
    }
    public class DangerData : CommunicationData
    {
        public DangerData(Policeman sender, GameObject @object, DateTime time) : base(sender, @object, time)
        {
            message = CommunicationMessage.DANGER;
        }
    }
    public class DismissData : CommunicationData
    {
        public DismissData(Policeman sender, GameObject @object, DateTime time) : base(sender, @object, time)
        {
            message = CommunicationMessage.DISMISSED;
        }
    }
    #endregion

    #region Singleton
    private static PoliceStation _instance;
    public static PoliceStation Instance { get { return _instance; } }

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

    public List<Policeman> policemen = new List<Policeman>();
    public int PolicemenCount { get { return policemen.Count; } }

    public Policeman alertingPoliceman;


    public void RegisterAgent(Policeman policeman)
    {
        if (!policemen.Contains(policeman))
        {
            policemen.Add(policeman);
        }
        policeman.onAlert += OnPolicemanAlert;
        policeman.onSpotted += OnPolimanSpottedEnemy;
        policeman.onAlertDismiss += OnAlertDismiss;
        policeman.onCapturedDanger += OnDangerCaptured;
    }

    

    // Event Listeners
    #region Event Listeners
    private void OnPolicemanAlert(Policeman policeman, GameObject @object, DateTime time)
    {
        alertingPoliceman = policeman;
        SendMessageToAll(CommunicationMessage.ALERT, new AlertData(policeman, @object, time));
        alertingPoliceman = null;
    }

    private void OnPolimanSpottedEnemy(Policeman policeman, GameObject @object, DateTime time)
    {
        alertingPoliceman = policeman;
        SendMessageToAll(CommunicationMessage.DANGER, new DangerData(policeman, @object, time));
        alertingPoliceman = null;
    }

    private void OnAlertDismiss(Policeman policeman, DateTime time)
    {
        alertingPoliceman = policeman;
        SendMessageToAll(CommunicationMessage.DISMISSED, new DismissData(policeman, null, time));
        alertingPoliceman = null;
    }

    private void OnDangerCaptured(Policeman policeman, GameObject @object, DateTime time)
    {
        if (@object.tag == "Player")
        {
        }
    }
    #endregion

    private void SendMessageToAll(CommunicationMessage msg, CommunicationData data, CommunicationTarget target = CommunicationTarget.RECEIVERS)
    {
        if (alertingPoliceman != null)
        {
            foreach (Policeman p in policemen)
            {
                if (target == CommunicationTarget.RECEIVERS)
                {
                    if (p.ID == data.sender.ID)
                    {
                        continue;
                    }
                }
                p.ReceivedAlert(msg, data);
            }
        }
    }
}
