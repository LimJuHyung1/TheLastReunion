using System.Diagnostics;
using UnityEngine.Localization.Settings;

public class NPCRoleInfoManager
{
    public string GetRole(string npcName) => GetNPCRoleByName(npcName)?.role;
    public string GetAudience(string npcName) => GetNPCRoleByName(npcName)?.audience;
    public string GetInformation(string npcName) => GetNPCRoleByName(npcName)?.information;
    public string GetTask(string npcName) => GetNPCRoleByName(npcName)?.task;
    public string GetRule(string npcName) => GetNPCRoleByName(npcName)?.rule;
    public string GetStyle(string npcName) => GetNPCRoleByName(npcName)?.style;
    public string GetConstraint(string npcName) => GetNPCRoleByName(npcName)?.constraint;
    public string GetFormat(string npcName) => GetNPCRoleByName(npcName)?.format;

    /// <summary>
    /// string name �޴� ���� ���� �ʿ��� - jsonManager���� �̸��� �߶� ����
    /// </summary>
    /// <param name="npcName"></param>
    /// <returns></returns>
    private NPCRoleInfo GetNPCRoleByName(string npcName)
    {
        var currentLocale = LocalizationSettings.SelectedLocale;

        if (JsonManager.npcRoleInfoList != null)
        {
            if (currentLocale.Identifier.Code == "ja")
            {
                foreach (NPCRoleInfo info in JsonManager.npcRoleInfoList.npcRoleInfoList)
                {
                    string name = info.role.Substring(0, info.role.IndexOf('��'));
                    if (name == npcName)
                    {
                        return info;
                    }
                }
            }
            else
            {
                foreach (NPCRoleInfo info in JsonManager.npcRoleInfoList.npcRoleInfoList)
                {
                    string name = info.role.Substring(0, info.role.IndexOf(','));
                    if (name == npcName)
                    {
                        return info;
                    }
                }
            }
        }
        return null;
    }
}
