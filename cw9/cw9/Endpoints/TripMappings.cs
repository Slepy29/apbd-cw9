using cw9.DTOs;
using cw9.Models;
using cw9.Services;
using Microsoft.AspNetCore.Mvc;

namespace cw9.Endpoints;

public static class TripMappings
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapGet("/api/trips", async ([FromServices] IDbService db, int page = 1, int pageSize = 10) =>
        {
            var trips = await db.GetTrips();
            var count = trips.Count;
            var totalPages = (int)Math.Ceiling(count / (double)pageSize);

            var paginatedTrips = trips
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new GetTripsDTO
                {
                    Name = e.Name,
                    DateFrom = e.DateFrom,
                    Maxpeople = e.MaxPeople,
                    Clients = e.ClientTrips.Select(c => new GetClientDTO
                    {
                        FirstName = c.Client.Firstname,
                        LastName = c.Client.Lastname
                    }),
                    Countries = e.Countries.Select(c => new GetCountryDTO { Name = c.Name })
                })
                .ToList();

            var result = new PageDTO()
            {
                pageNum = page,
                pageSize = pageSize,
                allPages = totalPages,
                trips = paginatedTrips
            };

            return Results.Ok(result);
        });

        app.MapDelete("/api/clients/{id}", async (int id, [FromServices] IDbService db) =>
        {
            var client = await db.GetClient(id);

            if (client == null)
            {
                return Results.NotFound("Client not found");
            }

            if (client.ClientTrips.Any())
            {
                return Results.BadRequest("Client is included in some trips, remove him before deleting");
            }

            await db.RemoveClient(id);
            return Results.NoContent();
        });

        app.MapPost("/api/trips/{idTrip}/clients", async (int idTrip, AddClientDTO clientDto, [FromServices] IDbService db) =>
        {
            if (await db.ValidateTrip(idTrip, clientDto.Pesel))
            {
                var newClient = new Client
                {
                    Firstname = clientDto.Firstname,
                    Lastname = clientDto.Lastname,
                    Email = clientDto.Email,
                    Telephone = clientDto.Telephone,
                    Pesel = clientDto.Pesel,
                    ClientTrips = new List<ClientTrip>()
                };

                var trip = await db.GetTrip(idTrip);

                if (trip != null)
                {
                    var newClientTrip = new ClientTrip
                    {
                        Client = newClient,
                        Trip = trip,
                        RegisteredAt = DateTime.Now,
                        PaymentDate = null
                    };

                    await db.CreateClientTrip(newClientTrip);
                    return Results.Created($"/api/trips/{idTrip}/clients", newClientTrip);
                }
            }

            return Results.BadRequest();
        });
    }
}