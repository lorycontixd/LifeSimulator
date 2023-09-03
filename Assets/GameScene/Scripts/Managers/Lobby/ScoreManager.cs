using Lore.Game.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    #region Utility
    public class Scores
    {
        public float Score;
        public int GameDaysSurvived;
        public float SecondsSurvived;
        public float FinalMoney;
        public int DiseasesCured;
        public int BuildingsConstructed;
        public int FinalPopulation;
        public float MoneyGainedFromInvestments;
        
        private bool isSet = true;

        public Scores(bool readFromManagers = false)
        {
            if (readFromManagers)
            {
                try
                {
                    GameDaysSurvived = TimeManager.Instance.DaysPassed;
                    SecondsSurvived = TimeManager.Instance.TimeSinceStart;
                    FinalMoney = MoneyManager.Instance.Money;
                    DiseasesCured = SicknessManager.Instance.TotalDiseasesCured;
                    BuildingsConstructed = BuildingManager.Instance.BuildingsBuilt;
                    FinalPopulation = 1;
                    MoneyGainedFromInvestments = InvestManager.Instance.InvestmentsMoneyGained;
                    Score = CalculateScore();
                }
                catch
                {

                }
            }
            
        }
        public Scores(int gameDaysSurvived, float secondsSurvived, float finalMoney, int diseasesCured, int buildingsConstructed, int finalPopulation, float moneyGainedFromInvestments)
        {
            GameDaysSurvived = gameDaysSurvived;
            SecondsSurvived = secondsSurvived;
            FinalMoney = finalMoney;
            DiseasesCured = diseasesCured;
            BuildingsConstructed = buildingsConstructed;
            FinalPopulation = finalPopulation;
            MoneyGainedFromInvestments = moneyGainedFromInvestments;
            Score = CalculateScore();
            isSet = true;
        }
        public float CalculateScore()
        {
            if (SecondsSurvived < 15f)
            {
                return 0f;
            }
            float score = GameDaysSurvived + DiseasesCured + BuildingsConstructed + FinalPopulation;
            Debug.Log($"Calculating score ===> {GameDaysSurvived} + {DiseasesCured} + {BuildingsConstructed} + {FinalPopulation} === {score}");
            score = 3f * score;
            return score;
        }

        public void SaveToFile(ES3Settings saveSettings)
        {
            if (isSet)
            {
                ES3.Save("GameDaysSurvived", GameDaysSurvived, saveSettings);
                ES3.Save("SecondsSurvived", SecondsSurvived, saveSettings);
                ES3.Save("FinalMoney", FinalMoney, saveSettings);
                ES3.Save("DiseasesCured", DiseasesCured, saveSettings);
                ES3.Save("BuildingsConstructed", BuildingsConstructed, saveSettings);
                ES3.Save("FinalPopulation", FinalPopulation, saveSettings);
                ES3.Save("MoneyFromInvestments", MoneyGainedFromInvestments, saveSettings);
                ES3.Save("Score", Score, saveSettings);
                Debug.Log($"Saving score to file: {Score}");
            }
        }
        public static Scores LoadFromFile(ES3Settings saveSettings)
        {
            Scores scores = new Scores();
            try
            {
                scores.GameDaysSurvived = ES3.Load<int>("GameDaysSurvived", saveSettings);
                scores.SecondsSurvived = ES3.Load<float>("SecondsSurvived", saveSettings);
                scores.FinalMoney = ES3.Load<float>("FinalMoney", saveSettings);
                scores.DiseasesCured = ES3.Load<int>("DiseasesCured", saveSettings);
                scores.BuildingsConstructed = ES3.Load<int>("BuildingsConstructed", saveSettings);
                scores.FinalPopulation = ES3.Load<int>("FinalPopulation", saveSettings);
                scores.MoneyGainedFromInvestments = ES3.Load<float>("MoneyFromInvestments", saveSettings);
                scores.Score = ES3.Load<float>("Score", saveSettings);


                /*
                ES3.LoadInto("GameDaysSurvived", scores.GameDaysSurvived, saveSettings);
                ES3.LoadInto("SecondsSurvived", scores.SecondsSurvived, saveSettings);
                ES3.LoadInto("FinalMoney", scores.FinalMoney, saveSettings);
                ES3.LoadInto("DiseasesCured", scores.DiseasesCured, saveSettings);
                ES3.LoadInto("BuildingsConstructed", scores.BuildingsConstructed, saveSettings);
                ES3.LoadInto("FinalPopulation", scores.FinalPopulation, saveSettings);
                ES3.LoadInto("MoneyFromInvestments", scores.MoneyGainedFromInvestments, saveSettings);
                ES3.LoadInto("Score", scores.Score, saveSettings);*/
                Debug.Log($"Loading score to file: {scores.Score}");
                return scores;
            }
            catch (Exception e)
            {
                Debug.LogError($"LFF: {e}");
                return null;
            }
        }

        public static bool SaveToFile(Scores scores, ES3Settings saveSettings)
        {
            ES3.Save("GameDaysSurvived", scores.GameDaysSurvived, saveSettings);
            ES3.Save("SecondsSurvived", scores.SecondsSurvived, saveSettings);
            ES3.Save("FinalMoney", scores.FinalMoney, saveSettings);
            ES3.Save("DiseasesCured", scores.DiseasesCured, saveSettings);
            ES3.Save("BuildingsConstructed", scores.BuildingsConstructed, saveSettings);
            ES3.Save("FinalPopulation", scores.FinalPopulation, saveSettings);
            ES3.Save("MoneyFromInvestments", scores.MoneyGainedFromInvestments, saveSettings);
            ES3.Save("Score", scores.Score, saveSettings);
            Debug.Log($"Saving score to file: {scores.Score}");

            float highscore = ES3.KeyExists("HighScore", saveSettings) ? ES3.Load<float>("HighScore", saveSettings) : -1f;

            if (scores.Score > highscore)
            {
                ES3.Save("HighScore", scores.Score);
            }
            return true;
        }

        /*public void SaveComponents(object obj, ES3Writer writer, ES3Settings settings = null)
        {
            var instance = (Scores)obj;

            writer.WriteProperty<int>("GameDaysSurvived", GameDaysSurvived);
            writer.WriteProperty<float>("SecondsSurvived", SecondsSurvived);
            writer.WriteProperty<float>("FinalMoney", FinalMoney);
            writer.WriteProperty<int>("DiseasesCured", DiseasesCured);
            writer.WriteProperty<int>("BuildingsConstructed", BuildingsConstructed);
            writer.WriteProperty<int>("FinalPopulation", FinalPopulation);
            writer.WriteProperty<float>("MoneyFromInvestments", MoneyGainedFromInvestments);
            writer.WriteProperty<float>("Score", Score);
        }

        public Scores LoadComponents(ES3Reader reader, object obj)
        {
            var instance = (Scores)obj;
            foreach ( string propertyName in reader.Properties)
            {
                switch(propertyName)
                {
                    case "GameDaysSurvived":
                        instance.GameDaysSurvived = reader.Read<int>();
                        break;
                    case "SecondsSurvived":
                        instance.SecondsSurvived = reader.Read<float>();
                        break;
                    case "FinalMoney":
                        instance.FinalMoney = reader.Read<float>();
                        break;
                    case "DiseasesCured":
                        instance.DiseasesCured = reader.Read<int>();
                        break;
                    case "BuildingsConstructed":
                        instance.BuildingsConstructed = reader.Read<int>();
                        break;
                    case "FinalPopulation":
                        instance.FinalPopulation = reader.Read<int>();
                        break;
                    case "MoneyFromInvestments":
                        instance.MoneyGainedFromInvestments = reader.Read<float>();
                        break;
                    case "Score":
                        instance.Score = reader.Read<float>();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }
            return instance;
        }*/

    }
    #endregion

    #region Singleton
    private static ScoreManager _instance;
    public static ScoreManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            _instance = this;
        }
    }
    #endregion

    public string SaveFilename = "savefile.es3";
    private string SavePath;
    private ES3Settings settings;
    public Scores lastScores { get; private set; }
    public Scores currentScores { get; private set; }
    public float HighestScore { get; private set; }

    public Action onScoresLoaded;
    public Action<Scores,ES3Settings> onScoresSaved;


    private void Start()
    {
        SavePath = Path.Combine(Application.persistentDataPath, SaveFilename);
        Debug.Log($"[ScoreManager] Save path: {SavePath}", this);
        settings = new ES3Settings(SavePath, ES3.EncryptionType.None);
        lastScores = LoadLastScores();
        if (lastScores != null)
        {
            HighestScore = ES3.Load<float>("HighScore", settings);
        }
        else
        {
            HighestScore = -1f;
        }
    }

    public void SetScores(Scores scores)
    {
        currentScores = scores;
    }

    public Scores LoadLastScores()
    {
        onScoresLoaded?.Invoke();
        return Scores.LoadFromFile(settings);
    }

    public void SaveScores()
    {
        if (currentScores == null) {
            Debug.LogWarning("You havent set the scores yet. Please call SetScores");
            return;
        }
        Scores.SaveToFile(currentScores, settings);
        onScoresSaved?.Invoke(currentScores, settings);
        Debug.Log($"[ScoreManager] Saved scores to {settings.path}!");
    }

}
