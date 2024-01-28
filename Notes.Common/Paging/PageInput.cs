using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Notes.Common.Paging
{
    public class PageInput
    {
        public string? SortBy { get; set; }
        
        [DefaultValue(Paging.SortOrder.Asc)]
        public SortOrder? SortOrder { get; set; }
        
        [Range(0, int.MaxValue)]
        public int Offset { get; set; }
        
        [Range(1, 500)]
        public int PageSize { get; set; }
    }
}