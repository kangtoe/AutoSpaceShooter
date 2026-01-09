using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    public GameObject diePrefab;

    [HideInInspector]
    public UnityEvent onDead = new UnityEvent();

    [HideInInspector]
    public UnityEvent onDamaged = new UnityEvent();

    [SerializeField] float maxHealth = 100;
    [SerializeField] float? currHealth = null;

    [Header("Sounds")]
    public AudioClip deathSound;
    public AudioClip hitSound;

    public float MaxHealth => maxHealth;
    public float CurrHealth
    {
        get{
            if (currHealth == null) currHealth = maxHealth;
            return currHealth.Value;
        }
    }

    bool isDead;
    public bool IsDead => isDead;


    // Start is called before the first frame update
    protected void Start()
    {
        // 사망 시 이벤트 체인 등록
        // TODO :        
        // 플레이어 기체 파괴시 게임 오버 함수 추가 등록
        {
            onDead.AddListener(delegate {
                //Debug.Log("onDeadLocal");

                
                if (diePrefab)
                {
                    Instantiate(diePrefab, transform.position, diePrefab.transform.rotation);                    
                }
                
                Destroy(gameObject);
            });
        }        
    }

    virtual public void GetDamaged(float damage, GameObject attacker = null)
    {
        if (isDead) return;

        currHealth = CurrHealth - damage;
        if (currHealth < 0) currHealth = 0;
        onDamaged.Invoke();      

        // dead check
        if (currHealth == 0)
        {
            SoundManager.Instance.PlaySound(deathSound);

            isDead = true;
            onDead.Invoke();
        }
        else
        {
            SoundManager.Instance.PlaySound(hitSound);
        }        
    }    

    public void SetMaxHealth(float amount, bool adjustCurrHealth = false)
    {
        float adjust = amount - maxHealth;

        maxHealth += adjust;
        if (adjustCurrHealth) currHealth += adjust;
    }

    public void InitHealth()
    {
        currHealth = maxHealth;
    }
}
