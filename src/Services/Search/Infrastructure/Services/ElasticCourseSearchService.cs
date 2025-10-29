using Codemy.Search.Application.DTOs;
using Codemy.Search.Application.Interfaces;
using Elasticsearch.Net;
using Nest;

namespace Codemy.Search.Infrastructure.Services
{
    public class ElasticCourseSearchService : ICourseSearchService
    {
        private readonly IElasticClient _client;
        private readonly string _index = "courses";

        public ElasticCourseSearchService(IElasticClient client)
        {
            _client = client;
        }

        public async Task IndexBulkAsync(IEnumerable<CourseIndexDto> courses)
        {
            var exists = await _client.Indices.ExistsAsync(_index);
            if (!exists.Exists)
                await _client.Indices.CreateAsync(_index, c => c.Map<CourseIndexDto>(m => m.AutoMap()));

            var bulk = await _client.BulkAsync(b => b.Index(_index).IndexMany(courses));
            if (bulk.Errors) throw new Exception(bulk.DebugInformation);
        }

        public async Task<PagedResult<CourseSearchResult>> SearchAsync(CourseSearchRequest request)
        {
            var from = (request.Page - 1) * request.PageSize;
            QueryContainer query = new MatchAllQuery();

            if (!string.IsNullOrWhiteSpace(request.Q))
            {
                query = new MultiMatchQuery
                {
                    Fields = Infer.Fields<CourseIndexDto>(p => p.Title, p => p.Description),
                    Query = request.Q,
                    Fuzziness = Fuzziness.Auto
                };
            }

            var search = await _client.SearchAsync<CourseIndexDto>(s => s
                .Index(_index)
                .From(from)
                .Size(request.PageSize)
                .Query(_ => query)
            );

            return new PagedResult<CourseSearchResult>
            {
                Page = request.Page,
                PageSize = request.PageSize,
                Total = search.Total,
                Items = search.Documents.Select(d => new CourseSearchResult
                {
                    Id = d.Id,
                    Title = d.Title,
                    Description = d.Description,
                    Category = d.Category,
                    Price = d.Price,
                    Level = d.Level,
                    Rating = d.Rating
                }).ToList()
            };
        }
    }
}
