using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkManager : MonoBehaviour
{
    #region Singleton
    private static TalkManager _instance;
    public static TalkManager Instance { get { return _instance; } }

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

    [Header("Talk Encounter Settings")]
    [SerializeField] private float maxProbability = 0.7f;
    [SerializeField, Range(0f, 0.4f)] float probabilityRandomWeight = 0f;

    [Header("Talk Duration Settings")]
    [SerializeField, Range(0f, 10f)] private float maxTalkDuration = 5f;
    [SerializeField, Range(0f, 0.4f)] private float durationRandomWeight = 0f;
    [SerializeField, Range(1f, 3f)] private float maximumDurationRandomContribution = 2f;

    [Header("Debug")]
    [SerializeField] private bool debugMessages = false;
    [SerializeField] private bool debugTest = false;
    [SerializeField] private Personality testPlayerPersonality;
    [SerializeField] private Personality testNPCPersonality;
    private Dictionary<Personality, float> talkingProbabilities = new Dictionary<Personality, float>();
    private Dictionary<Personality, float> talkingDurations = new Dictionary<Personality, float>();


    private void Start()
    {
        talkingProbabilities = new Dictionary<Personality, float>(){
            { Personality.RESERVED, 0.2f},
            { Personality.CHEERFUL, 0.6f},
            { Personality.FRIENDLY, 1f },
            { Personality.TALKATIVE, 0.9f },
            { Personality.DEPRESSED, 0.35f }
        };

        talkingDurations = new Dictionary<Personality, float>()
        {
            {Personality.RESERVED,  0.1f},
            {Personality.TALKATIVE, 1f},
            {Personality.CHEERFUL, 0.7f },
            {Personality.FRIENDLY, 0.7f },
            {Personality.DEPRESSED, 0.15f }
        };
        if (maximumDurationRandomContribution > 0.8f * maxTalkDuration)
        {
            maximumDurationRandomContribution = 0.75f * maxTalkDuration;
        }

        if (debugTest)
        {
            float prob = CalculateTalkingProbability(testPlayerPersonality, testNPCPersonality, 0f);
            float duration = CalculateTalkingDuration(testPlayerPersonality, testNPCPersonality);
        }
    }


    public float CalculateTalkingProbability(
        Personality playerPersonality,
        Personality npcPersonality,
        float friendLevel
    )
    {
        if (maxProbability <= 0)
        {
            return 0f;
        }
        float probabilityPercentage = talkingProbabilities[playerPersonality] * talkingProbabilities[npcPersonality];
        float randomComponent = Random.Range(0f, probabilityRandomWeight);
        float randomSign = Random.value < .5 ? 1 : -1;
        float finalProbability = probabilityPercentage * maxProbability * (friendLevel <= 1f ? 1f : 2f) + randomComponent * randomSign;
        finalProbability = Mathf.Clamp(finalProbability, 0f, 1f);
        if (debugMessages)
            Debug.Log($"[TalkManager->Probability] Probability percentage: {probabilityPercentage}, Overall probability: {finalProbability}", this);
        return finalProbability;
    }

    public float CalculateTalkingDuration(
        Personality playerPersonality,
        Personality npcPersonality
    )
    {
        float durationPercentage = talkingDurations[playerPersonality] * talkingDurations[npcPersonality];
        float randomComponent = Random.Range(0f, durationRandomWeight * maximumDurationRandomContribution);
        float randomSign = Random.value < .5 ? 1 : -1;
        float finalDuration = durationPercentage * maxTalkDuration + randomComponent * randomSign;
        if (debugMessages)
            Debug.Log($"[TalkManager->Duration] Duration percentage: {durationPercentage}, randomComponent: {randomComponent * randomSign},  overall duration: {finalDuration}", this);
        return finalDuration;
    }
}
