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
            // Không cần _index — Elasticsearch sẽ dùng DefaultIndex
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
            var from = (request.Page - 1) * request.PageSize;

            // Base query (search by keyword)
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

            // Build filter queries dynamically
            var filterQueries = new List<QueryContainer>();

            if (request.Filters != null && request.Filters.Any())
            {
                foreach (var filter in request.Filters)
                {
                    if (filter.Value is null) continue;

                    var valueStr = filter.Value.ToString();

                    // Check if the filter value is a collection (for terms query)
                    if (filter.Value is IEnumerable<object> values && !(filter.Value is string))
                    {
                        filterQueries.Add(new TermsQuery
                        {
                            Field = $"{filter.Key}.keyword",
                            Terms = values
                        });
                    }
                    else
                    {
                        filterQueries.Add(new TermQuery
                        {
                            Field = $"{filter.Key}.keyword",
                            Value = valueStr
                        });
                    }
                }
            }

            // Combine main query + filters
            var finalQuery = new BoolQuery
            {
                Must = new QueryContainer[] { query },
                Filter = filterQueries
            };

            // Handle sorting
            Func<SortDescriptor<CourseIndexDto>, IPromise<IList<ISort>>> sortSelector = s => s;
            if (!string.IsNullOrEmpty(request.SortBy))
            {
                sortSelector = s => request.SortDesc
                    ? s.Descending(request.SortBy)
                    : s.Ascending(request.SortBy);
            }

            // Execute Elasticsearch query
            var search = await _client.SearchAsync<CourseIndexDto>(s => s
                .From(from)
                .Size(request.PageSize)
                .Query(_ => finalQuery)
                .Sort(sortSelector)
            );

            // Return paged result
            return new PagedResult<CourseSearchResult>
            {
                Page = request.Page,
                PageSize = request.PageSize,
                Total = search.Total,
                Items = search.Documents.Select(d => new CourseSearchResult
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
                }).ToList()
            };
        }
    }
}
