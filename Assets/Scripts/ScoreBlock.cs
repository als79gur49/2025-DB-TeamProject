using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Buffers;
using DG.Tweening.Core.Easing;
using UnityEngine.AI;



public class ScoreBlock : MonoBehaviour
{
    private int score = 10;

    private float upHeight = 2.3f;
    private float upDuration = 0.5f;

    private Transform targetTransform;

    // 흡수 1회만 작동 플래그
    private bool flag = true;
    private float absorbedTime = 1f;

    private MemoryPool<ScoreBlock> memoryPool;

    // 기본 생성되는 객체 재생성 여부
    private ScoreBlockSpawner spawner;
    private bool canRespawn;
    private Vector3 originPosition;

    public void Setup(int score, Color c, float size,
        MemoryPool<ScoreBlock> memoryPool,
        bool canRespawn,
            ScoreBlockSpawner spawner = null)
    {
        this.score = score;

        flag = true;

        // 메쉬 인스턴스화 시켜서 각자
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        
        renderer.GetPropertyBlock(block);
        block.SetColor("_BaseColor", c); 
        renderer.SetPropertyBlock(block);
        
        transform.localScale = Vector3.one * size;

        this.memoryPool = memoryPool;
        
        this.canRespawn = canRespawn;
            this.spawner = spawner;
            this.originPosition = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Player>(out Player player))
        {
            AbsorbScoreObject(other.gameObject, player);
        }
    }

    // Entity 죽여서 쏟아져 나오는 모션
    public void LaunchUpwards()
    {
        Sequence launchSeq = DOTween.Sequence();
        Tween yoyoTween = transform.DOMoveY(0.5f, 1f).SetRelative(true).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo).Pause();

        // launchSeq.Append(transform.DOMoveY(upHeight, upDuration) //y축 이동
        //             .SetRelative(true)
        //             .SetEase(Ease.OutQuad))
        //         .Join(transform.DOMoveX(0.5f * Random.Range(-1f, 1), upDuration * 0.8f) // x, z축 이동
        //             .SetRelative(true)
        //             .SetEase(Ease.OutQuad))
        //         .Join(transform.DOMoveZ(0.5f * Random.Range(-1f, 1), upDuration * 0.8f)
        //             .SetRelative(true)
        //             .SetEase(Ease.OutQuad))
        //         .Insert(upDuration * 0.7f, yoyoTween); //이후 둥둥 떠다니는 애니메이션

        launchSeq.Append(transform.DOMoveY(upHeight, upDuration) //y축 이동
                    .SetRelative(true)
                    .SetEase(Ease.OutQuad))
                .Join(transform.DOMoveX(0.5f * Random.Range(-1f, 1), upDuration * 0.8f) // x, z축 이동
                    .SetRelative(true)
                    .SetEase(Ease.OutQuad))
                .Join(transform.DOMoveZ(0.5f * Random.Range(-1f, 1), upDuration * 0.8f)
                    .SetRelative(true)
                    .SetEase(Ease.OutQuad))
                .AppendCallback(() =>
                {
                    //yoyoTween.Goto(Random.Range(0, 1f), true); // 해당 위치로 바로 가서 실행해서, 순간이동하는 문제점
                    yoyoTween.Play();
                });

    }

    public void YoYoMoving()
    {
        Tween yoyoTween = transform.DOMoveY(0.5f, 1f).SetRelative(true).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
        yoyoTween.Goto(Random.Range(0, 1f), true);
    }

    // 접촉되어 흡수될 때
    public void AbsorbScoreObject(GameObject target, Entity entity)
    {
        if (flag == true)
        {
            float t = 1f;

            // 임시: target의 예상 위치 계산
            Vector3 dir = (target.GetComponent<NavMeshAgent>().destination - target.transform.position) * target.GetComponent<NavMeshAgent>().speed * 0.7f;

            // 회전 트윈
            Tween rotateTween = transform.DORotate(new Vector3(0, 1000, 0), t, RotateMode.FastBeyond360);
            // 약간 커졌다 작아지는 트윈
            Tween scaleTween = transform.DOScale(0, t).SetEase(Ease.InElastic);
            // 목표 위치 이동 트윈. 
            Tweener moveTweener = transform.DOMove(target.transform.position + dir * t, t)
                .SetEase(Ease.InOutBack)
                .OnComplete(() => AddScoreTo(entity));

            flag = false;
        }
    }

    private void AddScoreTo(Entity entity)
    {
        entity.AddScore(score);
        
        memoryPool.DeactivatePoolItem(this);

        if(canRespawn)
        {
            spawner.EnQueuePosition(originPosition);
        }
    }
}

/*
 if (flag == true)
        {
            float t = 1f;
            GameObject tmpT = target;
            targetTransform = target.transform;

            Sequence rotateSeq = DOTween.Sequence();

            Tween rotateTween = transform.DORotate(new Vector3(0, 1000, 0), t, RotateMode.FastBeyond360);
            Tweener moveTweener = transform.DOMove(targetTransform.position, t)
                .SetEase(Ease.InOutBack);
            Tween scaleTween = transform.DOScale(0, t).SetEase(Ease.InElastic);

            // 별도의 업데이트 로직으로 타겟 위치 추적
            DOTween.To(() => 0f, x => {
                // 매 프레임마다 타겟의 현재 위치로 목표점 업데이트
                if (targetTransform != null && moveTweener != null)
                {
                    moveTweener.ChangeEndValue(targetTransform.position, true);
                }
            }, 1f, t);

            rotateSeq.Append(rotateTween) // 제자리 회전
                .Join(moveTweener) // 타겟 방향으로 이동
                .Join(scaleTween) // 스케일 조정
                .OnUpdate(() => {
                    // 매 프레임마다 현재 타겟 위치로 이동
                    if (tmpT != null)
                    {
                        Debug.Log("upd");
                        targetTransform = tmpT.transform;
                        moveTweener.ChangeEndValue(targetTransform.position, false);
                    }
                })
                .AppendCallback(() => AddScoreTo(entity)); // 시퀀스 끝나면 점수 추가

            flag = false;
        }
 */