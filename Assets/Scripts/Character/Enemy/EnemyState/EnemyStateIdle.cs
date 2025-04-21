using UnityEngine;

public class EnemyStateIdle: IEnemyState
{
    private static readonly int Idle = Animator.StringToHash("Idle");
    private EnemyController _enemyController;

    public void Enter(EnemyController enemyController)
    {
        _enemyController = enemyController;
        _enemyController.EnemyAnimator.SetBool(Idle, true);
    }

    public void Update()
    {
        var detectPlayerTransform = _enemyController.DetectPlayerInCircle();
        if (detectPlayerTransform)
        {
            _enemyController.SetState(EnemyState.Trace);
        }
    }

    public void Exit()
    {
        _enemyController.EnemyAnimator.SetBool(Idle, false);
        _enemyController = null;
    }
}