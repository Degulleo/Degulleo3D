//퇴근 후 발생할 수 있는 돌발 이벤트

using System.Collections.Generic;


public enum AfterWorkEventType
{
    None,
    TeamGathering,
    OvertimeWork
}

public static class HousingConstants
{
    //돌발 이벤트 확률 계산
    public static int AFTER_WORK_DENOMINATOR = 2;
    //돌발 이벤트 보여줄 시간
    public static float SUDDENEVENT_IAMGE_SHOW_TIME = 4.0f;
    //전환효과(Switching) 패널 애니메이션 시간
    public static float SWITCH_PANEL_AINIM_DURATION = 2.0f;


    #region 상호작용 멘트

    public static readonly Dictionary<ActionType, InteractionTexts> interactions =
        new Dictionary<ActionType, InteractionTexts>
        {
            { ActionType.Sleep, new InteractionTexts("침대에서 잘까?","숙면으로 시간 당 체력 1을 회복한다.", 
                ".","쿨쿨 자는 중")},
            { ActionType.Housework, new InteractionTexts("밀린 집안일을 처리할까?","체력 1을 사용하고 좋은일이 일어날지도 모른다",
                "힘들어서 못해..","세탁하는 중")},
            { ActionType.Dungeon, new InteractionTexts("던전에 입장할까?","체력 3을 사용하고 3시간이 흐른다.",
                "던전에 갈 체력이 되지 않아..","던전 진입하는 중")},
            { ActionType.Work, new InteractionTexts("출근한다.","체력 3을 사용하고 저녁 6시에나 돌아오겠지..", 
                "도저히 출근할 체력이 안되는걸..?","출근하는 중")},
            { ActionType.Eat, new InteractionTexts("식사를 하자","1시간 동안 체력 1을 회복한다.","밥 먹는 중") }
        };

    public struct InteractionTexts
    {
        public string ActionText { get; private set; }
        public string DescriptionText { get; private set; }
        public string LackOfHealth { get; private set; }
        public string AnimationText { get; private set; }

        public InteractionTexts(string actionText, string descriptionText, string lackOfHealth, string animationText = "")
        {
            ActionText = actionText;
            DescriptionText = descriptionText;
            LackOfHealth = lackOfHealth;
            AnimationText = animationText;
        }
    }
    #endregion

    #region 돌발 이벤트

    public static readonly Dictionary<AfterWorkEventType, string> SuddenEventTexts =
        new Dictionary<AfterWorkEventType, string>
        {
            { AfterWorkEventType.OvertimeWork, "부장님이 퇴근을 안하셔.. 야근할까?" },
            { AfterWorkEventType.TeamGathering, "갑자기 팀 회식이 잡혔다. 참석 하러 가자"}
        };
    
    public struct SuddenEventTypes
    {
        public AfterWorkEventType AfterWorkEvent { get; private set; }
        public string AfterWorkEventText { get; private set; }

        public SuddenEventTypes(AfterWorkEventType afterWorkEvent, string afterWorkEventText)
        {
            AfterWorkEvent = afterWorkEvent;
            AfterWorkEventText = afterWorkEventText;
        }
    }

    #endregion
    
    // 랜덤 값에 일치하는 함수를 리턴하기 위한 딕셔너리
    // TODO: AFTER_WORK_DENOMINATOR 값 확정 후에 키 값 수정
    public static readonly Dictionary<int, AfterWorkEventType> AfterWorkEvents = new Dictionary<int, AfterWorkEventType> {
        {0, AfterWorkEventType.OvertimeWork},
        {1, AfterWorkEventType.TeamGathering}
    };
}
