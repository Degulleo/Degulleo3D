using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    private Canvas _canvas;
    private Dictionary<string, GameObject> _panels = new Dictionary<string, GameObject>();

    private void Awake()
    {
        if (_canvas == null)
        {
            _canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        }
        LoadPanels();
    }
    
    //패널을 딕셔너리에 오브젝트 이름으로 로드
    private void LoadPanels()
    {
        GameObject[] prefabs = Resources.LoadAll<GameObject>("Prefabs/Panels");

        foreach (GameObject prefab in prefabs)
        {
            _panels[prefab.name] = prefab;
        }
    }
    //패널 이름을 딕셔너리 키로 찾음
    public GameObject GetPanel(string panelName)
    {
        if (_panels.TryGetValue(panelName, out GameObject prefab))
        {
            return Instantiate(prefab, _canvas.transform);
        }
        else
        {
            Debug.LogError($"패널 '{panelName}'을 찾을 수 없습니다.");
        }
        return null;
    }
    
}
