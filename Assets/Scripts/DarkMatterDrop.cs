using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Script for DarkMatter that accepts drops from draggable buttons
/// Changes image and plays audio when a ButtonDrag is dropped on it
/// </summary>
public class DarkMatterDrop : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("References")]
    [SerializeField] private Image darkMatterImage;
    
    [Header("Child Buttons")]
    [Tooltip("Button that deactivates the loop and resets image")]
    [SerializeField] private GameObject deactivateButton;
    
    [Tooltip("Button that mutes/unmutes the loop")]
    [SerializeField] private GameObject muteButton;
    
    private Sprite originalSprite;
    private bool hasBeenDropped = false;
    private Collider2D darkMatterCollider;
    private AudioSource audioSource;
    private int soundManagerMusicId = -1; // Track SoundManager music ID
    private bool isMuted = false;
    private bool childrenActive = false;
    
    // Store audio clip and button drag for metronome control
    private AudioClip storedAudioClip;
    private ButtonDrag storedButtonDrag;
    
    private void Awake()
    {
        if (darkMatterImage == null)
            darkMatterImage = GetComponent<Image>();
        
        // Store original sprite
        if (darkMatterImage != null)
        {
            originalSprite = darkMatterImage.sprite;
        }
        
        // Get collider reference (assumes collider is manually set up)
        darkMatterCollider = GetComponent<Collider2D>();
        
        // Get or add AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Find child buttons if not assigned
        if (deactivateButton == null || muteButton == null)
        {
            FindChildButtons();
        }
        
        // Deactivate children on start
        DeactivateChildren();
    }
    
    private void Start()
    {
        // Ensure children are deactivated at start
        DeactivateChildren();
        
        // Set up button callbacks if buttons are assigned
        SetupChildButtons();
    }
    
    private void FindChildButtons()
    {
        // Try to find children by name if not assigned
        if (deactivateButton == null)
        {
            // Look for common names (exact match first, then try variations)
            Transform deactivateTransform = transform.Find("DeactivateButton");
            if (deactivateTransform == null)
                deactivateTransform = transform.Find("Deactivate");
            if (deactivateTransform == null)
                deactivateTransform = transform.Find("StopButton");
            
            if (deactivateTransform != null)
                deactivateButton = deactivateTransform.gameObject;
        }
        
        if (muteButton == null)
        {
            // Look for common names (note: scene has "MuteButton " with trailing space)
            Transform muteTransform = transform.Find("MuteButton ");
            if (muteTransform == null)
                muteTransform = transform.Find("MuteButton");
            if (muteTransform == null)
                muteTransform = transform.Find("Mute");
            
            if (muteTransform != null)
                muteButton = muteTransform.gameObject;
        }
        
        // If still not found, try to find by index (first two children)
        if (deactivateButton == null && transform.childCount > 0)
        {
            deactivateButton = transform.GetChild(0).gameObject;
        }
        
        if (muteButton == null && transform.childCount > 1)
        {
            muteButton = transform.GetChild(1).gameObject;
        }
    }
    
    private void DeactivateChildren()
    {
        if (deactivateButton != null)
            deactivateButton.SetActive(false);
        
        if (muteButton != null)
            muteButton.SetActive(false);
        
        childrenActive = false;
    }
    
    private void ActivateChildren()
    {
        if (deactivateButton != null)
            deactivateButton.SetActive(true);
        
        if (muteButton != null)
            muteButton.SetActive(true);
        
        childrenActive = true;
    }
    
    private void ToggleChildren()
    {
        if (childrenActive)
        {
            DeactivateChildren();
        }
        else
        {
            ActivateChildren();
        }
    }
    
    private void SetupChildButtons()
    {
        // The buttons now handle their own functionality via DeactivateButton and MuteButton scripts
        // The button scripts will automatically find this DarkMatterDrop component
        // No need to set up anything here - the buttons are self-contained
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        // Don't toggle if clicking on a child button
        if (eventData.pointerEnter != null)
        {
            GameObject clickedObject = eventData.pointerEnter;
            
            // Check if the click was on a child button or its children
            if (deactivateButton != null && (clickedObject == deactivateButton || clickedObject.transform.IsChildOf(deactivateButton.transform)))
                return;
            if (muteButton != null && (clickedObject == muteButton || clickedObject.transform.IsChildOf(muteButton.transform)))
                return;
        }
        
        // Toggle children when DarkMatter is clicked
        ToggleChildren();
    }
    
    /// <summary>
    /// Public method to deactivate the loop - called by DeactivateButton
    /// </summary>
    public void DeactivateLoop()
    {
        // Reset image to original
        ResetToOriginal();
        
        // Deactivate children after deactivating
        DeactivateChildren();
    }
    
    /// <summary>
    /// Public method to set mute state - called by MuteButton
    /// </summary>
    public void SetMute(bool muted)
    {
        isMuted = muted;
        
        // Mute/unmute local AudioSource
        if (audioSource != null)
        {
            audioSource.mute = isMuted;
        }
        
        // Keep soundManagerMusicId field for compatibility, but we don't use it anymore
        soundManagerMusicId = -1;
    }
    
    /// <summary>
    /// Handle button drop when collision is detected
    /// Called by ButtonDrag when dropped while colliding
    /// </summary>
    public void HandleButtonDrop(ButtonDrag buttonDrag)
    {
        if (buttonDrag == null)
        {
            Debug.LogWarning("[DarkMatterDrop] HandleButtonDrop called with null buttonDrag");
            return;
        }
        
        Debug.Log($"[DarkMatterDrop] HandleButtonDrop called with button: {buttonDrag.name}, Sprite: {buttonDrag.ButtonSprite?.name}, Audio: {buttonDrag.AudioClip?.name}");
        
        // Change DarkMatter image to button's sprite
        if (darkMatterImage != null && buttonDrag.ButtonSprite != null)
        {
            darkMatterImage.sprite = buttonDrag.ButtonSprite;
            hasBeenDropped = true;
            Debug.Log($"[DarkMatterDrop] Image changed to: {buttonDrag.ButtonSprite.name}");
        }
        else
        {
            Debug.LogWarning($"[DarkMatterDrop] Cannot change image - darkMatterImage: {darkMatterImage != null}, ButtonSprite: {buttonDrag.ButtonSprite != null}");
        }
        
        // Store audio clip and button drag for metronome control (don't play immediately)
        if (buttonDrag.AudioClip != null)
        {
            storedAudioClip = buttonDrag.AudioClip;
            storedButtonDrag = buttonDrag;
            
            // Stop any existing audio first
            StopAudio();
            
            // Set up AudioSource but don't play yet - wait for GeoMusicPanel to trigger
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
            
            audioSource.clip = storedAudioClip;
            audioSource.loop = true;
            audioSource.mute = isMuted;
            
            Debug.Log($"[DarkMatterDrop] Audio clip stored (not playing yet): {buttonDrag.AudioClip.name}");
        }
        else
        {
            Debug.LogWarning("[DarkMatterDrop] No audio clip to store");
        }
    }
    
    public void OnDrop(PointerEventData eventData)
    {
        // Check if the dropped object is a ButtonDrag
        ButtonDrag buttonDrag = eventData.pointerDrag?.GetComponent<ButtonDrag>();
        
        if (buttonDrag != null)
        {
            // Change DarkMatter image to button's sprite
            if (darkMatterImage != null && buttonDrag.ButtonSprite != null)
            {
                darkMatterImage.sprite = buttonDrag.ButtonSprite;
                hasBeenDropped = true;
            }
            
            // Store audio clip and button drag for metronome control (don't play immediately)
            if (buttonDrag.AudioClip != null)
            {
                storedAudioClip = buttonDrag.AudioClip;
                storedButtonDrag = buttonDrag;
                
                // Set up AudioSource but don't play yet - wait for GeoMusicPanel to trigger
                if (audioSource == null)
                    audioSource = GetComponent<AudioSource>();
                if (audioSource == null)
                    audioSource = gameObject.AddComponent<AudioSource>();
                
                audioSource.clip = storedAudioClip;
                audioSource.loop = true;
                
                Debug.Log($"[DarkMatterDrop] Audio clip stored via OnDrop (not playing yet): {buttonDrag.AudioClip.name}");
            }
            
            // Button will automatically reset to original position in OnEndDragHandler
            // No need to call ResetPosition here
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Optional: Visual feedback when dragging over DarkMatter
        // For example, highlight or scale effect
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        // Optional: Remove visual feedback when leaving DarkMatter
    }
    
    /// <summary>
    /// Reset DarkMatter to original state
    /// </summary>
    public void ResetToOriginal()
    {
        if (darkMatterImage != null && originalSprite != null)
        {
            darkMatterImage.sprite = originalSprite;
            hasBeenDropped = false;
        }
        
        // Stop and clear audio
        StopAudio();
        
        // Clear stored audio clip
        storedAudioClip = null;
        storedButtonDrag = null;
        
        if (audioSource != null)
        {
            audioSource.clip = null;
        }
    }
    
    /// <summary>
    /// Stop any playing audio
    /// </summary>
    private void StopAudio()
    {
        // Stop only this dark matter's AudioSource
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
            // Don't clear the clip, just stop playing
        }
        
        // Keep this field for compatibility, but we don't use it with SoundManager any more
        soundManagerMusicId = -1;
    }
    
    /// <summary>
    /// Public method to trigger playback - called by GeoMusicPanel metronome
    /// </summary>
    public void TriggerPlayback()
    {
        // Determine which clip to play: storedAudioClip or the current audioSource.clip
        AudioClip clipToPlay = null;

        if (storedAudioClip != null)
        {
            clipToPlay = storedAudioClip;
        }
        else if (audioSource != null && audioSource.clip != null)
        {
            clipToPlay = audioSource.clip;
        }

        if (clipToPlay == null)
        {
            // Nothing to play
            return;
        }

        // Ensure we have an AudioSource
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        // Stop only this dark matter's audio
        StopAudio();

        // Configure and play the local AudioSource
        audioSource.clip = clipToPlay;
        audioSource.loop = true;
        audioSource.mute = isMuted;   // keep using existing mute flag
        audioSource.Play();

        Debug.Log($"[DarkMatterDrop] Triggered playback via local AudioSource: {clipToPlay.name}");
    }
    
    /// <summary>
    /// Check if this dark matter has an audio clip ready to play
    /// </summary>
    public bool HasAudioClip()
    {
        return (storedAudioClip != null) || (audioSource != null && audioSource.clip != null);
    }
}

