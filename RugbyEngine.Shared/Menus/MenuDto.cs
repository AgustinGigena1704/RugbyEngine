namespace RugbyEngine.Shared.Menus
{
    public class MenuDto
    {
        public required string Title { get; set; }
        public string? ToolTip { get; set; } = null;
        public string? Icon { get; set; } = null;
        public string? Route { get; set; } = null;
        public string? Role { get; set; } = null;
        public List<MenuDto> Items { get; set; } = new List<MenuDto>();

    }
}
