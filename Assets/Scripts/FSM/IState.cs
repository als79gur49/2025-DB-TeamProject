public interface IState
{
    //���� �� 1ȸ ȣ��
    public void Enter(AIInput input);

    //���� ������Ʈ �� �� �� �����Ӹ��� ȣ��
    public void Execute(AIInput input);

    //���� ������ �� 1ȸ ȣ��
    public void Exit(AIInput input);

}
