using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Buffers;



public class ScoreBlock : MonoBehaviour
{
    private int score = 10;

    private float upHeight = 2.3f;
    private float upDuration = 0.5f;

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

            Sequence rotateSeq = DOTween.Sequence();
            rotateSeq.Append(transform.DORotate(new Vector3(0, 1000, 0), t, RotateMode.FastBeyond360)) //제자리 회전
                .Join(transform.DOMove(target.transform.position, t).SetEase(Ease.InOutBack)) // 타겟 방향으로 이동
                .Join(transform.DOScale(0, t).SetEase(Ease.InElastic)) // 스케일 조정
                .AppendCallback(() => AddScoreTo(entity));

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
