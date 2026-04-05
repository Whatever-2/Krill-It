using UnityEngine;

public class TurretBehavior : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform FirePoint;
    [SerializeField] private GameObject shootRange;
    [SerializeField] private float shootRangeDistance = 5f;
    [SerializeField] private Vector2 shootRangeScale = Vector2.one;
    [SerializeField] private bool shootRangeActive = true;
    [SerializeField] private AnimationCurve trajectoryAnimationCurve;
    [SerializeField] private float bulletTravelTime = 0.7f;
    [SerializeField] private float bulletHeight = 1f;
    public float shooterTimer = 1.0f;

    private float timer;

    private void Start()
    {
        timer = shooterTimer;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            CheckRangeActive();
            if (shootRangeActive)
            {
                Shoot();
            }
            resetTimer();
        }
    }

    private void Shoot()
    {
        if (bulletPrefab == null || FirePoint == null)
            return;

        var projectileObject = Instantiate(bulletPrefab, FirePoint.position, FirePoint.rotation);
        var projectile = projectileObject.GetComponent<BulletController>();
        if (projectile == null)
            return;

        if (shootRange != null)
        {
            float diameter = shootRangeDistance * 2f;
            shootRange.transform.localScale = new Vector3(diameter * shootRangeScale.x, diameter * shootRangeScale.y, 1f);
        }

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject nearestEnemy = null;
        float minDistance = Mathf.Infinity;
        Vector3 turretPos = transform.position;

        foreach (GameObject enemy in enemies)
        {
            if (!IsWithinShootRange(enemy.transform.position))
                continue;

            float distance = Vector3.Distance(turretPos, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null)
        {
            projectile.InitializeCurve(FirePoint.position, nearestEnemy.transform.position, trajectoryAnimationCurve, bulletTravelTime, bulletHeight);
        }
        else
        {
            Destroy(projectileObject);
        }
    }

    private bool IsWithinShootRange(Vector3 targetPosition)
    {
        Vector2 offset = targetPosition - transform.position;
        float radiusX = shootRangeDistance * shootRangeScale.x;
        float radiusY = shootRangeDistance * shootRangeScale.y;

        if (Mathf.Approximately(radiusX, radiusY))
        {
            return offset.sqrMagnitude <= (radiusX * radiusX);
        }

        float normalizedX = offset.x / radiusX;
        float normalizedY = offset.y / radiusY;
        return (normalizedX * normalizedX) + (normalizedY * normalizedY) <= 1f;
    }

    private void CheckRangeActive()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        bool enemyInRange = false;

        foreach (GameObject enemy in enemies)
        {
            if (IsWithinShootRange(enemy.transform.position))
            {
                enemyInRange = true;
                break;
            }
        }

        shootRangeActive = enemyInRange;
    }

    private void resetTimer()
    {
        timer = shooterTimer;
    }
}
