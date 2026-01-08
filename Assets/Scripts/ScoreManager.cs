using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoSingleton<ScoreManager>
{
    int currScore = 0;
    public int CurrScore => currScore;

    private void Start()
    {
        UpdateScoreUI();
    }

    public void AddScore(int score)
    {        
        // 내 로컬 점수 증가
        currScore += score;
        UpdateScoreUI();

        // 슈터 경험치 획득
        //GameObject myShip = PlayerSpwaner.Instance.GetMyPlayer();
        //PlayerShooter shooter = myShip.GetComponent<PlayerShooter>();
        //shooter.GetExp(score);
    }

    void UpdateScoreUI()
    {
        UiManager.Instance.SetScoreText(currScore);
    }
}
