using UnityEngine;
using Utilities;

public class PlayerAttackState : PlayerBaseState
{
    private float startingSpeed;
    private int currentAttackNum;
    private int totalAttackNum = 3;
    private float comboTime = 0f;
    public PlayerAttackState(NewPlayerController player, Animator animator) : base(player, animator)
    {
    }

    public override void OnEnter()
    {
        comboTime = player.weaponController.equippedWeapon.AttackInfos[0].comboTime;
        player._movementSpeed = player.weaponController.equippedWeapon.AttackInfos[0].movementSpeedDuringAttack;
        //comboTime = 1;
        //comboTime = 1.1f;
        currentAttackNum = 1;
        startingSpeed = player._movementSpeed;
        animator.CrossFade(AttackHash, crossFadeDuration);
        //player._movementSpeed = 2.5f;
    }

    public void ProgressCombo()
    {
        currentAttackNum++;
        if (currentAttackNum == 2)
        {
            //comboTime = 1f;
            //comboTime = 0.5f;
            //player._movementSpeed = 2f;
            comboTime = player.weaponController.equippedWeapon.AttackInfos[1].comboTime;
            player._movementSpeed = player.weaponController.equippedWeapon.AttackInfos[1].movementSpeedDuringAttack;
            animator.CrossFade(AttackHash2, crossFadeDuration);
        }

        if (currentAttackNum == 3)
        {
            comboTime = player.weaponController.equippedWeapon.AttackInfos[2].comboTime;
            player._movementSpeed = player.weaponController.equippedWeapon.AttackInfos[2].movementSpeedDuringAttack;
            //player._movementSpeed = 1.5f;
            animator.CrossFade(AttackHash3, crossFadeDuration);
        }
    }

    public override void Update()
    {
        if (comboTime > 0)
        {
            comboTime -= Time.deltaTime;
        }
    }

    public override void OnExit()
    {
        player._movementSpeed = startingSpeed;
    }

    public bool CanAttack()
    {
        return currentAttackNum < totalAttackNum && comboTime <= 0;
    }

}
