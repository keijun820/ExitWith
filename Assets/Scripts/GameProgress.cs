using System;
using UnityEngine;
using TMPro;

public enum QuestState { NotStarted, InProgress, Completed }

/// <summary>
/// 게임 진행 상태를 관리합니다.
/// - 능력(Tab, Q) 해금 여부
/// - 풍선 퀘스트 상태
/// - 플레이어가 풍선을 들고 있는지
/// 씬에 빈 오브젝트 하나를 만들어 붙여두세요. (예: "GameManager")
/// </summary>
public class GameProgress : MonoBehaviour
{
    public static GameProgress Instance { get; private set; }

    [Header("퀘스트 정보")]
    [SerializeField] string questTitle = "풍선 찾기";
    [SerializeField] string findBalloonText = "아이가 잃어버린 풍선을 찾자.";
    [SerializeField] string returnBalloonText = "풍선을 아이에게 가져다주자. (E로 대화)";

    [Header("퀘스트 표시 UI (선택)")]
    [Tooltip("화면 구석에 퀘스트를 표시할 TextMeshPro 텍스트. 비워둬도 됩니다.")]
    [SerializeField] TMP_Text questText;

    public bool AbilitiesUnlocked { get; private set; }
    public QuestState BalloonQuest { get; private set; } = QuestState.NotStarted;
    public bool HasBalloon { get; private set; }

    public event Action OnAbilitiesUnlocked;
    public event Action<QuestState> OnQuestStateChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        RefreshQuestUI();
    }

    public void UnlockAbilities()
    {
        if (AbilitiesUnlocked) return;
        AbilitiesUnlocked = true;
        Debug.Log("능력 해금: 이제 Tab, Q를 사용할 수 있습니다.");
        OnAbilitiesUnlocked?.Invoke();
    }

    public void StartQuest()
    {
        if (BalloonQuest != QuestState.NotStarted) return;
        SetQuestState(QuestState.InProgress);
        Debug.Log($"퀘스트 수락: {questTitle}");
    }

    /// <summary>풍선을 주웠을 때 호출</summary>
    public void PickUpBalloon()
    {
        if (HasBalloon) return;
        HasBalloon = true;
        Debug.Log("풍선을 획득했습니다. 아이에게 가져다주세요.");
        RefreshQuestUI();
    }

    /// <summary>아이에게 풍선을 건넸을 때 호출</summary>
    public void GiveBalloon()
    {
        if (!HasBalloon) return;
        HasBalloon = false;
        Debug.Log("아이에게 풍선을 건넸습니다.");
        RefreshQuestUI();
    }

    public void CompleteQuest()
    {
        if (BalloonQuest != QuestState.InProgress) return;
        SetQuestState(QuestState.Completed);
        Debug.Log($"퀘스트 클리어: {questTitle}");
    }

    void SetQuestState(QuestState state)
    {
        BalloonQuest = state;
        RefreshQuestUI();
        OnQuestStateChanged?.Invoke(state);
    }

    void RefreshQuestUI()
    {
        if (questText == null) return;

        switch (BalloonQuest)
        {
            case QuestState.NotStarted:
                questText.text = "";
                break;
            case QuestState.InProgress:
                string goal = HasBalloon ? returnBalloonText : findBalloonText;
                questText.text = $"[퀘스트] {questTitle}\n- {goal}";
                break;
            case QuestState.Completed:
                questText.text = $"[퀘스트 완료] {questTitle}";
                break;
        }
    }
}