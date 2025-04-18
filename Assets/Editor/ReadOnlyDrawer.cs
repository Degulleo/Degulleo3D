#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

// PlayerStatsTest.ReadOnlyAttribute를 위한 에디터 속성 드로어
[CustomPropertyDrawer(typeof(PlayerStatsTest.ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // 이전 GUI 활성화 상태 저장
        bool wasEnabled = GUI.enabled;
        
        // 필드 비활성화 (읽기 전용)
        GUI.enabled = false;
        
        // 속성 그리기
        EditorGUI.PropertyField(position, property, label, true);
        
        // GUI 활성화 상태 복원
        GUI.enabled = wasEnabled;
    }
}
#endif