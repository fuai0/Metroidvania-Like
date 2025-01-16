using UnityEngine;

public class PlayerBlackholeState : PlayerState
{
    private float flyTime = .4f;
    private bool skillUsed;

    private float defaultGravity;
    public PlayerBlackholeState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
    }

    public override void Enter()
    {
        base.Enter();

        skillUsed = false;
        stateTimer = flyTime;

        defaultGravity = rb.gravityScale;
        rb.gravityScale = 0;
    }

    public override void Exit()
    {
        base.Exit();
        rb.gravityScale = defaultGravity;
        player.fx.MakeTransprent(false);
    }

    public override void Update()
    {
        base.Update();

        if(stateTimer > 0)
        {
            rb.linearVelocity = new Vector2(0, 15);
        }

        if(stateTimer < 0)
        {
            rb.linearVelocity = new Vector2(0, -.1f);

            if(!skillUsed)
            {
                if (player.skill.blackHole.CanUseSkill())
                {
                    skillUsed = true;
                }
            }
        }

        if(player.skill.blackHole.SkillCompleted())
        {
            stateMachine.ChangeState(player.airState);
        }
    }
}
