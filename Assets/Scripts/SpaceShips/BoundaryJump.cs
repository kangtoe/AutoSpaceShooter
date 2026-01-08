using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// 오브젝트가 화면 밖으로 벗어났을 때,
// 반대편 (오른쪽 <-> 왼쪽 / 위쪽 <-> 아래쪽) 가장자리 이동
public class BoundaryJump : MonoBehaviour
{
    [SerializeField]
    int destoryJumpCount = 5;

    [SerializeField]
    bool addForceOppsiteOnJump = false;

    [SerializeField]
    bool initVelocityOnJump = true;    

    Rigidbody2D rbody;    
    Collider2D ShipCollider; // player ship 사이즈를 알아오기 위해 사용
    //TrailEffect effect;

    Vector2 cameraSize;
    Vector2 shipSize;

    float jumpableTime = 0;
    float jumpInterval = 0.5f; // 한번 가장자리 이동이 실행되면, 직후 이 기간동안은 다시 가장자리 이동체크를 중단

    [HideInInspector]
    public UnityEvent onJump;

    // Start is called before the first frame update
    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        ShipCollider = GetComponentInChildren<Collider2D>();
        //effect = GetComponentInChildren<TrailEffect>();

        cameraSize = GetCameraSize();
        shipSize = GetPlayerShipBoundSize();

        float interval = 0.1f;
        InvokeRepeating(nameof(JumpToOppsiteCheck), 0, interval);
    }

    Vector2 GetCameraSize()
    {
        float cameraSizeX = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x * 2;
        float cameraSizeY = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0)).y * 2;
        return new Vector2(cameraSizeX, cameraSizeY);
    }

    Vector2 GetPlayerShipBoundSize()
    {
        return ShipCollider.bounds.size;
    }

    // 화면에서 벗어난 가장자리 방향에 따라 반대편 가장자리로 이동 
    void JumpToOppsiteCheck()
    {
        if (Time.time < jumpableTime) return;
        
        float moveX = cameraSize.x / 2 + shipSize.x / 2;
        float moveY = cameraSize.y / 2 + shipSize.y / 2;
        Vector3 pos = transform.position;

        // x축
        if (pos.x < -moveX) JumpToOppsite(Edge.Right);
        else if (pos.x > moveX) JumpToOppsite(Edge.Left); 

        // y축
        if (pos.y < -moveY) JumpToOppsite(Edge.Up);
        else if (pos.y > moveY) JumpToOppsite(Edge.Down);

        void JumpToOppsite(Edge jumpedEdge)
        {
            //Debug.Log("JumpToOppsite");
            //if (effect) effect.TrailDistachRPC();

            if (jumpedEdge == Edge.Up)      pos = new Vector2(pos.x, moveY);
            if (jumpedEdge == Edge.Down)    pos = new Vector2(pos.x, -moveY);            
            if (jumpedEdge == Edge.Right)   pos = new Vector2(moveX, pos.y);
            if (jumpedEdge == Edge.Left)    pos = new Vector2(-moveX, pos.y);
            transform.position = pos;

            AddForceToOppsite(jumpedEdge);
            onJump.Invoke();

            jumpableTime = Time.time + jumpInterval;

            if (destoryJumpCount >= 0)
            {                
                if (destoryJumpCount == 0) Destroy(gameObject);
                else destoryJumpCount--;
            }            
        }

        // 가장 자리 이동 후 반대편으로 약간 밀어준다
        // -> 점프 직후 속력이 충분하지 못해 다시 점프가 일어나는 현상을 줄여준다
        void AddForceToOppsite(Edge jumpedEdge)
        {
            Vector2 dir = Vector2.zero;

            if (addForceOppsiteOnJump)
            {
                if (jumpedEdge == Edge.Up) dir = Vector2.down;
                if (jumpedEdge == Edge.Down) dir = Vector2.up;
                if (jumpedEdge == Edge.Right) dir = Vector2.left;
                if (jumpedEdge == Edge.Left) dir = Vector2.right;
            }
            else
            {
                dir = transform.up;
            }
            

            if (initVelocityOnJump) rbody.linearVelocity = Vector2.zero;
            float movePower = 2f;
            rbody.AddForce(rbody.mass * dir * movePower, ForceMode2D.Impulse);
        }
    }    
}
