using Cwiczenie12.DTOs;
using Cwiczenie12.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;

namespace Cwiczenie12.Services;

public class DbService : IDbService
{
    private readonly TripsDbContext _context;

    public DbService(TripsDbContext context)
    {
        _context = context;
    }

    public async Task<PagesDTO> GetTrips(int page = 1, int pageSize = 10)
    {
        var totalCount = await _context.Trips.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        page = Math.Max(1, page);
        pageSize = Math.Max(1, pageSize);

        var trips = await _context.Trips
            .OrderByDescending(t => t.DateFrom)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TripDTO
            {
                Name = t.Name,
                Description = t.Description,
                DateFrom = t.DateFrom,
                DateTo = t.DateTo,
                MaxPeople = t.MaxPeople,
                Countries = t.IdCountries
                    .Select(ct => new CountryDTO
                    {
                        Name = ct.Name
                    }).ToList(),
                Clients = t.ClientTrips
                    .Select(ct => new ClientDTO
                    {
                        FirstName = ct.IdClientNavigation.FirstName,
                        LastName = ct.IdClientNavigation.LastName,
                    }).ToList()
            }).ToListAsync();

        return new PagesDTO()
        {
            pageNum = page,
            pageSize = pageSize,
            allPages = totalPages,
            trips = trips
        };
    }

    public async Task DeleteClientAsync(int clientId)
    {
        var client = await _context.Clients.FindAsync(clientId);
        if (client == null)
        {
            throw new Exception($"Client with id {clientId} not found");
        }
        if (await _context.ClientTrips.AnyAsync(ct => ct.IdClient == clientId))
        {
            throw new Exception($"Klient {clientId} ma przypisane wycieczki");
        }
        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();
    }

    public async Task AddClientToTripAsync(int idTrip, ClientToTripDTO clientToTrip)
    {
        var client = await _context.Clients.FirstOrDefaultAsync(c => c.Pesel == clientToTrip.Pesel);

        if (client != null)
        {
            bool zapisany = await _context.ClientTrips.AnyAsync(ct => ct.IdTrip == idTrip && ct.IdClient == client.IdClient);
            if (zapisany)
            {
                throw new Exception("Klient juz jest zapisany na ta wycieczke");
            }
            throw new Exception("Klient juz istnieje");
        }

        var newClient = new Client
        {
            FirstName = clientToTrip.FirstName,
            LastName = clientToTrip.LastName,
            Email = clientToTrip.Email,
            Telephone = clientToTrip.Telephone,
            Pesel = clientToTrip.Pesel
        };
        _context.Clients.Add(newClient);
        await _context.SaveChangesAsync();

        var trip = await _context.Trips.FirstOrDefaultAsync(t => t.IdTrip == clientToTrip.IdTrip);
        if (trip == null)
        {
            throw new Exception("Wycieczka nie istnieje");
        }
        if (trip.DateFrom <= DateTime.Now)
        {
            throw new Exception("Nie mozna zapisac sie na wycieczke, ktora juz sie odbyla");
        }
        
        var newClientTrip = new ClientTrip
        {
            IdClient = newClient.IdClient,
            IdTrip = trip.IdTrip,
            RegisteredAt = DateTime.Now,
            PaymentDate = clientToTrip.PaymentDate
        };
        _context.ClientTrips.Add(newClientTrip);
        await _context.SaveChangesAsync();
    }
}