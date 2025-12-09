using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using GameLogic.Enum;

/// <summary>
/// Script for Moon_Button that can be dragged and contains DrumLoop audio and Moon image data
/// Can automatically load data from UpgradeDB based on LoopClipType
/// </summary>
public class MoonButtonDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("References")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private Image buttonImage;
    
    [Header("UpgradeDB Reference")]
    [Tooltip("Reference to UpgradeDB (will auto-find if not assigned)")]
    [SerializeField] private UpgradeDB upgradeDB;
    
    [Header("Loop Clip Type")]
    [Tooltip("The LoopClipType for this button. On Awake, button will get icon and audio from UpgradeDB based on this type.")]
    [SerializeField] private LoopClipType loopClipType;
    
    [Header("Data - Auto-loaded from UpgradeDB based on loopClipType")]
    [SerializeField] private Sprite moonSprite;
    [SerializeField] private AudioClip drumLoopClip;
    
    private Vector2 startPosition;
    private RectTransform rectTransform;
    
    // Public properties to access data
    public Sprite MoonSprite => moonSprite;
    public AudioClip DrumLoopClip => drumLoopClip;
    
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        
        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();
        
        if (buttonImage == null)
            buttonImage = GetComponent<Image>();
        
        // Find UpgradeDB if not assigned
        if (upgradeDB == null)
        {
            if (DatabaseManager.HasInstance && DatabaseManager.Instance.upgradeDB != null)
            {
                upgradeDB = DatabaseManager.Instance.upgradeDB;
            }
            else
            {
                // Try to load from Resources
                upgradeDB = Resources.Load<UpgradeDB>("UpgradeDB");
            }
        }
        
        // Load icon and audio clip from UpgradeDB based on loopClipType
        LoadDataFromUpgradeDB();
        
        // Set the button image
        if (buttonImage != null && moonSprite != null)
        {
            buttonImage.sprite = moonSprite;
        }
    }
    
    /// <summary>
    /// Load icon and audio clip from UpgradeDB based on loopClipType
    /// Called on Awake to automatically populate button data
    /// </summary>
    private void LoadDataFromUpgradeDB()
    {
        if (upgradeDB == null || upgradeDB.assistDataList == null)
        {
            Debug.LogWarning($"[MoonButtonDrag] UpgradeDB not found. Cannot load data for LoopClipType: {loopClipType}");
            return;
        }
        
        // Find assist data that matches the loopClipType
        UpgradeDB.AssistUpgradeData matchingData = null;
        foreach (var assistData in upgradeDB.assistDataList)
        {
            if (assistData.loopClipType == loopClipType)
            {
                matchingData = assistData;
                break;
            }
        }
        
        if (matchingData == null)
        {
            Debug.LogWarning($"[MoonButtonDrag] No assist data found in UpgradeDB for LoopClipType: {loopClipType}");
            return;
        }
        
        // Load icon from UpgradeDB
        if (matchingData.icon != null)
        {
            moonSprite = matchingData.icon;
        }
        else
        {
            Debug.LogWarning($"[MoonButtonDrag] No icon found in UpgradeDB for LoopClipType: {loopClipType}");
        }
        
        // Load audio clip from UpgradeDB (use first clip if available)
        if (matchingData.loopClips != null && matchingData.loopClips.Count > 0)
        {
            drumLoopClip = matchingData.loopClips[0];
        }
        else
        {
            Debug.LogWarning($"[MoonButtonDrag] No audio clips found in UpgradeDB for LoopClipType: {loopClipType}");
        }
    }
    
    /// <summary>
    /// Called when loopClipType is changed in inspector (Editor only)
    /// Automatically reloads data from UpgradeDB when loopClipType changes
    /// </summary>
    private void OnValidate()
    {
        // Only run in editor, not in play mode
        if (Application.isPlaying)
            return;
        
#if UNITY_EDITOR
        // Find UpgradeDB in project if not assigned
        if (upgradeDB == null)
        {
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:UpgradeDB");
            if (guids.Length > 0)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                upgradeDB = UnityEditor.AssetDatabase.LoadAssetAtPath<UpgradeDB>(path);
            }
        }
        
        // Reload data from UpgradeDB when loopClipType changes
        if (upgradeDB != null)
        {
            LoadDataFromUpgradeDB();
            
            // Update button image in editor
            if (buttonImage != null && moonSprite != null)
            {
                buttonImage.sprite = moonSprite;
            }
        }
#endif
    }
    
    private void Start()
    {
        startPosition = rectTransform.anchoredPosition;
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Disable Button component during drag to prevent click events
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.interactable = false;
        }
        
        // Optional: Add visual feedback when dragging starts
        // For example, slightly scale up or change alpha
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        // Make the button follow the mouse
        if (canvas != null)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                eventData.position,
                canvas.worldCamera,
                out localPoint);
            
            rectTransform.position = canvas.transform.TransformPoint(localPoint);
        }
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        // Re-enable Button component after drag
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.interactable = true;
        }
        
        // Check if we dropped on a valid target
        // If not, return to start position
        if (eventData.pointerEnter == null)
        {
            rectTransform.anchoredPosition = startPosition;
        }
    }
    
    /// <summary>
    /// Reset button position to start position
    /// </summary>
    public void ResetPosition()
    {
        rectTransform.anchoredPosition = startPosition;
    }
}

