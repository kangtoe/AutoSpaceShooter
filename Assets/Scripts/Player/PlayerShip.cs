using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Damageable))]
public class PlayerShip : MonoBehaviour
{    
    [Header("systems")]
    [SerializeField] ShooterBase shooter;
    [SerializeField] HeatSystem heatSystem;
    [SerializeField] MoveStandard movement;
    [SerializeField] Impactable impactable;
    [SerializeField] BrakeSystem brakeSystem;
    [SerializeField] Damageable damageable;

    [Header("amounts")]
    [SerializeField] float heatPerShot = 5;

    [Header("sounds")]
    [SerializeField] AudioClip failSound;

    bool MoveForwardInput => InputManager.Instance.MoveForwardInput;
    Vector2 MoveDirectionInput => InputManager.Instance.MoveDirectionInput;

    bool FireInput => InputManager.Instance.FireInput;    

    bool canPrintOverHeat = true;

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

    // Update is called once per frame
    void FixedUpdate()
    {        
        MoveCheck();        
    }

    private void Update()
    {
        bool onCombat = GameManager.Instance.GameState == GameState.OnCombat;
        bool onTitle = GameManager.Instance.GameState == GameState.OnTitle;

        if (!onCombat && !onTitle) return;

        FireCheck();
    }

    void MoveCheck()
    {
        if (MoveForwardInput)
        {
            movement.Move();
        }

        if(MoveDirectionInput != Vector2.zero)
        {
            movement.Move(MoveDirectionInput);
        }
    }

    void FireCheck()
    {
        if (!FireInput)
        {
            canPrintOverHeat = true;
            return;
        } 

        if (heatSystem.OverHeated)
        {
            if (canPrintOverHeat)
            {
                UiManager.Instance.CreateText("OverHeat!", transform.position);
                SoundManager.Instance.PlaySound(failSound);
            } 

            canPrintOverHeat = false;
            return;
        }
        else
        {
            canPrintOverHeat = true;
        }
        
        bool fired = shooter.TryFire();
        if (fired)
        {
            heatSystem.AdjustHeat(heatPerShot);
        }
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

        heatSystem.InitHeat();

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
            case UpgradeField.Heat:
                heatPerShot = amount;
                break;
        }
    }
}
