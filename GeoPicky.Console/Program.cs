using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;

using GeoPicky.Console.Utils;

using HtmlAgilityPack;

using RestSharp;

namespace GeoPicky.Console
{
  internal class Program
  {
    private static readonly string Cookies = ConfigurationManager.AppSettings["Cookies"];

    private static string EncodeUrl(string str)
    {
      return HttpUtility.UrlEncode(str);
    }

    private static void Main(string[] args)
    {
      if (string.IsNullOrWhiteSpace(Cookies))
      {
        throw new ApplicationException("Cookies is empty. Please fill in the cookies obtained from the browser session");
      }

      var cities = args == null || args.Length == 0
        ? new[] { "Espoo", "Helsinki", "Vantaa", "Lohja", "Porvoo" }
        : args;
      var queue = new Multitask().Queue;
      var res = new ConcurrentDictionary<string, IList<DataRow>>();
      queue.Start((c, i) => Query(c, i, res), cities);
      Reporter.WriteOutput(res);
    }

    private static RestRequest GetRequest()
    {
      var r = new RestRequest("", Method.GET);
      r.AddHeader("Connection", " keep-alive");
      r.AddHeader("Cache-Control", " max-age=0");
      r.AddHeader("Upgrade-Insecure-Requests", " 1");
      r.AddHeader("User-Agent",
        @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.97 Safari/537.36 Edg/83.0.478.45");
      r.AddHeader("Accept",
        @"text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
      r.AddHeader("Accept-Language", "en-US,en;q=0.9");
      r.AddHeader("Cookie", Cookies);
      return r;
    }

    private static KeyValuePair<string, IReadOnlyList<DataRow>> Query(string c, int i,
      IDictionary<string, IList<DataRow>> res)
    {
      var idx = 0;
      var list = new List<DataRow>();
      res[c] = list;
      while (true)
        try
        {
          var req = GetRequest();
          var locClient = new RestClient(@$"https://www.geocaching.com/play/search?origin={EncodeUrl(c)}")
          { Encoding = Encoding.UTF8 };
          var locRes = locClient.Execute(req);
          if (!locRes.IsSuccessful)
          {
            System.Console.Error.WriteLine($"Failed to query location for [{c}]");
            return new KeyValuePair<string, IReadOnlyList<DataRow>>(c, list);
          }

          var param = locRes.ResponseUri.Query;
          var dataClient =
            new RestClient(
                $@"https://www.geocaching.com/play/search/more-results{param}&startIndex={idx}&selectAll=false")
            { Encoding = Encoding.UTF8 };
          var webRes = dataClient.Execute<Response>(req);
          if (webRes.IsSuccessful)
          {
            var doc = new HtmlDocument();
            doc.LoadHtml(webRes.Data.HtmlString);
            var nodes = doc.DocumentNode.SelectNodes("//tr");
            if (nodes == null) break;

            var rows = nodes.Select(DataRow.FromHtml).ToArray();
            var okRows = rows.Where(r => string.IsNullOrWhiteSpace(r.Status)).ToArray();
            System.Console.WriteLine($"[{c}] [Batch {idx}]: Found {rows.Length} - Accept {okRows.Length}");
            list.AddRange(okRows);
            if (!webRes.Data.ShowLoadMore) break;

            var next = rows.Max(r => r.Number) + 1;
            if (next > idx)
              idx = next;
            else
              break;
          }
          else
          {
            break;
          }
        }
        catch (Exception e)
        {
          System.Console.Error.WriteLine($"Failed to get data for [{c}]: {e}");
          break;
        }

      return new KeyValuePair<string, IReadOnlyList<DataRow>>(c, list);
    }
  }
}