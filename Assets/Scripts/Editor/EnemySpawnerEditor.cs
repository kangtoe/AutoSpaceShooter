using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemySpawner))]
public class EnemySpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EnemySpawner script = (EnemySpawner)target;

        if (GUILayout.Button(nameof(script.AddSpawnInfo)))
        {
            script.AddSpawnInfo();
        }

        GUI.enabled = false; // 읽기 전용 설정
        float spawnEndTime = script.GetSpawnEndTime();
        EditorGUILayout.FloatField(nameof(spawnEndTime), spawnEndTime);
        GUI.enabled = true; // 원래 상태로 되돌림
    }
}