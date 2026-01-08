using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 탄환이 사라지는 조건 3가지
// 1. 생성 후 일정 시간이 경과.
// 2. 화면 밖으로 나감
// 3. 물체 충돌
public class BulletBase : MonoBehaviour
{   
    [Header("Sounds")]   
    public AudioClip onHitSound;

    [Space]
    [SerializeField] bool destoryOnHit = true;
    [SerializeField] bool removeOtherBullet = false;
    int ownerLayer;

    [Space]
    public GameObject hitEffect;    
    public LayerMask targetLayer; // 해당 오브젝트와 충돌을 검사할 레이어        
    public int damage;
    public int impact;
    public float movePower;

    public float liveTime = 10;
    float spwanedTime = 0;    

    Rigidbody2D rBody;
    protected Rigidbody2D RBody
    {
        get {
            rBody = GetComponent<Rigidbody2D>();
            return rBody;
        }
    }

    SpriteRenderer sprite;
    protected SpriteRenderer Sprite {
        get {
            sprite = GetComponent<SpriteRenderer>();
            return sprite;
        }
    }

    Collider2D coll;
    protected Collider2D Coll
    {
        get
        {
            coll = GetComponent<Collider2D>();
            return coll;
        }
    }

    TrailRenderer trail;

    private void Update()
    {  
        spwanedTime += Time.deltaTime;
        if (liveTime < spwanedTime)
        {
            OnHitDestory(null, false);
        } 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(removeOtherBullet) RemoveOtherBullet(other);

        // targetLayer 검사
        if (1 << other.gameObject.layer == targetLayer.value)
        {
            OnHitDestory(other);
        }
    }

    void RemoveOtherBullet(Collider2D other)
    {
        BulletBase bullet = other.GetComponent<BulletBase>();

        if (!bullet) return;
        if (bullet.ownerLayer == ownerLayer) return;

        Destroy(bullet.gameObject);
    }

    protected void OnHitDestory(Collider2D hitColl = null, bool playSound = true)
    {        
        if (destoryOnHit) Destroy(gameObject);
        
        if(playSound) SoundManager.Instance.PlaySound(onHitSound);

        if (trail)
        {
            //Debug.Log("trail disttach");
            trail.transform.parent = null;
            trail.autodestruct = true;
        }

        if (hitEffect) 
        {
            //Debug.Log("Instantiate hitEffect");            
            Vector2 point = hitColl ? hitColl.ClosestPoint(transform.position) : transform.position;
            GameObject go = Instantiate(hitEffect, point, transform.rotation);

            // if hit effect is pulse, the pulse do damage (not this projectile)
            Pulse pulse = go.GetComponent<Pulse>();
            if (pulse)
            {
                pulse.Init(ownerLayer, targetLayer, damage, impact, 0f, 99f);
                return;
            }
        }

        if (hitColl)
        {
            // 피해주기
            Damageable damageable = hitColl.GetComponent<Damageable>();
            if (!damageable) damageable = hitColl.attachedRigidbody.GetComponent<Damageable>();
            if (damageable)
            {
                damageable.GetDamaged(damage, gameObject);
            }

            // 힘 가하기       
            Rigidbody2D rbody = hitColl.attachedRigidbody;
            if (rbody)
            {
                Vector2 dir = (hitColl.transform.position - transform.position).normalized;
                //Vector2 dir = transform.up;            
                rbody.AddForce(dir * impact, ForceMode2D.Impulse);
                //Debug.Log(name + " to " + rbody.name + " || " + dir * impact);


            }
        }        
    }

    // shooter에서 생성 시 호출
    virtual public void Init(int ownerLayer, int targetLayer, int damage, int impact, float movePower, float liveTime, AudioClip onHitSound = null)
    {
        //Debug.Log("init");
        this.ownerLayer = ownerLayer;
        this.targetLayer = targetLayer;
        this.damage = damage;
        this.impact = impact;
        this.movePower = movePower;
        this.liveTime = liveTime;
        this.onHitSound = onHitSound;        

        if(RBody) RBody.linearVelocity = transform.up * movePower;
        //Debug.Log("velocity : " + rbody.velocity);

        //ColorCtrl colorCtrl = GetComponent<ColorCtrl>();
        //Color color = new Color(colorR, colorG, colorB);
        //colorCtrl.SetColor(color);
    }
}
