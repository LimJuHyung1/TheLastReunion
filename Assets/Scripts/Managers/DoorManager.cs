using UnityEngine;

public class DoorManager : MonoBehaviour
{
    public AudioClip[] openDoorSound = new AudioClip[2];
    public GameObject[] _0F_doors;
    public GameObject[] _1F_doors;

    private Door[] _0F_doors_comp;
    private Door[] _1F_doors_comp;

    void Start()
    {
        _0F_doors_comp = InitializeDoors(_0F_doors, new int[] { 1, 2, 4, 5 });
        _1F_doors_comp = InitializeDoors(_1F_doors, new int[] { 1, 2 });
    }

    // �� ������Ʈ �ʱ�ȭ �޼���
    private Door[] InitializeDoors(GameObject[] doors, int[] reverseAngleIndices)
    {
        Door[] doorComponents = new Door[doors.Length];
        for (int i = 0; i < doors.Length; i++)
        {
            doorComponents[i] = doors[i].GetComponent<Door>();
            SetDoorProperties(doorComponents[i], i, reverseAngleIndices);
        }
        return doorComponents;
    }

    // �� �Ӽ� ����
    private void SetDoorProperties(Door door, int index, int[] reverseAngleIndices)
    {
        door.SetOpenAngle(ShouldReverseAngle(index, reverseAngleIndices) ? -120f : 120f);
        door.SetAudioClip(GetRandomAudioClip());
    }

    // ���� ����� Ŭ�� ����
    private AudioClip GetRandomAudioClip()
    {
        int randomValue = Random.Range(0, openDoorSound.Length);
        return openDoorSound[randomValue];
    }

    // Ư�� �ε����� �ݴ� ������ �����ؾ� �ϴ��� Ȯ��
    private bool ShouldReverseAngle(int index, int[] reverseAngleIndices)
    {
        foreach (int reverseIndex in reverseAngleIndices)
        {
            if (index == reverseIndex) return true;
        }
        return false;
    }
}
