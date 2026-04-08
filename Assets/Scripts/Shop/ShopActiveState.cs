using System.Dynamic;
using System.Reflection;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations;

public class ShopActiveState : MonoBehaviour
{
    [SerializeField] private GameObject shopUI;
    [SerializeField] private GameObject ShopArea;
    private Animator shopAnimator;
   private bool isShopActive = false;

    void Start()
    {
        shopAnimator = shopUI.GetComponent<Animator>();

        if (shopUI == null)
        {
            Debug.LogError("Shop UI is not assigned in the inspector.");
            return;
        }
        isShopActive = false;
        UpdateShopState();
    }

    void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            ClickOutsideShop();
        }
    }

    private void UpdateShopState()
    {
        shopAnimator.SetBool("isActive", isShopActive);
    }

    public void DisableShop()
    {
        isShopActive = false;
        UpdateShopState();
    }
    public void EnableShop()
    {
        isShopActive = true;
        UpdateShopState();
    }
    private void ClickOutsideShop()
    {
        if (CheckMouseOverShop() && isShopActive)
        {
            DisableShop();
        }
    }
    private bool CheckMouseOverShop()
    {
        //Checks for overlap between mouse position and active shop panel, returns false if mouse is over shop to prevent closing shop when clicking inside it
        if (Mouse.current == null)
            return false;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        RectTransform shopRect = ShopArea.GetComponent<RectTransform>();
        
        if (shopRect == null)
        {
            Debug.LogError("Shop Area does not have a RectTransform component.");
            return false;
        }
                
        return !RectTransformUtility.RectangleContainsScreenPoint(shopRect, mousePos);
    }
}
