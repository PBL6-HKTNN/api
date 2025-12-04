using Codemy.Search.Application.DTOs;
using Codemy.Search.Application.Interfaces;
using Elasticsearch.Net;
using Nest;

namespace Codemy.Search.Infrastructure.Services
{
    public class ElasticCourseSearchService : ICourseSearchService
    {
        private readonly IElasticClient _client;

        public ElasticCourseSearchService(IElasticClient client)
        {
            _client = client;
        }

        public async Task IndexBulkAsync(IEnumerable<CourseIndexDto> courses)
        {
            // Ensure the index exists
            var exists = await _client.Indices.ExistsAsync(_client.ConnectionSettings.DefaultIndex);

            if (!exists.Exists)
                await _client.Indices.CreateAsync(_client.ConnectionSettings.DefaultIndex,
                    c => c.Map<CourseIndexDto>(m => m.AutoMap()));

            var bulk = await _client.BulkAsync(b => b.IndexMany(courses));

            if (bulk.Errors)
                throw new Exception($"Bulk indexing failed: {bulk.DebugInformation}");
        }

        public async Task<PagedResult<CourseSearchResult>> SearchAsync(CourseSearchRequest request)
        {
            // Pagination
            var from = (request.Page - 1) * request.PageSize;

            // base query (search q)
            QueryContainer query = new MatchAllQuery();

            if (!string.IsNullOrWhiteSpace(request.Q))
            {
                query = new MultiMatchQuery
                {
                    Fields = Infer.Fields<CourseIndexDto>(
                        x => x.Title,
                        x => x.Description
                    ),
                    Query = request.Q,
                    Fuzziness = Fuzziness.Auto
                };
            }

            // filters
            var filters = new List<QueryContainer>();

            if (!string.IsNullOrWhiteSpace(request.Language))
            {
                filters.Add(new TermQuery
                {
                    Field = "language.keyword",
                    Value = request.Language
                });
            }

            if (!string.IsNullOrWhiteSpace(request.Level))
            {
                int parsedLevel = request.Level.ToLower() switch
                {
                    "beginner" => 0,
                    "intermediate" => 1,
                    "advanced" => 2,
                    _ => -1
                };

                if (parsedLevel != -1)
                {
                    filters.Add(new TermQuery
                    {
                        Field = "level",
                        Value = parsedLevel
                    });
                }
            }

            // final query
            var finalQuery = new BoolQuery
            {
                Must = new QueryContainer[] { query },
                Filter = filters
            };

            // sorting
            Func<SortDescriptor<CourseIndexDto>, IPromise<IList<ISort>>> sortSelector = s => s;

            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                sortSelector = request.SortBy.ToLower() switch
                {
                    "name"   => request.SortDesc ? (s => s.Descending("title.keyword")) : (s => s.Ascending("title.keyword")),
                    "rating" => request.SortDesc ? (s => s.Descending("averageRating")) : (s => s.Ascending("averageRating")),
                    _        => sortSelector
                };
            }

            // execute search
            var response = await _client.SearchAsync<CourseIndexDto>(s => s
                .Query(q => finalQuery)
                .Sort(sortSelector)
                .From(from)
                .Size(request.PageSize)
            );

            var items = response.Documents.Select(d => new CourseSearchResult
            {
                Id = d.Id,
                InstructorId = d.InstructorId,
                Title = d.Title,
                Description = d.Description,
                Thumbnail = d.Thumbnail,
                Status = d.Status,
                Duration = d.Duration,
                Price = d.Price,
                Level = d.Level,
                NumberOfModules = d.NumberOfModules,
                CategoryId = d.CategoryId,
                Language = d.Language,
                NumberOfReviews = d.NumberOfReviews,
                AverageRating = d.AverageRating
            }).ToList();

            return new PagedResult<CourseSearchResult>
            {
                Page = request.Page,
                PageSize = request.PageSize,
                Total = response.Total,
                Items = items
            };
        }
    }
}
