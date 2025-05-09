// ProductSearchService.cs
// Complete implementation of Elasticsearch product search with pagination, filtering and sorting
// Usage: 
// 1. Create ElasticsearchClient with your connection settings
// 2. Instantiate ProductSearchService with the client
// 3. Call SearchProductsAsync with your search criteria

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Elastic.Clients.Elasticsearch;
//using Elastic.Clients.Elasticsearch.Core.Search;
//using Elastic.Clients.Elasticsearch.QueryDsl;
//using Elastic.Clients.Elasticsearch.Aggregations;

//namespace ElasticsearchProductSearch
//{
//    public class Product
//    {
//        public string Id { get; set; }
//        public string Name { get; set; }
//        public string Description { get; set; }
//        public decimal Price { get; set; }
//        public string Category { get; set; }
//        public string Brand { get; set; }
//        public int StockQuantity { get; set; }
//        public DateTime CreatedDate { get; set; }
//        public double AverageRating { get; set; }
//        public bool IsActive { get; set; }
//        public List<string> Tags { get; set; } = new List<string>();
//    }

//    public class ProductSearchRequest
//    {
//        public int PageNumber { get; set; } = 1;
//        public int PageSize { get; set; } = 10;
//        public string SearchTerm { get; set; }
//        public string Category { get; set; }
//        public string Brand { get; set; }
//        public decimal? MinPrice { get; set; }
//        public decimal? MaxPrice { get; set; }
//        public bool? InStockOnly { get; set; }
//        public bool? ActiveOnly { get; set; }
//        public List<string> Tags { get; set; } = new List<string>();
//        public string SortBy { get; set; } = "createdDate";
//        public bool SortDescending { get; set; } = true;
//        public double? MinRating { get; set; }
//    }

//    public class ProductSearchResult
//    {
//        public IEnumerable<Product> Products { get; set; }
//        public long TotalCount { get; set; }
//        public int PageNumber { get; set; }
//        public int PageSize { get; set; }
//        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
//        public Dictionary<string, long> CategoryCounts { get; set; } = new Dictionary<string, long>();
//        public Dictionary<string, long> BrandCounts { get; set; } = new Dictionary<string, long>();
//        public Dictionary<string, long> TagCounts { get; set; } = new Dictionary<string, long>();
//    }

//    public class ProductSearchService
//    {
//        private readonly ElasticsearchClient _client;
//        private const string IndexName = "products";

//        public ProductSearchService(ElasticsearchClient client)
//        {
//            _client = client ?? throw new ArgumentNullException(nameof(client));
//        }

//        public async Task<ProductSearchResult> SearchProductsAsync(ProductSearchRequest request)
//        {
//            if (request == null)
//                throw new ArgumentNullException(nameof(request));

//            ValidateRequest(request);

//            var query = BuildQuery(request);
//            var sortOptions = BuildSortOptions(request);
//            int from = (request.PageNumber - 1) * request.PageSize;

//            var searchRequest = new SearchRequest<Product>(IndexName)
//            {
//                From = from,
//                Size = request.PageSize,
//                Query = query,
//                Sort = sortOptions,
//                Aggregations = BuildAggregations()
//            };

//            var response = await _client.SearchAsync<Product>(searchRequest);

//            if (!response.IsValidResponse)
//            {
//                throw new Exception($"Product search failed: {response.DebugInformation}");
//            }

//            return CreateSearchResult(response, request);
//        }

//        private void ValidateRequest(ProductSearchRequest request)
//        {
//            if (request.PageNumber < 1)
//                throw new ArgumentException("Page number must be greater than 0", nameof(request.PageNumber));

//            if (request.PageSize < 1 || request.PageSize > 100)
//                throw new ArgumentException("Page size must be between 1 and 100", nameof(request.PageSize));
//        }

//        private Query BuildQuery(ProductSearchRequest request)
//        {
//            var boolQuery = new BoolQuery();

//            AddSearchTermQuery(request, boolQuery);
//            AddCategoryFilter(request, boolQuery);
//            AddBrandFilter(request, boolQuery);
//            AddPriceRangeFilter(request, boolQuery);
//            AddStockFilter(request, boolQuery);
//            AddActiveFilter(request, boolQuery);
//            AddRatingFilter(request, boolQuery);
//            AddTagsFilter(request, boolQuery);

//            return boolQuery.HasAnyConditions() ? boolQuery : new MatchAllQuery();
//        }

//        private void AddSearchTermQuery(ProductSearchRequest request, BoolQuery boolQuery)
//        {
//            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
//            {
//                boolQuery.Must = new Query[]
//                {
//                    new MultiMatchQuery
//                    {
//                        Query = request.SearchTerm,
//                        Fields = new[] { "name", "description", "tags" },
//                        Operator = Operator.Or,
//                        Fuzziness = new Fuzziness("AUTO")
//                    }
//                };
//            }
//        }

//        private void AddCategoryFilter(ProductSearchRequest request, BoolQuery boolQuery)
//        {
//            if (!string.IsNullOrWhiteSpace(request.Category))
//            {
//                boolQuery.Filter = (boolQuery.Filter ?? new List<Query>()).Append(
//                    new TermQuery("category.keyword") { Value = request.Category }
//                );
//            }
//        }

//        private void AddBrandFilter(ProductSearchRequest request, BoolQuery boolQuery)
//        {
//            if (!string.IsNullOrWhiteSpace(request.Brand))
//            {
//                boolQuery.Filter = (boolQuery.Filter ?? new List<Query>()).Append(
//                    new TermQuery("brand.keyword") { Value = request.Brand }
//                );
//            }
//        }

//        private void AddPriceRangeFilter(ProductSearchRequest request, BoolQuery boolQuery)
//        {
//            if (request.MinPrice.HasValue || request.MaxPrice.HasValue)
//            {
//                var rangeQuery = new RangeQuery("price");

//                if (request.MinPrice.HasValue)
//                    rangeQuery.Gte = request.MinPrice.Value;

//                if (request.MaxPrice.HasValue)
//                    rangeQuery.Lte = request.MaxPrice.Value;

//                boolQuery.Filter = (boolQuery.Filter ?? new List<Query>()).Append(rangeQuery);
//            }
//        }

//        private void AddStockFilter(ProductSearchRequest request, BoolQuery boolQuery)
//        {
//            if (request.InStockOnly == true)
//            {
//                boolQuery.Filter = (boolQuery.Filter ?? new List<Query>()).Append(
//                    new RangeQuery("stockQuantity") { Gt = 0 }
//                );
//            }
//        }

//        private void AddActiveFilter(ProductSearchRequest request, BoolQuery boolQuery)
//        {
//            if (request.ActiveOnly == true)
//            {
//                boolQuery.Filter = (boolQuery.Filter ?? new List<Query>()).Append(
//                    new TermQuery("isActive") { Value = true }
//                );
//            }
//        }

//        private void AddRatingFilter(ProductSearchRequest request, BoolQuery boolQuery)
//        {
//            if (request.MinRating.HasValue)
//            {
//                boolQuery.Filter = (boolQuery.Filter ?? new List<Query>()).Append(
//                    new RangeQuery("averageRating") { Gte = request.MinRating.Value }
//                );
//            }
//        }

//        private void AddTagsFilter(ProductSearchRequest request, BoolQuery boolQuery)
//        {
//            if (request.Tags != null && request.Tags.Any())
//            {
//                boolQuery.Filter = (boolQuery.Filter ?? new List<Query>()).Append(
//                    new TermsQuery
//                    {
//                        Field = "tags.keyword",
//                        Terms = new TermsQueryField(request.Tags.Select(t => (FieldValue)t).ToArray())
//                    }
//                );
//            }
//        }

//        private SortOptions BuildSortOptions(ProductSearchRequest request)
//        {
//            if (string.IsNullOrWhiteSpace(request.SortBy))
//                return null;

//            var field = request.SortBy.ToLower() switch
//            {
//                "price" => "price",
//                "name" => "name.keyword",
//                "rating" => "averageRating",
//                "stock" => "stockQuantity",
//                _ => "createdDate"
//            };

//            return new SortOptions
//            {
//                Field = field,
//                Order = request.SortDescending ? SortOrder.Desc : SortOrder.Asc
//            };
//        }

//        private AggregationDictionary BuildAggregations()
//        {
//            return new AggregationDictionary
//            {
//                { "categories", new TermsAggregation("category") { Field = "category.keyword", Size = 10 } },
//                { "brands", new TermsAggregation("brand") { Field = "brand.keyword", Size = 10 } },
//                { "tags", new TermsAggregation("tags") { Field = "tags.keyword", Size = 20 } }
//            };
//        }

//        private ProductSearchResult CreateSearchResult(SearchResponse<Product> response, ProductSearchRequest request)
//        {
//            var categoryAgg = response.Aggregations.GetTerms("categories");
//            var brandAgg = response.Aggregations.GetTerms("brands");
//            var tagAgg = response.Aggregations.GetTerms("tags");

//            return new ProductSearchResult
//            {
//                Products = response.Documents,
//                TotalCount = response.Total,
//                PageNumber = request.PageNumber,
//                PageSize = request.PageSize,
//                CategoryCounts = categoryAgg?.Buckets.ToDictionary(b => b.Key.ToString(), b => b.DocCount) ?? new Dictionary<string, long>(),
//                BrandCounts = brandAgg?.Buckets.ToDictionary(b => b.Key.ToString(), b => b.DocCount) ?? new Dictionary<string, long>(),
//                TagCounts = tagAgg?.Buckets.ToDictionary(b => b.Key.ToString(), b => b.DocCount) ?? new Dictionary<string, long>()
//            };
//        }
//    }

//    public static class ElasticsearchExtensions
//    {
//        public static bool HasAnyConditions(this BoolQuery boolQuery)
//        {
//            return (boolQuery.Must != null && boolQuery.Must.Any()) ||
//                   (boolQuery.Filter != null && boolQuery.Filter.Any()) ||
//                   (boolQuery.Should != null && boolQuery.Should.Any()) ||
//                   (boolQuery.MustNot != null && boolQuery.MustNot.Any());
//        }
//    }
//}