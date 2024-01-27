using System.ComponentModel.DataAnnotations;

namespace Notes.Common.Paging
{
    public class PageInput
    {
        public string SortBy { get; set; }
        
        public SortOrder SortOrder { get; set; }
        
        public int Offset { get; set; }
        
        [Range(1, int.MaxValue)]
        public int PageSize { get; set; }
    }
}