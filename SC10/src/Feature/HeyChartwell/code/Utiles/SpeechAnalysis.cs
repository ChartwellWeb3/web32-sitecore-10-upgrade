using System.Text.RegularExpressions;
using System.Web;
using Chartwell.Foundation.utility;
using Sitecore;
using Sitecore.Data.Items;

namespace Chartwell.Feature.HeyChartwell.Utiles
{
  public class SpeechAnalysis
  {
    private readonly ChartwellUtiles _c;

    Item item = Context.Item;
    Item parent = Context.Item.Parent;

    public SpeechAnalysis()
    {
      _c = new ChartwellUtiles();
    }
    public string RecognizeCommand(string com)
    {
      if (com.Like("newest residence"))
        return HttpContext.Current.Session["city-url"].ToString();

      return "";
    }
  }

  public static class MyStringExtensions
  {
    public static bool Like(this string toSearch, string toFind)
    {
      return new Regex(@"\A" + new Regex(@"\.|\$|\^|\{|\[|\(|\||\)|\*|\+|\?|\\").Replace(toFind, ch => @"\" + ch).Replace('_', '.').Replace("%", ".*") + @"\z", RegexOptions.Singleline).IsMatch(toSearch);
    }
  }

}