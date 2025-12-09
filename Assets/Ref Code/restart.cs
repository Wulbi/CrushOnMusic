using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class restart : MonoBehaviour
{
    public void loadScene(int scene)
    {
        // Reload the current scene to reset it
        // This works for any scene, making it a true "restart" button
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}


//This source code is originally bought from www.anysourcecode.com
// Visit www.anysourcecode.com
//
//Contact us at:
//
//Email : hello@anysourcecode.com
//Facebook: https://www.facebook.com/anysourcecode
//Twitter: https://x.com/anysourcecode
//Instagram: https://www.instagram.com/anysourcecode
//Youtube: http://www.youtube.com/@anysourcecode
//LinkedIn: www.linkedin.com/anysourcecode
//Pinterest: https://www.pinterest.com/anysourcecode/
