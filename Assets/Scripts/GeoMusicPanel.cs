using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Metronome controller for DarkMatter audio loops in the Geocentrism scene.
/// Controls when darkmatters play their loops based on a configurable interval.
/// </summary>
public class GeoMusicPanel : MonoBehaviour
{
    [Header("Metronome Settings")]
    [Tooltip("Interval in seconds between each metronome beat (when darkmatters play their loops)")]
    public float metronomeInterval = 12f;
    
    private List<DarkMatterDrop> darkMatters = new List<DarkMatterDrop>();
    private float timer = 0f;
    private bool isInitialized = false;
    
    void Start()
    {
        InitializeDarkMatters();
    }
    
    void Update()
    {
        if (!isInitialized)
        {
            InitializeDarkMatters();
        }
        
        // Update metronome timer
        timer += Time.deltaTime;
        
        // Check if it's time to trigger the metronome
        if (timer >= metronomeInterval)
        {
            TriggerMetronome();
            timer = 0f; // Reset timer
        }
    }
    
    /// <summary>
    /// Find all DarkMatterDrop components in children
    /// </summary>
    private void InitializeDarkMatters()
    {
        darkMatters.Clear();
        
        // Find all DarkMatterDrop components in children
        DarkMatterDrop[] foundDarkMatters = GetComponentsInChildren<DarkMatterDrop>();
        
        foreach (DarkMatterDrop darkMatter in foundDarkMatters)
        {
            if (darkMatter != null)
            {
                darkMatters.Add(darkMatter);
            }
        }
        
        isInitialized = true;
        Debug.Log($"[GeoMusicPanel] Initialized with {darkMatters.Count} darkmatters. Metronome interval: {metronomeInterval} seconds");
    }
    
    /// <summary>
    /// Trigger all darkmatters to play their loops
    /// </summary>
    private void TriggerMetronome()
    {
        int triggeredCount = 0;
        
        foreach (DarkMatterDrop darkMatter in darkMatters)
        {
            if (darkMatter != null && darkMatter.HasAudioClip())
            {
                darkMatter.TriggerPlayback();
                triggeredCount++;
            }
        }
        
        if (triggeredCount > 0)
        {
            Debug.Log($"[GeoMusicPanel] Metronome triggered - {triggeredCount} darkmatters started playing");
        }
    }
    
    /// <summary>
    /// Manually trigger the metronome (useful for testing or external control)
    /// </summary>
    public void ManualTrigger()
    {
        TriggerMetronome();
        timer = 0f; // Reset timer after manual trigger
    }
    
    /// <summary>
    /// Reset the metronome timer
    /// </summary>
    public void ResetTimer()
    {
        timer = 0f;
    }
}
