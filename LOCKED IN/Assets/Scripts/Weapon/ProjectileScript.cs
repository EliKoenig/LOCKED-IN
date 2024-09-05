using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ProjectileScript : MonoBehaviour
{

    public GameObject projectile;

    public float shootForce, upwardForce;

    public float timeBetweenShooting, spread, reloadTime, timeBetweenShots;
    public int magSize, bulletsPerTap;
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;
    public float spreadDistance;

    bool shooting, readyToShoot, reloading;

    public Camera cam;
    public Transform attackPoint;


    //graphics
    public GameObject muzzleFlash;
    public TextMeshProUGUI ammoDisplay;

    public bool allowInvoke = true;
    // Start is called before the first frame update
    void Awake()
    {
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

        bulletsLeft = magSize;
        readyToShoot = true;
    }

    private void MyInput()
    {
        //check if allowed to hold down button
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKey(KeyCode.Mouse0);

        //reloading
        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magSize && !reloading) Reload();

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
            ammoDisplay.SetText(bulletsLeft + " / " + magSize / bulletsLeft);
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
            Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);
        }

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

    private void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }
    private void ReloadFinished()
    {
        bulletsLeft = magSize;
        reloading = false;
    }
}
