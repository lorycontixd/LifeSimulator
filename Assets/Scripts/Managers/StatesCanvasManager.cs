using DG.Tweening;
using GOAP;
using Lore.Game.Characters;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class StatesCanvasManager : MonoBehaviour
{
    [SerializeField] private GameObject buttonGroup = null;
    [SerializeField] private Button buttonGroupCloseButton;
    [SerializeField] private Button buttonGroupOpenButton;

    [Header("Panels")]
    [SerializeField] private GameObject timeInfoPanel = null;
    [SerializeField] private GameObject statesPanel = null;
    [SerializeField] private DebugPanel debugPanel = null;

    [Header("Components")]
    [SerializeField] private TextMeshProUGUI currentActionText = null;

    [Header("Prefabs")]
    [SerializeField] private GameObject titleTextPrefab = null;
    [SerializeField] private GameObject stateTextPrefab = null;
    [SerializeField] private GameObject daySliderPrefab = null;
    [SerializeField] private Gradient daySliderGradient = null;

    [Header("Settings")]
    [SerializeField] private bool debugMode;
    [SerializeField] private bool openStatesPanelOnStart = false;
    [SerializeField] private bool openTimePanelOnStart = false;
    [SerializeField] private bool openDebugPanelOnStart = false;
    [SerializeField] private float buttonGroupCloseTranslation = 60f;
    [SerializeField, Range(1f, 4f)] private float buttonGroupCloseDuration = 2f;

    private bool IsStatesPanelOpen;
    private bool IsTimePanelOpen;
    private bool IsDebugPanelOpen;
    private bool IsButtonGroupOpen = true;
    private GamePlayer player;

    private void Start()
    {
        //TimeManager2.Instance.onNewDay += OnNewDay;
        //TimeManager2.Instance.onDayPartChange += OnDayPartChange;

        timeInfoPanel.SetActive(openTimePanelOnStart);
        statesPanel.SetActive(openStatesPanelOnStart);
        debugPanel.gameObject.SetActive(openDebugPanelOnStart);
        IsTimePanelOpen = openTimePanelOnStart;
        IsStatesPanelOpen = openStatesPanelOnStart;
        IsDebugPanelOpen = openDebugPanelOnStart;
        buttonGroup.SetActive(true);
        IsButtonGroupOpen = true;
        buttonGroupOpenButton.gameObject.SetActive(false);
        buttonGroupCloseButton.gameObject.SetActive(true);

        player = GameObject.FindFirstObjectByType<GamePlayer>();

        DOTween.Init();
    }
    private void Update()
    {
        if (IsStatesPanelOpen)
        {
            SpawnStates();
        }
        if (IsTimePanelOpen)
        {
            SpawnTimeInfoStates();
        }
        SetCurrentAction();
    }

    public void Title(string text, Transform parent = null)
    {
        if (titleTextPrefab != null)
        {
            GameObject clone = Instantiate(titleTextPrefab, parent);
            TextMeshProUGUI textComponent = clone.GetComponent<TextMeshProUGUI>();
            textComponent.text = text;
        }
    }
    public void SpawnStateObject(string key, object value, Transform parent = null)
    {
        if (stateTextPrefab != null)
        {
            GameObject clone = Instantiate(stateTextPrefab, parent);
            TextMeshProUGUI textComponent = clone.GetComponent<TextMeshProUGUI>();
            textComponent.text = $"{key} - {value}";
        }
    }

    public void SetCurrentAction()
    {
        if (currentActionText == null) { return; }
        currentActionText.text = player.currentAction != null ? $"Current action: {player.currentAction.actionName}" : "None";
    }

    public void CloseAllPanels()
    {
        statesPanel.SetActive(false);
        IsStatesPanelOpen = false;

        timeInfoPanel.SetActive(false);
        IsTimePanelOpen = false;

        debugPanel.gameObject.SetActive(false);
        IsDebugPanelOpen = false;
    }
    public void ToggleStatesPanel()
    {
        statesPanel.SetActive(!IsStatesPanelOpen);
        IsStatesPanelOpen = !IsStatesPanelOpen;
    }

    public void ToggleTimeInfoPanel()
    {
        timeInfoPanel.SetActive(!IsTimePanelOpen);
        IsTimePanelOpen = !IsTimePanelOpen;
    }
    public void ToggleDebugPanel()
    {
        debugPanel.gameObject.SetActive(!IsDebugPanelOpen);
        IsDebugPanelOpen = !IsDebugPanelOpen;
    }
    public void CloseButtonGroup()
    {
        buttonGroup.transform.DOMove(buttonGroup.transform.position + Vector3.up * buttonGroupCloseTranslation, buttonGroupCloseDuration);
        buttonGroupCloseButton.transform.DOMove(buttonGroupCloseButton.transform.position + Vector3.up * buttonGroupCloseTranslation, buttonGroupCloseDuration);
        buttonGroupOpenButton.transform.DOMove(buttonGroupOpenButton.transform.position + Vector3.up * buttonGroupCloseTranslation, buttonGroupCloseDuration);
        buttonGroupOpenButton.gameObject.SetActive(true);
        buttonGroupCloseButton.gameObject.SetActive(false);
        CloseAllPanels();
    }
    public void OpenButtonGroup()
    {
        buttonGroup.transform.DOMove(buttonGroup.transform.position - Vector3.up * buttonGroupCloseTranslation, buttonGroupCloseDuration);
        buttonGroupCloseButton.transform.DOMove(buttonGroupCloseButton.transform.position - Vector3.up * buttonGroupCloseTranslation, buttonGroupCloseDuration);
        buttonGroupOpenButton.transform.DOMove(buttonGroupOpenButton.transform.position - Vector3.up * buttonGroupCloseTranslation, buttonGroupCloseDuration);
        buttonGroupOpenButton.gameObject.SetActive(false);
        buttonGroupCloseButton.gameObject.SetActive(true);
    }


    // ##### Time info
    public void TimeInfoClear()
    {
        for(int i=0; i<timeInfoPanel.transform.childCount; i++)
        {
            Destroy(timeInfoPanel.transform.GetChild(i).gameObject);
        }
    }

    public void SpawnTimeInfoStates()
    {
        TimeInfoClear();
        Title("Time Info", timeInfoPanel.transform);
        SpawnStateObject("Days Passed", TimeManager.Instance.DaysPassed, timeInfoPanel.transform);
        SpawnStateObject("Day Part", TimeManager.Instance.CurrentDayPart, timeInfoPanel.transform);
        DayBarSpawn();
    }

    public void DayBarSpawn()
    {
        if (daySliderPrefab != null)
        {
            GameObject clone = Instantiate(daySliderPrefab, timeInfoPanel.transform);
            Slider slider = clone.GetComponentInChildren<Slider>();
            slider.value = TimeManager.Instance.DayPercentage;
            Image image = slider.GetComponentInChildren<Image>();
            if (daySliderGradient == null)
            {
                switch (TimeManager.Instance.CurrentDayPart)
                {
                    case DayPart.MORNING:
                        image.color = new Color(255f, 165f, 0f, 1f);
                        break;
                    case DayPart.AFTERNOON:
                        image.color = Color.red;
                        break;
                    case DayPart.EVENING:
                        image.color = Color.green;
                        break;
                    case DayPart.NIGHT:
                        image.color = Color.blue;
                        break;
                }
            }
            else
            {
                image.color = daySliderGradient.Evaluate(TimeManager.Instance.DayPercentage);
            }
        }
    }

    private void OnNewDay(int daysPassed)
    {
        SpawnTimeInfoStates();
    }
    private void OnDayPartChange(DayPart oldPart, DayPart newPart)
    {
        SpawnTimeInfoStates();
    }

    // ##### States
    public void StatesClear()
    {
        Debug.Log($"Clearing {statesPanel.transform.childCount} text objects", this);
        for (int i = 0; i < statesPanel.transform.childCount; i++)
        {
            Destroy(statesPanel.transform.GetChild(i).gameObject);
        }
    }

    public void SpawnStates()
    {
        Player player = GameObject.FindFirstObjectByType<Player>();
        if (player == null)
        {
            return;
        }
        Dictionary<string, object> beliefs = player.beliefs.states;
        StatesClear();
        Title("Player beliefs", statesPanel.transform);
        foreach(KeyValuePair<string, object> kvp in beliefs)
        {
            SpawnStateObject(kvp.Key, kvp.Value, statesPanel.transform);
        }
        Title("World states", statesPanel.transform);
        Dictionary<string, object> worldStates = GWorld.Instance.GetWorld().states;
        foreach (KeyValuePair<string, object> kvp in worldStates)
        {
            SpawnStateObject(kvp.Key, kvp.Value, statesPanel.transform);
        }
    }


    
}
