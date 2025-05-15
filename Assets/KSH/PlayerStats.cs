using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class PlayerStats : MonoBehaviour,ISaveable
{
    public class StatsChangeData // 변경된 스탯 데이터
    {
        public float Time { get; }
        public float Health { get; }
        public float Reputation { get; }

        public StatsChangeData(float time, float health, float reputation)
        {
            Time = time;
            Health = health;
            Reputation = reputation;
        }
    }
    
    private GameConstants _gameConstants;
    private ValueByAction _valueByAction;
    
    public float TimeStat { get; private set; }
    public float HealthStat { get; private set; }
    public float ReputationStat { get; private set; }
    
    public event Action OnDayEnded;
    public event Action Exhaustion; // 탈진
    public event Action Overslept; // 늦잠
    public event Action ZeroReputation; // 평판 0 이벤트
    public event Action<StatsChangeData> OnStatsChanged; // 스탯 변경 이벤트
    public event Action OnWorked; // 퇴근 이벤트 (출근 이후 집에 돌아올 시간에 발생)
    
    private float previousAddHealth = 0f;
    
    public static PlayerStats Instance;
    
    private float additionalMaxHealth = 0f;
    public float MaxHealth => _gameConstants.maxHealth + additionalMaxHealth;
    
    public ActionType LastAction { get; private set; }
    
    // 결근 이벤트 관련 변수
    private bool _hasWorkedToday = false;
    public bool HasWorkedToday => _hasWorkedToday;
    private bool _hasCheckedAbsenceToday = false; // 결근 체크, 하루에 결근 여러 번 체크 안하기 위함
    public event Action OnAbsent; // 결근 
    
    // 말풍선
    private GameObject _messagePanelInstance;
    private SpeechBubbleFollower _speechBubbleFollower;
    private bool _isActiveBubble;
    private bool _hasShownBubbleToday; // 하루에 말풍선 하나만 표시하기
    private InteractionAnimationPanelController _interactionAnimation; // 상호작용 패널 Active 여부 확인
    private HousingCanvasController _housingCanvasController; // 돌발 패널 Active 여부 확인 

    private int _mealCount;
    public int MealCount => _mealCount;
    private const int MAX_MEAL_COUNT = 2; // 하루 2회 제한
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 데이터값 유지용
        }
        else
        {
            Destroy(gameObject);
        }
        
        _gameConstants = new GameConstants();
        HealthStat = _gameConstants.baseHealth;
        TimeStat = _gameConstants.baseTime;
        ReputationStat = _gameConstants.baseReputation;
    }
    
    private void Start()
    {
        _valueByAction = new ValueByAction();
        _valueByAction.Initialize(); // 값 초기화
        
        LoadMessagePanel();
        CheckBubble();
        
        GameManager.Instance.SetEvents();
        SceneManager.sceneLoaded += OnSceneLoaded; // 씬 전환 이벤트
        _mealCount = 0; // 식사 횟수 0회
    }
    
    #region 말풍선(Bubble) 관련
     
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 새 씬에서 메시지 패널 다시 로드
        LoadMessagePanel();
        CheckBubble();
    }
    
    // OnDestroy에서 이벤트 구독 해제
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void LoadMessagePanel()
    {
        if (_messagePanelInstance != null) // 기존 패널 파괴
        {
            Destroy(_messagePanelInstance);
            _messagePanelInstance = null;
        }
        
        GameObject messagePanelPrefab = Resources.Load<GameObject>("Prefabs/MessagePanel");
    
        if (messagePanelPrefab != null)
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            
            _messagePanelInstance = Instantiate(messagePanelPrefab, canvas.transform);
            _speechBubbleFollower = _messagePanelInstance.GetComponent<SpeechBubbleFollower>();
            _speechBubbleFollower.SetPlayerTransform();
        
            if (_speechBubbleFollower != null)
            {
                _isActiveBubble = false;
                _hasShownBubbleToday = false;
                _speechBubbleFollower.HideMessage();
            }
        }
    }
    
    private void CheckBubble()
    {
        if (_isActiveBubble)
        {
            _isActiveBubble = false;
            HideBubble();
        }
        
        if (TimeStat >= 8.0f && TimeStat < 9.0f && !_isActiveBubble && !_hasShownBubbleToday)
        {
            _hasShownBubbleToday = true;
            _isActiveBubble = true;
            ShowBubble();
        }
    }
    
    public void SetInteractionPanelController(InteractionAnimationPanelController panelController)
    {
        _interactionAnimation = panelController;
    }

    public void SetHousingCanvasController(HousingCanvasController canvasController)
    {
        _housingCanvasController = canvasController;
    }

    public void ShowBubble()
    {
        if (_interactionAnimation != null && _interactionAnimation.IsPanelActive()) return;
        if (_housingCanvasController != null && _housingCanvasController.IsSuddenPanelActive()) return;
        
        if(_isActiveBubble)
            _speechBubbleFollower.ShowMessage();
    }

    public void HideBubble()
    {
        _speechBubbleFollower.HideMessage();
    }

    public void ShowAndHideBubble(string text)
    {
        _speechBubbleFollower.ShowAndHide(text);
    }

    #endregion

    // 현재 체력으로 해당 행동이 가능한 지 확인
    public bool CanPerformByHealth(ActionType actionType)
    {
        ActionEffect effect = _valueByAction.GetActionEffect(actionType);

        return (HealthStat >= (effect.healthChange * -1));
    }
    
    // 결근 체크
    public void CheckAbsent()
    {
        if (_hasWorkedToday || _hasCheckedAbsenceToday)
            return;
            
        // 9시가 지났는데 출근하지 않은 경우
        if (TimeStat >= 9.0f && !_hasWorkedToday)
        {
            PerformAbsent();
        }
    }

    public void PerformAbsent() // 강제 결근 이벤트, 오류 발생
    {
        _hasCheckedAbsenceToday = true; // 결근 체크 완료 표시
        OnAbsent?.Invoke();

        PerformAction(ActionType.Absence); // 평판 -3
    }
    
    // 행동 처리 메서드
    public void PerformAction(ActionType actionType)
    {
        // 액션에 따른 스탯 소모 값 가져오기
        ActionEffect effect = _valueByAction.GetActionEffect(actionType);
        LastAction = actionType;
        // 선 처리: 특수 보상 먼저 (야근, 회식)
        if (actionType == ActionType.TeamDinner)
        {
            additionalMaxHealth += 1f;
        }

        // 순수 스탯 변경 적용
        ModifyTime(effect.timeChange, actionType);
        ModifyHealth(effect.healthChange);
        ModifyReputation(effect.reputationChange);

        // UI 업데이트용
        OnStatsChanged?.Invoke(new StatsChangeData(TimeStat, HealthStat, ReputationStat));

        // 출근 후 퇴근 이벤트 발생
        if (actionType == ActionType.Work)
        {
            _hasWorkedToday = true;
            OnWorked?.Invoke();
        }

        if (actionType == ActionType.Eat)
        {
            _mealCount++;
        }
    }


    public bool CanEat()
    {
        return _mealCount < MAX_MEAL_COUNT; // 식사 횟수 0,1 일 때만 true
    }

    // 출근 가능 여부 확인 메서드
    public bool CanWork()
    {
        bool isTimeToWork = TimeStat is >= 8.0f and < 9.0f; // 8시에서 9시 사이만 true
        bool isCanPerformWork = CanPerformByHealth(ActionType.Work); // 체력상 가능한지 확인
        
        return isTimeToWork && isCanPerformWork;
    }
    
    // 하루 종료 처리
    private void EndDay(float time, ActionType actionType) //bool isForced? 해서 true면 강제 수면이라 8시에 깨는
    {
        bool isDayEnded = false;

        // 수면 행동 처리
        if (actionType == ActionType.Sleep || actionType == ActionType.TeamDinner || actionType == ActionType.OvertimeWork) // 다음 날 오전 8시 기상
        {
            // 다음 날 오전 8시 - 현재 시간 값
            float nowTime = TimeStat - time;
            float remainTime = CalculateTimeToWakeUp(nowTime);
                
            TimeStat = _gameConstants.baseTime; // 아침 8시 기상
                
            // 체력 회복
            ModifyHealth(remainTime);
            
            // 일반 수면의 경우, 시간이 8시 이후일 때만 하루가 종료된 것으로 판단
            isDayEnded = nowTime >= 8.0f;
            
            // 회복량이 8 이하면 늦잠 이벤트 발동
            if (remainTime < _gameConstants.limitRecover)
            {
                Debug.Log($"수면이 8시간 미만입니다. 수면 시간: {remainTime}");
                Overslept?.Invoke(); // 늦잠 이벤트
            }
        }
        else if (actionType == ActionType.OverSlept) // 늦잠, 오전 8시에 행동을 결정하기에 하루 지남 X
        {
            // 다음 날 오후 3~6시 사이 기상, 추가 체력 회복
            float randomWakeUpTime = Random.Range(15, 19);
            TimeStat = randomWakeUpTime;
            
            // 추가 체력 회복
            float remainHealth = _gameConstants.limitRecover - previousAddHealth; // 체력 회복 총량 8 - 이전 회복 값 = 총 8회복
            ModifyHealth(remainHealth);
        } 
        else if (actionType == ActionType.ForcedSleep) // 탈진
        {
            // 오전 0~8시 사이에 잠들면 하루가 지나지 않은 것으로 처리
            float nowTime = TimeStat - time;
            bool isEarlyMorning = nowTime >= 0 && nowTime < 8;
            isDayEnded = !isEarlyMorning;
            
            // 다음 날 오후 3~6시 사이 기상
            float randomWakeUpTime = Random.Range(15, 19);
            TimeStat = randomWakeUpTime;
        }
        else // 수면 이외의 행동
        {
            isDayEnded = true;
            TimeStat -= _gameConstants.maxTime; 
        }
        
        // 하루가 실제로 종료된 경우에만 이벤트 발생
        if (isDayEnded)
        {
            // 식사 횟수 초기화
            _mealCount = 0;
            
            OnDayEnded?.Invoke();
        }
    }
    
    public float CalculateTimeToWakeUp(float timeStat)
    {
        float wakeUpTime = _gameConstants.wakeUpTime;
        if (timeStat < wakeUpTime) // 현재 시간이 0~7시 사이인 경우
        {
            // 당일 오전 8시까지 남은 시간
            return wakeUpTime - timeStat;
        }
        else
        {
            return ( wakeUpTime + 24f ) - timeStat; // 다음 날 오전 8시까지 남은 시간
        }
    }

    #region Modify Stats
    
    // 행동에 따른 내부 스탯 변경 메서드
    public void ModifyTime(float time, ActionType actionType)
    {
        TimeStat += time;
        
        if (TimeStat >= _gameConstants.maxTime)
        {
            EndDay(time, actionType);
        }

        if (TimeStat is >= 8.0f and < 9.0f)
        {
            _hasWorkedToday = false;
            _hasCheckedAbsenceToday = false;
            _hasShownBubbleToday = false;
        }
        
        CheckBubble();
    }
    
    public void ModifyHealth(float health)
    {
        previousAddHealth = health; // 이전 회복량 저장
        HealthStat += health;
        
        // 혹시 모를 음수 값 처리
        if (HealthStat <= 0)
        {
            HealthStat = 0.0f;
            // 현재는 0 되자마자 발생하도록 처리하였는데 다른 방식으로의 처리가 필요하다면 말씀해주십시오.
            // 동작 이후에 스탯을 깎는다는 기준하에 작성하였습니다. (동작 전에는 CanPerformByHealth()를 통해 행동 가능 여부 판단)
            
            // 탈진 이벤트 발생
            Debug.Log("탈진! 체력 0");
            Exhaustion?.Invoke();
        }

        if (HealthStat > MaxHealth)
        {
            HealthStat = MaxHealth;
        }
    }

    public void ModifyReputation(float reputation)
    {
        if (reputation == 0f) return;
        
        // float 연산 시 계산 오차가 발생할 수도 있기에 소수점 두 번째에서 반올림하도록 처리
        if(ReputationStat > 0)
        {
            ReputationStat = Mathf.Round((ReputationStat + reputation) * 100f) / 100f;
        }
        else
        {
            ReputationStat = 0f;
        }

        if (ReputationStat <= 0)
        {
            ZeroReputation?.Invoke();
            ReputationStat = 0.0f;
        }

        if (ReputationStat > _gameConstants.maxReputation)
        {
            ReputationStat = _gameConstants.maxReputation;
        }
    }
    
    #endregion

    public void ApplySaveData(Save save)
    {
        if (save?.homeSave != null)
        {
            TimeStat = Mathf.Clamp(save.homeSave.time, 0, _gameConstants.maxTime);
            HealthStat = Mathf.Clamp(save.homeSave.health, 0, _gameConstants.maxHealth);
            ReputationStat = Mathf.Clamp(save.homeSave.reputation, 0, _gameConstants.maxReputation);
            _mealCount = Mathf.Clamp(save.homeSave.mealCount, 0, 2);
            _hasCheckedAbsenceToday = save.homeSave.hasCheckedAbsenceToday;
            _hasShownBubbleToday = save.homeSave.hasShownBubbleToday;
            
            //UI적용
            OnStatsChanged?.Invoke(new StatsChangeData(TimeStat, HealthStat, ReputationStat));
        }
    }

    public Save ExtractSaveData()
    {
        return new Save
        {
            homeSave = new HomeSave
            {
                time = Mathf.Clamp(this.TimeStat,0,_gameConstants.maxTime),
                health = Mathf.Clamp(this.HealthStat,0,_gameConstants.maxHealth),
                reputation = Mathf.Clamp(this.ReputationStat,0,_gameConstants.maxReputation),
                mealCount = Mathf.Clamp(this._mealCount,0,2),
                
                hasCheckedAbsenceTodaySet = true,
                hasCheckedAbsenceToday = this._hasCheckedAbsenceToday,
                
                hasShownBubbleTodaySet = true,
                hasShownBubbleToday = this._hasShownBubbleToday,
            }
        };
    }
}
