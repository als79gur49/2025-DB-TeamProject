public interface IState
{
    //���� �� 1ȸ ȣ��
    public void Enter(Entity entity);

    //���� ������Ʈ �� �� �� �����Ӹ��� ȣ��
    public void Execute(Entity entity);

    //���� ������ �� 1ȸ ȣ��
    public void Exit(Entity entity);

}
