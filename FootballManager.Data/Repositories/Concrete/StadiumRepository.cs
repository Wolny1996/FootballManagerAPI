﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FootballManager.Data.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Polly;

namespace FootballManager.Data.Repositories.Concrete
{
    public class StadiumRepository : IStadiumRepository
    {
        private readonly FootballManagerContext _context;
        private readonly ILogger<StadiumRepository> _logger;

        public StadiumRepository(FootballManagerContext context, ILogger<StadiumRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Stadium> GetSpecificStadium(string stadiumName)
        {
            var query = _context.Stadiums
                    .Include(s => s.Club);

            var policy = Policy.Handle<SqlException>().WaitAndRetryAsync(new[]
            {
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(15),
            }, (ext, timeSpan, retryCount, context) =>
                {
                    _logger.LogError(ext, $"Error - try retry (count: {retryCount}, timeSpan: {timeSpan})");
                });

            var searchedStadium = await policy.ExecuteAsync(() => query.FirstOrDefaultAsync(s => s.StadiumName == stadiumName));

            if (searchedStadium == null)
            {
                _logger.LogInformation($"Stadium with {stadiumName} doesn't exists.");
                // TODO return error object with proper error code.
                throw new Exception("Entity not founded.");
            }

            return searchedStadium;
        }

        public async Task<IEnumerable<Stadium>> GetAllStadiums()
        {
            var query = _context.Stadiums
                .Include(s => s.Club)
                .AsNoTracking();

            var policy = Policy.Handle<SqlException>().WaitAndRetryAsync(new[]
            {
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(15),
            }, (ext, timeSpan, retryCount, context) =>
                {
                    _logger.LogError(ext, $"Error - try retry (count: {retryCount}, timeSpan: {timeSpan})");
                });

            var stadiums = await policy.ExecuteAsync(() => query.ToListAsync());

            if (stadiums == null)
            {
                _logger.LogInformation($"No stadiums in database.");
                // TODO return error object with proper error code.
                throw new Exception("Entity not founded.");
            }

            return stadiums;
        }

        public async Task PutExistingStadium(string stadiumName, string stadiumClub, Stadium updatedStadium)
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

            var existingStadium = await policy.ExecuteAsync(() =>
                _context.Stadiums.FirstOrDefaultAsync(s => s.StadiumName == stadiumName));

            if (existingStadium == null)
            {
                _logger.LogInformation($"Footballer with name {stadiumName} doesn't exists.");
                // TODO return error object with proper error code.
                throw new Exception("Entity not founded.");
            }

            var existingClub = await _context.Clubs.FirstOrDefaultAsync(c => c.ClubName == stadiumClub);

            if (existingClub == null)
            {
                _logger.LogInformation($"Club with name {stadiumClub} doesn't exists.");
                // TODO return error object with proper error code.
                throw new Exception("Entity not founded.");
            }

            existingStadium.StadiumName = updatedStadium.StadiumName;
            existingStadium.Capacity = updatedStadium.Capacity;
            existingStadium.Club = existingClub;
            _context.Update(existingStadium);
            await _context.SaveChangesAsync();
        }

        public async Task PostNewStadium(string addedStadiumClub, Stadium newStadium)
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
                _context.Clubs.FirstOrDefaultAsync(c => c.ClubName == addedStadiumClub));

            if (existingClub == null)
            {
                _logger.LogInformation($"Club with name {addedStadiumClub} doesn't exists.");
                // TODO return error object with proper error code.
                throw new Exception("Entity not founded.");
            }

            newStadium.Club = existingClub;
            _context.Stadiums.Add(newStadium);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSpecificStadium(string stadiumName)
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

            var existingStadium = await policy.ExecuteAsync(() =>
                _context.Stadiums.FirstOrDefaultAsync(s => s.StadiumName == stadiumName));

            if (existingStadium == null)
            {
                _logger.LogInformation($"Stadium with name {stadiumName} doesn't exists.");
                // TODO return error object with proper error code.
                throw new Exception("Entity not founded.");
            }

            _context.Stadiums.Remove(existingStadium);
            await _context.SaveChangesAsync();
        }
    }
}