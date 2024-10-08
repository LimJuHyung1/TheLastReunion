using UnityEngine;

public class Furniture : Thing
{
    public string Material { get; set; }
    public float Weight { get; set; }

    public void Initialize(string name, string description, string material, float weight)
    {
        base.Initialize(name, description);  // �θ� Ŭ������ Initialize �޼��� ȣ��
        Material = material;
        Weight = weight;
    }
}
