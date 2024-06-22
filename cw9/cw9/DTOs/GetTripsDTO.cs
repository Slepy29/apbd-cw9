namespace cw9.DTOs;

public class GetTripsDTO
{
    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime DateFrom { get; set; }

    public DateTime DateTo { get; set; }

    public int Maxpeople { get; set; }

    public IEnumerable<GetClientDTO> Clients { get; set; } = new List<GetClientDTO>();

    public IEnumerable<GetCountryDTO> Countries { get; set; } = new List<GetCountryDTO>();
}