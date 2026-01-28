using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoSingleton<UpgradeManager>
{
    [Header("=== Upgrade Data ===")]
    [Tooltip("StatConfigDatabase SO (필수) - 업그레이드 정보")]
    [SerializeField] private StatConfigDatabase statDatabase;

    [Header("=== Audio ===")]
    [SerializeField] AudioClip upgradeSound;
    [SerializeField] AudioClip upgradeFailSound;

    [Header("=== UI ===")]
    [SerializeField] Color textHighlight;

    List<UpgradeButtonUI> UpgradeButtons => UiManager.Instance.UpgradeButtonUIList;

    // 스탯별 현재 레벨 추적 (로그라이크 방식)
    Dictionary<UpgradeField, int> statLevels = new();

    // 현재 제시된 업그레이드 옵션들
    List<UpgradeOption> currentOptions = new List<UpgradeOption>();

    // Start is called before the first frame update
    void Start()
    {
        // StatConfigDatabase 검증
        if (statDatabase == null)
        {
            Debug.LogError("[UpgradeManager] StatConfigDatabase가 할당되지 않았습니다!");
            return;
        }

        // 1. PlayerStatsManager 초기화 (StatConfigDatabase 로드)
        // MonoSingleton.Instance getter에서 자동으로 Initialize() 호출됨
        var _ = PlayerStatsManager.Instance;

        // 2. UpgradeData 초기화 (StatConfigDatabase 로드 + DisplayName 병합)
        UpgradeData.Initialize(statDatabase);

        // 3. UI 업데이트
        UiManager.Instance.SetUpgradePointText(PlayerStatsManager.Instance.upgradePoint);

        // 버튼 리스너 등록
        for (int i = 0; i < UpgradeButtons.Count; i++)
        {
            int index = i; // 클로저를 위한 로컬 변수
            UpgradeButtons[i].Button.onClick.AddListener(delegate {
                SelectUpgrade(index);
            });
        }
    }


    void SelectUpgrade(int index)
    {
        if (PlayerStatsManager.Instance.upgradePoint < 1)
        {
            Vector2 touchPos = InputManager.Instance.PointerPosition;
            UiManager.Instance.CreateText("No Point!", touchPos);
            UiManager.Instance.ShakeUI();
            SoundManager.Instance.PlaySound(upgradeFailSound);
            return;
        }

        if (index < 0 || index >= currentOptions.Count)
        {
            Debug.LogError($"Invalid upgrade index: {index}");
            return;
        }

        UpgradeOption option = currentOptions[index];

        // 최대 레벨 체크
        if (option.currentLevel >= option.maxLevel)
        {
            Vector2 touchPos = InputManager.Instance.PointerPosition;
            UiManager.Instance.CreateText("Max Level!", touchPos);
            UiManager.Instance.ShakeUI();
            SoundManager.Instance.PlaySound(upgradeFailSound);
            return;
        }

        // 스탯 레벨 증가
        if (!statLevels.ContainsKey(option.field))
            statLevels[option.field] = 0;

        statLevels[option.field]++;

        // PlayerStats에 증분 적용
        PlayerStatsManager.Instance.ApplyUpgrade(option.field, option.incrementValue);

        // PlayerShip의 컴포넌트에 업데이트된 스텟 반영
        // (SetMaxDurability/SetMaxShield의 adjustCurrent 매개변수가 현재 값 조정을 처리)
        GameManager.Instance.PlayerShip.ApplyStatFromPlayerStats(option.field);

        // 포인트 차감
        PlayerStatsManager.Instance.upgradePoint--;
        UiManager.Instance.SetUpgradePointText(PlayerStatsManager.Instance.upgradePoint);

        SoundManager.Instance.PlaySound(upgradeSound);
        UiManager.Instance.ShakeUI();

        // 포인트가 남아있으면 새로운 옵션 생성, 없으면 창 닫기
        if (PlayerStatsManager.Instance.upgradePoint > 0)
        {
            GenerateRandomUpgrades();
        }
        else
        {
            GameManager.Instance.ToggleUpgradeState(false);
        }
    }

    void GenerateRandomUpgrades()
    {
        // 3개의 랜덤 업그레이드 옵션 생성
        currentOptions = UpgradeData.GetRandomUpgradeOptions(3, statLevels);

        // UI 업데이트
        for (int i = 0; i < UpgradeButtons.Count; i++)
        {
            if (i < currentOptions.Count)
            {
                UpdateUpgradeButton(UpgradeButtons[i], currentOptions[i]);
                UpgradeButtons[i].gameObject.SetActive(true);
            }
            else
            {
                UpgradeButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void UpdateUpgradeButton(UpgradeButtonUI btn, UpgradeOption option)
    {
        // 타이틀: 한글 이름
        btn.SetTitle(option.displayName);

        // 설명: 증가량 + 레벨 정보
        string colorCode = ColorUtility.ToHtmlStringRGBA(textHighlight);
        string desc = $"<color=#{colorCode}>{option.description}</color>";
        btn.SetDesc(desc);
    }

    public void PointUp(int amount = 1)
    {
        PlayerStatsManager.Instance.upgradePoint += amount;
        UiManager.Instance.SetUpgradePointText(PlayerStatsManager.Instance.upgradePoint);

        // 이미 업그레이드 화면이 열려있으면 옵션만 갱신
        if (GameManager.Instance.GameState == GameState.OnUpgrade)
        {
            GenerateRandomUpgrades();
            return;
        }

        // 업그레이드 화면 열기
        GenerateRandomUpgrades();
        GameManager.Instance.ToggleUpgradeState(true);
    }
}
