using BSBoilerPlate.Models.DTOs;

namespace BSBoilerPlate.Models.DTOs
{
    public class LogListDto
    {
        public List<LogListItemDto> Items { get; set; } = new();
        public int ItemTotalCount { get; set; } = 0; // The total count of items before paging
    }
}
