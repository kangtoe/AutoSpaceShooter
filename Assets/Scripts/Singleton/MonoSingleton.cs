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
    /// 매니저 초기화 (GameManager에서 명시적으로 호출)
    /// 하위 클래스에서 override하여 구현
    /// </summary>
    public virtual void Initialize()
    {
        // 기본 구현 없음 - 하위 클래스에서 필요 시 override
    }

    /// <summary>
    /// 초기화 완료 여부
    /// </summary>
    public bool IsInitialized { get; protected set; } = false;
}