using UnityEngine;
using UnityEngine.SceneManagement;

public class Temp_ButtonHelio : MonoBehaviour
{
    public SimpleButton buttonHelio;
    
    private void Awake()
    {
        buttonHelio.OnClick = OnClickedHelio;
    }
    
    private void OnClickedHelio()
    {
        SceneManager.LoadScene("Scenes/Heliocentrism");
    }
}
