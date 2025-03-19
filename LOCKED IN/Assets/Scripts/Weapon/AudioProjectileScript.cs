using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

public class AudioProjectileScript : MonoBehaviour
{

    public GameObject projectile;

    public int score = 0;
    public float shootForce, upwardForce;

    public float timeBetweenShooting, spread, reloadTime, timeBetweenShots, pulloutTime;
    public int magSize, bulletsPerTap, damage;
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;
    public float spreadDistance;
    public string reloadAnim, recoilAnim, pulloutAnim;

    private Vector3 originPos;
    private Quaternion originRotation;

    bool shooting, readyToShoot, reloading;

    public Camera cam;
    public Transform attackPoint;
    public AudioSource source;
    public AudioClip shootClip, enemyDie, hurt, hitClip, music, reload;




    //graphics
    public GameObject muzzleFlash, reloadText;
    public Transform muzzleFlashPos;
    public TextMeshProUGUI ammoDisplay;
    public Animator animator;
    public Sprite weaponIcon;
    public UnityEngine.UI.Image weaponIconUI;


    public bool allowInvoke = true;
    // Start is called before the first frame update
    void Awake()
    {
        //gameObject.tag = weaponTag;
        if (cam == null)
        {
            cam = Camera.main;  // Automatically assigns the Main Camera
        }

        // Assign the TextMeshProUGUI object in the scene to 'ammoDisplay'
        if (ammoDisplay == null)
        {
            ammoDisplay = GameObject.Find("Ammo Display").GetComponent<TextMeshProUGUI>();
            // Replace "AmmoText" with the actual name of your TextMeshProUGUI object in the scene
        }



        animator = GetComponent<Animator>();

        bulletsLeft = magSize;
        readyToShoot = true;
        originPos = transform.localPosition;
        originRotation = transform.localRotation;
    }

    public void OnEnable()
    {
        animator.Play("New State");  // Ensure the animation is reset
        reloading = false;  // Stop reload status if any
        transform.localPosition = originPos;
        transform.localRotation = originRotation;
        StartCoroutine(StartPullout());
    }
    public void OnDisable()
    {
        if (reloading)
        {
            StopCoroutine(StartReload());
            reloading = false;
            animator.Play("New State");  // Reset animation
        }
        transform.localPosition = originPos;
        transform.localRotation = originRotation;
    }
    public void Start()
    {
        weaponIconUI = GameObject.Find("Weapon Image").GetComponent<UnityEngine.UI.Image>();
        weaponIconUI.sprite = weaponIcon;
    }
    private void MyInput()
    {
        //check if allowed to hold down button
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        //reloading
        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magSize && !reloading && allowInvoke) Reload();

        //Reload auto if mag empty and try to shoot
        if (readyToShoot && shooting && !reloading && bulletsLeft <= 0) Reload();

        //shooting
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = 0;
            source.PlayOneShot(shootClip);
            Shoot();
        }
    }
    // Update is called once per frame
    void Update()
    {
        MyInput();

        //set ammo display
        if (ammoDisplay != null)
        {
            ammoDisplay.SetText(bulletsLeft + " / " + magSize);
        }

    }

    public void Shoot()
    {
        readyToShoot = false;

        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit))
        {
            MeleeEnemy meleeEnemy = hit.transform.GetComponent<MeleeEnemy>();
            if (meleeEnemy != null)
            {
                source.PlayOneShot(hitClip);
                if (meleeEnemy.health - damage <= 0)
                {
                    score += 1;
                    source.PlayOneShot(enemyDie);
                }

                meleeEnemy.TakeDamage(damage);

            }
            EnemyGun enemyGun = hit.transform.GetComponentInParent<EnemyGun>();
            if (enemyGun != null)
            {
                source.PlayOneShot(hitClip);
                if (enemyGun.health - damage <= 0)
                {
                    score += 1;
                    source.PlayOneShot(enemyDie);
                }

                enemyGun.TakeDamage(damage);
            }
        }
        // this is for bounding projectiles: currentBullet.GetComponent<Rigidbody>().AddForce(cam.transform.up * upwardForce, ForceMode.Impulse);

        //instantiate muzzle flash
        if (muzzleFlash != null)
        {
            GameObject Flash = Instantiate(muzzleFlash, muzzleFlashPos);
            Destroy(Flash, 0.1f);
        }

        StartCoroutine(StartRecoil());

        bulletsLeft--;
        bulletsShot++;

        //invoke resetshot function
        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;
        }
        if (bulletsShot < bulletsPerTap && bulletsLeft > 0)
            Invoke("Shoot", timeBetweenShots);
    }

    IEnumerator StartRecoil()
    {
        animator.Play(recoilAnim);
        yield return new WaitForSeconds(timeBetweenShooting);
        animator.Play("New State");
    }

    IEnumerator StartReload()
    {
        animator.Play(reloadAnim);
        source.PlayOneShot(reload);
        reloadText.SetActive(true);
        yield return new WaitForSeconds(reloadTime);
        reloadText.SetActive(false);
        animator.Play("New State");
    }
    IEnumerator StartPullout()
    {
        animator.Play(pulloutAnim);
        yield return new WaitForSeconds(pulloutTime);
        animator.Play("New State");
    }
    private void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }

    private void Reload()
    {
        reloading = true;
        StartCoroutine(StartReload());
        Invoke("ReloadFinished", reloadTime);
    }
    private void ReloadFinished()
    {
        bulletsLeft = magSize;
        reloading = false;
    }
}

