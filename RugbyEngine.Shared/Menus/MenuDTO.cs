namespace RugbyEngine.Shared.Menus
{
    public class MenuDTO
    {
        public required string Title { get; set; }
        public string? ToolTip { get; set; } = null;
        public string? Icon { get; set; } = null;
        public string? Route { get; set; } = null;
        public List<MenuDTO> Items { get; set; } = new List<MenuDTO>();

    }
}
