using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Weapon : MonoBehaviour
{
    [field: SerializeField] public WeaponDataSO Data { get; private set; }
    public int CurrentAttackCounter
    {
        get => currentAttackCounter;
        private set => currentAttackCounter = value >= Data.NumberOfAttacks ? 0 : value;
    }

    public event Action OnEnter;
    public event Action OnExit;
    private int currentAttackCounter;
    private Animator animator;
    public RuntimeAnimatorController animatorOverrideController;
    private GameObject baseGameObject;
    public AnimationEventHandler EventHandler { get; private set; }
    public NewPlayerController newPlayerController { get; private set; }
    private Hitbox hitbox;
    public float hitboxActivationDelay;
    public int damage;

    private float startingMovementSpeed;

    public void Enter()
    {
        Debug.Log($"{transform.name} enter");
        StartCoroutine(ActivateHitbox());

        animator.SetBool("active", true);
        animator.SetInteger("counter", currentAttackCounter);


        OnEnter?.Invoke();
    }

    private void Exit()
    {
        animator.SetBool("active", false);
        animator.SetInteger("counter", currentAttackCounter);

        CurrentAttackCounter++;

        newPlayerController._movementSpeed = newPlayerController._maximumMovementSpeed;

        OnExit?.Invoke();
    }

    void Awake()
    {
        animator = GetComponentInParent<Animator>();
        hitbox = GetComponent<Hitbox>();
        EventHandler = GetComponent<AnimationEventHandler>();
    }

    public void SetPlayer(NewPlayerController controller)
    {
        newPlayerController = controller;
    }

    private IEnumerator ActivateHitbox()
    {
        float elapsedTime = 0f;
        elapsedTime += Time.deltaTime;
        while (elapsedTime < hitboxActivationDelay)
        {
            elapsedTime += Time.deltaTime;
            newPlayerController._movementSpeed = Mathf.Lerp(newPlayerController._maximumMovementSpeed, Data.MovementSpeedDuringAttack, elapsedTime / hitboxActivationDelay);
            yield return null;
        }
        hitbox.ActivateHitbox(damage);
        EventHandler.StartMovementTrigger();
        yield return new WaitForSeconds(1 - hitboxActivationDelay);
        Exit();
        yield return null;
    }
}
