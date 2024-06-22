using cw9.Models;

namespace cw9.Services;

public interface IDbService
{
    public Task<List<Trip>> GetTrips();
    public Task RemoveClient(int id);
    public Task<Client?> GetClient(int id);
    public Task<bool> ValidateTrip(int tripId, string pesel);
    public Task CreateClientTrip(ClientTrip clientTrip);
    public Task<Trip?> GetTrip(int id);
}