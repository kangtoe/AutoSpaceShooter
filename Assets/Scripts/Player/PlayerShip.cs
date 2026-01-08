using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Damageable))]
public class PlayerShip : MonoBehaviour
{    
    [Header("systems")]
    [SerializeField] ShooterBase shooter;
    [SerializeField] Impactable impactable;
    [SerializeField] Damageable damageable;

    // MoveStandard와 RotateByInput 컴포넌트가 자동으로 이동/회전 처리
    // ShooterBase 컴포넌트가 자동으로 사격 처리

    // Start is called before the first frame update
    void Start()
    {
        UpdateHealthUI();        

        damageable.onDamaged.AddListener(delegate
        {
            UpdateHealthUI();
            UiManager.Instance.ShakeUI();
        });
        damageable.onDead.AddListener(delegate
        {
            GameManager.Instance.GameOver();
        });

        LevelManager.Instance.onLevelUp.AddListener(delegate {
            UiManager.Instance.CreateText("Level Up!", transform.position);
        });
    }


    void UpdateHealthUI()
    {
        int curr = (int)damageable.CurrHealth;
        int max = (int)damageable.MaxHealth;
        UiManager.Instance.SetHealthUI(curr, max);
    }

    public void InitShip(bool stackFull = false)
    {
        if(stackFull) UiManager.Instance.CreateText("Restore All!", transform.position);

        damageable.InitHealth();
        UpdateHealthUI();
    }

    public void SetSystem(UpgradeField _type, float amount)
    {
        switch (_type)
        {
            case UpgradeField.Shield:
                damageable.SetMaxHealth(amount * 100, true);
                UpdateHealthUI();
                break;
            case UpgradeField.OnImpact:
                impactable.SetDamageAmount(amount * 100);
                break;
            case UpgradeField.MultiShot:
                shooter.SetMultiShot((int)amount);
                break;
        }
    }
}
