using System;
using UnityEngine;
using UnityEngine.Playables;

public class cutsceneManager : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;
    [SerializeField] private PlayerController playerController; // 플레이어 이동 스크립트
    [SerializeField] GameObject playerCamera;
    [SerializeField] GameObject kidCamera;
    [SerializeField] GameObject cinematicCamera;
    bool isPlayerCam;

    private void Awake()
    {
        director.stopped += OnCutsceneEnd;
    }

    public void PlayCutscene()
    {
        playerController.enabled = false; // 입력 잠금
        isPlayerCam = playerCamera.activeSelf;
        playerCamera.SetActive(false);
        kidCamera.SetActive(false);
        cinematicCamera.SetActive(true);
        director.Play();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L)) PlayCutscene();
    }

    private void OnCutsceneEnd(PlayableDirector d)
    {
        playerController.enabled = true; // 입력 복구
        playerCamera.SetActive(isPlayerCam);
        kidCamera.SetActive(!isPlayerCam);
        cinematicCamera.SetActive(false);
    }

    private void OnDestroy()
    {
        director.stopped -= OnCutsceneEnd;
    }
}