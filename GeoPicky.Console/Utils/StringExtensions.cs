using System.Globalization;
using System.Linq;

namespace GeoPicky.Console.Utils
{
  public static class StringExtensions
  {
    /// <summary>
    ///   To the double.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>The value.</returns>
    public static double ToDouble(this string target)
    {
      double result;
      double.TryParse(target.Replace(',', '.'), out result);
      return result;
    }

    /// <summary>
    ///   Returns a <see cref="System.String" /> that represents this instance.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>
    ///   A <see cref="System.String" /> that represents this instance.
    /// </returns>
    public static string ToInvariantString(this double? value)
    {
      return value?.ToString("0.00", CultureInfo.InvariantCulture) ?? string.Empty;
    }

    /// <summary>
    ///   Returns a <see cref="System.String" /> that represents this instance.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>
    ///   A <see cref="System.String" /> that represents this instance.
    /// </returns>
    public static string ToInvariantString(this double value)
    {
      return value.ToString("0.00", CultureInfo.InvariantCulture);
    }

    /// <summary>
    ///   Gets the valid name for a thread by filtering out invalid characters from <paramref name="input" />.
    /// </summary>
    /// <param name="input">The target.</param>
    /// <returns>The valid thread name.</returns>
    public static string GetValidThreadName(this string input)
    {
      return new string(input.Where(c => char.IsLetter(c) || char.IsDigit(c)).ToArray());
    }
  }
}