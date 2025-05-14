using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonPanelController : MonoBehaviour
{
    [SerializeField] private Slider _bossHealthBar; // 0~1 value
    [SerializeField] private Image[] _playerHealthImages; // color 값 white / black 로 조정
    private int _countHealth = 0;
    private int visibleHeartCount = 3; // 강화 레벨로 설정됨

    //PC 키보드 입력 시 버튼 색상 변경
    [SerializeField] private GameObject dashPressedImage;
    [SerializeField] private GameObject attackPressedImage;
    private float pressedTime = .1f;
    
    private void Start()
    {
        int level = UpgradeManager.Instance.upgradeStat.CurrentUpgradeLevel(StatType.Heart); // 1~3

        visibleHeartCount = 3 + (level - 1); // level 1=3개, 2=4개, 3=5개

        for (int i = 0; i < _playerHealthImages.Length; i++)
        {
            var color = _playerHealthImages[i].color;
            color.a = (i < visibleHeartCount) ? 1f : 0f;
            color = (i < visibleHeartCount) ? Color.white : new Color(1,1,1,0);
            _playerHealthImages[i].color = color;
        }

        _countHealth = 0;
    }

    public void SetBossHealthBar(float hp) // hp: 0~300
    {
        float normalizedHp = hp / 300f; // 0~1 사이 값으로 조정
        _bossHealthBar.value = normalizedHp;
    }

    // false 반환 시 사망 처리
    public bool SetPlayerHealth()
    {
        StartCoroutine(WaitForOneSecond());
        // out of index error 방지
        if (_countHealth >= visibleHeartCount) return false;
        
        _playerHealthImages[visibleHeartCount-(_countHealth+1)].color = Color.black;
        _countHealth++;
        return _countHealth < visibleHeartCount;
    }
    
    IEnumerator WaitForOneSecond()
    {
        yield return new WaitForSeconds(1.0f);
    }
    
    
    #region PC입력 동안 버튼 색상 변경

    public void DashTouchMotion()
    {
        StopCoroutine(DashButtonColorChange());
        StartCoroutine(DashButtonColorChange());
    }
    
    public void AttackTouchMotion()
    {
        StopCoroutine(AttackButtonColorChange());
        StartCoroutine(AttackButtonColorChange());
    }
    
    private IEnumerator DashButtonColorChange()
    {
        dashPressedImage.SetActive(true);
        yield return new WaitForSeconds(pressedTime);
        dashPressedImage.SetActive(false);
    
    }
    private IEnumerator AttackButtonColorChange()
    {
        attackPressedImage.SetActive(true);
        yield return new WaitForSeconds(pressedTime);
        attackPressedImage.SetActive(false);
    }
    
    
    #endregion
}
