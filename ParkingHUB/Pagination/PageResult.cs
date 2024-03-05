namespace ParkingHUB.Pagination
{
    public class PageResult<T>
    {
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int? PreviousPage { get; set; }
        public int? NextPage { get; set; }
        public List<T> Results { get; set; }
        public string? Location {  get; set; }
        public string? PlateLicense {  get; set; }
        public DateTime? CheckIn {  get; set; }
        public DateTime? CheckOut {  get; set; }
        public int? Availabel {  get; set; }
    }
}
