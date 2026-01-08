using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MoveWave))]
public class BulletWaveTwin : BulletBase
{
    [HideInInspector] public bool bSpawnTwin = true;
    
    MoveWave moveWave;
    MoveWave MoveWave {
        get {
            if(!moveWave) moveWave = GetComponent<MoveWave>();
            return moveWave;
        }
    }

    public override void Init(int ownerLayer, int targetLayer, int damage, int impact, float movePower, float liveTime, AudioClip onHitSound = null)
    {
        base.Init(ownerLayer, targetLayer, damage, impact, movePower, liveTime, onHitSound);

        RBody.linearVelocity = Vector2.zero;
        MoveWave.movePower = movePower;

        // twin 생성
        if (bSpawnTwin)
        {
            BulletWaveTwin twin = Instantiate(this);
            twin.bSpawnTwin = false;
            twin.Init(ownerLayer, targetLayer, damage, impact, movePower, liveTime);            

            twin.MoveWave.waveInverse = !MoveWave.waveInverse;
        }
    }
}
