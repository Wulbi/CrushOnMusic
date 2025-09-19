using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
   public GameObject[] sources;

    void Start()
    {

        //Get every single audio sources in the scene.
        //sources = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        //print(sources[1].name);
    }

    void Update()
    {

       //print( sources[0].GetComponent<Character>().activeChild.name);
       // sources[0].transform.GetChild(int.Parse(sources[0].GetComponent<Character>().activeChild.name)).gameObject.GetComponent<AudioSource>().Play();
       
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
