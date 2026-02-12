using UnityEngine;
using UnityEngine.UI;

public class PlayerTouchFollow : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10f;

    [Header("Shooting")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float fireRate = 0.2f;
    [SerializeField] private Transform firePoint;

    [Header("UI")]
    [SerializeField] private Image cooldownImage;  // world-space UI image

    [Header("Squash & Stretch")]
    [SerializeField] private float stretchAmount = 1.2f;  // how tall it gets
    [SerializeField] private float returnSpeed = 10f;     // speed to return to normal

    private Camera mainCam;
    private float fixedY;
    private float nextFireTime = 0f;

    private Vector3 originalScale;

    void Start()
    {
        mainCam = Camera.main;
        fixedY = transform.position.y;
        originalScale = transform.localScale;
    }

    void Update()
    {
        HandleMovement();
        HandleShooting();
        UpdateCooldownUI();
        ApplySquashReturn();
    }

    void HandleMovement()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            Vector3 touchPos = mainCam.ScreenToWorldPoint(touch.position);
            Vector3 targetPos = new Vector3(touchPos.x, fixedY, transform.position.z);

            transform.position = Vector3.Lerp(transform.position, targetPos, moveSpeed * Time.deltaTime);
        }
    }

    void HandleShooting()
    {
        if (Input.touchCount > 0 && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;

            // Reset cooldown image
            if (cooldownImage != null)
                cooldownImage.fillAmount = 1f;

            // Apply squash & stretch upward
            StretchUpward();
        }
    }

    void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
    }

    void UpdateCooldownUI()
    {
        if (cooldownImage != null && cooldownImage.fillAmount > 0)
        {
            cooldownImage.fillAmount -= Time.deltaTime / fireRate;
            cooldownImage.fillAmount = Mathf.Clamp01(cooldownImage.fillAmount);
        }
    }

    // Apply the stretching effect when shooting
    void StretchUpward()
    {
        // Y becomes taller, X shrinks automatically to balance
        transform.localScale = new Vector3(
            originalScale.x * (1f - (stretchAmount - 1f)), // small squeeze
            originalScale.y * stretchAmount,               // stretch upward
            originalScale.z
        );
    }

    // Smoothly return to original scale every frame
    void ApplySquashReturn()
    {
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            originalScale,
            returnSpeed * Time.deltaTime
        );
    }
}



