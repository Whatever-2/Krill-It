using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnTurret : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject[] turretPrefab;
    [SerializeField] private TMPro.TextMeshProUGUI moneyText;
    [SerializeField] private int[] turretCost = { 100 };

    private bool isPlacingTurret = false;
    private GameObject currentTurretGhost;
    private int currentTurretIndex = -1;
    private const float ghostAlpha = 0.5f;

    void Start()
    {
        if (turretPrefab == null || turretPrefab.Length == 0)
        {
            Debug.LogError("Turret Prefab is not assigned in the inspector.");
        }

        if (moneyText == null)
        {
            Debug.LogWarning("Money text is not assigned. CanAffordTurret will fail if called.");
        }
    }

    void Update()
    {
        if (!isPlacingTurret || currentTurretGhost == null)
            return;

        Vector3 worldPos = GetMouseWorldPosition();
        currentTurretGhost.transform.position = worldPos;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (IsValidPlacement(worldPos))
            {
                PlaceTurret(worldPos);
            }
            else
            {
                Debug.Log("Invalid placement. Please place the turret on the ground.");
            }
        }
    }

    public void SpawnTurretOnCursor(int turretIndex)
    {
        if (turretPrefab == null || turretIndex < 0 || turretIndex >= turretPrefab.Length)
        {
            Debug.LogError("Invalid turret index or turret prefab not assigned.");
            return;
        }

        if (isPlacingTurret)
        {
            Debug.Log("Already placing a turret. Please place the current turret before spawning another.");
            return;
        }

        if (!CanAffordTurret(turretIndex))
        {
            Debug.Log("Not enough money to place turret.");
            return;
        }

        Vector3 spawnPosition = GetMouseWorldPosition();
        currentTurretGhost = Instantiate(turretPrefab[turretIndex], spawnPosition, Quaternion.identity);
        currentTurretIndex = turretIndex;
        SetGhostProperties(currentTurretGhost);
        isPlacingTurret = true;
    }

    public bool CanAffordTurret(int turretIndex)
    {
        if (moneyText == null)
            return false;

        if (turretCost == null || turretIndex < 0 || turretIndex >= turretCost.Length)
            return false;

        if (!int.TryParse(moneyText.text, out int playerMoney))
            return false;

        return playerMoney >= turretCost[turretIndex];
    }

    private void PlaceTurret(Vector3 position)
    {
        if (currentTurretIndex < 0 || currentTurretIndex >= turretPrefab.Length)
        {
            Debug.LogError("Invalid turret index for placement.");
            return;
        }

        Instantiate(turretPrefab[currentTurretIndex], position, Quaternion.identity);
        DestroyCurrentGhost();
        isPlacingTurret = false;
        currentTurretIndex = -1;
        PayForTurret(currentTurretIndex);
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        Camera cam = mainCamera != null ? mainCamera : Camera.main;
        Vector3 worldPos = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0));
        worldPos.z = 0;
        return worldPos;
    }

    private bool IsValidPlacement(Vector3 position)
    {
        Collider2D[] hits = Physics2D.OverlapPointAll(position);
        foreach (Collider2D hit in hits)
        {
            if (hit == null || hit.isTrigger)
                continue;

            if (!hit.CompareTag("Ground"))
                return false;
        }

        return true;
    }

    private void SetGhostProperties(GameObject ghost)
    {
        foreach (SpriteRenderer spriteRenderer in ghost.GetComponentsInChildren<SpriteRenderer>())
        {
            Color currentColor = spriteRenderer.color;
            currentColor.a = ghostAlpha;
            spriteRenderer.color = currentColor;
        }

        foreach (Collider2D collider in ghost.GetComponentsInChildren<Collider2D>())
        {
            collider.enabled = false;
        }
    }

    private void DestroyCurrentGhost()
    {
        if (currentTurretGhost != null)
        {
            Destroy(currentTurretGhost);
            currentTurretGhost = null;
        }
    }

    private void PayForTurret(int turretIndex)
    {
        if (moneyText == null || turretCost == null || turretIndex < 0 || turretIndex >= turretCost.Length)
            return;

        if (!int.TryParse(moneyText.text, out int playerMoney))
            return;

        playerMoney -= turretCost[turretIndex];
        moneyText.text = playerMoney.ToString();
    }
}
