using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using System.Web.Mvc;

namespace lab28_api_cards.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            //makes a call to the API to generate a new deck of cards
            HttpWebRequest WR = WebRequest.CreateHttp("https://deckofcardsapi.com/api/deck/new/shuffle/?deck_count=1");
            WR.UserAgent = ".NET Framework Test Client";

            HttpWebResponse Response;
            try
            {
                Response = (HttpWebResponse)WR.GetResponse();
            }
            catch (WebException e)
            {
                ViewBag.Error = "Exception";
                ViewBag.ErrorDescription = e.Message;
                return View();
            }

            if (Response.StatusCode != HttpStatusCode.OK)
            {
                ViewBag.Error = Response.StatusCode;
                ViewBag.ErrorDescription = Response.StatusDescription;
                return View();
            }

            //reads response
            StreamReader reader = new StreamReader(Response.GetResponseStream());
            string DeckData = reader.ReadToEnd();

            //parses DeckData for a Deck ID, creates variable "currentID" from the Deck ID.  prints Deck ID to a viewbag.
            try
            {
                JObject JsonData = JObject.Parse(DeckData);
                //pushes deck id to a viewbag
                ViewBag.DeckID = JsonData["deck_id"];
                //pulls current DeckID
                Session["deckID"] = (string)JsonData["deck_id"];

            }
            catch (Exception e)
            {
                ViewBag.Error = "JSON Issue";
                ViewBag.ErrorDescription = e.Message;
                return View();
            }

            return View();
        }

        public ActionResult Deal()
        {
            string id = (string)Session["deckID"];
            string link = $"https://deckofcardsapi.com/api/deck/{id}/draw/?count=5";

            HttpWebRequest WR2 = WebRequest.CreateHttp(link);
            WR2.UserAgent = ".NET Framework Test Client";

            HttpWebResponse Response;
            try
            {
                Response = (HttpWebResponse)WR2.GetResponse();
            }

            catch (WebException e)
            {
                ViewBag.Error = "Exception";
                ViewBag.ErrorDescription = e.Message;
                return View();
            }

            StreamReader reader = new StreamReader(Response.GetResponseStream());
            string DeckData = reader.ReadToEnd();

            try
            {
                JObject JsonData = JObject.Parse(DeckData);
                ViewBag.DeckID = JsonData["deck_id"];
                //card 1
                ViewBag.cards = JsonData["cards"];
                ViewBag.CardsRemaining = JsonData["remaining"];
                Session["CardsRemaining"] = (string)JsonData["remaining"];
            }
            catch (Exception e)
            {
                ViewBag.Error = "JSON Issue";
                ViewBag.ErrorDescription = e.Message;
                return View();
            }

            return View();
        }
    }
}