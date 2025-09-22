using UnityEngine;

public class UITheme : MonoBehaviour
{
    public static UITheme I { get; private set; }
    [SerializeField] private ThemePalette palette;
    public ThemePalette Colors => palette;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
    }
}