/// <summary>
/// 세이브 데이터를 주고 받는 함수 인터페이스
/// </summary>
public interface ISaveable
{
    void ApplySaveData(Save save);
    Save ExtractSaveData();
}
