using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using GameLogic.Enum;

/// <summary>
/// Dynamically generates GeoMusicButton prefabs based on GlobalManager.activeLoopTypes
/// Attach this script to the ScrollView Content object in the Geocentrism scene
/// </summary>
public class GeoButtonPanel : MonoBehaviour
{
    [Header("Button Prefab")]
    [Tooltip("Reference to the GeoMusicButton prefab (ButtonDrag component)")]
    public ButtonDrag buttonPrefab;
    
    [Header("Spawn Settings")]
    [Tooltip("Optional: Define the order in which buttons should spawn. If empty, uses activeLoopTypes order.")]
    public LoopClipType[] spawnOrder;
    
    [Header("Button Root")]
    [Tooltip("Transform where buttons will be instantiated (usually the ScrollView Content)")]
    public Transform buttonRoot;
    
    private List<ButtonDrag> spawnedButtons = new List<ButtonDrag>();
    
    private void OnEnable()
    {
        RefreshButtons();
    }
    
    /// <summary>
    /// Clears existing buttons and spawns new ones based on activeLoopTypes
    /// </summary>
    public void RefreshButtons()
    {
        // Clear all existing buttons
        ClearButtons();
        
        // Validate references
        if (buttonPrefab == null)
        {
            Debug.LogWarning("[GeoButtonPanel] buttonPrefab is not assigned!");
            return;
        }
        
        if (buttonRoot == null)
        {
            // Try to automatically find the ScrollRect's content if this script
            // is attached to the parent "ButtonPanel" instead of the "Content".
            ScrollRect scrollRect = GetComponentInChildren<ScrollRect>();
            if (scrollRect != null && scrollRect.content != null)
            {
                buttonRoot = scrollRect.content;
            }
            else
            {
                // Fallback: use this transform
                buttonRoot = transform;
            }
        }
        
        if (!GlobalManager.HasInstance)
        {
            Debug.LogWarning("[GeoButtonPanel] GlobalManager instance not found!");
            return;
        }
        
        // Get active loop types from GlobalManager
        HashSet<LoopClipType> active = GlobalManager.Instance.activeLoopTypes;
        
        if (active == null || active.Count == 0)
        {
            Debug.Log("[GeoButtonPanel] No active loop types found. Buttons will be spawned when assists are upgraded.");
            return;
        }
        
        // Determine which types to spawn
        List<LoopClipType> typesToSpawn = new List<LoopClipType>();
        
        if (spawnOrder != null && spawnOrder.Length > 0)
        {
            // Use spawnOrder, but only include types that are in activeLoopTypes
            foreach (LoopClipType type in spawnOrder)
            {
                if (active.Contains(type))
                {
                    typesToSpawn.Add(type);
                }
            }
        }
        else
        {
            // Use all active types (order will be based on enum order)
            typesToSpawn.AddRange(active);
        }
        
        // Spawn buttons for each active type
        foreach (LoopClipType type in typesToSpawn)
        {
            GameObject buttonObj = Instantiate(buttonPrefab.gameObject, buttonRoot);
            ButtonDrag button = buttonObj.GetComponent<ButtonDrag>();
            
            if (button != null)
            {
                // Initialize the button with the loop type
                button.Initialize(type);
                spawnedButtons.Add(button);
            }
            else
            {
                Debug.LogWarning($"[GeoButtonPanel] ButtonDrag component not found on prefab for type: {type}");
                Destroy(buttonObj);
            }
        }
        
        Debug.Log($"[GeoButtonPanel] Spawned {spawnedButtons.Count} buttons based on activeLoopTypes");
    }
    
    /// <summary>
    /// Clears all spawned buttons
    /// </summary>
    private void ClearButtons()
    {
        foreach (ButtonDrag button in spawnedButtons)
        {
            if (button != null)
            {
                Destroy(button.gameObject);
            }
        }
        
        spawnedButtons.Clear();
        
        // Also destroy any remaining children (safety cleanup)
        Transform root = buttonRoot != null ? buttonRoot : transform;
        for (int i = root.childCount - 1; i >= 0; i--)
        {
            Transform child = root.GetChild(i);
            if (child != null && child.GetComponent<ButtonDrag>() != null)
            {
                Object.Destroy(child.gameObject);
            }
        }
    }
}
