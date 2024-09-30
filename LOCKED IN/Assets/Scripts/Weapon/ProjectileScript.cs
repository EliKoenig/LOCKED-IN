using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

public class ProjectileScript : MonoBehaviour
{

    public GameObject projectile;

    public float shootForce, upwardForce;

    public float timeBetweenShooting, spread, reloadTime, timeBetweenShots, pulloutTime;
    public int magSize, bulletsPerTap;
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;
    public float spreadDistance;
    public string reloadAnim, recoilAnim, pulloutAnim;

    private Vector3 originPos;
    private Quaternion originRotation;

    bool shooting, readyToShoot, reloading;

    public Camera cam;
    public Transform attackPoint;




    //graphics
    public GameObject muzzleFlash;
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
        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magSize && !reloading && allowInvoke   ) Reload();

        //Reload auto if mag empty and try to shoot
        if(readyToShoot && shooting && !reloading && bulletsLeft <= 0) Reload();

        //shooting
        if(readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = 0;

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

    private void Shoot()
    {
        readyToShoot = false;

        //raycast to find target
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        //check if ray hits something
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(75); //Just a point far away from the player


        //calc direction from attackpoint to targetpoint
        Vector3 dirWithoutSpread = targetPoint - attackPoint.position;

        //calc spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);
        float z = Random.Range(-spread, spread);

        //calc new direction with spread
        Vector3 dirWithSpread = dirWithoutSpread + new Vector3(x, y, z);

        //instantiate bullet
        GameObject currentBullet = Instantiate(projectile, attackPoint.position, Quaternion.identity);
        //rotate bullet in shoot dir
        currentBullet.transform.forward = dirWithSpread.normalized;

        //add force to bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(dirWithSpread.normalized * shootForce, ForceMode.Impulse);
        // this is for bounding projectiles: currentBullet.GetComponent<Rigidbody>().AddForce(cam.transform.up * upwardForce, ForceMode.Impulse);
        
        //instantiate muzzle flash
        if(muzzleFlash != null)
        {
            GameObject Flash = Instantiate(muzzleFlash, muzzleFlashPos);
            Destroy(Flash, 0.1f);
        }

        StartCoroutine(StartRecoil());

        bulletsLeft--;
        bulletsShot++;

        //invoke resetshot function
        if(allowInvoke)
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
        yield return new WaitForSeconds(reloadTime);
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
