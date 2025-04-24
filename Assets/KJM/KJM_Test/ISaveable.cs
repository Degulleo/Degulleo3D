/// <summary>
/// 세이브 데이터를 주고 받는 함수 인터페이스
/// </summary>
public interface ISaveable
{
    void ApplySaveData(Save save);
    Save ExtractSaveData();
}


// 인터페이스 사용예시
//
//     public void ApplySaveData(Save save)
//     {
//         if (save?.homeSave != null)
//         {
//             mealCount = save.homeSave.mealCount;
//         }
//     }
//
//     public Save ExtractSaveData()
//     {
//         // 자신이 책임지는 부분만 채움, 나머지는 null로 둠
//         return new Save
//         {
//             homeSave = new HomeSave
//             {
//                 mealCount = mealCount
//             }
//         };
//     }
