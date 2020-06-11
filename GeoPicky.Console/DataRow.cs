using System;
using System.Globalization;
using System.Linq;
using HtmlAgilityPack;

namespace GeoPicky.Console
{
  public class DataRow
  {
    public int Number { get; set; }

    public string Details { get; set; }

    public string LastVisit { get; set; }

    public double Terrain { get; set; }

    public string Size { get; set; }

    public double Difficult { get; set; }

    public double Favorite { get; set; }

    public string Status { get; set; }

    public string Name { get; set; }

    public string ID { get; set; }

    private static string GetText(HtmlNode node)
    {
      if (node == null) return null;

      return (node.InnerText ?? "").Trim().Trim('\r').Trim('\n');
    }

    private static double GetDouble(HtmlNode node)
    {
      double res;
      var str = GetText(node);
      return double.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out res) ? res : 0;
    }

    private static int GetInt(string str)
    {
      int res;
      return int.TryParse(str, out res) ? res : 0;
    }

    private static string GetDetails(string details)
    {
      return ((details ?? "").Split("|", StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "").Trim();
    }

    public static DataRow FromHtml(HtmlNode node)
    {
      try
      {
        var num = GetInt(node.Attributes["data-rownumber"].Value);
        var id = node.Attributes["data-id"].Value;
        var name = node.Attributes["data-name"].Value;
        var status = GetText(node.SelectSingleNode(".//span[@class='status']"));
        var details = GetText(node.SelectSingleNode(".//*[@class='cache-details']"));
        var fav = GetDouble(node.SelectSingleNode(".//*[@data-column='FavoritePoint']"));
        var size = GetText(node.SelectSingleNode(".//*[@data-column='ContainerSize']"));
        var difficult = GetDouble(node.SelectSingleNode(".//*[@data-column='Difficulty']"));
        var terrain = GetDouble(node.SelectSingleNode(".//*[@data-column='Terrain']"));
        var lastVisit = GetText(node.SelectSingleNode(".//*[@data-column='DateLastVisited']"));
        return new DataRow
        {
          Number = num,
          ID = id,
          Name = name,
          Status = status,
          Details = GetDetails(details),
          Favorite = fav,
          Size = size,
          Difficult = difficult,
          Terrain = terrain,
          LastVisit = lastVisit
        };
      }
      catch (Exception ex)
      {
        System.Console.Error.WriteLine("Failed to parse data row: " + ex);
      }

      return null;
    }
  }
}