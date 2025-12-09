using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Button that mutes/unmutes the loop in DarkMatter
/// </summary>
public class MuteButton : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Reference to DarkMatterDrop component (will auto-find if not assigned)")]
    [SerializeField] private DarkMatterDrop darkMatterDrop;
    
    private Button button;
    private bool isMuted = false;
    
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
            // Toggle mute state
            isMuted = !isMuted;
            
            // Call the mute method on DarkMatterDrop
            darkMatterDrop.SetMute(isMuted);
        }
        else
        {
            Debug.LogWarning("[MuteButton] DarkMatterDrop reference not found!");
        }
    }
    
    /// <summary>
    /// Get current mute state
    /// </summary>
    public bool IsMuted => isMuted;
}

