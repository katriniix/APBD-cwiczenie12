using Cwiczenie12.DTOs;
using Cwiczenie12.Models;

namespace Cwiczenie12.Services;

public interface IDbService
{
    Task<PagesDTO> GetTrips(int page = 1, int pageSize = 10);
    Task DeleteClientAsync(int clientId);
    Task AddClientToTripAsync(int idTrip, ClientToTripDTO clientToTrip);
}