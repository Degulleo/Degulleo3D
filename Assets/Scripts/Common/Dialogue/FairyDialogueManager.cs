using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class FairyDialogueManager
{
    private ChatWindowController _chatWindow;
    private string _dialogueFileName;
    
    private GamePhase _currentGamePhase = GamePhase.Intro; // 현재 게임 단계
    
    private DialogueData _database;
    private Dictionary<string, DialogueStruct> _dialogueDict;
    private Dictionary<string, List<DialogueStruct>> _phaseDialogues;
    
    // 이미 보여준 게임플레이 대화 추적
    private HashSet<string> _shownGameplayDialogueIds;

    private string _fairyName = "냉장고 요정";
    
    public FairyDialogueManager(ChatWindowController chatWindow, string dialogueFileName = "dialogue.json")
    {
        _chatWindow = chatWindow;
        _dialogueFileName = dialogueFileName;
        _shownGameplayDialogueIds = new HashSet<string>();
        
        LoadDialogueData();
        OrganizeDialogues();
    }
    
    // 대화 데이터베이스 로드
    private void LoadDialogueData()
    {
        string filePath = "Dialogues/" + _dialogueFileName.Replace(".json", "");
        TextAsset jsonFile = Resources.Load<TextAsset>(filePath);
        
        if (jsonFile == null)
        {
            Debug.LogError($"경로 오류로 인한 대화 데이터 로딩 실패 : {filePath}");
            _database = new DialogueData { dialogues = new List<DialogueStruct>() };
            return;
        }
        
        try {
            _database = JsonUtility.FromJson<DialogueData>(jsonFile.text);
            
            _dialogueDict = new Dictionary<string, DialogueStruct>(); // 대화 사전 초기화
            foreach (DialogueStruct entry in _database.dialogues)
            {
                _dialogueDict[entry.id] = entry;
            }
        }
        catch (Exception e) {
            Debug.LogError($"JSON 파싱 오류: {e.Message}");
            _database = new DialogueData { dialogues = new List<DialogueStruct>() };
        }
    }
    
    // 대화를 단계별로 분류
    private void OrganizeDialogues()
    {
        _phaseDialogues = new Dictionary<string, List<DialogueStruct>>();
        
        foreach (DialogueStruct entry in _database.dialogues)
        {
            // 단계별 분류
            if (!_phaseDialogues.ContainsKey(entry.phase))
            {
                _phaseDialogues[entry.phase] = new List<DialogueStruct>();
            }
            _phaseDialogues[entry.phase].Add(entry);
        }
    }
    
    // 게임 단계 설정, GamePlay는 따로 실행
    public void SetGamePhase(GamePhase phase)
    {
        _currentGamePhase = phase;
        
        if (phase == GamePhase.Intro)
        {
            StartPhaseDialogue("intro");
        }
        else if (phase == GamePhase.End)
        {
            StartPhaseDialogue("end");
        } 
        else if (phase == GamePhase.ZeroEnd)
        {
            StartPhaseDialogue("zero");
        } 
        else if (phase == GamePhase.FailEnd)
        {
            StartPhaseDialogue("fail");
        } 
    }
    
    // 단계별 시작 대화 찾기 및 시작
    private void StartPhaseDialogue(string phaseName)
    {
        if (!_phaseDialogues.ContainsKey(phaseName) || _phaseDialogues[phaseName].Count == 0)
        {
            Debug.LogWarning($"No dialogues found for phase: {phaseName}");
            return;
        }
        
        // 첫 대화 찾기 (요정이 먼저 말함)
        DialogueStruct startEntry = _phaseDialogues[phaseName]
            .FirstOrDefault(d => d.name == _fairyName && !string.IsNullOrEmpty(d.nextId));
        
        if (startEntry == null)
        {
            startEntry = _phaseDialogues[phaseName][0];
        }
        
        // 대화 시퀀스 시작
        StartDialogueSequence(startEntry.id);
    }
    
    // Gameplay일 때 랜덤 대화 출력
    public void ShowRandomGameplayDialogue()
    {
        if (_currentGamePhase != GamePhase.Gameplay) return;
        
        // 요정 대화 중 아직 보여주지 않은 것 찾기
        var availableDialogues = _phaseDialogues["gameplay"]
            .Where(d => d.name == _fairyName && !_shownGameplayDialogueIds.Contains(d.id))
            .ToList();
        
        // 모든 대화를 다 보여줬다면 다시 초기화
        if (availableDialogues.Count == 0)
        {
            _shownGameplayDialogueIds.Clear();
            availableDialogues = _phaseDialogues["gameplay"]
                .Where(d => d.name == _fairyName)
                .ToList();
        }
        
        // 랜덤 대화 선택
        if (availableDialogues.Count > 0)
        {
            int randomIndex = Random.Range(0, availableDialogues.Count);
            DialogueStruct randomDialogue = availableDialogues[randomIndex];
            
            // 보여준 대화 기록
            _shownGameplayDialogueIds.Add(randomDialogue.id);
            
            // 대화 시퀀스 시작
            StartDialogueSequence(randomDialogue.id);
        }
    }
    
    // 대화 ID로 대화 시작
    public void StartDialogueSequence(string startDialogueId)
    {
        if (!_dialogueDict.ContainsKey(startDialogueId)) return;
        
        DialogueStruct startEntry = _dialogueDict[startDialogueId]; // 시작 대화
        
        List<DialogueStruct> sequence = BuildDialogueSequence(startEntry); // 대화 시퀀스 구성
        
        // 대화창에 대화 시퀀스 전달
        _chatWindow.SetDialogueSequence(sequence);
        _chatWindow.ShowWindow();
    }
    
    // 연결된 대화 시퀀스 구성
    private List<DialogueStruct> BuildDialogueSequence(DialogueStruct startEntry)
    {
        List<DialogueStruct> sequence = new List<DialogueStruct>();
        HashSet<string> visitedIds = new HashSet<string>(); // 순환 참조 방지
        
        DialogueStruct currentEntry = startEntry;
        while (currentEntry != null && !visitedIds.Contains(currentEntry.id))
        {
            sequence.Add(currentEntry);
            visitedIds.Add(currentEntry.id);
            
            // 다음 대화가 없으면 종료
            if (string.IsNullOrEmpty(currentEntry.nextId) || 
                !_dialogueDict.ContainsKey(currentEntry.nextId))
            {
                break;
            }
            
            // 다음 대화로 이동
            currentEntry = _dialogueDict[currentEntry.nextId];
        }
        
        return sequence;
    }
    
    // 특정 대화 ID로 대화 시작 (세이브/로드 시 사용)
    public void StartDialogueById(string dialogueId)
    {
        if (_dialogueDict.ContainsKey(dialogueId))
        {
            StartDialogueSequence(dialogueId);
        }
        else
        {
            Debug.LogError($"Dialogue ID not found: {dialogueId}");
        }
    }
}