using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class Thing : MonoBehaviour
{
    [SerializeField] public string Name { get; private set; }
    [SerializeField] public string Description { get; private set; }

    public void Initialize(string name, string description)
    {
        Name = name;
        Description = description;
    }

    List<EvidenceInfo> LoadThingDataFromJSON(string path)
    {
        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<List<EvidenceInfo>>(json);
    }
}
