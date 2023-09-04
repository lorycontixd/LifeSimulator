using Lore.Game.Characters;
using Michsky.MUIP;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCanvas : MonoBehaviour
{
    #region Singleton
    private static PlayerCanvas _instance;
    public static PlayerCanvas Instance { get { return _instance; } }

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

    [SerializeField] private ProgressBar actionBar;
    private bool IsActive;
    private float Timestamp;

    private void Start()
    {
        actionBar.gameObject.SetActive(false);

        GamePlayer player = FindFirstObjectByType<GamePlayer>();
        player.onActionComplete += OnActionComplete;
    }

    private void OnActionComplete(GAction action, float actualDuration)
    {
        SetAction(actualDuration);
    }

    private void Update()
    {
        if (IsActive)
        {
            actionBar.ChangeValue(Timestamp);
            //LookAtCamera();
            Timestamp += Time.deltaTime;
        }   
    }
    public void SetAction(float duration)
    {
        if (duration <= 0f)
        {
            return;
        }
        StartCoroutine(SetActionCo(duration));
    }
    private void LookAtCamera()
    {
        Vector3 dirToCamera = Camera.main.transform.position - actionBar.transform.position;
        Vector3 targetDirection = new Vector3(dirToCamera.x, actionBar.transform.position.y, dirToCamera.z);
        actionBar.transform.LookAt(targetDirection);
    }
    private IEnumerator SetActionCo(float duration)
    {
        actionBar.gameObject.SetActive(true);
        Timestamp = 0f;
        actionBar.maxValue = duration;
        IsActive = true;
        yield return new WaitForSeconds(duration);
        IsActive = false;
        actionBar.gameObject.SetActive(false);
    }
}
