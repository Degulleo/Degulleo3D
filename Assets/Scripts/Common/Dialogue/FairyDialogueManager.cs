using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

// 일반 클래스로 변경된 FairyDialogueManager
public class FairyDialogueManager
{
    private ChatWindowController _chatWindow;
    private string _dialogueFileName;
    
    // 현재 게임 단계
    private GamePhase _currentGamePhase = GamePhase.Intro;
    
    private DialogueData _database;
    private Dictionary<string, DialogueStruct> _dialogueDict;
    
    // 단계별 대화 모음
    private Dictionary<string, List<DialogueStruct>> _phaseDialogues;
    
    // 이미 보여준 게임플레이 대화 추적
    private HashSet<string> _shownGameplayDialogueIds;
    
    // 생성자 
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
            Debug.LogError($"Failed to load dialogue database: {filePath}");
            _database = new DialogueData { dialogues = new List<DialogueStruct>() };
            return;
        }
        
        Debug.Log($"JSON 파일 내용: {jsonFile.text.Substring(0, Mathf.Min(200, jsonFile.text.Length))}..."); // 파일 내용 확인
        
        try {
            _database = JsonUtility.FromJson<DialogueData>(jsonFile.text);
            Debug.Log($"대화 항목 수: {_database.dialogues.Count}");
        
            // 검증: 각 단계별 대화 항목 수 확인
            Dictionary<string, int> phaseCount = new Dictionary<string, int>();
            foreach (var dialogue in _database.dialogues)
            {
                if (!phaseCount.ContainsKey(dialogue.phase))
                    phaseCount[dialogue.phase] = 0;
                phaseCount[dialogue.phase]++;
            }
        
            foreach (var phase in phaseCount)
            {
                Debug.Log($"단계 '{phase.Key}': {phase.Value}개 항목");
            }
        
            // 대화 사전 초기화
            _dialogueDict = new Dictionary<string, DialogueStruct>();
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
    
    // 게임 단계 설정
    public void SetGamePhase(GamePhase phase)
    {
        _currentGamePhase = phase;
        
        // Intro 또는 End 단계로 진입 시 자동으로 해당 대화 시작
        if (phase == GamePhase.Intro)
        {
            StartPhaseDialogue("intro");
        }
        else if (phase == GamePhase.End)
        {
            StartPhaseDialogue("end");
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
        
        // 해당 단계의 첫 대화 항목 찾기 (요정이 먼저 말하는 대화)
        DialogueStruct startEntry = _phaseDialogues[phaseName]
            .FirstOrDefault(d => d.name == "냉장고 요정" && !string.IsNullOrEmpty(d.nextId));
        
        if (startEntry == null)
        {
            // 적합한 시작 대화가 없으면 첫 번째 대화 사용
            startEntry = _phaseDialogues[phaseName][0];
        }
        
        // 대화 시퀀스 시작
        StartDialogueSequence(startEntry.id);
    }
    
    // 랜덤 게임플레이 대화 보여주기
    public void ShowRandomGameplayDialogue()
    {
        if (_currentGamePhase != GamePhase.Gameplay)
        {
            Debug.LogWarning("Random dialogue can only be shown during gameplay phase");
            return;
        }
        
        // 게임플레이 단계의 요정 대화 중 아직 보여주지 않은 것 찾기
        var availableDialogues = _phaseDialogues["gameplay"]
            .Where(d => d.name == "냉장고 요정" && !_shownGameplayDialogueIds.Contains(d.id))
            .ToList();
        
        // 모든 대화를 다 보여줬다면 다시 초기화
        if (availableDialogues.Count == 0)
        {
            _shownGameplayDialogueIds.Clear();
            availableDialogues = _phaseDialogues["gameplay"]
                .Where(d => d.name == "냉장고 요정")
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
        if (!_dialogueDict.ContainsKey(startDialogueId))
        {
            Debug.LogError($"Dialogue ID not found: {startDialogueId}");
            return;
        }
        
        // 시작 대화 항목 가져오기
        DialogueStruct startEntry = _dialogueDict[startDialogueId];
        
        // 대화 시퀀스 구성
        List<DialogueStruct> sequence = BuildDialogueSequence(startEntry);
        
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
    
    // 요정에게 말 걸기 (게임 중 언제든 사용 가능)
    public void TalkToFairy()
    {
        switch (_currentGamePhase)
        {
            case GamePhase.Intro:
                StartPhaseDialogue("intro");
                break;
            case GamePhase.Gameplay:
                ShowRandomGameplayDialogue();
                break;
            case GamePhase.End:
                StartPhaseDialogue("end");
                break;
        }
    }
    
    // 특정 대화 ID로 대화 시작 (외부에서 호출 가능)
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
    
    // 완료 콜백 설정
    public void SetOnCompleteCallback(ChatWindowController.OnComplete callback)
    {
        _chatWindow.onComplete = callback;
    }
}