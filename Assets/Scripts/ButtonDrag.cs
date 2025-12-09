using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using GameLogic.Enum;

/// <summary>
/// Generic script for draggable buttons that can be assigned any sprite and audio clip
/// Uses EventTrigger approach similar to Sprunki_Sample scene
/// Can automatically load data from UpgradeDB based on LoopClipType
/// </summary>
public class ButtonDrag : MonoBehaviour
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
    
    [Header("Draggable Data - Auto-loaded from UpgradeDB based on loopClipType")]
    [Tooltip("The sprite/image data this button represents (auto-loaded from UpgradeDB on Awake)")]
    [SerializeField] private Sprite buttonSprite;
    
    [Tooltip("The audio clip/loop this button represents (auto-loaded from UpgradeDB on Awake)")]
    [SerializeField] private AudioClip audioClip;
    
    private Vector2 startPosition;
    private Vector3 startLocalScale;
    private Quaternion startLocalRotation;
    private RectTransform rectTransform;
    private EventTrigger eventTrigger;
    private Collider2D buttonCollider;
    private bool isDragging = false;
    private DarkMatterDrop currentCollidingDarkMatter = null;
    private Transform originalParent;
    
    // Public properties to access data
    public Sprite ButtonSprite => buttonSprite;
    public AudioClip AudioClip => audioClip;
    public LoopClipType LoopClipType => loopClipType;
    
    /// <summary>
    /// Initialize the button with a specific loop clip type
    /// Loads icon and audio from UpgradeDB based on the type
    /// </summary>
    public void Initialize(LoopClipType type)
    {
        loopClipType = type;
        
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
        
        // Apply the sprite to the button image
        if (buttonImage == null)
            buttonImage = GetComponent<Image>();
            
        if (buttonImage != null && buttonSprite != null)
        {
            buttonImage.sprite = buttonSprite;
        }
    }
    
    private void Awake()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
        
        if (buttonCollider == null)
            buttonCollider = GetComponent<Collider2D>();
        
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
        
        // Ensure Image has raycastTarget enabled for drag detection
        if (buttonImage != null)
        {
            buttonImage.raycastTarget = true;
            
            // Set button image if sprite is assigned
            if (buttonSprite != null)
            {
                buttonImage.sprite = buttonSprite;
            }
        }
        
        // Ensure EventSystem exists (create one if missing)
        if (EventSystem.current == null)
        {
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<EventSystem>();
            eventSystemObj.AddComponent<StandaloneInputModule>();
        }
        
        // Set up EventTrigger for drag functionality
        SetupEventTrigger();
    }
    
    /// <summary>
    /// Load icon and audio clip from UpgradeDB based on loopClipType
    /// Called on Awake to automatically populate button data
    /// </summary>
    private void LoadDataFromUpgradeDB()
    {
        if (upgradeDB == null || upgradeDB.assistDataList == null)
        {
            Debug.LogWarning($"[ButtonDrag] UpgradeDB not found. Cannot load data for LoopClipType: {loopClipType}");
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
            Debug.LogWarning($"[ButtonDrag] No assist data found in UpgradeDB for LoopClipType: {loopClipType}");
            return;
        }
        
        // Load icon from UpgradeDB
        if (matchingData.icon != null)
        {
            buttonSprite = matchingData.icon;
        }
        else
        {
            Debug.LogWarning($"[ButtonDrag] No icon found in UpgradeDB for LoopClipType: {loopClipType}");
        }
        
        // Load audio clip from UpgradeDB (use first clip if available)
        if (matchingData.loopClips != null && matchingData.loopClips.Count > 0)
        {
            audioClip = matchingData.loopClips[0];
        }
        else
        {
            Debug.LogWarning($"[ButtonDrag] No audio clips found in UpgradeDB for LoopClipType: {loopClipType}");
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
            if (buttonImage != null && buttonSprite != null)
            {
                buttonImage.sprite = buttonSprite;
            }
        }
#endif
    }
    
    private void SetupEventTrigger()
    {
        // Get or add EventTrigger component
        eventTrigger = GetComponent<EventTrigger>();
        if (eventTrigger == null)
        {
            eventTrigger = gameObject.AddComponent<EventTrigger>();
        }
        
        // Clear existing triggers
        eventTrigger.triggers.Clear();
        
        // Add Drag event (eventID 5 = EventTriggerType.Drag)
        EventTrigger.Entry dragEntry = new EventTrigger.Entry();
        dragEntry.eventID = EventTriggerType.Drag;
        dragEntry.callback.AddListener((data) => { OnDragHandler((PointerEventData)data); });
        eventTrigger.triggers.Add(dragEntry);
        
        // Add BeginDrag event (eventID 2 = EventTriggerType.BeginDrag)
        EventTrigger.Entry beginDragEntry = new EventTrigger.Entry();
        beginDragEntry.eventID = EventTriggerType.BeginDrag;
        beginDragEntry.callback.AddListener((data) => { OnBeginDragHandler((PointerEventData)data); });
        eventTrigger.triggers.Add(beginDragEntry);
        
        // Add EndDrag event (eventID 3 = EventTriggerType.EndDrag)
        EventTrigger.Entry endDragEntry = new EventTrigger.Entry();
        endDragEntry.eventID = EventTriggerType.EndDrag;
        endDragEntry.callback.AddListener((data) => { OnEndDragHandler((PointerEventData)data); });
        eventTrigger.triggers.Add(endDragEntry);
    }
    
    private void Start()
    {
        // Store original transform values
        startPosition = rectTransform.anchoredPosition;
        startLocalScale = rectTransform.localScale;
        startLocalRotation = rectTransform.localRotation;
        
        // Store original parent
        originalParent = rectTransform.parent;
    }
    
    private void OnBeginDragHandler(PointerEventData eventData)
    {
        isDragging = true;
        
        // Disable Button component during drag to prevent click events
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.interactable = false;
        }
        
        // Remember original parent before drag
        originalParent = rectTransform.parent;
        
        // Move under the root canvas so it is not clipped by ScrollView masks
        if (canvas != null)
        {
            rectTransform.SetParent(canvas.transform, worldPositionStays: true);
        }
    }
    
    private void OnDragHandler(PointerEventData eventData)
    {
        if (!isDragging) return;
        
        // Make the button follow the mouse
        if (canvas != null && rectTransform != null)
        {
            RectTransform canvasRect = canvas.transform as RectTransform;
            Vector3 worldPoint;
            
            // Convert screen point to world point in canvas coordinate space
            // This properly handles scaled content from SimpleZoom
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                canvasRect,
                eventData.position,
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                out worldPoint))
            {
                // Set the world position directly
                rectTransform.position = worldPoint;
            }
        }
        else
        {
            // Fallback: direct position update
            rectTransform.position = eventData.position;
        }
    }
    
    private void OnEndDragHandler(PointerEventData eventData)
    {
        isDragging = false;
        
        // Re-enable Button component after drag
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.interactable = true;
        }
        
        // Check for collision with DarkMatter using physics overlap
        CheckForDarkMatterCollision();
        
        // Restore original parent when drag ends
        if (originalParent != null)
        {
            rectTransform.SetParent(originalParent, worldPositionStays: true);
        }
        
        // Always return to original transform when dropped
        ResetToOriginalTransform();
        
        // Clear collision reference
        currentCollidingDarkMatter = null;
    }
    
    private void CheckForDarkMatterCollision()
    {
        if (buttonCollider == null) return;
        
        // Use Physics2D.OverlapCollider to check what we're overlapping with
        Collider2D[] overlappingColliders = new Collider2D[10];
        ContactFilter2D filter = new ContactFilter2D();
        filter.NoFilter(); // Check all layers
        
        int count = buttonCollider.Overlap(filter, overlappingColliders);
        
        for (int i = 0; i < count; i++)
        {
            DarkMatterDrop darkMatter = overlappingColliders[i].GetComponent<DarkMatterDrop>();
            if (darkMatter != null)
            {
                // Found DarkMatter - trigger the drop behavior
                darkMatter.HandleButtonDrop(this);
                return;
            }
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Track collision for visual feedback if needed
        DarkMatterDrop darkMatter = other.GetComponent<DarkMatterDrop>();
        if (darkMatter != null)
        {
            currentCollidingDarkMatter = darkMatter;
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        // Clear collision reference when leaving
        DarkMatterDrop darkMatter = other.GetComponent<DarkMatterDrop>();
        if (darkMatter != null && currentCollidingDarkMatter == darkMatter)
        {
            currentCollidingDarkMatter = null;
        }
    }
    
    /// <summary>
    /// Reset button to original transform (position, rotation, scale)
    /// </summary>
    public void ResetToOriginalTransform()
    {
        rectTransform.anchoredPosition = startPosition;
        rectTransform.localScale = startLocalScale;
        rectTransform.localRotation = startLocalRotation;
    }
    
    /// <summary>
    /// Reset button position to start position (for backwards compatibility)
    /// </summary>
    public void ResetPosition()
    {
        ResetToOriginalTransform();
    }
    
    /// <summary>
    /// Set the sprite data for this button
    /// </summary>
    public void SetSprite(Sprite sprite)
    {
        buttonSprite = sprite;
        if (buttonImage != null && sprite != null)
        {
            buttonImage.sprite = sprite;
        }
    }
    
    /// <summary>
    /// Set the audio clip data for this button
    /// </summary>
    public void SetAudioClip(AudioClip clip)
    {
        audioClip = clip;
    }
}
