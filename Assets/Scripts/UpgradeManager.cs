using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoSingleton<UpgradeManager>
{
    [SerializeField] AudioClip upgradeSound;
    [SerializeField] AudioClip upgradeFailSound;

    [SerializeField] Color textHighlight;

    List<UpgradeButtonUI> UpgradeButtons => UiManager.Instance.UpgradeButtonUIList;

    // 스탯별 현재 레벨 추적 (로그라이크 방식)
    Dictionary<UpgradeField, int> statLevels = new();

    // 현재 제시된 업그레이드 옵션들
    List<UpgradeOption> currentOptions = new List<UpgradeOption>();

    int upgradePoint = 0;

    // Start is called before the first frame update
    void Start()
    {
        // 초기 스탯 설정 (기본값)
        InitPlayershipStats();
        UiManager.Instance.SetUpgradePointText(upgradePoint);

        // 버튼 리스너 등록
        for (int i = 0; i < UpgradeButtons.Count; i++)
        {
            int index = i; // 클로저를 위한 로컬 변수
            UpgradeButtons[i].Button.onClick.AddListener(delegate {
                SelectUpgrade(index);
            });
        }
    }

    void InitPlayershipStats()
    {
        // 모든 스탯을 기본값으로 초기화
        // PlayerShip의 SetSystem은 이제 증분 방식이므로 여기선 호출하지 않음
    }

    void SelectUpgrade(int index)
    {
        if (upgradePoint < 1)
        {
            UiManager.Instance.CreateText("No Point!", true);
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
            UiManager.Instance.CreateText("Max Level!", true);
            UiManager.Instance.ShakeUI();
            SoundManager.Instance.PlaySound(upgradeFailSound);
            return;
        }

        // 스탯 레벨 증가
        if (!statLevels.ContainsKey(option.field))
            statLevels[option.field] = 0;

        statLevels[option.field]++;

        // PlayerStats에 증분 적용
        PlayerStats.Instance.ApplyUpgrade(option.field, option.incrementValue);

        // 내구도/실드 업그레이드 시 현재 값도 함께 증가
        if (option.field == UpgradeField.MaxDurability)
        {
            GameManager.Instance.PlayerShip.Damageable.ModifyDurability(option.incrementValue);
        }
        else if (option.field == UpgradeField.MaxShield)
        {
            GameManager.Instance.PlayerShip.Damageable.ModifyShield(option.incrementValue);
        }

        // 포인트 차감
        upgradePoint--;
        UiManager.Instance.SetUpgradePointText(upgradePoint);

        SoundManager.Instance.PlaySound(upgradeSound);

        // 포인트가 남아있으면 새로운 옵션 생성, 없으면 창 닫기
        if (upgradePoint > 0)
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
        upgradePoint += amount;
        UiManager.Instance.SetUpgradePointText(upgradePoint);

        // 랜덤 업그레이드 옵션 생성
        GenerateRandomUpgrades();

        GameManager.Instance.ToggleUpgradeState(true);
    }
}
