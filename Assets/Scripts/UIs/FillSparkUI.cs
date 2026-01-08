using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FillSparkUI : MonoBehaviour
{
    public Image fillImage;
    public Image uiPrefab;

    public int sparkCount = 1;
    public float fadeSpeed = 1;
    public float randomAngle = 20;
    public float rotateAngle = 0;
    public float popPower = 100;
    public float grav = 100;
    public float interval = 0.2f;

    private void Start()
    {
        DoSpark();
    }

    // Update is called once per frame
    void Update()
    {
        MoveToFillPoint();     
    }

    void MoveToFillPoint()
    {
        // fillAmount에 따른 끝 위치 계산
        float fillAmount = fillImage.fillAmount;
        RectTransform rect = fillImage.GetComponent<RectTransform>();

        // 채워진 이미지의 가로 크기
        float imageWidth = rect.rect.width;

        // 채워진 끝 위치 (fillAmount를 곱하여 실제 끝 위치 구하기)
        float fillEndPosition = imageWidth * fillAmount;

        // 이동시킬 이미지의 새로운 위치 설정
        Vector3 newPosition = rect.position;
        newPosition.x += fillEndPosition;
        newPosition.x -= rect.rect.width / 2;

        // 이동할 이미지의 위치 갱신
        transform.position = newPosition;
    }


    void DoSpark(int count = 1)
    {
        IEnumerator Cr()
        {
            while (true)
            {
                for (int i = 0; i < count; i++)
                {
                    Transform tf = Instantiate(uiPrefab, transform).transform;

                    float angle = Random.Range(-randomAngle, randomAngle) + rotateAngle;
                    Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

                    float power = popPower;
                    Vector3 vec = rotation * Vector3.up * power;

                    Rigidbody2D rbody = tf.GetComponent<Rigidbody2D>();
                    rbody.AddForce(vec, ForceMode2D.Impulse);
                    rbody.AddTorque(Random.Range(-90, 90));
                    rbody.gravityScale = grav;

                    Graphic gph = tf.GetComponent<Graphic>();
                    gph.color = fillImage.color;

                    FadeUI fade = tf.gameObject.AddComponent<FadeUI>();
                    fade.fadeSpeed = fadeSpeed;
                }

                yield return new WaitForSeconds(interval);
            }            
        }

        StartCoroutine(Cr());
    }
}

