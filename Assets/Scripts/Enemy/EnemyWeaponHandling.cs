using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyWeaponHandling : MonoBehaviour
{
    NavMeshAgent navMeshAgent;
    Enemy enemy;
    [SerializeField] private Transform weaponTransform;
    public Weapon weapon {get; private set;}
    [SerializeField] private Transform handTransform;
    private float ShotImpactForce = 200f;
    float shootingCooldown = 0;
    float rotateTargetSpeed = 3f;

    public bool HasWeapon {
        get {
            return weapon != null;
        }
    }

    private void Start() {
        navMeshAgent = GetComponent<NavMeshAgent>();
        enemy = GetComponent<Enemy>();
        SetWeapon(weaponTransform.GetComponent<Weapon>());
    }
    public void Shoot(Transform target){
        // Stop moving
        navMeshAgent.velocity = Vector3.zero;

        RotateTowardsTarget(target);

        // ShootingLogicHere
        if (Time.time >= shootingCooldown){
            shootingCooldown = Time.time + 1f/weapon.Data.fireRate;
            Debug.Log("Shoot");
            if (WeaponSystem.Instance.Shoot(weapon.ShootingPoint.position, transform.forward,
                Player.Instance.transform.position, weapon.ShootingPoint.position, weapon.Data.shootingDistance, ShotImpactForce, out RaycastHit hit))
                {
                    // TODO: Damage Logic Here, IDamageable.Damage
                    if (hit.collider.TryGetComponent(out IDamageable damageable))
                    {
                        damageable.TakeDamage(weapon.Data.damage);
                    }
                }
            
        }
    }

    private void RotateTowardsTarget(Transform target)
    {
        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
        // Look Towards The Target
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateTargetSpeed * Time.deltaTime);

    }

    public void SetWeapon(Weapon weapon){
        if (HasWeapon){
            if (!this.weapon.isReloading) {
                this.weapon.transform.parent = null;
                this.weapon.AddComponent<Rigidbody>();
                if (weapon.GetComponent<Rigidbody>() != null) Destroy(weapon.GetComponent<Rigidbody>());
                this.weapon = weapon;

                StartCoroutine(MoveWeaponDynamically(5f));
                StartCoroutine(RotateWeaponDynamically(5f));

                this.weapon.transform.parent = handTransform;
            }
        }
        else {
            if (weapon.GetComponent<Rigidbody>() != null) Destroy(weapon.GetComponent<Rigidbody>());
            this.weapon = weapon;

            StartCoroutine(MoveWeaponDynamically(5f));
            StartCoroutine(RotateWeaponDynamically(5f));

            this.weapon.transform.parent = handTransform;
        }
    }
        
    

    private void DropWeapon(){
        WeaponSystem.DropWeapon(weapon);
        weapon = null;
    }

    private IEnumerator MoveWeaponDynamically(float smoothness){
        float elapsedTime = 0f;

        while (elapsedTime < smoothness){
            Vector3 targetPosition = handTransform.position;
            weapon.transform.position = Vector3.Lerp(weapon.transform.position, targetPosition, elapsedTime / smoothness);
            float positionThreshold = 0.01f;
            float distance = Vector3.Distance(weapon.transform.position, targetPosition);
            if (distance < positionThreshold){
                weapon.transform.position = targetPosition;
                break;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator RotateWeaponDynamically(float smoothness)
    {
        float elapsedTime = 0f;

        while (elapsedTime < smoothness){
            Quaternion targetRotation = handTransform.rotation;
            weapon.transform.rotation = Quaternion.Slerp(weapon.transform.rotation, targetRotation, elapsedTime / smoothness);
            float rotationThreshold = 0.01f;
            float angle = Quaternion.Angle(weapon.transform.rotation, targetRotation);
            if (angle < rotationThreshold){
                weapon.transform.rotation = targetRotation;
                break;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

}
