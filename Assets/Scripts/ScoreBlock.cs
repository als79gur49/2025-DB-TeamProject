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

    // ��� 1ȸ�� �۵� �÷���
    private bool flag = true;
    private float absorbedTime = 1f;

    private MemoryPool<ScoreBlock> memoryPool;

    // �⺻ �����Ǵ� ��ü ����� ����
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

        // �޽� �ν��Ͻ�ȭ ���Ѽ� ����
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

    // Entity �׿��� ����� ������ ���
    public void LaunchUpwards()
    {
        Sequence launchSeq = DOTween.Sequence();
        Tween yoyoTween = transform.DOMoveY(0.5f, 1f).SetRelative(true).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo).Pause();

        // launchSeq.Append(transform.DOMoveY(upHeight, upDuration) //y�� �̵�
        //             .SetRelative(true)
        //             .SetEase(Ease.OutQuad))
        //         .Join(transform.DOMoveX(0.5f * Random.Range(-1f, 1), upDuration * 0.8f) // x, z�� �̵�
        //             .SetRelative(true)
        //             .SetEase(Ease.OutQuad))
        //         .Join(transform.DOMoveZ(0.5f * Random.Range(-1f, 1), upDuration * 0.8f)
        //             .SetRelative(true)
        //             .SetEase(Ease.OutQuad))
        //         .Insert(upDuration * 0.7f, yoyoTween); //���� �յ� ���ٴϴ� �ִϸ��̼�

        launchSeq.Append(transform.DOMoveY(upHeight, upDuration) //y�� �̵�
                    .SetRelative(true)
                    .SetEase(Ease.OutQuad))
                .Join(transform.DOMoveX(0.5f * Random.Range(-1f, 1), upDuration * 0.8f) // x, z�� �̵�
                    .SetRelative(true)
                    .SetEase(Ease.OutQuad))
                .Join(transform.DOMoveZ(0.5f * Random.Range(-1f, 1), upDuration * 0.8f)
                    .SetRelative(true)
                    .SetEase(Ease.OutQuad))
                .AppendCallback(() =>
                {
                    //yoyoTween.Goto(Random.Range(0, 1f), true); // �ش� ��ġ�� �ٷ� ���� �����ؼ�, �����̵��ϴ� ������
                    yoyoTween.Play();
                });

    }

    public void YoYoMoving()
    {
        Tween yoyoTween = transform.DOMoveY(0.5f, 1f).SetRelative(true).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
        yoyoTween.Goto(Random.Range(0, 1f), true);
    }

    // ���˵Ǿ� ����� ��
    public void AbsorbScoreObject(GameObject target, Entity entity)
    {
        if (flag == true)
        {
            float t = 1f;

            // �ӽ�: target�� ���� ��ġ ���
            Vector3 dir = (target.GetComponent<NavMeshAgent>().destination - target.transform.position) * target.GetComponent<NavMeshAgent>().speed * 0.7f;

            // ȸ�� Ʈ��
            Tween rotateTween = transform.DORotate(new Vector3(0, 1000, 0), t, RotateMode.FastBeyond360);
            // �ణ Ŀ���� �۾����� Ʈ��
            Tween scaleTween = transform.DOScale(0, t).SetEase(Ease.InElastic);
            // ��ǥ ��ġ �̵� Ʈ��. 
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

            // ������ ������Ʈ �������� Ÿ�� ��ġ ����
            DOTween.To(() => 0f, x => {
                // �� �����Ӹ��� Ÿ���� ���� ��ġ�� ��ǥ�� ������Ʈ
                if (targetTransform != null && moveTweener != null)
                {
                    moveTweener.ChangeEndValue(targetTransform.position, true);
                }
            }, 1f, t);

            rotateSeq.Append(rotateTween) // ���ڸ� ȸ��
                .Join(moveTweener) // Ÿ�� �������� �̵�
                .Join(scaleTween) // ������ ����
                .OnUpdate(() => {
                    // �� �����Ӹ��� ���� Ÿ�� ��ġ�� �̵�
                    if (tmpT != null)
                    {
                        Debug.Log("upd");
                        targetTransform = tmpT.transform;
                        moveTweener.ChangeEndValue(targetTransform.position, false);
                    }
                })
                .AppendCallback(() => AddScoreTo(entity)); // ������ ������ ���� �߰�

            flag = false;
        }
 */