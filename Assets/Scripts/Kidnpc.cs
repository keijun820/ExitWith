using UnityEngine;

/// <summary>
/// 아이 NPC.
/// - 플레이어가 가까이 오면 "E - 대화하기" 안내 표시
/// - E를 누르면 대화 시작
/// - 첫 대화가 끝나면 능력(Tab, Q) 해금 + 풍선 퀘스트 시작
/// - 퀘스트 상태에 따라 다른 대사 출력
/// 거리로 판정하므로 트리거 콜라이더가 필요 없습니다.
/// </summary>
public class KidNPC : MonoBehaviour
{
    [Header("연결")]
    [SerializeField] PlayerController player;
    [Tooltip("\"E - 대화하기\" 같은 안내 UI. 비워둬도 됩니다.")]
    [SerializeField] GameObject talkPrompt;

    [Header("설정")]
    [SerializeField] string npcName = "아이";
    [SerializeField] float talkRange = 2.5f;
    [SerializeField] KeyCode talkKey = KeyCode.E;

    [Header("대사")]
    [SerializeField, TextArea(2, 4)]
    string[] firstLines =
    {
        "흑흑... 아저씨, 제 풍선이 날아가 버렸어요.",
        "저 혼자서는 찾을 수가 없어요...",
        "제 풍선 좀 찾아주실 수 있어요?"
    };

    [Tooltip("퀘스트 중인데 풍선 없이 말을 걸었을 때")]
    [SerializeField, TextArea(2, 4)]
    string[] needBalloonLines =
    {
        "제 풍선은요...? 풍선이 꼭 필요해요!",
        "빨간색 풍선이에요. 찾으면 꼭 가져다주세요."
    };

    [Tooltip("풍선을 가진 채로 말을 걸었을 때")]
    [SerializeField, TextArea(2, 4)]
    string[] giveBalloonLines =
    {
        "앗, 제 풍선이다!",
        "찾아주셔서 정말 고마워요!",
        "(이제 Tab과 Q를 사용할 수 있다.)"
    };

    [SerializeField, TextArea(2, 4)]
    string[] questCompletedLines =
    {
        "제 풍선을 찾아주셔서 정말 고마워요!"
    };

    void Start()
    {
        if (player == null) player = FindFirstObjectByType<PlayerController>();
        if (talkPrompt != null) talkPrompt.SetActive(false);
    }

    void Update()
    {
        bool canTalk = CanTalk();

        if (talkPrompt != null && talkPrompt.activeSelf != canTalk)
            talkPrompt.SetActive(canTalk);

        if (canTalk && Input.GetKeyDown(talkKey))
            Talk();
    }

    bool CanTalk()
    {
        if (player == null) return false;

        DialogueManager dm = DialogueManager.Instance;
        if (dm == null || dm.IsOpen || dm.JustClosed) return false;

        if (player.kidControlling) return false;

        float distance = Vector3.Distance(player.transform.position, transform.position);
        return distance <= talkRange;
    }

    void Talk()
    {
        GameProgress gp = GameProgress.Instance;
        DialogueManager dm = DialogueManager.Instance;

        switch (gp.BalloonQuest)
        {
            case QuestState.NotStarted:
                dm.StartDialogue(npcName, firstLines, OnFirstTalkFinished);
                break;
            case QuestState.InProgress:
                if (gp.HasBalloon)
                    dm.StartDialogue(npcName, giveBalloonLines, OnBalloonGiven);
                else
                    dm.StartDialogue(npcName, needBalloonLines);
                break;
            case QuestState.Completed:
                dm.StartDialogue(npcName, questCompletedLines);
                break;
        }
    }

    // 첫 대화가 끝나면: 퀘스트만 시작 (능력은 아직 잠김)
    void OnFirstTalkFinished()
    {
        GameProgress.Instance.StartQuest();
    }

    // 풍선을 건네는 대화가 끝나면: 퀘스트 클리어 + 능력(Tab, Q) 해금
    void OnBalloonGiven()
    {
        GameProgress gp = GameProgress.Instance;
        gp.GiveBalloon();
        gp.CompleteQuest();
        gp.UnlockAbilities();
    }

    // 씬 뷰에서 대화 가능 범위 표시
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, talkRange);
    }
}