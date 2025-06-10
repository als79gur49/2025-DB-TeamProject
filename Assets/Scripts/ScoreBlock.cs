using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;



public class ScoreBlock : MonoBehaviour
{
    private int score = 10;

    //private float upHeight = 2.3f;
    //private float upDuration = 0.5f;

    private bool flag = true;
    //private float absorbedTime = 1f;

    public void Setup(int score, Color c, float size)
    {
        this.score = score;

        MeshRenderer renderer = GetComponent<MeshRenderer>();
        MaterialPropertyBlock block = new MaterialPropertyBlock();

        renderer.GetPropertyBlock(block);
        block.SetColor("_BaseColor", c); // "_BaseColor"일 수도 있음 (URP 등)
        renderer.SetPropertyBlock(block);

        transform.localScale = Vector3.one * size;
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
        // 포물선 형태 이동 이후 둥둥 떠다니는 트윈
        Sequence launchSeq = DOTween.Sequence();

        // x, z축 무작위 방향 이동값
        float xOffset = 3f * Random.Range(-1f, 1f);
        float zOffset = 3f * Random.Range(-1f, 1f);
        float upHeight = 2f;
        float duration = 1.2f;

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + new Vector3(xOffset, 0f, zOffset);
        Vector3 midPos = startPos + new Vector3(xOffset * 0.3f, upHeight, zOffset * 0.3f);

        Vector3[] path = new Vector3[] { startPos, midPos, endPos };

        launchSeq.Append(transform.DOPath(path, duration, PathType.CatmullRom)
            .SetEase(Ease.OutSine))
            .AppendCallback(() =>
            {
                // 도착 후 둥둥 떠다니기 시작
                transform.DOMoveY(0.5f, 1f)
                    .SetRelative(true)
                    .SetEase(Ease.InOutQuad)
                    .SetLoops(-1, LoopType.Yoyo);
            });

        /*
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
                .Join(transform.DOMoveX(5f * Random.Range(-1f, 1), upDuration * 0.8f) // x, z축 이동
                    .SetRelative(true)
                    .SetEase(Ease.OutQuad))
                .Join(transform.DOMoveZ(5f * Random.Range(-1f, 1), upDuration * 0.8f)
                    .SetRelative(true)
                    .SetEase(Ease.OutQuad))
                .AppendCallback(() =>
                {
                    //yoyoTween.Goto(Random.Range(0, 1f), true); // 해당 위치로 바로 가서 실행해서, 순간이동하는 문제점
                    yoyoTween.Play();
                });
        */
    }

    public void YoYoMoving()
    {
        Tween yoyoTween = transform.DOMoveY(0.5f, 1f).SetRelative(true).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
        yoyoTween.Goto(Random.Range(0, 1f), true);
    }

    // 접촉되어 흡수될 때
    public void AbsorbScoreObject(GameObject target, Player player)
    {
        if (flag == true)
        {
            float t = 1f;

            Sequence rotateSeq = DOTween.Sequence();
            rotateSeq.Append(transform.DORotate(new Vector3(0, 1000, 0), t, RotateMode.FastBeyond360)) //제자리 회전
                .Join(transform.DOMove(target.transform.position, t).SetEase(Ease.InOutBack)) // 타겟 방향으로 이동
                .Join(transform.DOScale(0, t).SetEase(Ease.InElastic)) // 스케일 조정
                .AppendCallback(() => AddScoreTo(player));

            flag = false;
        }
    }

    private void AddScoreTo(Player player)
    {
        player.AddScore(score);
        Destroy(gameObject);
        EntityGameManager.OnPlayerScoreAdd(score);
    }
}
