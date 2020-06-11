using System.Collections.Generic;
using System.IO;
using System.Linq;
using GeoPicky.Console.Utils;
using Newtonsoft.Json;

namespace GeoPicky.Console
{
  public static class Reporter
  {
    public static string GetHtml(string title, IList<DataRow> rows)
    {
      var sortedRows = rows.OrderByDescending(r => r.Favorite).ThenByDescending(r => r.LastVisit);
      var res = @$"
<html>
  <head>
      <meta charset=""UTF-8"">
      <link rel=""stylesheet"" href=""https://stackpath.bootstrapcdn.com/bootstrap/4.5.0/css/bootstrap.min.css"" crossorigin=""anonymous"">
      <link rel=""stylesheet"" href=""https://unpkg.com/bootstrap-table@1.16.0/dist/bootstrap-table.min.css"">
	    <link rel=""stylesheet"" href=""https://unpkg.com/bootstrap-table@1.16.0/dist/extensions/filter-control/bootstrap-table-filter-control.css"">
    </head>
  <body>    
    <div class=""page-header""><h2 class=""badge-Info pl-3"">{title}<span class=""badge badge-light m-3"">{rows.Count}</span></h2></div>
    <div class=""container-fluid"">";
      var table = @"
      <table data-toggle=""table"" data-show-columns=""true"" data-search=""true"" class=""table table-bordered table-hover"" data-pagination=""true"" data-page-size=""100"" data-sort-name=""Favorites"" data-sort-order=""desc"" data-filter-control=""true"" data-click-to-select=""true"">
        <thead><tr>
          <th data-field=""IsSelected"" data-checkbox=""true""></th>
          <th data-field=""ID"" data-sortable=""true"" data-filter-control=""input"">ID</th>
          <th data-field=""Name"" data-sortable=""true"" data-filter-control=""input"">Name</th>
          <th data-field=""Favorites"" data-sortable=""true"" data-filter-control=""select"">Favorites</th>
          <th data-field=""Size"" data-sortable=""true"" data-filter-control=""select"">Size</th>
          <th data-field=""Type"" data-sortable=""true"" data-filter-control=""select"">Type</th>
          <th data-field=""Difficult"" data-sortable=""true"" data-filter-control=""select"">Difficult</th>
          <th data-field=""LastFound"" data-sortable=""true"" data-filter-control=""input"">Last found</th>
          <th data-field=""Terrain"" data-sortable=""true"" data-filter-control=""select"">Terrain</th>
        </tr></thead>";
      foreach (var r in sortedRows)
      {
        var link = $@"https://www.geocaching.com/geocache/{r.ID}";
        table += @$"
        <tr>
            <td></td>
            <td>{r.Number}</td>
            <td><a href=""{link}"">{r.Name}</a></td>
            <td>{r.Favorite:0}</td>
            <td>{r.Size}</td>
            <td>{r.Details}</td>
            <td>{r.Difficult}</td>
            <td>{r.LastVisit}</td>
            <td>{r.Terrain:F1}</td>
        </tr>";
      }

      table += "</table>";
      return res + table + @"</div>
<script src=""https://code.jquery.com/jquery-3.5.1.slim.min.js"" crossorigin=""anonymous""></script>
<script src=""https://unpkg.com/bootstrap-table@1.16.0/dist/bootstrap-table.min.js""></script>
<script src=""https://unpkg.com/bootstrap-table@1.16.0/dist/extensions/filter-control/bootstrap-table-filter-control.min.js""></script>
</body></html>";
    }

    public static void WriteOutput(IDictionary<string, IList<DataRow>> rows)
    {
      var outputPath = Path.Combine(Path.GetFullPath("."), "Output");
      Directory.CreateDirectory(outputPath);
      foreach (var r in rows)
      {
        var name = r.Key.GetValidThreadName();
        var output = Path.Combine(outputPath, name);
        var htmlOut = $"{output}.html";
        File.WriteAllText(htmlOut, GetHtml(r.Key, r.Value));

        var jsonOut = $"{output}.json";
        File.WriteAllText(jsonOut, JsonConvert.SerializeObject(r.Value, Formatting.Indented));
      }
    }
  }
}