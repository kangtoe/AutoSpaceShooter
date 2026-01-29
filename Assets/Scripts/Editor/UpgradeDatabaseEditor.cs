using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// UpgradeDatabase의 인스펙터를 개선하여 각 업그레이드를 명확하게 표시
/// </summary>
[CustomEditor(typeof(UpgradeDatabase))]
public class UpgradeDatabaseEditor : Editor
{
    private bool showUpgrades = true;
    private Dictionary<string, bool> foldouts = new Dictionary<string, bool>();
    private bool groupByType = true;
    private bool showUniqueOnly = false;
    private bool showGeneralOnly = false;

    public override void OnInspectorGUI()
    {
        UpgradeDatabase database = (UpgradeDatabase)target;
        serializedObject.Update();

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("업그레이드 데이터베이스", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        // 옵션
        EditorGUILayout.BeginHorizontal();
        groupByType = EditorGUILayout.Toggle("타입별 그룹화", groupByType);
        showUniqueOnly = EditorGUILayout.Toggle("유니크만", showUniqueOnly);
        showGeneralOnly = EditorGUILayout.Toggle("일반만", showGeneralOnly);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(5);

        // 통계 정보
        DrawStatistics(database);
        EditorGUILayout.Space(10);

        // 유틸리티 버튼들
        DrawUtilityButtons(database);
        EditorGUILayout.Space(10);

        // 업그레이드 목록
        int displayCount = GetFilteredUpgrades(database).Count;
        showUpgrades = EditorGUILayout.Foldout(showUpgrades, $"업그레이드 목록 ({displayCount}개)", true);
        if (showUpgrades)
        {
            if (groupByType)
            {
                DrawUpgradesByType(database);
            }
            else
            {
                DrawUpgradesFlat(database);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    private List<UpgradeConfig> GetFilteredUpgrades(UpgradeDatabase database)
    {
        var upgrades = database.allUpgrades.Where(u => u != null);

        if (showUniqueOnly)
            upgrades = upgrades.Where(u => u.isUnique);
        else if (showGeneralOnly)
            upgrades = upgrades.Where(u => !u.isUnique);

        return upgrades.ToList();
    }

    private void DrawStatistics(UpgradeDatabase database)
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("통계", EditorStyles.boldLabel);

        int totalUpgrades = database.allUpgrades.Count;
        int uniqueUpgrades = database.allUpgrades.Count(u => u != null && u.isUnique);
        int generalUpgrades = database.allUpgrades.Count(u => u != null && !u.isUnique);

        // 중복 ID 체크
        var duplicates = database.allUpgrades
            .Where(u => u != null && !string.IsNullOrEmpty(u.upgradeId))
            .GroupBy(u => u.upgradeId)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        // ID 없는 항목 체크
        int noIdCount = database.allUpgrades.Count(u => u != null && string.IsNullOrEmpty(u.upgradeId));

        EditorGUILayout.LabelField($"총 업그레이드: {totalUpgrades}개");
        EditorGUILayout.LabelField($"유니크: {uniqueUpgrades}개 | 일반: {generalUpgrades}개");

        if (duplicates.Count > 0)
        {
            EditorGUILayout.HelpBox($"⚠️ 중복된 ID: {string.Join(", ", duplicates)}", MessageType.Error);
        }

        if (noIdCount > 0)
        {
            EditorGUILayout.HelpBox($"⚠️ ID 없는 항목: {noIdCount}개", MessageType.Warning);
        }

        if (duplicates.Count == 0 && noIdCount == 0 && totalUpgrades > 0)
        {
            EditorGUILayout.HelpBox("✓ 모든 업그레이드가 올바르게 설정되었습니다", MessageType.Info);
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawUtilityButtons(UpgradeDatabase database)
    {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("ID로 정렬"))
        {
            SortById(database);
        }

        if (GUILayout.Button("타입으로 정렬"))
        {
            SortByType(database);
        }

        if (GUILayout.Button("중복 제거"))
        {
            RemoveDuplicates(database);
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("모든 업그레이드 검증"))
        {
            ValidateAll(database);
        }

        if (GUILayout.Button("문서 기반 업그레이드 추가 (15개)"))
        {
            AddAllUpgradesFromDoc(database);
        }

        EditorGUILayout.EndHorizontal();
    }

    private void DrawUpgradesFlat(UpgradeDatabase database)
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        var filteredUpgrades = GetFilteredUpgrades(database);

        foreach (var upgrade in filteredUpgrades)
        {
            int index = database.allUpgrades.IndexOf(upgrade);
            DrawUpgradeItem(database, upgrade, index);
        }

        EditorGUILayout.EndVertical();

        // 새 업그레이드 추가 버튼
        if (GUILayout.Button("+ 새 업그레이드 추가"))
        {
            AddNewUpgrade(database);
        }
    }

    private void DrawUpgradesByType(UpgradeDatabase database)
    {
        var types = System.Enum.GetValues(typeof(UpgradeType)).Cast<UpgradeType>();

        foreach (var type in types)
        {
            var upgradesInType = GetFilteredUpgrades(database)
                .Where(u => u.upgradeType == type)
                .ToList();

            if (upgradesInType.Count == 0)
                continue;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField($"📁 {type} ({upgradesInType.Count})", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            foreach (var upgrade in upgradesInType)
            {
                int index = database.allUpgrades.IndexOf(upgrade);
                DrawUpgradeItem(database, upgrade, index);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }
    }

    private void DrawUpgradeItem(UpgradeDatabase database, UpgradeConfig upgrade, int index)
    {
        if (!foldouts.ContainsKey(upgrade.upgradeId))
        {
            foldouts[upgrade.upgradeId] = false;
        }

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        // 헤더: ID + 이름 + 유니크 여부
        EditorGUILayout.BeginHorizontal();

        string label = $"{upgrade.upgradeId} - {upgrade.displayName}";
        if (upgrade.isUnique)
            label += " [유니크]";

        foldouts[upgrade.upgradeId] = EditorGUILayout.Foldout(
            foldouts[upgrade.upgradeId],
            label,
            true
        );

        // 삭제 버튼
        GUI.backgroundColor = new Color(1f, 0.5f, 0.5f);
        if (GUILayout.Button("×", GUILayout.Width(25)))
        {
            if (EditorUtility.DisplayDialog("업그레이드 삭제",
                $"{upgrade.upgradeId}를 삭제하시겠습니까?", "삭제", "취소"))
            {
                Undo.RecordObject(database, "Remove Upgrade");
                database.allUpgrades.RemoveAt(index);
                EditorUtility.SetDirty(database);
            }
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.EndHorizontal();

        // 세부 내용
        if (foldouts[upgrade.upgradeId])
        {
            EditorGUI.indentLevel++;

            SerializedProperty upgradeProperty = serializedObject.FindProperty("allUpgrades").GetArrayElementAtIndex(index);

            // 기본 정보
            EditorGUILayout.LabelField("기본 정보", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(upgradeProperty.FindPropertyRelative("upgradeId"));
            EditorGUILayout.PropertyField(upgradeProperty.FindPropertyRelative("displayName"));
            EditorGUILayout.PropertyField(upgradeProperty.FindPropertyRelative("description"));
            EditorGUILayout.PropertyField(upgradeProperty.FindPropertyRelative("icon"));

            EditorGUILayout.Space(5);

            // 분류
            EditorGUILayout.LabelField("분류", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(upgradeProperty.FindPropertyRelative("upgradeType"));
            EditorGUILayout.PropertyField(upgradeProperty.FindPropertyRelative("isUnique"));
            EditorGUILayout.PropertyField(upgradeProperty.FindPropertyRelative("maxLevel"));

            EditorGUILayout.Space(5);

            // 스탯 효과
            EditorGUILayout.LabelField("스탯 효과", EditorStyles.boldLabel);
            DrawStatModifiers(database, upgrade, upgradeProperty);

            // 검증 버튼
            EditorGUILayout.Space(5);
            if (GUILayout.Button("이 업그레이드 검증"))
            {
                if (upgrade.Validate(out string errorMessage))
                {
                    EditorUtility.DisplayDialog("검증 성공", $"{upgrade.upgradeId} 검증 성공!", "확인");
                }
                else
                {
                    EditorUtility.DisplayDialog("검증 실패", errorMessage, "확인");
                }
            }

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(2);
    }

    private void DrawStatModifiers(UpgradeDatabase database, UpgradeConfig upgrade, SerializedProperty upgradeProperty)
    {
        SerializedProperty modifiersProperty = upgradeProperty.FindPropertyRelative("statModifiers");

        EditorGUI.indentLevel++;

        // 각 StatModifier를 커스텀 형식으로 표시
        for (int i = 0; i < upgrade.statModifiers.Count; i++)
        {
            StatModifier modifier = upgrade.statModifiers[i];
            SerializedProperty modifierProperty = modifiersProperty.GetArrayElementAtIndex(i);

            EditorGUILayout.BeginHorizontal();

            // 스탯 필드 선택
            SerializedProperty fieldProperty = modifierProperty.FindPropertyRelative("field");
            EditorGUILayout.PropertyField(fieldProperty, GUIContent.none, GUILayout.Width(150));

            // 증감 값
            SerializedProperty valueProperty = modifierProperty.FindPropertyRelative("valuePerLevel");
            string sign = modifier.valuePerLevel >= 0 ? "+" : "";
            EditorGUILayout.LabelField($"{sign}{modifier.valuePerLevel:F2}", GUILayout.Width(60));
            EditorGUILayout.PropertyField(valueProperty, GUIContent.none, GUILayout.Width(80));

            // 삭제 버튼
            GUI.backgroundColor = new Color(1f, 0.5f, 0.5f);
            if (GUILayout.Button("×", GUILayout.Width(25)))
            {
                upgrade.statModifiers.RemoveAt(i);
                EditorUtility.SetDirty(database);
                break;
            }
            GUI.backgroundColor = Color.white;

            EditorGUILayout.EndHorizontal();
        }

        // 추가 버튼
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(EditorGUI.indentLevel * 15);
        if (GUILayout.Button("+ 스탯 효과 추가", GUILayout.Width(150)))
        {
            upgrade.statModifiers.Add(new StatModifier
            {
                field = UpgradeField.FireRate,
                valuePerLevel = 0f
            });
            EditorUtility.SetDirty(database);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUI.indentLevel--;
    }

    private void SortById(UpgradeDatabase database)
    {
        Undo.RecordObject(database, "Sort Upgrades by ID");
        database.allUpgrades.Sort((a, b) => string.Compare(a.upgradeId, b.upgradeId, System.StringComparison.Ordinal));
        EditorUtility.SetDirty(database);
        Debug.Log($"[UpgradeDatabase] {database.allUpgrades.Count}개의 업그레이드를 ID 순서로 정렬했습니다.");
    }

    private void SortByType(UpgradeDatabase database)
    {
        Undo.RecordObject(database, "Sort Upgrades by Type");
        database.allUpgrades.Sort((a, b) =>
        {
            int typeCompare = a.upgradeType.CompareTo(b.upgradeType);
            if (typeCompare != 0) return typeCompare;

            int uniqueCompare = b.isUnique.CompareTo(a.isUnique);
            if (uniqueCompare != 0) return uniqueCompare;

            return string.Compare(a.upgradeId, b.upgradeId, System.StringComparison.Ordinal);
        });
        EditorUtility.SetDirty(database);
        Debug.Log($"[UpgradeDatabase] {database.allUpgrades.Count}개의 업그레이드를 타입 순서로 정렬했습니다.");
    }

    private void RemoveDuplicates(UpgradeDatabase database)
    {
        Undo.RecordObject(database, "Remove Duplicates");

        var seen = new HashSet<string>();
        var toRemove = new List<int>();

        for (int i = 0; i < database.allUpgrades.Count; i++)
        {
            var upgrade = database.allUpgrades[i];
            if (upgrade == null || string.IsNullOrEmpty(upgrade.upgradeId))
            {
                toRemove.Add(i);
                continue;
            }

            if (seen.Contains(upgrade.upgradeId))
            {
                toRemove.Add(i);
            }
            else
            {
                seen.Add(upgrade.upgradeId);
            }
        }

        for (int i = toRemove.Count - 1; i >= 0; i--)
        {
            database.allUpgrades.RemoveAt(toRemove[i]);
        }

        if (toRemove.Count > 0)
        {
            EditorUtility.SetDirty(database);
            Debug.Log($"[UpgradeDatabase] {toRemove.Count}개의 중복 항목을 제거했습니다.");
        }
        else
        {
            Debug.Log("[UpgradeDatabase] 중복 항목이 없습니다.");
        }
    }

    private void ValidateAll(UpgradeDatabase database)
    {
        int errorCount = 0;
        int warningCount = 0;

        foreach (var upgrade in database.allUpgrades)
        {
            if (upgrade == null)
            {
                errorCount++;
                continue;
            }

            if (!upgrade.Validate(out string errorMessage))
            {
                Debug.LogError($"[UpgradeDatabase] {upgrade.upgradeId}: {errorMessage}", database);
                errorCount++;
            }
        }

        if (errorCount == 0)
        {
            EditorUtility.DisplayDialog("검증 완료",
                $"모든 업그레이드가 올바르게 설정되었습니다.\n총 {database.allUpgrades.Count}개 검증 완료",
                "확인");
        }
        else
        {
            EditorUtility.DisplayDialog("검증 실패",
                $"오류: {errorCount}개\n콘솔 로그를 확인하세요.",
                "확인");
        }
    }

    private void AddNewUpgrade(UpgradeDatabase database)
    {
        Undo.RecordObject(database, "Add New Upgrade");

        var newUpgrade = new UpgradeConfig
        {
            upgradeId = "new_upgrade",
            displayName = "새 업그레이드",
            description = "",
            upgradeType = UpgradeType.Shooter,
            isUnique = false,
            maxLevel = 5,
            statModifiers = new List<StatModifier>()
        };

        database.allUpgrades.Add(newUpgrade);
        EditorUtility.SetDirty(database);
        Debug.Log("[UpgradeDatabase] 새 업그레이드를 추가했습니다.");
    }

    private void AddAllUpgradesFromDoc(UpgradeDatabase database)
    {
        Undo.RecordObject(database, "Add All Upgrades From Doc");

        int addedCount = 0;

        // === 유니크 업그레이드 (사격 관련) ===

        // 1. 제압사격
        database.allUpgrades.Add(new UpgradeConfig
        {
            upgradeId = "suppressive_fire",
            displayName = "제압사격",
            description = "분산도 증가, 발사 위력 감소, 연사속도 크게 증가",
            upgradeType = UpgradeType.Shooter,
            isUnique = true,
            maxLevel = 1,
            statModifiers = new List<StatModifier>
            {
                new StatModifier { field = UpgradeField.Spread, valuePerLevel = 15f },
                new StatModifier { field = UpgradeField.ProjectileDamage, valuePerLevel = -5f },
                new StatModifier { field = UpgradeField.FireRate, valuePerLevel = 1.5f }
            }
        });
        addedCount++;

        // 2. 공성포
        database.allUpgrades.Add(new UpgradeConfig
        {
            upgradeId = "siege_cannon",
            displayName = "공성포",
            description = "발사체 속도/위력/크기 증가, 연사속도 감소",
            upgradeType = UpgradeType.Shooter,
            isUnique = true,
            maxLevel = 1,
            statModifiers = new List<StatModifier>
            {
                new StatModifier { field = UpgradeField.ProjectileSpeed, valuePerLevel = 50f },
                new StatModifier { field = UpgradeField.ProjectileDamage, valuePerLevel = 15f },
                new StatModifier { field = UpgradeField.ProjectileSize, valuePerLevel = 0.5f },
                new StatModifier { field = UpgradeField.FireRate, valuePerLevel = -0.5f }
            }
        });
        addedCount++;

        // 3. 일제포화
        database.allUpgrades.Add(new UpgradeConfig
        {
            upgradeId = "volley_fire",
            displayName = "일제포화",
            description = "위력/연사속도 감소, 분산도 증가, 발사체 추가",
            upgradeType = UpgradeType.Shooter,
            isUnique = true,
            maxLevel = 1,
            statModifiers = new List<StatModifier>
            {
                new StatModifier { field = UpgradeField.ProjectileDamage, valuePerLevel = -5f },
                new StatModifier { field = UpgradeField.FireRate, valuePerLevel = -0.3f },
                new StatModifier { field = UpgradeField.Spread, valuePerLevel = 10f },
                new StatModifier { field = UpgradeField.MultiShot, valuePerLevel = 2f }
            }
        });
        addedCount++;

        // 4. 스마트 탄환
        database.allUpgrades.Add(new UpgradeConfig
        {
            upgradeId = "smart_bullet",
            displayName = "스마트 탄환",
            description = "탄환 속도/연사속도 감소, 유도 성능 추가",
            upgradeType = UpgradeType.Shooter,
            isUnique = true,
            maxLevel = 1,
            statModifiers = new List<StatModifier>
            {
                new StatModifier { field = UpgradeField.ProjectileSpeed, valuePerLevel = -30f },
                new StatModifier { field = UpgradeField.FireRate, valuePerLevel = -0.2f },
                new StatModifier { field = UpgradeField.HomingPower, valuePerLevel = 90f }
            }
        });
        addedCount++;

        // 5. 고폭탄
        database.allUpgrades.Add(new UpgradeConfig
        {
            upgradeId = "explosive_round",
            displayName = "고폭탄",
            description = "폭발 효과 추가, 피해의 20%를 폭발피해로 전환",
            upgradeType = UpgradeType.Shooter,
            isUnique = true,
            maxLevel = 1,
            statModifiers = new List<StatModifier>
            {
                new StatModifier { field = UpgradeField.ExplosionDamageRatio, valuePerLevel = 0.2f }
            }
        });
        addedCount++;

        // === 유니크 업그레이드 (기체 관련) ===

        // 6. 실험형 실드
        database.allUpgrades.Add(new UpgradeConfig
        {
            upgradeId = "experimental_shield",
            displayName = "실험형 실드",
            description = "실드 용량 감소, 실드 재생 시간 단축",
            upgradeType = UpgradeType.Ship,
            isUnique = true,
            maxLevel = 1,
            statModifiers = new List<StatModifier>
            {
                new StatModifier { field = UpgradeField.MaxShield, valuePerLevel = -30f },
                new StatModifier { field = UpgradeField.ShieldRegenDelay, valuePerLevel = -2f }
            }
        });
        addedCount++;

        // 7. 강화 실드
        database.allUpgrades.Add(new UpgradeConfig
        {
            upgradeId = "reinforced_shield",
            displayName = "강화 실드",
            description = "실드 용량 증가, 체력 감소",
            upgradeType = UpgradeType.Ship,
            isUnique = true,
            maxLevel = 1,
            statModifiers = new List<StatModifier>
            {
                new StatModifier { field = UpgradeField.MaxShield, valuePerLevel = 50f },
                new StatModifier { field = UpgradeField.MaxDurability, valuePerLevel = -30f }
            }
        });
        addedCount++;

        // 8. 강습 기체
        database.allUpgrades.Add(new UpgradeConfig
        {
            upgradeId = "assault_frame",
            displayName = "강습 기체",
            description = "추진력/회전 속도/충돌 피해 증가",
            upgradeType = UpgradeType.Ship,
            isUnique = true,
            maxLevel = 1,
            statModifiers = new List<StatModifier>
            {
                new StatModifier { field = UpgradeField.MoveSpeed, valuePerLevel = 2f },
                new StatModifier { field = UpgradeField.RotateSpeed, valuePerLevel = 50f },
                new StatModifier { field = UpgradeField.OnImpact, valuePerLevel = 10f }
            }
        });
        addedCount++;

        // 9. 전함
        database.allUpgrades.Add(new UpgradeConfig
        {
            upgradeId = "battleship",
            displayName = "전함",
            description = "받는 충돌 피해 경감, 내구도/중량 증가",
            upgradeType = UpgradeType.Ship,
            isUnique = true,
            maxLevel = 1,
            statModifiers = new List<StatModifier>
            {
                new StatModifier { field = UpgradeField.ImpactResist, valuePerLevel = 5f },
                new StatModifier { field = UpgradeField.MaxDurability, valuePerLevel = 50f },
                new StatModifier { field = UpgradeField.Mass, valuePerLevel = 2f }
            }
        });
        addedCount++;

        // 10. 수리 모듈
        database.allUpgrades.Add(new UpgradeConfig
        {
            upgradeId = "repair_module",
            displayName = "수리 모듈",
            description = "기체 회복 증가",
            upgradeType = UpgradeType.Ship,
            isUnique = true,
            maxLevel = 1,
            statModifiers = new List<StatModifier>
            {
                new StatModifier { field = UpgradeField.DurabilityRegenRate, valuePerLevel = 2f }
            }
        });
        addedCount++;

        // === 일반 업그레이드 ===

        // 1. 대구경
        database.allUpgrades.Add(new UpgradeConfig
        {
            upgradeId = "large_caliber",
            displayName = "대구경",
            description = "공격력 및 발사체 크기 증가",
            upgradeType = UpgradeType.Shooter,
            isUnique = false,
            maxLevel = 5,
            statModifiers = new List<StatModifier>
            {
                new StatModifier { field = UpgradeField.ProjectileDamage, valuePerLevel = 3f },
                new StatModifier { field = UpgradeField.ProjectileSize, valuePerLevel = 0.1f }
            }
        });
        addedCount++;

        // 2. 냉각 시스템
        database.allUpgrades.Add(new UpgradeConfig
        {
            upgradeId = "cooling_system",
            displayName = "냉각 시스템",
            description = "연사속도 증가",
            upgradeType = UpgradeType.Shooter,
            isUnique = false,
            maxLevel = 5,
            statModifiers = new List<StatModifier>
            {
                new StatModifier { field = UpgradeField.FireRate, valuePerLevel = 0.2f }
            }
        });
        addedCount++;

        // 3. 엔진 최적화
        database.allUpgrades.Add(new UpgradeConfig
        {
            upgradeId = "engine_optimization",
            displayName = "엔진 최적화",
            description = "이동속도 증가",
            upgradeType = UpgradeType.Ship,
            isUnique = false,
            maxLevel = 5,
            statModifiers = new List<StatModifier>
            {
                new StatModifier { field = UpgradeField.MoveSpeed, valuePerLevel = 0.5f }
            }
        });
        addedCount++;

        // 4. 추가 장갑
        database.allUpgrades.Add(new UpgradeConfig
        {
            upgradeId = "additional_armor",
            displayName = "추가 장갑",
            description = "내구도 증가",
            upgradeType = UpgradeType.Ship,
            isUnique = false,
            maxLevel = 5,
            statModifiers = new List<StatModifier>
            {
                new StatModifier { field = UpgradeField.MaxDurability, valuePerLevel = 10f }
            }
        });
        addedCount++;

        // 5. 실드 향상
        database.allUpgrades.Add(new UpgradeConfig
        {
            upgradeId = "shield_enhancement",
            displayName = "실드 향상",
            description = "실드 증가",
            upgradeType = UpgradeType.Ship,
            isUnique = false,
            maxLevel = 5,
            statModifiers = new List<StatModifier>
            {
                new StatModifier { field = UpgradeField.MaxShield, valuePerLevel = 10f }
            }
        });
        addedCount++;

        EditorUtility.SetDirty(database);
        Debug.Log($"[UpgradeDatabase] 문서 기반 {addedCount}개의 업그레이드를 추가했습니다.");
    }
}
