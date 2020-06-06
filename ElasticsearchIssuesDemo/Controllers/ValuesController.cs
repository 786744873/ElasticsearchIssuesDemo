using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Microsoft.AspNetCore.Mvc;
using Nest;
using Newtonsoft.Json;

namespace ElasticsearchIssuesDemo.Controllers
{
    [ApiController]
    public class ValuesController : ControllerBase
    {

        public static string _esUrl= "http://119.3.217.151:9202";

        private static readonly ElasticClient _elasticClient;

        static ValuesController()
        {
            // 首先，插入10万条测试数据
            // First, insert 100000 pieces of test data
            var nodes = new List<Uri> {
                new Uri(_esUrl)
            };
            var pool = new StaticConnectionPool(nodes);
            var settings = new ConnectionSettings(pool);
            settings.DefaultIndex("test");
            settings.DefaultTypeName("historydata");
            settings.DefaultFieldNameInferrer(name => name);
            _elasticClient = new ElasticClient(settings);

            var now = DateTime.Now;
            List<GatewayDevicePoint_ES> dataInsertList = new List<GatewayDevicePoint_ES>();
            for (int i = 0; i < 100000; i++)
            {
                dataInsertList.Add(new GatewayDevicePoint_ES
                {
                    PointId = "007e1f1f9932408da100aa0306401a4e_214",
                    Value = 0,
                    Time = now,
                    TimeTicks = now.Ticks
                });
                now = now.AddSeconds(-2);
            }
            _elasticClient.IndexMany<GatewayDevicePoint_ES>(dataInsertList);
        }

        // GET js
        [HttpGet]
        [Route("js")]
        public async Task<string> JS()
        {
            // 使用原生ajax方式访问restful接口可以获取到5千条数据，尽管有些数据不存在
            // Using native Ajax to access restful interface can get 5000 pieces of data, although some data does not exist
            var temlate = "{\"from\":0,\"size\":1,\"sort\":[{\"TimeTicks\":{\"order\":\"desc\"}}],\"query\":{\"bool\":{\"must\":[{\"match_phrase_prefix\":{\"PointId\":\"007e1f1f9932408da100aa0306401a4e_214\"}}],\"filter\":{\"range\":{\"AddTimeTicks\":{\"lte\":_time}}}}}}";
            StringBuilder sb = new StringBuilder();
            var now = DateTime.Now;
            for (int i = 0; i < 5000; i++)
            {
                sb.AppendLine("{}");
                sb.AppendLine(temlate.Replace("_time", now.AddSeconds(-1 * i).ToUniversalTime().Ticks.ToString()));
            }
            sb.AppendLine();
            var postByte = Encoding.UTF8.GetBytes(sb.ToString());
            Stream stream = new MemoryStream(postByte);
            var content = new StreamContent(stream);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            HttpClient httpClient = new HttpClient();
            var uri = new Uri(_esUrl+ "/test/_msearch");
            httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
            httpClient.DefaultRequestHeaders.ExpectContinue = false;
            var responseMessage = httpClient.PostAsync(uri, content).Result;
            var responseContent = await responseMessage.Content.ReadAsStringAsync();
            return responseContent;
        }

        // GET sdk
        [Route("sdk")]
        [HttpGet(nameof(SDK))]
        public async Task<string> SDK()
        {
            // 使用SDK方式无法获取到5千条数据，出现bug
            // Unable to get 5000 pieces of data using SDK, bug
            var now = DateTime.Now;
            var multiSearchRequest = new MultiSearchDescriptor();
            multiSearchRequest.MaxConcurrentSearches(5000);
            for (int i = 0; i < 3000; i++)
            {
                // 前一时间
                multiSearchRequest = multiSearchRequest.Search<GatewayDevicePoint_ES>(s => s
                             .From(0)
                             .Size(1)
                             //.Timeout("6000")
                             .Sort(st => st
                                 .Descending(new Field(nameof(GatewayDevicePoint_ES.TimeTicks)))
                             )
                             .Query(q => q
                                 .Bool(b => b
                                     .Must(mu => mu
                                         .MatchPhrasePrefix(m => m
                                             .Field(new Field(nameof(GatewayDevicePoint_ES.PointId)))
                                             .Query("007e1f1f9932408da100aa0306401a4e_214")
                                         )
                                     )
                                     .Filter(f => f
                                             .LongRange(r => r
                                                 .Field(new Field(nameof(GatewayDevicePoint_ES.TimeTicks)))
                                                 .LessThanOrEquals(now.AddSeconds(-1 * i).ToUniversalTime().Ticks)
                                             ), f => f
                                             .Bool(fb => fb
                                                 .MustNot(mn => mn
                                                     .Term(t => t
                                                         .Field(new Field(nameof(GatewayDevicePoint_ES.TimeTicks)))
                                                         .Value(0)
                                                     )
                                                 )
                                             )
                                     )
                                 )
                             )
                         );
            }
            var result = string.Empty;
            try
            {
                var response = _elasticClient.MultiSearch(multiSearchRequest);
                if (!response.IsValid)
                {
                    result = response.DebugInformation;
                }
                else
                {

                    result = "The amount of data is too small to reproduce the bug. Please increase the amount of inserted data in the static constructor" + Environment.NewLine + "数据量太少，无法重现bug，请在静态构造函数中扩大插入数据量";
                }
            }
            catch (Exception ex)
            {
                result = ex.ToString();
            }
            return await Task.FromResult(result);
        }
    }
}
