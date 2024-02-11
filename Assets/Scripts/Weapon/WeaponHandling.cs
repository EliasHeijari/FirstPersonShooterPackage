using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponHandling : MonoBehaviour
{
    public Weapon weapon {get; private set;}
    [SerializeField] private Transform handTransform;
    [SerializeField] private float weaponThrowForce = 300f;
    [SerializeField] private float ShotImpactForce = 200f;
    public int pistolMags {get; set;} = 0;
    public int subMachineMags {get; set;} = 0;
    public event EventHandler OnShoot;
    private float shootingCooldown;
    private float dropweaponCooldown;
    private float weaponThrowRate = 1f;
    public bool Hasweapon {
        get {
            return weapon != null;
        }
    }

    private void Update() {

        Handleweapon();

    }

    private void Handleweapon(){
        if (Hasweapon)
        {
            CheckShooting();

            CheckweaponDrop();

            CheckReload(); 
        }
    }

    private void CheckShooting(){
        // Check if player shoots and if its possible
        if (Input.GetKey(KeyCode.Mouse0) && Time.time >= shootingCooldown && weapon.magSize > 0 && !weapon.isReloading){
            shootingCooldown = Time.time + 1f/weapon.Data.fireRate;
            OnShoot?.Invoke(this, EventArgs.Empty);

            if (WeaponSystem.Instance.Shoot(Camera.main.transform.position, Camera.main.transform.forward,
                Player.Instance.transform.position, weapon.ShootingPoint.position, weapon.Data.shootingDistance, ShotImpactForce, out RaycastHit hit))
                {
                    if (hit.collider.TryGetComponent(out IDamageable damageable))
                    {
                        damageable.TakeDamage(weapon.Data.damage);
                    }
                }
            
        }
    }

    private void CheckweaponDrop(){
        if (Input.GetKey(KeyCode.G) && Time.time >= dropweaponCooldown && !weapon.isReloading){
            dropweaponCooldown = Time.time + 1f/weaponThrowRate;
            Dropweapon(); 
        }
    }

    private void CheckReload(){
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadWithRightMag();
        }
    }

    private void ReloadWithRightMag(){
        if (weapon.Data.weaponType == WeaponData.WeaponType.Pistol)
        {
            if (pistolMags > 0 && weapon.magSize < weapon.Data.maxMagSize){
            pistolMags -= 1;
            weapon.Reload();
            }
        }
        else if (weapon.Data.weaponType == WeaponData.WeaponType.SubMachine){
            if (subMachineMags > 0 && weapon.magSize < weapon.Data.maxMagSize){
            subMachineMags -= 1;
            weapon.Reload();
            }
        }
    }

    public void SetWeapon(Weapon weapon){
        if (Hasweapon){
            if (!weapon.isReloading) {
                this.weapon.transform.parent = null;
                this.weapon.AddComponent<Rigidbody>();
                if (weapon.GetComponent<Rigidbody>() != null) Destroy(weapon.GetComponent<Rigidbody>());
                this.weapon = weapon;

                StartCoroutine(MoveweaponDynamically(5f));
                StartCoroutine(RotateweaponDynamically(5f));

                this.weapon.transform.parent = handTransform;
            }
        }
        else {
            if (weapon.GetComponent<Rigidbody>() != null) Destroy(weapon.GetComponent<Rigidbody>());
            
            this.weapon = weapon;

            StartCoroutine(MoveweaponDynamically(5f));
            StartCoroutine(RotateweaponDynamically(5f));

            weapon.transform.parent = handTransform;
        }
        
    }

    private void Dropweapon(){
        WeaponSystem.DropWeapon(weapon, Camera.main.transform.forward, weaponThrowForce);
        weapon = null;
    }

    private IEnumerator MoveweaponDynamically(float smoothness){
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

    private IEnumerator RotateweaponDynamically(float smoothness){
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
