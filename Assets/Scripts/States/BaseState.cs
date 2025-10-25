public abstract class BaseState : IState
{
    protected readonly BaseUnitController controller;

    protected BaseState(BaseUnitController _controller)
    {
        controller = _controller;
    }

    public virtual void FixedUpdate()
    {

    }

    public virtual void OnEnter()
    {

    }

    public virtual void OnExit()
    {

    }

    public virtual void Update()
    {

    }
}
