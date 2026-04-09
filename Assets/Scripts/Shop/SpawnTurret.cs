using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class SpawnTurret : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject[] turretPrefab;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private int[] turretCost = { 100 };
    [SerializeField] private TextMeshProUGUI[] PriceTags;
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
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("Main Camera not found. Please assign a camera to the SpawnTurret script.");
            }
        }
        //sets array sizes for turret cost and price tags to match number of turret prefabs, prevents out of bounds errors when trying to access turret cost or price tag for a turret that doesn't have one assigned in the inspector
        if (turretCost == null || turretCost.Length != turretPrefab.Length)
        {
            turretCost = new int[turretPrefab.Length];
            for (int i = 0; i < turretCost.Length; i++)
            {
                turretCost[i] = 100; // Default cost if not set in inspector
            }
        }
        

        // Ensure turretCost array is properly initialized
        if (turretCost == null || turretCost.Length < turretPrefab.Length)
        {
            Debug.LogWarning("Turret cost array is not properly initialized. Defaulting to 100 for all turrets.");
            turretCost = new int[turretPrefab.Length];
            for (int i = 0; i < turretCost.Length; i++)
            {
                turretCost[i] = 100;
            }
        }  
        //instantiating price tags     
        if (PriceTags != null && PriceTags.Length >= turretCost.Length)
        {
            for (int i = 0; i < turretCost.Length; i++)
            {
                if (PriceTags[i] != null)
                {
                    PriceTags[i].text = turretCost[i].ToString();
                }
            }
        }
            else
            {
                Debug.LogWarning("PriceTags array is not properly assigned. Price tags will not display turret costs.");
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
                DestroyCurrentGhost();
                isPlacingTurret = false;
                currentTurretIndex = -1;
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

        GameObject turretInstance = Instantiate(turretPrefab[currentTurretIndex], position, Quaternion.identity);
        //activating animation after instantiating turret, if animator component exists on turret prefab
         Animator animator = turretInstance.GetComponent<Animator>();
         if (animator != null)
         {
             animator.enabled = true; // Enable animator to play shoot animation
         }
         else
         {
             Debug.LogWarning("Turret prefab does not have an Animator component. Shoot animation will not play.");
         }
         
        PayForTurret(currentTurretIndex);
        DestroyCurrentGhost();
        isPlacingTurret = false;
        currentTurretIndex = -1;

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

        foreach (Animator animator in ghost.GetComponentsInChildren<Animator>())
        {
            animator.enabled = false;
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

        ScoreManager scoreManager = moneyText.GetComponent<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.SubScore(turretCost[turretIndex]);
        }
    }
}
