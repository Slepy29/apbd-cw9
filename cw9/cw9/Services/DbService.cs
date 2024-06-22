using cw9.Data;
using cw9.Models;
using Microsoft.EntityFrameworkCore;

namespace cw9.Services;

public class DbService : IDbService
{
    private readonly DatabaseContext _context;

    public DbService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<List<Trip>> GetTrips()
    {
        return await _context.Trips
            .Include(t => t.ClientTrips)
            .ThenInclude(ct => ct.Client)
            .Include(t => t.Countries)
            .OrderBy(t => t.DateFrom)
            .ToListAsync();
    }

    public async Task RemoveClient(int id)
    {
        var client = await GetClient(id);
        if (client != null)
        {
            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Client?> GetClient(int id)
    {
        return await _context.Clients
            .Include(c => c.ClientTrips)
            .FirstOrDefaultAsync(c => c.IdClient == id);
    }

    public async Task<bool> ValidateTrip(int tripId, string pesel)
    {
        var existingClient = await _context.Clients.AnyAsync(c => c.Pesel == pesel);
        if (existingClient)
        {
            return false;
        }

        var isClientAssigned = await _context.ClientTrips.AnyAsync(ct => ct.Client.Pesel == pesel && ct.IdTrip == tripId);
        if (isClientAssigned)
        {
            return false;
        }

        var trip = await _context.Trips.AnyAsync(t => t.IdTrip == tripId && t.DateFrom > DateTime.Now);
        return trip;
    }

    public async Task CreateClientTrip(ClientTrip clientTrip)
    {
        _context.ClientTrips.Add(clientTrip);
        await _context.SaveChangesAsync();
    }

    public async Task<Trip?> GetTrip(int id)
    {
        return await _context.Trips.FirstOrDefaultAsync(t => t.IdTrip == id);
    }
}
