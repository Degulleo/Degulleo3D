using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    private Canvas _canvas;
    
    public void ChangeToGameScene()
    {
        SceneManager.LoadScene("Game"); // 던전 Scene
    }
    
    public void ChangeToMainScene()
    {
        SceneManager.LoadScene("Housing"); // Home Scene
    }
    
    // ToDo: Open Setting Panel 등 Panel 처리
    
    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ToDo: 씬 로드 시 동작 구현. ex: BGM 변경
        
        // UI용 Canvas 찾기
        // _canvas = GameObject.FindObjectOfType<Canvas>();
    }
    
    private void OnApplicationQuit()
    {
        // TODO: 게임 종료 시 로직 추가
    }
}
