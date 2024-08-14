namespace Idams.Core.Model.Dtos
{
    public class UserDto
    {
        public string EmpId { get; set; }
        public string EmpAccount { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<UserRoleDto> Roles { get; set; }
        public List<MenuDto> Menu { get; set; }
        public HierLvlDto HierLvl2 { get; set; }
        public HierLvlDto HierLvl3 { get; set; }
    }
}
