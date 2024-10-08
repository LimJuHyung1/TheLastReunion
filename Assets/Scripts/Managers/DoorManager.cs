using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    public AudioClip[] openDoorSound = new AudioClip[2];
    public GameObject[] _0F_doors;    
    public GameObject[] _1F_doors;    

    Door[] _0F_doors_comp;
    Door[] _1F_doors_comp;

    void Awake()
    {
        _0F_doors_comp = new Door[_0F_doors.Length];
        _1F_doors_comp = new Door[_1F_doors.Length];
        SetDoors();
    }

    /// <summary>
    /// Switch character settings by selector index
    /// </summary>
    // 초기 문 오브젝트 설정
    void SetDoors()
    {
        int randomValue;

        for (int i = 0; i < _0F_doors.Length; i++)
        {
            _0F_doors_comp[i] = _0F_doors[i].GetComponent<Door>();            

            if(i == 1 || i == 2 || i == 4 || i == 5)
            {
                _0F_doors_comp[i].openAngle = -120f;
            }
            else _0F_doors_comp[i].openAngle = 120f;

            randomValue = Random.Range(0, 2);

            if(randomValue == 0)
                _0F_doors[i].GetComponent<AudioSource>().clip = openDoorSound[0];
            else
                _0F_doors[i].GetComponent<AudioSource>().clip = openDoorSound[1];  
            
            _0F_doors_comp[i].SetAudio();
        }

        for(int j = 0; j < _1F_doors.Length; j++)
        {
            _1F_doors_comp[j] = _1F_doors[j].GetComponent<Door>();

            if (j == 1 || j == 2)
            {
                _1F_doors_comp[j].openAngle = -120f;
            }
            else _1F_doors_comp[j].openAngle = 120f;

            randomValue = Random.Range(0, 2);

            if (randomValue == 0)
                _1F_doors[j].GetComponent<AudioSource>().clip = openDoorSound[0];
            else
                _1F_doors[j].GetComponent<AudioSource>().clip = openDoorSound[1];

            _1F_doors_comp[j].SetAudio();
        }
    }
}
