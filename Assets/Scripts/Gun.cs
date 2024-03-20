using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{

    public enum FireMode
    {
        Auto,
        Burst,
        Single,
    }
    public FireMode fireMode;

    public Transform[] projectileSpawn;
    public Projectile projectile;
    public float msBetwwnShots = 100.0f;
    public float muzzleVelocity = 35.0f;
    public int bustCount;
    public int projecttilesPerMag;
    public float reloadTime = 0.3f;

    [Header("Recoil")]
    public Vector2 kickMinMax = new Vector2(0.05f, 0.2f);
    public Vector2 recoilAngleMinMax = new Vector2(3, 5);
    public float recoilMoveSettleTime = 0.1f;
    public float recoilRotationSettleTime = 0.1f;

    [Header("Effects")]
    public Transform shell;
    public Transform shellEjection;
    public AudioClip shootAudio;
    public AudioClip reloadAudio;


    MuzzleFlash muzzleFlash;
    float nextShotTime;

    bool triggerShotReleaseSinceLastShot;
    int shotRemainingInBurst; 
    int projectilesRemainingInMag;
    bool isReloading;

    Vector3 recoilSmoothdampVelocity;
    float recoilRotSmoothDampVelocity;
    float recoilAngle;

    // Use this for initialization
    void Start()
    {
        muzzleFlash = GetComponent<MuzzleFlash>();
        shotRemainingInBurst = bustCount;
        projectilesRemainingInMag = projecttilesPerMag;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref recoilSmoothdampVelocity, recoilMoveSettleTime);
        recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref recoilRotSmoothDampVelocity, recoilRotationSettleTime);
        transform.localEulerAngles = transform.localEulerAngles + Vector3.left * recoilAngle;

        if (isReloading == false
         && projectilesRemainingInMag <= 0)
        {
            Reload();
        }
    }


    void Shoot()
    {
        if (isReloading == false
         && Time.time > nextShotTime
         && projectilesRemainingInMag > 0)
        {
            switch (fireMode)
            {
                case FireMode.Burst:
                    if (shotRemainingInBurst == 0)
                    {
                        return;
                    }
                    shotRemainingInBurst--;
                    break;
                case FireMode.Single:
                    if (triggerShotReleaseSinceLastShot == false)
                    {
                        return;
                    }
                    break;
                case FireMode.Auto:
                default:
                    break;
            }
            for (int i = 0; i < projectileSpawn.Length; i++)
            {
                if (projectilesRemainingInMag <= 0)
                {
                    break;
                }
                projectilesRemainingInMag--;
                nextShotTime = Time.time + (msBetwwnShots / 1000.0f);
                Projectile newProjectile = Instantiate(projectile, projectileSpawn[i].position, projectileSpawn[i].rotation) as Projectile;
                newProjectile.SetSpeed(muzzleVelocity);
            }
            Instantiate(shell, shellEjection.position, shellEjection.rotation);
            muzzleFlash.Activate();
            transform.localPosition -= Vector3.forward * Random.Range(kickMinMax.x, kickMinMax.y);
            recoilAngle += Random.Range(recoilAngleMinMax.x, recoilAngleMinMax.y);
            recoilAngle = Mathf.Clamp(recoilAngle, 0, 30);

            AudioManager.instance.PlaySound(shootAudio, transform.position);
        }
    }

    public void Reload()
    {
        if (isReloading == false
         && projectilesRemainingInMag != projecttilesPerMag)
        {
            StartCoroutine(AnimateReload());
            AudioManager.instance.PlaySound(reloadAudio, transform.position);
        }
    }

    IEnumerator AnimateReload()
    {
        isReloading = true;
        yield return new WaitForSeconds(0.2f);

        float reloadSpeed = 1.0f / reloadTime;
        float percent = 0;
        Vector3 initialRot = transform.localEulerAngles;
        float maxReloadAngle = 30;

        while (percent < 1.0f)
        {
            percent += Time.deltaTime * reloadSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            float reloadAngle = Mathf.Lerp(0, maxReloadAngle, interpolation);
            transform.localEulerAngles = initialRot + Vector3.left * reloadAngle;

            yield return null;
        }

        isReloading = false;
        projectilesRemainingInMag = projecttilesPerMag;
    }

    public void Aim(Vector3 aimPoint)
    {
        if (isReloading == false)
        {
            transform.LookAt(aimPoint);
        }
    }

    public void OnTriggerHold()
    {
        Shoot();
        triggerShotReleaseSinceLastShot = false;
    }

    public void OnTriggerRelease()
    {
        triggerShotReleaseSinceLastShot = true;
        shotRemainingInBurst = bustCount;
    }

}
