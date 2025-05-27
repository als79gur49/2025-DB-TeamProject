using UnityEditor.Rendering;
using UnityEditor.UI;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AttackState : IState
{
    private float cooldown = 1f;
    private float timer;

    private float rotateMultiple = 3.0f;

    public void Enter(AIInput input)
    {
        timer = 0;

        input.Animation.SetIdle();

        //Debug.Log("Attack Enter");
    }

    public void Execute(AIInput input)
    {
        timer += Time.deltaTime;

        Vector3 direction = (input.target.transform.position - input.self.transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        input.self.transform.rotation = Quaternion.Lerp(input.self.transform.rotation, targetRotation, Time.deltaTime * rotateMultiple);

        if(timer > cooldown)
        {
            Attack(input);

            timer = 0;
        }

        //Debug.Log("Attack Execute");
    }

    public void Exit(AIInput input)
    {
        //Debug.Log("Attack Exit");
    }

    private void Attack(AIInput input)
    {
        if(input.self.TryGetComponent<IAttack>(out IAttack attack))
        {
            attack.Attack();
        }
    }

}
