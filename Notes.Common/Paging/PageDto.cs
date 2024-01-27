namespace Notes.Common.Paging
{
    public class PageDto<T>
    {
        public int Offset { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
        public IEnumerable<T> Result { get; set; }
    }
}