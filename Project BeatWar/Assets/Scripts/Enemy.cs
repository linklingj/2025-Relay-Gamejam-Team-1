using System.Collections;
using System.Collections.Generic;
using SeolMJ;
using UnityEngine;

public class Enemy : Entity
{
    public Enemy Original { get; private set; }

    public Pattern Weakness { get; private set; }
    
    [SerializeField] private Vector2[] spawnPoint = new Vector2[4];
    [SerializeField] GameObject subEnemyPrefab;

    public virtual void Enable(Enemy original)
    {
        Original = original;
        Weakness = PatternUtils.Random(Health);
        Beat = BeatManager.Instance.CurrentBeat;
        StartCoroutine(SpawnRoutine(BeatManager.Instance.CurrentBeat, BeatManager.Instance.CurrentSubBeat));
        base.Enable();
    }

    protected override void Update()
    {
        base.Update();
        // 적 못잡았는지 때 없애기
        if (Beat < BeatManager.Instance.CurrentBeat - 1)
        {
            Disable();
            Player.Instance.Deal(Health);
            EnemyManager.Instance.OnHitCaller.EmitAt(Player.Instance.transform.position);
        }
    }
    
    IEnumerator SpawnRoutine(int currentBeat, int currentSubBeat)
    {
        int lastSubBeat = currentSubBeat-1;

        while (true)
        {
            // 조건 체크: 게임중이며 현재 Beat 제한 안에 있어야 함
            if (!(StageManager.Instance.Started 
                  && !StageManager.Instance.Ended 
                  && BeatManager.Instance.CurrentBeat < currentBeat + 1))
            {
                yield break; // 조건 위배시 코루틴 종료
            }

            // subBeat가 바뀌었는지 확인
            if (BeatManager.Instance.CurrentSubBeat != lastSubBeat)
            {
                lastSubBeat = BeatManager.Instance.CurrentSubBeat;

                // subEnemy 생성
                if (Weakness.HasFlag(PatternUtils.ToPattern(lastSubBeat)))
                    SpawnSubEnemy(lastSubBeat);
            }

            yield return null; // 다음 프레임까지 대기
        }
    }
    
    List<GameObject> spawnedSubEnemies = new List<GameObject>();
    
    void SpawnSubEnemy(int subBeat)
    {
        Vector2 spawnPos = spawnPoint[subBeat % 4];
        GameObject subEnemy = Instantiate(subEnemyPrefab, transform);
        subEnemy.transform.localPosition = spawnPos;
        spawnedSubEnemies.Add(subEnemy);
    }

    public override void Disable()
    {
        foreach (var subEnemy in spawnedSubEnemies) Destroy(subEnemy);
        
        base.Disable();
    }
}
