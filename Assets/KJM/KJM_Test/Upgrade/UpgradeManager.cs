using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum StatType
{
    AttackPower = 1,
    AttackSpeed,
    MoveSpeed,
    DashCoolDown,
    Heart
}

public class UpgradeManager : Singleton<UpgradeManager>
{
    //현재 캔버스 찾는 함수 설정
    public Canvas canvas;
    
    public GameObject backgroundPanel;
    public Button upgradeButton;
    
    public UpgradeStat upgradeStat;
    
    private readonly List<int> statNumbers = new List<int> { 1, 2, 3, 4, 5 };
        
    public void GenerateUpgradeCard()
    {
        //카드 번호 셔플
        ShuffleStatNumber();
        
        //강화 수치가 맥스인 항목은 제외하는 로직

        //배경 패널 생성 애니메이션
       RectTransform backgroundRectTransform = Instantiate(backgroundPanel,canvas.transform).GetComponent<RectTransform>();
       
       var card1 = Instantiate(upgradeButton, backgroundRectTransform);
       card1.GetComponent<UpgradeCard>().Init((StatType)statNumbers[0]);
       
       var card2 = Instantiate(upgradeButton, backgroundRectTransform);
       card2.GetComponent<UpgradeCard>().Init((StatType)statNumbers[1]);
       
       var card3 = Instantiate(upgradeButton, backgroundRectTransform);
       card3.GetComponent<UpgradeCard>().Init((StatType)statNumbers[2]);
       
    }

    // 리스트 셔플
    private void ShuffleStatNumber()
    {
        for (int i = 0; i < statNumbers.Count; i++)
        {
            int randIndex = Random.Range(i, statNumbers.Count);
            (statNumbers[i], statNumbers[randIndex]) = (statNumbers[randIndex], statNumbers[i]);
        }
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
    }
}
