using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyHandler : MonoBehaviour
{
    private enum State{
        Patrol,
        Chase,
        Attack
    }
    State state;
    Enemy enemy;
    EnemyWeaponHandling weaponHandling;
    EnemyMovementHandler movementHandler;

    [Header("Detection Settings")]
    [Space(15)]
    [SerializeField] LayerMask playerLayer;
    [SerializeField] private float chaseRange = 8f;
    [SerializeField] private float attackRange = 4f;
    [SerializeField] private float minimumDetectionRadiusAngle = -40f;
    [SerializeField] private float maximumDetectionRadiusAngle = 65f;

    Vector3 playerLastSeenPos;

    // For Drawing Gizmos
    Vector3 lastSeenPosDraw;
    
    private void Start() {
        enemy = GetComponent<Enemy>();
        weaponHandling = GetComponent<EnemyWeaponHandling>();
        movementHandler = GetComponent<EnemyMovementHandler>();
    }
    private void Update() {
        UpdateState();
        switch(state){
            case State.Patrol:
                movementHandler.Patrol(); 
                break;
            case State.Chase:
                movementHandler.Chase(Player.Instance.transform);
                break;
            case State.Attack:
                weaponHandling.Shoot(Player.Instance.transform);
                break;
        }
        if (playerLastSeenPos != Vector3.zero)
            movementHandler.MoveTo(playerLastSeenPos);
    }

    private void UpdateState()
    {
        if (PlayerOnChaseRange() && IsPlayerOnSight())
        {
            state = State.Chase;
            if (PlayerOnAttackRange())
            {
                state = State.Attack;
            }
        }
        else
        {
            if (state == State.Chase || state == State.Attack)
            {
                playerLastSeenPos = Player.Instance.transform.position;
                lastSeenPosDraw = playerLastSeenPos;
            }
            else playerLastSeenPos = Vector3.zero;

            state = State.Patrol;
        }

    }

    private bool PlayerOnChaseRange(){
        if (Physics.CheckSphere(transform.position, chaseRange, playerLayer))
        {
            return true; 
        }
        return false;
    }
    private bool PlayerOnAttackRange(){
        if (Physics.CheckSphere(transform.position, attackRange, playerLayer))
        {
            return true; 
        }
        return false;
    }
    
    private bool IsPlayerOnSight(){
        float distanceToPlayer = Vector3.Distance(transform.position, Player.Instance.transform.position);
        Vector3 targetDirection = transform.position - Player.Instance.transform.position;
        float viewableAngle = Vector3.Angle(targetDirection, -transform.forward);

        float characterHeight = 2.5f;
        // Raycast now won't start from the floor
        Vector3 playerStartPoint = new Vector3(Player.Instance.transform.position.x, characterHeight, Player.Instance.transform.position.z);
        Vector3 enemyStartPoint = new Vector3(transform.position.x, characterHeight, transform.position.z);

        Debug.DrawLine(playerStartPoint, enemyStartPoint, Color.yellow);

        bool isOnSight = !Physics.Linecast(playerStartPoint, enemyStartPoint, gameObject.layer) && 
            distanceToPlayer <= chaseRange && 
            viewableAngle > minimumDetectionRadiusAngle && viewableAngle < maximumDetectionRadiusAngle;

        return isOnSight;
    }


    private void OnDrawGizmosSelected() {
        // Draw a yellow sphere, chase Range
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, chaseRange);
        // Draw a red sphere, attack Range
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, attackRange);
    }
    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(lastSeenPosDraw, 1f);
    }
}
