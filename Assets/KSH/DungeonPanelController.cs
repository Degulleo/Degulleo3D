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
        if (_countHealth > _playerHealthImages.Length - 1) return false;
        
        _playerHealthImages[_countHealth].color = Color.black;
        _countHealth++;
        return _countHealth <= _playerHealthImages.Length - 1;
    }
    
    IEnumerator WaitForOneSecond()
    {
        yield return new WaitForSeconds(1.0f);
    }
}
