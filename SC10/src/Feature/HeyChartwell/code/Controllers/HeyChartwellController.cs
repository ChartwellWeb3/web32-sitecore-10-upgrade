using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using Chartwell.Foundation.utility;
using System.Linq;
using System;

namespace Chartwell.Feature.HeyChartwell.Controllers
{
  public class HeyChartwellController : Controller
  {
    private static Random rnd = new Random();

    private readonly string constring = ConfigurationManager.ConnectionStrings["SitecoreOLP"].ToString();
    private readonly ChartwellUtiles _c;

    public HeyChartwellController()
    {
      _c = new ChartwellUtiles();
    }
    public ActionResult Index()
    {
      return View();
    }

    public JsonResult VoiceDictation(string com, string cityUrl)
    {
      var itemUrl = string.Empty;
      var response = IsValidRequest(com.ToLower().Replace(".", ""));

      if (response.Count > 0)
      {
        var r = rnd.Next(response.Count);

        if (!string.IsNullOrEmpty(response[r]) && (response[0].Like("%nearest residence%") 
          || response[0].Like("%closest residence%")))
          return Json(new { cityUrl, phrase = response[r] }, JsonRequestBehavior.AllowGet);

        else if(!string.IsNullOrEmpty(response[r]) &&  (!response[0].Like("%nearest residence%")
          || !response[0].Like("%closest residence%")))
        {          
          return Json(new { cityUrl = response[1], phrase = response[0] }, JsonRequestBehavior.AllowGet);
        }
        else
        {
          return Json(new { phrase = response[r] }, JsonRequestBehavior.AllowGet);
        }
      }

      return Json(new { cityUrl = "", phrase = "not a valid request"}, JsonRequestBehavior.AllowGet);
    }

    public List<string> IsValidRequest(string phrase)
    {
      SqlDataReader sqlDataReader;
      SqlConnection sqlConnection = new SqlConnection(constring);
      List<string> resp = new List<string>();

      sqlConnection.Open();

      using (var sqlCommand = new SqlCommand("sp_GetTriggerWords", sqlConnection) { CommandType = CommandType.StoredProcedure })
      {
        sqlCommand.Parameters.AddWithValue("@Phrase", phrase);
        sqlDataReader = sqlCommand.ExecuteReader();

        if (!sqlDataReader.Read())
        {
          return new List<string>();
        }

        resp.Add(sqlDataReader["Responses"].ToString());
        resp.Add(sqlDataReader["ResponseUrl"].ToString());
      }

      return resp;

    }

    public void DetermineRequest(string phrase)
    {
      var resp = IsValidRequest(phrase);


    }
  }
}
