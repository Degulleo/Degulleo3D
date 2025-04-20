using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackController : MonoBehaviour
{
    [Header("각종 전조 오브젝트")]
    [SerializeField] private GameObject verticalWarningArea;
    [SerializeField] private GameObject horizontalWarningArea;
    [SerializeField] private GameObject chariotWarningArea;
    [SerializeField] private GameObject dynamoWarningArea;

    // 배열에 담아서 관리
    private List<GameObject> warningAreas;
    private GameObject _activeArea;

    private void Awake()
    {
        // Awake 시점에 리스트로 묶어두면 이후 유지보수 편리
        warningAreas = new List<GameObject>()
        {
            verticalWarningArea,
            horizontalWarningArea,
            chariotWarningArea,
            dynamoWarningArea
        };
    }

    // 랜덤 전조 호출
    // 랜덤 전조 호출
    public void TriggerRandomWarning(Vector3 spawnPosition, Quaternion spawnRotation)
    {
        // 0 ~ Count-1 사이 랜덤 인덱스
        int idx = Random.Range(0, warningAreas.Count);
        GameObject selected = warningAreas[idx];

        // 예시: Instantiate 방식으로 화면에 띄우기
        _activeArea = Instantiate(selected, spawnPosition, spawnRotation);
    }

    public void DestroyWarningArea()
    {
        Destroy(_activeArea);
    }
}