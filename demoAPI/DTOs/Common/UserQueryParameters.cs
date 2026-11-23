namespace demoAPI.DTOs.Common
{
    public class UserQueryParameters
    {
        private const int MaxPageSize = 100000;

        public int PageNumber { get; set; } = 1;

        private int _pageSize = 10;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }

        // Search
        public string? SearchTerm { get; set; }

        // Filter
        public string? Role { get; set; }

        // Sort
        public string? SortBy { get; set; }

        public bool IsDescending { get; set; } = false;
    }
}

