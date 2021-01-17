using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.Collections.Specialized;

namespace FXEmuSharp
{
    class Program
    {
        static string listingURL = "https://keymaster.fivem.net/api/validate/";
        static string ingressURL = "https://servers-ingress-live.fivem.net/ingress";
        static string nucleusURL = "https://cfx.re/api/register/?v=2";


        static Dictionary<string, string> keyMap = new Dictionary<string, string>()
        {
            { "30120", "xxx" },
            { "30121", "xxx" },
            { "30122", "xxx" },
            { "30123", "xxx" },
            { "30124", "xxx" },
            { "30125", "xxx" },
            { "30126", "xxx" },
            { "30127", "xxx" },
            { "30128", "xxx" },
            { "30129", "xxx" },
            { "30130", "xxx" }
        };


        static Dictionary<string, string> nucleusKeys = new Dictionary<string, string>();
        static Dictionary<string, string> listingKeys = new Dictionary<string, string>();




        static void retreiveListingToken(string port, string licenseKey)
        {
            HttpWebRequest webRequest = HttpWebRequest.CreateHttp(listingURL+licenseKey);
            webRequest.UserAgent = "curl/7.64.1-DEV";

            using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
            {
                using (StreamReader streamReader = new StreamReader(webResponse.GetResponseStream()))
                {
                    string response = streamReader.ReadToEnd();

                    var json_serializer = new JavaScriptSerializer();
                    var routes_list = (IDictionary<string, object>)json_serializer.DeserializeObject(response);

                    nucleusKeys.Add(port, routes_list["nucleus_token"].ToString());
                    listingKeys.Add(port, routes_list["listing_token"].ToString());
                }
            }
        }

        static void sendIngress(string listingToken, string gamePort)
        {
            HttpWebRequest webRequest = HttpWebRequest.CreateHttp(ingressURL);
            {
                byte[] data = Encoding.UTF8.GetBytes("{ \"ipOverride\" : \"x.x.x.x\", \"listingToken\" : \"" + listingToken+"\", \"port\" : "+gamePort+", \"useDirectListing\" : true }");

                webRequest.Method = "POST";
                webRequest.ContentType = "application/json";
                webRequest.ContentLength = data.Length;

                using (Stream stream = webRequest.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }

            try
            {
                using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
                {
                    using (StreamReader streamReader = new StreamReader(webResponse.GetResponseStream()))
                    {
                        string response = streamReader.ReadToEnd();
                        Console.WriteLine("ingressResponse: " + response);
                    }
                }
            }catch(Exception ex) { }
        }

        static void sendNucleusRequest(String nucleusToken, String gamePort)
        {
            HttpWebRequest webRequest = HttpWebRequest.CreateHttp(nucleusURL);
            {
                byte[] data = Encoding.UTF8.GetBytes("{\"ipOverride\":\"x.x.x.x\",\"port\":\"" + gamePort+"\",\"token\":\""+nucleusToken+"\"}");

                webRequest.Method = "POST";
                webRequest.ContentType = "application/json";
                webRequest.ContentLength = data.Length;

                using (Stream stream = webRequest.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }

            using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
            {
                using (StreamReader streamReader = new StreamReader(webResponse.GetResponseStream()))
                {
                    string response = streamReader.ReadToEnd();
                    Console.WriteLine("nucleusResponse: " + response);
                }
            }
        }

        static void Main(string[] args)
        {

            foreach (KeyValuePair<string, string> entry in keyMap)
            {

                retreiveListingToken(entry.Key, entry.Value);
                String listingToken = listingKeys.FirstOrDefault(x => x.Key == entry.Key).Value;
                String nucleusToken = nucleusKeys.FirstOrDefault(x => x.Key == entry.Key).Value;

                if (listingToken != null)
                {
                    Console.WriteLine("[+] ListingToken for " + entry.Key+" retreived.");
                    sendIngress(listingToken, entry.Key);
                    sendNucleusRequest(nucleusToken, entry.Key);
                }
                else
                {
                    Console.WriteLine("ERROR: LISTINGTOKEN FOR " + entry.Key + " IS NULL!");
                }

            }


            while (true)
            {
                System.Threading.Thread.Sleep(30000);

                foreach (KeyValuePair<string, string> listingEntry in listingKeys)
                {
                    sendIngress(listingEntry.Value, listingEntry.Key);
                }
            }
        }
    }
}
