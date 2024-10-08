using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject player;
    public Camera[] cam;

    void Start()
    {
        for (int i = 0; i < cam.Length; i++)
        {
            cam[i].GetComponent<AudioListener>().enabled = false;
        }
        cam[0].GetComponent<AudioListener>().enabled = true;
    }    

    public void ChangeCam()
    {        
        for(int i = 0; i < cam.Length; i++)
        {
            if(i != 0)            
                cam[i].gameObject.SetActive(false);
        }        
    }    
}
