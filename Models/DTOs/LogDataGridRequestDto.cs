using MudBlazor;

namespace BSBoilerPlate.Models.DTOs
{
    public class LogDataGridRequestDto
    {
        public int Page { get; set; } = 0; // The page number for the data we're requesting
        public int PageSize { get; set; } = 10; // The number of items per page

        public ICollection<SortDefinition<LogListItemDto>> SortDefinitions { get; set; }

        public ICollection<IFilterDefinition<LogListItemDto>> FilterDefinitions { get; set; }
    }
}
