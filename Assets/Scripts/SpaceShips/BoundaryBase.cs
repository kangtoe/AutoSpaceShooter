using UnityEngine;

// 화면 경계 처리 베이스 클래스
// BoundaryJump, BoundaryDeath 등이 상속
[DisallowMultipleComponent]
public abstract class BoundaryBase : MonoBehaviour
{
    protected Collider2D shipCollider;
    protected Vector2 cameraSize;
    protected Vector2 shipSize;

    float checkInterval = 0.1f;

    protected virtual void Start()
    {
        shipCollider = GetComponentInChildren<Collider2D>();
        if (shipCollider == null)
        {
            Debug.LogError("BoundaryBase requires Collider2D component!");
            enabled = false;
            return;
        }

        cameraSize = GetCameraSize();
        shipSize = GetShipBoundSize();

        InvokeRepeating(nameof(CheckBoundary), 0, checkInterval);
    }

    Vector2 GetCameraSize()
    {
        float cameraSizeX = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x * 2;
        float cameraSizeY = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0)).y * 2;
        return new Vector2(cameraSizeX, cameraSizeY);
    }

    Vector2 GetShipBoundSize()
    {
        return shipCollider.bounds.size;
    }

    void CheckBoundary()
    {
        float boundaryX = cameraSize.x / 2 + shipSize.x / 2;
        float boundaryY = cameraSize.y / 2 + shipSize.y / 2;
        Vector3 pos = transform.position;

        // 화면 밖으로 벗어났는지 체크
        if (pos.x < -boundaryX || pos.x > boundaryX ||
            pos.y < -boundaryY || pos.y > boundaryY)
        {
            OnBoundaryExit(pos, boundaryX, boundaryY);
        }
    }

    // 경계를 벗어났을 때의 동작 (파생 클래스에서 구현)
    protected abstract void OnBoundaryExit(Vector3 currentPos, float boundaryX, float boundaryY);
}
