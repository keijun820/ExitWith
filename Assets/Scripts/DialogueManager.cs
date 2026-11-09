using System;
using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// 화면(Game 뷰)에 대화창을 띄웁니다.
/// - E를 누르면 다음 대사로 넘어갑니다.
/// - 글자가 타이핑되는 중에 E를 누르면 문장이 바로 전부 표시됩니다.
/// Canvas 아래의 오브젝트(예: "DialogueManager")에 붙이세요.
/// </summary>
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI 연결")]
    [SerializeField] GameObject dialoguePanel;   // 대화창 배경 패널
    [SerializeField] TMP_Text speakerText;       // 말하는 사람 이름
    [SerializeField] TMP_Text bodyText;          // 대사 내용
    [SerializeField] GameObject nextHint;        // "E ▶" 같은 안내 (선택)

    [Header("설정")]
    [SerializeField] KeyCode advanceKey = KeyCode.E;
    [Tooltip("초당 표시할 글자 수. 0이면 한 번에 표시합니다.")]
    [SerializeField] float charsPerSecond = 30f;

    public bool IsOpen { get; private set; }

    /// <summary>이번 프레임에 대화창이 닫혔는지. (닫자마자 같은 E 입력으로 다시 열리는 것 방지)</summary>
    public bool JustClosed => Time.frameCount == closedFrame;

    string[] lines;
    int index;
    Action onFinished;
    Coroutine typingRoutine;
    bool isTyping;
    int openedFrame = -1;
    int closedFrame = -1;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }

    public void StartDialogue(string speaker, string[] newLines, Action finished = null)
    {
        if (IsOpen || newLines == null || newLines.Length == 0) return;

        lines = newLines;
        index = 0;
        onFinished = finished;
        IsOpen = true;
        openedFrame = Time.frameCount;

        if (speakerText != null) speakerText.text = speaker;
        if (dialoguePanel != null) dialoguePanel.SetActive(true);

        ShowLine();
    }

    void Update()
    {
        // 대화를 시작한 그 프레임의 E 입력으로 바로 넘어가지 않도록 건너뜀
        if (!IsOpen || Time.frameCount == openedFrame) return;

        if (Input.GetKeyDown(advanceKey))
        {
            if (isTyping)
            {
                FinishTyping();
                return;
            }

            index++;
            if (index < lines.Length) ShowLine();
            else EndDialogue();
        }
    }

    void ShowLine()
    {
        if (typingRoutine != null) StopCoroutine(typingRoutine);

        if (charsPerSecond <= 0f)
        {
            bodyText.text = lines[index];
            bodyText.maxVisibleCharacters = int.MaxValue;
            isTyping = false;
            if (nextHint != null) nextHint.SetActive(true);
        }
        else
        {
            typingRoutine = StartCoroutine(TypeLine(lines[index]));
        }
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        if (nextHint != null) nextHint.SetActive(false);

        bodyText.text = line;
        bodyText.maxVisibleCharacters = 0;
        bodyText.ForceMeshUpdate();
        int total = bodyText.textInfo.characterCount;

        float delay = 1f / charsPerSecond;
        for (int i = 1; i <= total; i++)
        {
            bodyText.maxVisibleCharacters = i;
            yield return new WaitForSeconds(delay);
        }

        FinishTyping();
    }

    void FinishTyping()
    {
        if (typingRoutine != null) StopCoroutine(typingRoutine);
        typingRoutine = null;
        bodyText.maxVisibleCharacters = int.MaxValue;
        isTyping = false;
        if (nextHint != null) nextHint.SetActive(true);
    }

    void EndDialogue()
    {
        IsOpen = false;
        closedFrame = Time.frameCount;
        if (dialoguePanel != null) dialoguePanel.SetActive(false);

        Action callback = onFinished;
        onFinished = null;
        callback?.Invoke();
    }
}