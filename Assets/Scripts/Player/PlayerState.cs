using UnityEngine;

public class PlayerState
{
    protected PlayerStateMachine stateMachine;
    protected Player player;

    protected Rigidbody2D rb;

    protected float xInput;
    protected float yInput;
    protected string animBoolName;

    protected float stateTimer;
    protected bool triggerCalled;

    public PlayerState(Player _player,PlayerStateMachine _stateMachine, string _animBoolName)
    {
        this.player = _player;
        this.stateMachine = _stateMachine;
        this.animBoolName = _animBoolName;
    }

    public virtual void Enter()
    {
        player.anim.SetBool(animBoolName, true);
        rb = player.rb;
        triggerCalled = false;
    }

    public virtual void Update()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");

        player.anim.SetFloat("yVelocity", rb.linearVelocity.y);

        stateTimer -= Time.deltaTime;
    }

    public virtual void Exit()
    {
        player.anim.SetBool(animBoolName, false);
    }


    public virtual void AnimationFinishTrigger()
    {
        triggerCalled = true;
    }


}
