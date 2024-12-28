using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System;
using ElasticSearch_NET_ANGULAR.Model;
using Elastic.Clients.Elasticsearch;
using System.Threading.Tasks;
using System.Threading;
using Elastic.Clients.Elasticsearch.Requests;
using Elastic.Clients.Elasticsearch.Nodes;
using Elastic.Clients.Elasticsearch.QueryDsl;

namespace ElasticSearch_NET_ANGULAR.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ElasticsearchClient _client;

        public ProductController(ElasticsearchClient client)
        {
            _client = client;
        }
        [HttpGet]
        public async Task<List<Product>> GetAll(CancellationToken cancellationToken)
        {
            try
            {
                SearchRequest searchRequest = new("products")
                {
                    Size = 100,
                    Sort = new List<SortOptions>
        {
            SortOptions.Field(new Field("name.keyword"),new FieldSort(){Order = SortOrder.Asc}),
        },
                    //Query = new MatchQuery(new Field("name"))
                    //{
                    //    Query = "domates"
                    //},
                    //Query = new WildcardQuery(new Field("name"))
                    //{
                    //   Value = "*domates*"
                    //},
                    //Query = new FuzzyQuery(new Field("name"))
                    //{
                    //    Value = "domatse"
                    //},
           //         Query = new BoolQuery
           //         {
           //             Should = new Query[]
           //{
           //    new MatchQuery(new Field("name"))
           //     {
           //         Query = "domates"
           //     },
           //     new FuzzyQuery(new Field("description"))
           //     {
           //         Value = "domatse"
           //     }
           //}
           //         }
                };
                // Elasticsearch'ten sonuçları al
                SearchResponse<Product> response = await _client.SearchAsync<Product>(searchRequest, cancellationToken);

                // Eğer sonuçlar varsa, döndür
                if (response.IsValidResponse && response.Hits.Any())
                {
                    return response.Hits.Select(hit => hit.Source).ToList();
                }

                // Sonuç yoksa, boş liste döndür
                return new List<Product>();
            }
            catch (Exception ex)
            {
                // Hata durumunda log eklenebilir
                throw new Exception($"Hata: {ex.Message}");
            }
        }
        [HttpPost]
        public async Task<bool> Post(CreateProductDto request, CancellationToken cancellationToken)
        {
            try
            {
                Product product = new()
                {
                    Name = request.Name,
                    Price = request.Price,
                    Stock = request.Stock,
                    Description = request.Description
                };

                CreateRequest<Product> createRequest = new(product.Id.ToString())
                {
                    Document = product,
                };

                CreateResponse createResponse = await _client.CreateAsync(createRequest, cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                // Hata durumunda log eklenebilir
                throw new Exception($"Hata: {ex.Message}");
            }
        }

        [HttpGet("GetOnName/{filter}")]
        public async Task<Product> GetOnName(string filter, CancellationToken cancellationToken)
        {
            try
            {
                SearchRequest searchRequest = new("products")
                {
                    Size = 100,
                    Sort = new List<SortOptions>
        {
            SortOptions.Field(new Field("name.keyword"),new FieldSort(){Order = SortOrder.Asc}),
        },
                    //Query = new MatchQuery(new Field("name"))
                    //{
                    //    Query = "domates"
                    //},
                    //Query = new WildcardQuery(new Field("name"))
                    //{
                    //   Value = "*domates*"
                    //},
                    //Query = new FuzzyQuery(new Field("name"))
                    //{
                    //    Value = "domatse"
                    //},
                    Query = new BoolQuery
                    {
                        Should = new Query[]
                    {
                        new MatchQuery(new Field("name"))
                         {
                             Query = filter
                         },
                        
                    }
                    }
                };
                // Elasticsearch'ten sonuçları al
                SearchResponse<Product> response = await _client.SearchAsync<Product>(searchRequest, cancellationToken);

                // Eğer sonuçlar varsa, döndür
                if (response.IsValidResponse && response.Hits.Any())
                {
                    return response.Hits.Select(hit => hit.Source).FirstOrDefault();
                }

                // Sonuç yoksa, boş liste döndür
                return new Product();
            }
            catch (Exception ex)
            {
                // Hata durumunda log eklenebilir
                throw new Exception($"Hata: {ex.Message}");
            }
        }
    }
}
