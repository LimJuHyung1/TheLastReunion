using UnityEngine;

public class Furniture : Thing
{
    public string Material { get; set; }
    public float Weight { get; set; }

    public void Initialize(string name, string description, string material, float weight)
    {
        base.Initialize(name, description);  // 부모 클래스의 Initialize 메서드 호출
        Material = material;
        Weight = weight;
    }
}
