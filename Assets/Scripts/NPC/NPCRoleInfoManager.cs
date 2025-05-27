using System.Diagnostics;

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
    /// string name 받는 로직 수정 필요함 - jsonManager에서 이름을 잘라서 받음
    /// </summary>
    /// <param name="npcName"></param>
    /// <returns></returns>
    private NPCRoleInfo GetNPCRoleByName(string npcName)
    {
        if (JsonManager.npcRoleInfoList != null)
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
        return null;
    }
}
