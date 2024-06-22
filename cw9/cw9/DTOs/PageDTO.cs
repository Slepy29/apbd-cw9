namespace cw9.DTOs;

public class PageDTO
{
    public int pageNum { get; set; }
    public int pageSize { get; set; }
    public int allPages { get; set; }
    public IEnumerable<GetTripsDTO> trips { get; set; } = new List<GetTripsDTO>();
}