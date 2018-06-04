using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Classes
    [System.Serializable]
    public class ModeLength
    {
        [SerializeField] private new string name;
        public float[] value = { 0, 0, 0 };

        public ModeLength(string v)
        {
            this.name = v;
        }
    }

    [System.Serializable]
    public class ModesLengthNew
    {
        public ModeLength[] modes = { new ModeLength("Scatter 1"),
            new ModeLength("Chase 1"), new ModeLength("Scatter 2"),
            new ModeLength("Chase 2"), new ModeLength("Scatter 3"),
            new ModeLength("Chase 3"), new ModeLength("Scatter 4") };      
    }
    #endregion

    [Header("General Settings")]
    [SerializeField] private FloatVariable currentLvl;
    [SerializeField] private GameObject getReadyPrefab;

    [Header("Pacman Settings")]
    [SerializeField] private Transform pacMan;
    [SerializeField] private FloatReference startHP;
    [SerializeField] private FloatVariable currentHP;

    [Header("Ghost Settings")]
    [SerializeField] private FloatVariable frightningTime;
    [Tooltip("Продолжительность режимов на 1 /2-4/5+ уровнях")]
    [SerializeField]private ModesLengthNew modesTime;

    [Header("Score Settings")]
    [SerializeField] private GameObject bonusScorePref;
    [SerializeField] private FloatVariable scoreVariable;
    [SerializeField] private FloatVariable highScoreVariable;
    [SerializeField] private FloatVariable bonusScoreVariable;
    [Tooltip("Количество очков за первое съеденное приведение")]
    [SerializeField] private FloatReference defaultGhostScoreVariable;
    [Tooltip("Переменная с количеством очков за съеденное приведение")]
    [SerializeField] private FloatVariable ghostScoreVariable;

    [Header("Set Settings")]
    [SerializeField] private CoinsSet activeCoinsSet;
    [SerializeField] private CoinsSet allCoinsSet;

    [Header("Event Settings")]
    [SerializeField] private GameEvent endFrightEvent;
    [SerializeField] private GameEvent changeModeEvent;
    [SerializeField] private GameEvent coinCollectedEvent;
    [SerializeField] private GameEvent updateHighScoreEvent;
    [SerializeField] private GameEvent updateLVLEvent;
    [SerializeField] private GameEvent hpValueChangedEvent;
    [SerializeField] private GameEvent resetToDefaultEvent;

    [Header("UI Settings")]
    [SerializeField] private GameObject pauseMenu;

    #region Privates
    private Coroutine frightCoroutine;
    private float remainingFrightTime;
    private float remainingStageTime;

    private int currentStage=0;

    private bool gameOver;
    #endregion

    private void Start()
    {
        RestartGame();
        UpdateHighScore();
    }

    private void Update()
    {
        if (!gameOver)
        {
            EndLvlController();
            NextStageTimer();
            if (Input.GetButtonDown("Cancel"))
            {
                Pause();
            }
        }
    }

    private void OnApplicationQuit()
    {
        UpdateHighScore();
    }

    /// <summary>
    /// Проверяем окончен ли уроень
    /// </summary>
    private void EndLvlController()
    {
        if(activeCoinsSet.Items.Count<=0)
        {
            gameOver = true;
            currentLvl.ApplyChange(+1);
            if (updateLVLEvent != null) updateLVLEvent.Raise();
            else Debug.LogError("UpdateLVLEvent is null for @" + this.name);
            StartLvl();
        }
    }

    /// <summary>
    /// Ставим паузу или выходим из паузы
    /// </summary>
    public void Pause()
    {
        Time.timeScale = Time.timeScale < 1 ? 1 : 0;
        pauseMenu.SetActive(!pauseMenu.activeSelf);
    }

    /// <summary>
    /// Выходим из приложения
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    /// <summary>
    /// Добавляем очки к счету
    /// </summary>
    /// <param name="value"></param>
    private void AddScore(float value)
    {
        if (scoreVariable != null) scoreVariable.ApplyChange(value);
        else Debug.LogError("ScoreVariable or/and CoinValue is null for @" + this.name);

        if (coinCollectedEvent != null) coinCollectedEvent.Raise();
        else Debug.LogError("CoinCollectedEvent is null for @" + this.name);
    }

    /// <summary>
    /// Обнуляем счет
    /// </summary>
    private void ResetScore()
    {
        if (scoreVariable != null) scoreVariable.SetValue(0);
        else Debug.Log("ScoreVariable is null! @"+this.name);

        if (coinCollectedEvent != null) coinCollectedEvent.Raise();
        else Debug.LogError("CoinCollectedEvent is null for @" + this.name);
    }

    /// <summary>
    /// Показываем всплывающее окно со очками
    /// </summary>
    public void ShowBonusScore()
    {
        if (bonusScorePref != null) Instantiate(bonusScorePref, pacMan.localPosition+Vector3.up,Quaternion.identity,null);
        else Debug.LogError("BonusScorePref is null!");
    }      

    /// <summary>
    /// Задаем значение переменных как в начале уровня
    /// </summary>
    private void ResetToDefault()
    {        
        if (remainingFrightTime > 0)
        {
            StopCoroutine(frightCoroutine);
        }
        remainingFrightTime = 0;
        currentStage = 0;
        gameOver = false;
        UpdateRemainingStageTime();
        if (resetToDefaultEvent != null) resetToDefaultEvent.Raise();
        else Debug.LogError("ResetToDefaultEvent is null for @" + this.name);
    }

    private void GameOver()
    {
        gameOver = true;
        UpdateHighScore();
        Pause();
    }

    /// <summary>
    /// Обновляем лучший счет
    /// </summary>
    private void UpdateHighScore()
    {
        if (!PlayerPrefs.HasKey("HighScore") || PlayerPrefs.GetFloat("HighScore") < scoreVariable.Value)
        {
            PlayerPrefs.SetFloat("HighScore", scoreVariable.Value);
            PlayerPrefs.Save();
        }
        if (highScoreVariable != null) highScoreVariable.SetValue(PlayerPrefs.GetFloat("HighScore"));
        else Debug.LogError("HighScoreVariable is null for @" + this.name);

        if (updateHighScoreEvent != null) updateHighScoreEvent.Raise();
        else Debug.LogError("UpdateHighScoreEvent is null for @" + this.name);
        Debug.Log(PlayerPrefs.HasKey("HighScore"));
    }

    /// <summary>
    /// Перезапускаем уровень
    /// </summary>
    public void RestartGame()
    {
        currentLvl.SetValue(1);
        if (updateLVLEvent != null) updateLVLEvent.Raise();
        else Debug.LogError("UpdateLVLEvent is null for @" + this.name);
        ResetScore();
        ChangeHP(startHP.Value, true);
        SetupFrightningTime();
        StartLvl();
    }

    /// <summary>
    /// Задаем продолжительность режима испуга
    /// </summary>
    private void SetupFrightningTime()
    {
        frightningTime.SetValue(19 - currentLvl.Value);
    }

    /// <summary>
    /// Запускаем уровень
    /// </summary>
    private void StartLvl()
    {
        ActivateAllCoins();
        ResetToDefault();
        StartCoroutine(StartLvlCoroutine());
    }

    /// <summary>
    /// Пауза перед началом уровня
    /// </summary>
    /// <returns></returns>
    IEnumerator StartLvlCoroutine()
    {
        gameOver = true;
        Time.timeScale = 0;
        getReadyPrefab.SetActive(true);
        yield return new WaitForSecondsRealtime(3);
        getReadyPrefab.SetActive(false);
        Time.timeScale = 1;
        gameOver = false;

    }

    /// <summary>
    /// Аквтиыируем все монетки на карте
    /// </summary>
    private void ActivateAllCoins()
    {
        if(allCoinsSet!=null)
        {
            for (int i = 0; i < allCoinsSet.Items.Count; i++)
            {
                allCoinsSet.Items[i].gameObject.SetActive(true);
            }
        }
        else
        {
            Debug.LogError("AllCoinsSet is null for @" + this.name);
        }
    }

    #region PlayerMethods
    /// <summary>
    /// Обновляем количество ХП у игрока
    /// </summary>
    /// <param name="value"></param>
    /// <param name="equalize"></param>
    private void ChangeHP(float value, bool equalize=false)
    {
        if (currentHP != null)
        {
            if (equalize) currentHP.SetValue(value);
            else currentHP.ApplyChange(value);
        }
        else Debug.LogError("CurrentHP is null for @" + this.name);

        if (hpValueChangedEvent != null) hpValueChangedEvent.Raise();
        else Debug.LogError("HPValueChangedEvent is null for @" + this.name);
    }

    /// <summary>
    /// Вызываем если призрак поймал игрока
    /// </summary>
    public void GotPlayer()
    {
        StartCoroutine(GotPlayerCoroutine());
    }

    IEnumerator GotPlayerCoroutine()
    {        
        Time.timeScale = 0;// время для проигрывания анимации
        yield return new WaitForSecondsRealtime(2);
        Time.timeScale = 1;        
        //перезапускаем уровень или GameOver
        if (currentHP.Value > 0)
        {
            ChangeHP(-1);
            ResetToDefault();
            StartCoroutine(StartLvlCoroutine());
        }
        else
        {
            GameOver();
        }
    }
    #endregion

    #region GhostMethods
    /// <summary>
    /// Отсчитываем таймер до смены режима
    /// </summary>
    private void NextStageTimer()
    {
        if (currentStage < modesTime.modes.Length && remainingFrightTime<=0)
        {
            if (remainingStageTime > 0)
            {
                remainingStageTime -= Time.deltaTime;
            }
            else
            {
                currentStage++;
                if (changeModeEvent != null) changeModeEvent.Raise();
                else Debug.Log("ChangeModeEvent is null!");
                UpdateRemainingStageTime();
            }
        }
    }

    /// <summary>
    /// Обновляем выремя оставшееся до смены режима
    /// </summary>
    private void UpdateRemainingStageTime()
    {
        switch((int)currentLvl.Value)
        {
            case 1:
                remainingStageTime = modesTime.modes[currentStage].value[0];
                break;
            case 2:
            case 3:
            case 4:
                remainingStageTime = modesTime.modes[currentStage].value[1];
                break;
            default:
                remainingStageTime = modesTime.modes[currentStage].value[2];
                break;
        }
        //remainingStageTime = currentStage%2==0? stageTime[currentStage/2].scatterLength.Value : (currentStage != stageTime.Count*2 ? stageTime[currentStage/2].chaseLength.Value : 0);
    }

    public void StartFrightTimer()
    {
        if (remainingFrightTime>0)
        {
            try
            {
                StopCoroutine(frightCoroutine);
            }
            catch (System.Exception e)
            {
                Debug.Log(e);
            }
        }
        frightCoroutine = StartCoroutine(FrightTimerCoroutine());
    }

    /// <summary>
    /// Счетчик режима испуга
    /// </summary>
    /// <returns></returns>
    IEnumerator FrightTimerCoroutine()
    {
        if (ghostScoreVariable != null & defaultGhostScoreVariable != null) ghostScoreVariable.Value = defaultGhostScoreVariable.Value;
        else Debug.LogError("GhostScoreVariable or/and DefaultGhostScoreVariable is null!");
        remainingFrightTime = frightningTime.Value;
        while (remainingFrightTime > 0)
        {
            remainingFrightTime -= Time.deltaTime;
            yield return null;
        }
        endFrightEvent.Raise();
    }

    public void GotGhost()
    {
        StartCoroutine(GotGhostCoroutine());
    }

    IEnumerator GotGhostCoroutine()
    {
        bonusScoreVariable.SetValue(ghostScoreVariable);
        AddScore(bonusScoreVariable.Value);
        ghostScoreVariable.SetValue(ghostScoreVariable.Value*2);
        ShowBonusScore();
        Time.timeScale = 0;        
        yield return new WaitForSecondsRealtime(2);
        Time.timeScale = 1;
    }
    #endregion    
}
