using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerController))]
public class PlayerControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 기본 인스펙터를 그리기
        base.OnInspectorGUI();
        
        // 타겟 컴포넌트 참조 가져오기
        PlayerController playerController = (PlayerController)target;
        
        // 여백 추가
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("상태 디버그 정보", EditorStyles.boldLabel);


        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("현재 상태", playerController.CurrentState.ToString(),
            EditorStyles.boldLabel);
        EditorGUILayout.EndVertical();

        
        // 지면 접촉 상태
        GUI.backgroundColor = Color.white;
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("캐릭터 디버그 정보", EditorStyles.boldLabel);
        // GUI.enabled = false;
        // EditorGUILayout.Toggle("지면 접촉", playerController.IsGrounded);
        // GUI.enabled = true;
        
        // 강제로 상태 변경 버튼
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("BattleMode"))
            playerController.SwitchBattleMode();
        // if (GUILayout.Button("Attack"))
        //     playerController.SetState(PlayerState.Attack);
        // if (GUILayout.Button("Hit"))
        //     playerController.SetState(PlayerState.Hit);
        // if (GUILayout.Button("Dead"))
        //     playerController.SetState(PlayerState.Dead);
        
        EditorGUILayout.EndHorizontal();
    }

    private void OnEnable()
    {
        EditorApplication.update += OnEditorUpdate;
    }

    private void OnDisable()
    {
        EditorApplication.update -= OnEditorUpdate;
    }

    private void OnEditorUpdate()
    {
        if (target != null)
            Repaint();
    }
}
