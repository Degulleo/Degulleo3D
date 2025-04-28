//퇴근 후 발생할 수 있는 돌발 이벤트

using System.Collections.Generic;

public enum AfterWorkEvent
{
    None,
    TeamGathering,
    OvertimeWork
}

public static class HousingConstants
{
    //돌발 이벤트 확률 계산
    public static int AFTER_WORK_DENOMINATOR = 4;


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
            {ActionType.Eat, new InteractionTexts("식사를 하자","1시간 동안 체력 1을 회복한다.","밥 먹는 중")}
        };
    #endregion

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
}
