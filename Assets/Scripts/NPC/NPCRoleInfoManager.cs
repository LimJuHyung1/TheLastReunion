using System.Diagnostics;

public class NPCRoleInfoManager
{
    public string GetRole(string npcName) => GetNPCRoleByName(npcName)?.role;
    public string GetInstructions(string npcName) => GetNPCRoleByName(npcName)?.instructions;
    public string GetBackground(string npcName) => GetNPCRoleByName(npcName)?.background;
    public string GetFriends(string npcName) => GetNPCRoleByName(npcName)?.friends;
    public string GetAlibi(string npcName) => GetNPCRoleByName(npcName)?.alibi;
    public string GetResponseGuidelines(string npcName) => GetNPCRoleByName(npcName)?.responseGuidelines;

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
