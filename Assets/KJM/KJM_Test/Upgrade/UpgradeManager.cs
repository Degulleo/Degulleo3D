using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum StatType
{
    AttackPower = 1,
    AttackSpeed,
    MoveSpeed,
    DashCoolDown,
    Heart,
    Max
}

public class UpgradeManager : Singleton<UpgradeManager>
{
    Canvas canvas;
    
    public GameObject backgroundPanel;
    public Button upgradeButton;
    
    public UpgradeStat upgradeStat;
    
    private readonly List<int> statNumbers = new List<int> { 1, 2, 3, 4, 5 };
    private List<int> stats = new List<int>();
    
    private RectTransform backgroundRectTransform;
    private List<Button> cards = new List<Button>();
    
    private bool isHome = false;

    public void Start()
    {
        if(canvas == null)
            canvas = FindObjectOfType<Canvas>();
    }
    
    public void StartUpgrade()
    {
        DrawStatNumber();
        
        //배경 패널 생성
        if(backgroundRectTransform == null)
            backgroundRectTransform = Instantiate(backgroundPanel,canvas.transform).GetComponent<RectTransform>(); 
        //배경 패널 애니메이션 적용
        backgroundRectTransform.gameObject.SetActive(true);
        StartCoroutine(CoFade(backgroundRectTransform.gameObject, 0f,0.7f,0.2f));

        EnsureCardListSize(3);
        
        //카드 생성
        if (stats.Count == 0)
        {
            //모든 강화가 MAX일때
            if(cards[0] == null)
                cards[0] = Instantiate(upgradeButton, backgroundRectTransform);
            
            cards[0].gameObject.SetActive(true);
            cards[0].GetComponent<UpgradeCard>().Init(StatType.Max);
            StartCoroutine(CoFade(cards[0].gameObject, 0f,1f,0.4f, () =>
            {
                cards[0].GetComponent<CanvasGroup>().interactable = true;
            }));
        }
        else
        {
            for (int i = 0; i < Mathf.Min(stats.Count, 3); i++)
            {
                if(cards[i] == null)
                    cards[i] = Instantiate(upgradeButton, backgroundRectTransform);
            
                cards[i].gameObject.SetActive(true);
                cards[i].GetComponent<UpgradeCard>().Init((StatType)stats[i]);
            
                var index = i;
                StartCoroutine(CoFade(cards[i].gameObject, 0f,1f,0.4f, () =>
                {
                    cards[index].GetComponent<CanvasGroup>().interactable = true;
                }));
            }
        }
    }
    
    /// <summary>
    /// 랜덤한 강화 카드 1장
    /// </summary>
    public void StartUpgradeInHome()
    {
        isHome = true;
        
        DrawStatNumber();
        
        //배경 패널 생성
        if(backgroundRectTransform == null)
            backgroundRectTransform = Instantiate(backgroundPanel,canvas.transform).GetComponent<RectTransform>(); 
        //배경 패널 애니메이션 적용
        backgroundRectTransform.gameObject.SetActive(true);
        StartCoroutine(CoFade(backgroundRectTransform.gameObject, 0f,0.7f,0.2f));

        EnsureCardListSize(3);
        
        if(cards[0] == null)
            cards[0] = Instantiate(upgradeButton, backgroundRectTransform);
            
        cards[0].gameObject.SetActive(true);
        cards[0].GetComponent<UpgradeCard>().Init((StatType)stats[0]);
        StartCoroutine(CoFade(cards[0].gameObject, 0f,1f,0.4f, () =>
        {
            cards[0].GetComponent<CanvasGroup>().interactable = true;
        }));
        
    }
    
    
    /// <summary>
    /// 중복되지 않는 랜덤한 스탯 번호 뽑기
    /// </summary>
    private void DrawStatNumber()
    {
        stats.Clear();

        //번호 셔플
        for (int i = 0; i < statNumbers.Count; i++)
        {
            int randIndex = Random.Range(i, statNumbers.Count);
            (statNumbers[i], statNumbers[randIndex]) = (statNumbers[randIndex], statNumbers[i]);
        }
        
        //이미 강화 수치가 MAX이면 제외
        foreach (var t in statNumbers)
        {
            if (upgradeStat.IsMax((StatType)t))
            {
                continue;
            }
            else
            {
                stats.Add(t);
            }
        }
    }
    
    /// <summary>
    /// 리스트 사이즈 보장
    /// </summary>
    /// <param name="index"></param>
    private void EnsureCardListSize(int index)
    {
        while (cards.Count <= index)
        {
            cards.Add(null);
        }
    }
    
    /// <summary>
    /// 업그레이드 창 비활성화
    /// </summary>
    public void DestroyUpgradeCard()
    {
        // 카드 비활성화
        if (stats.Count == 0|| isHome)
        {
            StartCoroutine(CoFade(cards[0].gameObject, 1f,0f,0.4f,() =>
            {
                cards[0].gameObject.SetActive(false);
                backgroundRectTransform.gameObject.SetActive(false);
            }));
            
            isHome = false;
        }
        else
        {
            for (int i = 0; i < Mathf.Min(stats.Count, 3); i++)
            {
                var index = i;
                StartCoroutine(CoFade(cards[i].gameObject, 1f,0f,0.4f,() =>
                {
                    cards[index].gameObject.SetActive(false);
                    if (index == Mathf.Min(stats.Count, 3) - 1)
                    {
                        backgroundRectTransform.gameObject.SetActive(false);
                    }
                }));
            }
        }
        
        //배경 패널 비활성화
        StartCoroutine(CoFade(backgroundRectTransform.gameObject, 0.7f,0f,0.2f));
        
        if (SceneManager.GetActiveScene().name == "ReDungeon")
        {
            StartCoroutine(DelayedSceneChange()); // n초 대기 후 전환
        }
    }
    
    //씬 전환 대기 코루틴
    private IEnumerator DelayedSceneChange()
    {
        yield return new WaitForSeconds(2f);
        GameManager.Instance.ChangeToHomeScene();
    }
    
    
    /// <summary>
    /// 페이드 애니메이션 코루틴
    /// </summary>
    /// <param name="target"></param>
    /// <param name="fromAlpha"></param>
    /// <param name="toAlpha"></param>
    /// <param name="duration"></param>
    /// <param name="onComplete"></param>
    /// <returns></returns>
    public IEnumerator CoFade(GameObject target, float fromAlpha, float toAlpha, float duration,System.Action onComplete = null)
    {
        float elapsedTime = 0f;

        CanvasGroup canvasGroup = target.GetComponent<CanvasGroup>();
        Image image = target.GetComponent<Image>();

        if (canvasGroup == null && image == null)
        {
            Debug.LogError("CanvasGroup도 Image도 없습니다");
            yield break;
        }

        // 초기 설정
        if (canvasGroup != null)
        {
            canvasGroup.alpha = fromAlpha;
            canvasGroup.interactable = false;
        }
        else if (image != null)
        {
            Color color = image.color;
            color.a = fromAlpha;
            image.color = color;
        }

        while (elapsedTime < duration)
        {
            //float alpha = Mathf.Lerp(fromAlpha, toAlpha, elapsedTime / duration);
            float t = Mathf.Clamp01(elapsedTime / duration);
            float alpha = Mathf.Lerp(fromAlpha, toAlpha, t);

            if (canvasGroup != null)
            {
                canvasGroup.alpha = alpha;
            }
            else if (image != null)
            {
                Color color = image.color;
                color.a = alpha;
                image.color = color;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 최종 보정 (정확한 값으로 마무리)
        if (canvasGroup != null)
        {
            canvasGroup.alpha = toAlpha;
        }
        else if (image != null)
        {
            Color color = image.color;
            color.a = toAlpha;
            image.color = color;
        }
        onComplete?.Invoke();
    }
    
    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(canvas == null)
            canvas = FindObjectOfType<Canvas>();
    }
    
}
