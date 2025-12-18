using UnityEngine;
using System.Collections;

public class StageController : MonoBehaviour
{
    [Header("References")]
    public TextDisplayManager textManager;
    public SeedController seed;
    public TreeGrowthController treeGrowth;
    public AudioSource bgmAudioSource;
    public EnvironmentManager environmentManager; // 추가

    [Header("Environment")]
    public GameObject darkBackground;
    public GameObject meadowScene;

    [Header("Audio")]
    public AudioClip calmMusicClip; // 잔잔한 음악

    [Header("Stage Control")]
    public KeyCode nextStageKey = KeyCode.Space;

    [Header("Debug Settings")]
    [Tooltip("디버그용: 시작할 단계 번호 (0 = Stage 1부터 시작)")]
    public int startFromStage = 0;

    [Header("Tree Prefabs")]
    public GameObject sproutPrefab;
    public GameObject smallTreePrefab;
    public GameObject mediumTreePrefab;
    public GameObject largeTreePrefab;
    public GameObject hugeTreePrefab; // Stage 8용 - 매우 큰 나무

    private int currentStage = 0;
    private bool canProgress = false;

    void Start()
    {
        if (startFromStage == 0)
        {
            StartStage1();
        }
        else
        {
            // 디버그: 특정 단계부터 시작
            JumpToStage(startFromStage);
        }
    }

    void Update()
    {
        // 스페이스바로 다음 단계 진행
        if (canProgress && Input.GetKeyDown(nextStageKey))
        {
            canProgress = false;
            currentStage++;
            ExecuteStage(currentStage);
        }
    }

    // 디버그: 특정 단계로 점프
    void JumpToStage(int stageNumber)
    {
        Debug.Log($"[DEBUG] Stage {stageNumber}부터 시작");
        currentStage = stageNumber;

        // 해당 단계 이전의 필수 초기화 작업
        PrepareForStage(stageNumber);

        // 해당 단계 실행
        ExecuteStage(stageNumber);
    }

    // 특정 단계 실행 전 필요한 초기화
    void PrepareForStage(int stageNumber)
    {
        // Stage 4 이상: 씨앗은 이미 심어진 상태
        if (stageNumber >= 6)
        {
            if (seed != null)
            {
                seed.gameObject.SetActive(false);
            }
        }

        // Stage 5 이상: 새싹이 이미 자란 상태
        if (stageNumber >= 8 && stageNumber < 10)
        {
            if (sproutPrefab != null)
            {
                StartCoroutine(treeGrowth.GrowTree(sproutPrefab, treeGrowth.transform.position, 1f, 1f, TreeGrowthController.GrowthAnchor.Bottom));
            }
        }

        // Stage 6 이상: 작은 나무
        if (stageNumber >= 10 && stageNumber < 12)
        {
            if (smallTreePrefab != null)
            {
                StartCoroutine(treeGrowth.GrowTree(smallTreePrefab, treeGrowth.transform.position, 1.2f, 1.2f, TreeGrowthController.GrowthAnchor.Bottom));
            }
        }

        // Stage 7 이상: 중간 나무
        if (stageNumber >= 12 && stageNumber < 14)
        {
            if (mediumTreePrefab != null)
            {
                StartCoroutine(treeGrowth.GrowTree(mediumTreePrefab, treeGrowth.transform.position, 1.8f, 1.8f, TreeGrowthController.GrowthAnchor.Bottom));
            }
        }

        // Stage 8 이상: 큰 나무
        if (stageNumber >= 14)
        {
            if (hugeTreePrefab != null)
            {
                StartCoroutine(treeGrowth.GrowTree(hugeTreePrefab, treeGrowth.transform.position, 4.0f, 4.0f, TreeGrowthController.GrowthAnchor.Bottom));
            }

            // 초원 활성화
            if (meadowScene != null)
            {
                meadowScene.SetActive(true);
            }
            if (darkBackground != null)
            {
                darkBackground.SetActive(false);
            }

            // 음악 재생
            if (bgmAudioSource != null && calmMusicClip != null && !bgmAudioSource.isPlaying)
            {
                bgmAudioSource.clip = calmMusicClip;
                bgmAudioSource.loop = true;
                bgmAudioSource.volume = 0.3f;
                bgmAudioSource.Play();
            }
        }
    }

    void ExecuteStage(int stage)
    {
        switch (stage)
        {
            case 1: StartCoroutine(StartStage1_5()); break;
            case 2: StartCoroutine(StartStage2()); break;
            case 3: StartCoroutine(StartStage2_5()); break;
            case 4: StartCoroutine(StartStage3()); break;
            case 5: StartCoroutine(StartStage3_5()); break;
            case 6: StartCoroutine(StartStage4()); break;
            case 7: StartCoroutine(StartStage4_5()); break;
            case 8: StartCoroutine(StartStage5()); break;
            case 9: StartCoroutine(StartStage5_5()); break;
            case 10: StartCoroutine(StartStage6()); break;
            case 11: StartCoroutine(StartStage6_5()); break;
            case 12: StartCoroutine(StartStage7()); break;
            case 13: StartCoroutine(StartStage7_5()); break;
            case 14: StartCoroutine(StartStage8()); break;
            case 15: StartCoroutine(StartStage8_5()); break;
            case 16: StartCoroutine(StartStage9()); break;
        }
    }

    void StartStage1()
    {
        Debug.Log("Stage 1 시작");
        StartCoroutine(Stage1Sequence());
    }

    IEnumerator Stage1Sequence()
    {
        yield return new WaitForSeconds(3f);
        textManager.ShowText("지금 당신의 마음속에 자라나서\n당신을 괴롭게 하는 고민이 무엇인가요?");
        yield return new WaitForSeconds(5f);
        canProgress = true;
        Debug.Log("Space를 눌러 다음 단계로");
    }

    IEnumerator StartStage1_5()
    {
        Debug.Log("Stage 1.5 시작");
        textManager.HideText();
        yield return new WaitForSeconds(1f);
        canProgress = true;
    }

    IEnumerator StartStage2()
    {
        Debug.Log("Stage 2 시작 - 씨앗 빛남");
        StartCoroutine(seed.Glow());
        yield return new WaitForSeconds(0.5f);
        textManager.ShowText("네, 좋습니다.\n앞에 있는 희망의 씨앗을 땅에 심어볼까요?");
        yield return new WaitForSeconds(4f);
        canProgress = true;
        Debug.Log("Space를 눌러 다음 단계로");
    }

    IEnumerator StartStage2_5()
    {
        Debug.Log("Stage 2.5 시작 - 씨앗 심기");
        yield return StartCoroutine(seed.PlantSeed());
        yield return new WaitForSeconds(1f);
        canProgress = true;
        Debug.Log("Stage 3 준비 완료 - Space 누르기");
    }

    IEnumerator StartStage3()
    {
        Debug.Log("Stage 3 시작");
        yield return new WaitForSeconds(2f);
        textManager.ShowText("좋아요.\n내가 잘하고 있는지 불안해졌던 순간에 대해\n조금 더 말해볼까요?");
        yield return new WaitForSeconds(5f);
        canProgress = true;
        Debug.Log("Space를 눌러 다음 단계로");
    }

    IEnumerator StartStage3_5()
    {
        Debug.Log("Stage 3.5 시작 - 텍스트 사라짐");
        textManager.HideText();
        yield return new WaitForSeconds(1f);
        canProgress = true;
        Debug.Log("Stage 4 준비 완료");
    }

    IEnumerator StartStage4()
    {
        Debug.Log("Stage 4 시작 - 새싹 등장");

        // Tree 오브젝트의 실제 위치 사용
        Vector3 groundPos = treeGrowth.transform.position;
        yield return StartCoroutine(treeGrowth.GrowTree(sproutPrefab, groundPos, 1f));

        yield return new WaitForSeconds(0.5f);

        //textManager.ShowText("그 순간, 내가 잘하고 있는지 불안할 때\n어떤 기분이 들었나요? 자세히 떠올려봐도 괜찮아요.");
        textManager.ShowText("그랬군요. 다른 사람들과 나를 비교하게 되면서\n마음속에 어떤 감정들이 올라왔나요?\n천천히 떠올려보세요");

        yield return new WaitForSeconds(5f);

        canProgress = true;
        Debug.Log("Space를 눌러 다음 단계로");
    }

    IEnumerator StartStage4_5()
    {
        Debug.Log("Stage 4.5 시작 - 텍스트 사라짐");
        textManager.HideText();
        yield return new WaitForSeconds(1f);
        canProgress = true;
    }

    IEnumerator StartStage5()
    {
        Debug.Log("Stage 5 시작 - 작은 나무로 성장");

        // Pivot을 기준으로 성장 (마지막 파라미터)
        yield return StartCoroutine(
            treeGrowth.ReplaceTree(
                smallTreePrefab,
                2.4f,  // targetScale
                1.2f,  // startScale
                TreeGrowthController.GrowthAnchor.Bottom  // anchor 지정!
            )
        );

        yield return new WaitForSeconds(0.5f);

        //textManager.ShowText("그 기분을 느꼈을 때\n당신은 어떤 생각이 들었나요?");
        textManager.ShowText("불안한 마음이 들었군요.\n그때 머릿속에 가장 먼저 든 생각은 무엇이었나요?");


        yield return new WaitForSeconds(5f);

        canProgress = true;
        Debug.Log("Space를 눌러 다음 단계로");
    }

    IEnumerator StartStage5_5()
    {
        Debug.Log("Stage 5.5 시작 - 텍스트 사라짐");

        textManager.HideText();

        yield return new WaitForSeconds(1f);

        canProgress = true;
    }

    IEnumerator StartStage6()
    {
        Debug.Log("Stage 6 시작 - 중간 나무로 성장");

        // 작은 나무를 중간 나무로 교체하면서 성장
        // Pivot 기준으로 성장 (또는 원하는 anchor 사용)
        yield return StartCoroutine(
            treeGrowth.ReplaceTree(
                mediumTreePrefab,
                3.0f,  // targetScale - 더 큰 크기
                1.8f,  // startScale - 이전 나무보다 약간 큰 크기에서 시작
                TreeGrowthController.GrowthAnchor.Bottom  // 또는 Bottom
            )
        );

        yield return new WaitForSeconds(0.5f);

        // 텍스트 표시
        //textManager.ShowText("그런 생각을 들게 한 외부 요인이 있을까요?\n어떤 상황이 당신을 괴롭혔다고 생각하나요?");
        textManager.ShowText("그런 생각이 들게 만든 계기나 상황이 있었을까요?\n어떤 점이 특히 마음을 힘들게 했나요?");

        yield return new WaitForSeconds(6f); // 텍스트가 좀 길어서 시간 추가

        canProgress = true;
        Debug.Log("Space를 눌러 다음 단계로");
    }

    IEnumerator StartStage6_5()
    {
        Debug.Log("Stage 6.5 시작 - 텍스트 사라짐");

        textManager.HideText();

        yield return new WaitForSeconds(1f);

        canProgress = true;
        Debug.Log("Stage 7 준비 완료");
    }
    IEnumerator StartStage7()
    {
        Debug.Log("Stage 7 시작 - 큰 나무로 성장 + 음악");

        yield return StartCoroutine(
            treeGrowth.ReplaceTree(
                largeTreePrefab,
                3.0f,
                1.8f,
                TreeGrowthController.GrowthAnchor.Bottom
            )
        );

        if (bgmAudioSource != null && calmMusicClip != null)
        {
            bgmAudioSource.clip = calmMusicClip;
            bgmAudioSource.loop = true;
            bgmAudioSource.volume = 0.3f;
            bgmAudioSource.Play();
            Debug.Log("배경음악 재생 시작");
        }

        yield return new WaitForSeconds(0.5f);

        // 첫 번째 텍스트 표시
        textManager.ShowText("왠지 모르게 위축이 되었군요.\n잠시 숨을 고르고,\n비교에서 한 발 물러나 생각해볼게요.");

        yield return new WaitForSeconds(4f);

        // 첫 번째 텍스트 완전히 페이드아웃 될 때까지 대기
        yield return StartCoroutine(textManager.HideTextCoroutine());

        yield return new WaitForSeconds(1f);

        // 두 번째 텍스트 표시
        textManager.ShowText("그런 위축되는 감정과 반대로 생각해볼 여지가 있을까요?\n잘 떠오르지 않는다면, 내가 왜 이런 선택을 했었는지\n돌아보는 것도 도움이 될거에요");

        yield return new WaitForSeconds(7f);

        canProgress = true;
        Debug.Log("Space를 눌러 다음 단계로");
    }

    IEnumerator StartStage7_5()
    {
        Debug.Log("Stage 7.5 시작 - 텍스트 사라짐");

        textManager.HideText();

        yield return new WaitForSeconds(1f);

        canProgress = true;
        Debug.Log("Stage 8 준비 완료");
    }
    IEnumerator StartStage8()
    {
        Debug.Log("Stage 8 시작 - 초원 전환 + 큰 나무");

        if (environmentManager == null)
        {
            Debug.LogError("EnvironmentManager가 연결되지 않았습니다!");
            yield break;
        }

        // 나무 교체와 화면 전환을 동시에 시작
        Coroutine treeGrowth_Coroutine = StartCoroutine(
            treeGrowth.ReplaceTree(
                hugeTreePrefab,
                4.0f,
                4.0f,
                TreeGrowthController.GrowthAnchor.Bottom
            )
        );

        Coroutine transitionCoroutine = StartCoroutine(environmentManager.TransitionToMeadow());

        // 두 코루틴 모두 완료될 때까지 대기
        yield return treeGrowth_Coroutine;
        yield return transitionCoroutine;

        yield return new WaitForSeconds(1f);

        // 텍스트 표시
        textManager.ShowText("앞선 질문들에 스스로 답변을 해보면서\n상황을 다양한 시각에서 바라보았어요.\n이제 균형 잡힌 시각에서 상황을 다시 설명해볼까요?\n답은 꼭 내지 않아도 좋아요");

        yield return new WaitForSeconds(8f);

        canProgress = true;
        Debug.Log("Space를 눌러 다음 단계로");
    }

    IEnumerator StartStage8_5()
    {
        Debug.Log("Stage 8.5 시작 - 텍스트 사라지고 다시 어두워짐");

        // 텍스트 페이드아웃 완료까지 대기
        yield return StartCoroutine(textManager.HideTextCoroutine());

        yield return new WaitForSeconds(0.5f);

        // 다시 어두운 배경으로 (Tree도 함께 사라짐)
        yield return StartCoroutine(environmentManager.TransitionToDark());

        yield return new WaitForSeconds(1f);

        canProgress = true;
        Debug.Log("Stage 9 준비 완료");
    }

    IEnumerator StartStage9()
    {
        Debug.Log("Stage 9 시작 - 마무리 텍스트");

        yield return new WaitForSeconds(2f);

        // 첫 번째 텍스트
        textManager.ShowText("오늘 당신은 당신의 고민을 털어놓았고,\n다시 돌아보는 시간을 가졌어요");

        yield return new WaitForSeconds(5f);

        // 페이드아웃 대기
        yield return StartCoroutine(textManager.HideTextCoroutine());

        yield return new WaitForSeconds(1.5f);

        // 두 번째 텍스트
        textManager.ShowText("어떤가요?\n이 경험이 당신에게 위로가 되었으면 좋겠어요.\n힘들 때 마다 언제든지 다시 찾아오세요.");

        yield return new WaitForSeconds(6f);

        // 페이드아웃 대기
        yield return StartCoroutine(textManager.HideTextCoroutine());

        yield return new WaitForSeconds(1.5f);

        // 세 번째 텍스트
        textManager.ShowText("저는 언제나,\n당신의 마음 속에 함께할거에요");

        yield return new WaitForSeconds(5f);

        // 페이드아웃 대기
        yield return StartCoroutine(textManager.HideTextCoroutine());

        yield return new WaitForSeconds(1f);

        // 전체 화면 페이드 아웃으로 종료
        yield return StartCoroutine(FinalFadeOut());

        Debug.Log("경험 종료");
    }

    IEnumerator FinalFadeOut()
    {
        CanvasGroup fadePanel = environmentManager.fadePanel;
        UnityEngine.UI.Image img = fadePanel.GetComponent<UnityEngine.UI.Image>();

        if (img != null)
            img.color = Color.black;

        float elapsed = 0;
        float duration = 3f; // 천천히 페이드아웃

        while (elapsed < duration)
        {
            fadePanel.alpha = Mathf.Lerp(0, 1, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        fadePanel.alpha = 1;

        // 음악도 페이드아웃 (선택사항)
        if (bgmAudioSource != null && bgmAudioSource.isPlaying)
        {
            float startVolume = bgmAudioSource.volume;
            elapsed = 0;

            while (elapsed < 2f)
            {
                bgmAudioSource.volume = Mathf.Lerp(startVolume, 0, elapsed / 2f);
                elapsed += Time.deltaTime;
                yield return null;
            }

            bgmAudioSource.Stop();
        }

        yield return new WaitForSeconds(2f);

        // 원하면 씬 재시작 또는 종료
        // UnityEngine.SceneManagement.SceneManager.LoadScene(0); // 재시작
        // Application.Quit(); // 종료
    }

}