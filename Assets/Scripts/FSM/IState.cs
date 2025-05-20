public interface IState
{
    //시작 시 1회 호출
    public void Enter(Entity entity);

    //상태 업데이트 할 때 매 프레임마다 호출
    public void Execute(Entity entity);

    //상태 종료할 때 1회 호출
    public void Exit(Entity entity);

}
