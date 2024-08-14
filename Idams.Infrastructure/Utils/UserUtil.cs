using Idams.Core.Enums;
using Idams.Core.Model.Dtos;

namespace Idams.Infrastructure.Utils
{
    public class UserUtil
    {
        public static void DetermineUserPrivelege(UserDto user, out string? hierlvl2, out string? hierlvl3)
        {
            bool allData = false;
            bool lvl2 = false;
            hierlvl2 = null;
            hierlvl3 = null;
            foreach (var role in user.Roles)
            {
                switch (role.Enum)
                {
                    case RoleEnum.SUPER_ADMIN:
                    case RoleEnum.ADMIN_SHU:
                    case RoleEnum.PLANNING_SHU:
                    case RoleEnum.PIC_SHU:
                    case RoleEnum.REVIEWER_SHU:
                        allData = true;
                        break;
                    case RoleEnum.ADMIN_REGIONAL:
                    case RoleEnum.PLANNING_REGIONAL:
                    case RoleEnum.PIC_REGIONAL:
                    case RoleEnum.REVIEWER_REGIONAL:
                        lvl2 = true;
                        break;
                }
            }

            if (allData)
                return;
            hierlvl2 = string.IsNullOrWhiteSpace(user.HierLvl2.Value) ? null : user.HierLvl2?.Key;
            if (lvl2)
                return;
            hierlvl3 = string.IsNullOrWhiteSpace(user.HierLvl3.Value) ? null : user.HierLvl3?.Key;
        }
    }
}
