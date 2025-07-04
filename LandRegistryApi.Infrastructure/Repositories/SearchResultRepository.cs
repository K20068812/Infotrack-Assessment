﻿using LandRegistryApi.Core.Entities;
using LandRegistryApi.Core.Interfaces;
using LandRegistryApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LandRegistryApi.Infrastructure.Repositories
{
    public class SearchResultRepository : ISearchResultRepository
    {
        private readonly ApplicationDbContext _context;
        public SearchResultRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<SearchResult>> GetAllSearchResultsAsync(string targetUrl, string searchQuery, string searchEngine)
        {
            return await _context.SearchResults
                .Where(sr => sr.TargetUrl == targetUrl && sr.SearchQuery == searchQuery && sr.SearchEngine == searchEngine)
                .OrderByDescending(sr => sr.SearchDate)
                .ToListAsync();
        }

        public async Task<SearchResult> SaveSearchResultAsync(SearchResult result)
        {
            _context.SearchResults.Add(result);
            await _context.SaveChangesAsync();
            return result;
        }
    }
}
