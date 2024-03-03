using ParkingHUB.Pagination;

namespace ParkingHUB.Interface
{
    public interface IPagination<T>
    {
        Task<PageResult<T>> GetParkingListPagination(PaginationPage filter);
    }
}
