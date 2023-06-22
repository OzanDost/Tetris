using System.Linq;
using Cinemachine;

namespace Game
{
    public static class Extensions
    {
        public static void RemoveMembersRange(this CinemachineTargetGroup targetGroup, int index, int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (index >= 0 && index < targetGroup.m_Targets.Length)
                {
                    var targetsList = targetGroup.m_Targets.ToList();
                    targetsList.RemoveAt(index);
                    targetGroup.m_Targets = targetsList.ToArray();
                }
                else
                {
                    break;
                }
            }
        }
    }
}