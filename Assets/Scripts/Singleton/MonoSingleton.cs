using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static object lockObject = new object();
    static T instance = null;

    public static bool has_instance => instance != null;

    public static T Instance
    {
        get
        {
            lock (lockObject) // for Thread-Safe
            {
                if (!has_instance)
                {
                    instance = FindFirstObjectByType<T>();

                    if (!has_instance)
                    {
                        GameObject go = new GameObject();
                        go.name = typeof(T).ToString();
                        instance = go.AddComponent<T>();
                    }

                    // 인스턴스 생성 후 자동 초기화
                    if (instance is MonoSingleton<T> singleton)
                    {
                        singleton.Initialize();
                    }
                }
                return instance;
            }
        }
    }

    /// <summary>
    /// 매니저 초기화 (첫 Instance 접근 시 자동 호출)
    /// 하위 클래스에서 override하여 구현
    /// </summary>
    /// <returns>true: 성공적으로 초기화됨, false: 이미 초기화되어 있음</returns>
    public virtual bool Initialize()
    {
        // 기본 구현: 중복 초기화 방지
        if (IsInitialized) return false;

        IsInitialized = true;
        return true;
    }

    /// <summary>
    /// 초기화 완료 여부
    /// </summary>
    public bool IsInitialized { get; protected set; } = false;
}