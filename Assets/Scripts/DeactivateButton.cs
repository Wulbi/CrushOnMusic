using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Button that deactivates the loop and resets DarkMatter image to original
/// </summary>
public class DeactivateButton : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Reference to DarkMatterDrop component (will auto-find if not assigned)")]
    [SerializeField] private DarkMatterDrop darkMatterDrop;
    
    private Button button;
    
    private void Awake()
    {
        button = GetComponent<Button>();
        
        // Find DarkMatterDrop if not assigned
        if (darkMatterDrop == null)
        {
            darkMatterDrop = GetComponentInParent<DarkMatterDrop>();
            if (darkMatterDrop == null)
            {
                // Try to find in scene
                darkMatterDrop = FindFirstObjectByType<DarkMatterDrop>();
            }
        }
    }
    
    private void Start()
    {
        // Set up button click handler
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnButtonClicked);
        }
    }
    
    private void OnButtonClicked()
    {
        if (darkMatterDrop != null)
        {
            // Call the deactivate method on DarkMatterDrop
            darkMatterDrop.DeactivateLoop();
        }
        else
        {
            Debug.LogWarning("[DeactivateButton] DarkMatterDrop reference not found!");
        }
    }
}

