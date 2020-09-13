using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FootballManager.Data.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Polly;

namespace FootballManager.Data.Repositories.Concrete
{
    public class ClubRepository : IClubRepository
    {
        private readonly FootballManagerContext _context;
        private readonly ILogger<ClubRepository> _logger;

        public ClubRepository(FootballManagerContext context, ILogger<ClubRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Club> GetSpecificClub(string clubName)
        {
            var query = _context.Clubs
                .Include(c => c.Stadium)
                .Include(c => c.Coach)
                .Include(c => c.Footballers)
                .Include(c => c.ClubTournaments)
                .ThenInclude(ct => ct.Tournaments);

            var policy = Policy.Handle<SqlException>().WaitAndRetryAsync(new[]
            {
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(15),
            }, (ext, timeSpan, retryCount, context) =>
            {
                _logger.LogError(ext, $"Error - try retry (count: {retryCount}, timeSpan: {timeSpan})");
            });

            var searchedClub = await policy.ExecuteAsync(() => query.FirstOrDefaultAsync(c => c.ClubName == clubName));

            if (searchedClub == null)
            {
                _logger.LogInformation($"Club with name {clubName} doesn't exists.");
                // TODO return error object with proper error code.
                throw new Exception("Entity not founded.");
            }

            return searchedClub;
        }

        public async Task<IEnumerable<Club>> GetAllClubs()
        {
            var query = _context.Clubs
                .Include(c => c.Stadium)
                .Include(c => c.Coach)
                .Include(c => c.Footballers)
                .Include(c => c.ClubTournaments)
                .ThenInclude(ct => ct.Tournaments);

            var policy = Policy.Handle<SqlException>().WaitAndRetryAsync(new[]
            {
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(15),
            }, (ext, timeSpan, retryCount, context) =>
            {
                _logger.LogError(ext, $"Error - try retry (count: {retryCount}, timeSpan: {timeSpan})");
            });

            var clubs = await policy.ExecuteAsync(() => query.ToListAsync());

            if (clubs == null)
            {
                _logger.LogInformation($"No clubs in database.");
                // TODO return error object with proper error code.
                throw new Exception("Entity not founded.");
            }

            return clubs;
        }

        public async Task PutExistingClub(string clubName, Club updatedClub)
        {
            var policy = Policy.Handle<SqlException>().WaitAndRetryAsync(new[]
            {
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(15),
            }, (ext, timeSpan, retryCount, context) =>
                {
                    _logger.LogError(ext, $"Error - try retry (count: {retryCount}, timeSpan: {timeSpan})");
                });

            var existingClub = await policy.ExecuteAsync(() =>
                _context.Clubs.FirstOrDefaultAsync(c => c.ClubName == clubName));

            if (existingClub == null)
            {
                _logger.LogInformation($"Club with name {clubName} doesn't exists.");
                // TODO return error object with proper error code.
                throw new Exception("Entity not founded.");
            }

            existingClub.ClubName = updatedClub.ClubName;
            existingClub.City = updatedClub.City;
            existingClub.Founded = updatedClub.Founded;
            _context.Update(existingClub);
            await _context.SaveChangesAsync();
        }

        public async Task PostNewClub(Club newClub)
        {
            var policy = Policy.Handle<SqlException>().WaitAndRetryAsync(new[]
            {
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(15),
            }, (ext, timeSpan, retryCount, context) =>
                {
                    _logger.LogError(ext, $"Error - try retry (count: {retryCount}, timeSpan: {timeSpan})");
                });

            _context.Clubs.Add(newClub);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSpecificClub(string clubName)
        {
            var policy = Policy.Handle<SqlException>().WaitAndRetryAsync(new[]
            {
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(15),
            }, (ext, timeSpan, retryCount, context) =>
                {
                    _logger.LogError(ext, $"Error - try retry (count: {retryCount}, timeSpan: {timeSpan})");
                });

            var existingClub = await policy.ExecuteAsync(() =>
                _context.Clubs.FirstOrDefaultAsync(c => c.ClubName == clubName));

            if (existingClub == null)
            {
                _logger.LogInformation($"Club with name {clubName} doesn't exists.");
                // TODO return error object with proper error code.
                throw new Exception("Entity not founded.");
            }

            _context.Clubs.Remove(existingClub);
            await _context.SaveChangesAsync();
        }
    }
}