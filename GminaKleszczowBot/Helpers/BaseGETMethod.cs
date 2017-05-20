using HtmlAgilityPack;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace GksKatowiceBot.Helpers
{
    public class BaseGETMethod
    {


        public static IList<Attachment> GetCardsAttachmentsAktualnosci(ref List<IGrouping<string, string>> hrefList, bool newUser = false)
        {
            List<Attachment> list = new List<Attachment>();

            string urlAddress = "https://www.kleszczow.pl";
            // string urlAddress = "http://www.orlenliga.pl/";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
            request.Accept = "text/html";
            request.Method = "GET";
            request.UserAgent = ".NET Framework Test Client";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            var listTemp2 = new List<System.Linq.IGrouping<string, string>>();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if (response.CharacterSet == null)
                {
                    readStream = new StreamReader(receiveStream);
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                }

                string data = readStream.ReadToEnd();

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(data);

                string matchResultDivId = "media";
                string xpath = String.Format("//div[@class='{0}']", matchResultDivId);
                var people = doc.DocumentNode.SelectNodes(xpath).Select(p => p.InnerHtml);
                string text = "";
                foreach (var person in people)
                {
                    text += person;
                }

                HtmlDocument doc2 = new HtmlDocument();

                doc2.LoadHtml(text);
                hrefList = doc2.DocumentNode.SelectNodes("//a")
                                  .Select(p => p.GetAttributeValue("href", "not found")).Where(p => p.Contains("/aktualnosci/")).GroupBy(p => p.ToString())
                                  .ToList();

                var imgList = doc2.DocumentNode.SelectNodes("//img")
                                  .Select(p => p.GetAttributeValue("src", "not found"))
                                  .ToList();

                var titleList = doc2.DocumentNode.SelectNodes("//h2").Select(p=>p.InnerText)
                                  .ToList();

                response.Close();
                readStream.Close();

                int index = 5;

         //       DataTable dt = GetWiadomosciPilka();

                if (newUser == true)
                {

                    //    if (.Count > 0)
                    //    {
                    //        List<int> deleteList = new List<int>();
                    //        var listTemp = new List<System.Linq.IGrouping<string, string>>();
                    //        var imageListTemp = new List<string>();
                    //        var titleListTemp = new List<string>();

                    //        for (int i = 0; i < hrefList.Count; i++)
                    //        {
                    //            if (dt.Rows[dt.Rows.Count - 1]["Wiadomosc1"].ToString() != hrefList[i].Key && dt.Rows[dt.Rows.Count - 1]["Wiadomosc2"].ToString() != hrefList[i].Key &&
                    //                dt.Rows[dt.Rows.Count - 1]["Wiadomosc3"].ToString() != hrefList[i].Key && dt.Rows[dt.Rows.Count - 1]["Wiadomosc4"].ToString() != hrefList[i].Key && dt.Rows[dt.Rows.Count - 1]["Wiadomosc5"].ToString() != hrefList[i].Key
                    //            )
                    //            {
                    //                listTemp.Add(hrefList[i]);
                    //                imageListTemp.Add("http://www.gkskatowice.eu" + imgList[i]);
                    //                titleListTemp.Add(titleList[i].Replace("&quot;", ""));
                    //            }
                    //            listTemp2.Add(hrefList[i]);
                    //        }
                    //        hrefList = listTemp;
                    //        index = hrefList.Count;
                    //        imgList = imageListTemp;
                    //        titleList = titleListTemp;
                    //        //   AddWiadomosc(listTemp2);
                    //    }
                    //    else
                    //    {
                    //        index = 5;
                    //        //   AddWiadomosc(hrefList);
                    //    }
                    //}

                    for (int i = 0; i < index; i++)
                    {
                        string link = "";
                        if (hrefList[i].Key.Contains("http"))
                        {
                            link = hrefList[i].Key;
                        }
                        else
                        {
                            link = "http://www.gkskatowice.eu" + hrefList[i].Key;
                            //link = "http://www.orlenliga.pl/" + hrefList[i].Key;
                        }

                        if (link.Contains("video"))
                        {
                            list.Add(GetHeroCard(
                            titleList[i].ToString(), "", "",
                            new CardImage(url: imgList[i]),
                            new CardAction(ActionTypes.OpenUrl, "Oglądaj video", value: link),
                            new CardAction(ActionTypes.OpenUrl, "Udostępnij", value: "https://www.facebook.com/sharer/sharer.php?u=" + link))
                            );
                        }
                        else
                            if (link.Contains("gallery"))
                        {
                            list.Add(GetHeroCard(
                            titleList[i].ToString(), "", "",
                            new CardImage(url: imgList[i]),
                            new CardAction(ActionTypes.OpenUrl, "Przeglądaj galerie", value: link),
                            new CardAction(ActionTypes.OpenUrl, "Udostępnij", value: "https://www.facebook.com/sharer/sharer.php?u=" + link))
                            );
                        }
                        else
                        {
                            list.Add(GetHeroCard(
                            titleList[i].ToString().Replace("&#8211;","-"), "", "",
                            new CardImage(url: imgList[i]),
                            new CardAction(ActionTypes.OpenUrl, "Więcej", value: link),
                            new CardAction(ActionTypes.OpenUrl, "Udostępnij", value: "https://www.facebook.com/sharer/sharer.php?u=" + link))
                            );
                        }

                        //  list.Add(new Microsoft.Bot.Connector.VideoCard(titleList[i], "", "",null)
                    }
                }
                if (listTemp2.Count > 0)
                {
                    hrefList = listTemp2;
                }
            }
                return list;

        }

        public static IList<Attachment> GetCardsAttachmentsKultura(ref List<IGrouping<string, string>> hrefList, bool newUser = false)
        {
            List<Attachment> list = new List<Attachment>();

            string urlAddress = "https://www.kleszczow.pl/kultura-komunikaty/";
            // string urlAddress = "http://www.orlenliga.pl/";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
            request.Accept = "text/html";
            request.Method = "GET";
            request.UserAgent = ".NET Framework Test Client";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            var listTemp2 = new List<System.Linq.IGrouping<string, string>>();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if (response.CharacterSet == null)
                {
                    readStream = new StreamReader(receiveStream);
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                }

                string data = readStream.ReadToEnd();

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(data);

                string matchResultDivId = "media";
                string xpath = String.Format("//div[@class='{0}']", matchResultDivId);
                var people = doc.DocumentNode.SelectNodes(xpath).Select(p => p.InnerHtml);
                string text = "";
                foreach (var person in people)
                {
                    text += person;
                }

                HtmlDocument doc2 = new HtmlDocument();

                doc2.LoadHtml(text);
                hrefList = doc2.DocumentNode.SelectNodes("//a")
                                  .Select(p => p.GetAttributeValue("href", "not found")).Where(p => p.Contains("/aktualnosci/")).GroupBy(p => p.ToString())
                                  .ToList();

                var imgList = doc2.DocumentNode.SelectNodes("//img")
                                  .Select(p => p.GetAttributeValue("src", "not found"))
                                  .ToList();

                var titleList = doc2.DocumentNode.SelectNodes("//h2").Select(p => p.InnerText)
                                  .ToList();

                response.Close();
                readStream.Close();

                int index = 5;

                //       DataTable dt = GetWiadomosciPilka();

                if (newUser == true)
                {

                    //    if (.Count > 0)
                    //    {
                    //        List<int> deleteList = new List<int>();
                    //        var listTemp = new List<System.Linq.IGrouping<string, string>>();
                    //        var imageListTemp = new List<string>();
                    //        var titleListTemp = new List<string>();

                    //        for (int i = 0; i < hrefList.Count; i++)
                    //        {
                    //            if (dt.Rows[dt.Rows.Count - 1]["Wiadomosc1"].ToString() != hrefList[i].Key && dt.Rows[dt.Rows.Count - 1]["Wiadomosc2"].ToString() != hrefList[i].Key &&
                    //                dt.Rows[dt.Rows.Count - 1]["Wiadomosc3"].ToString() != hrefList[i].Key && dt.Rows[dt.Rows.Count - 1]["Wiadomosc4"].ToString() != hrefList[i].Key && dt.Rows[dt.Rows.Count - 1]["Wiadomosc5"].ToString() != hrefList[i].Key
                    //            )
                    //            {
                    //                listTemp.Add(hrefList[i]);
                    //                imageListTemp.Add("http://www.gkskatowice.eu" + imgList[i]);
                    //                titleListTemp.Add(titleList[i].Replace("&quot;", ""));
                    //            }
                    //            listTemp2.Add(hrefList[i]);
                    //        }
                    //        hrefList = listTemp;
                    //        index = hrefList.Count;
                    //        imgList = imageListTemp;
                    //        titleList = titleListTemp;
                    //        //   AddWiadomosc(listTemp2);
                    //    }
                    //    else
                    //    {
                    //        index = 5;
                    //        //   AddWiadomosc(hrefList);
                    //    }
                    //}

                    for (int i = 0; i < index; i++)
                    {
                        string link = "";
                        if (hrefList[i].Key.Contains("http"))
                        {
                            link = hrefList[i].Key;
                        }
                        else
                        {
                            link = "http://www.gkskatowice.eu" + hrefList[i].Key;
                            //link = "http://www.orlenliga.pl/" + hrefList[i].Key;
                        }

                        if (link.Contains("video"))
                        {
                            list.Add(GetHeroCard(
                            titleList[i].ToString(), "", "",
                            new CardImage(url: imgList[i]),
                            new CardAction(ActionTypes.OpenUrl, "Oglądaj video", value: link),
                            new CardAction(ActionTypes.OpenUrl, "Udostępnij", value: "https://www.facebook.com/sharer/sharer.php?u=" + link))
                            );
                        }
                        else
                            if (link.Contains("gallery"))
                        {
                            list.Add(GetHeroCard(
                            titleList[i].ToString(), "", "",
                            new CardImage(url: imgList[i]),
                            new CardAction(ActionTypes.OpenUrl, "Przeglądaj galerie", value: link),
                            new CardAction(ActionTypes.OpenUrl, "Udostępnij", value: "https://www.facebook.com/sharer/sharer.php?u=" + link))
                            );
                        }
                        else
                        {
                            list.Add(GetHeroCard(
                            titleList[i].ToString().Replace("&#8211;", "-"), "", "",
                            new CardImage(url: imgList[i]),
                            new CardAction(ActionTypes.OpenUrl, "Więcej", value: link),
                            new CardAction(ActionTypes.OpenUrl, "Udostępnij", value: "https://www.facebook.com/sharer/sharer.php?u=" + link))
                            );
                        }

                        //  list.Add(new Microsoft.Bot.Connector.VideoCard(titleList[i], "", "",null)
                    }
                }
                if (listTemp2.Count > 0)
                {
                    hrefList = listTemp2;
                }
            }
            return list;

        }

        public static IList<Attachment> GetCardsAttachmentsKultura(ref List<IGrouping<string, string>> hrefList, bool newUser = false)
        {
            List<Attachment> list = new List<Attachment>();

            string urlAddress = "https://www.kleszczow.pl/kultura-komunikaty/";
            // string urlAddress = "http://www.orlenliga.pl/";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
            request.Accept = "text/html";
            request.Method = "GET";
            request.UserAgent = ".NET Framework Test Client";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            var listTemp2 = new List<System.Linq.IGrouping<string, string>>();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if (response.CharacterSet == null)
                {
                    readStream = new StreamReader(receiveStream);
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                }

                string data = readStream.ReadToEnd();

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(data);

                string matchResultDivId = "media";
                string xpath = String.Format("//div[@class='{0}']", matchResultDivId);
                var people = doc.DocumentNode.SelectNodes(xpath).Select(p => p.InnerHtml);
                string text = "";
                foreach (var person in people)
                {
                    text += person;
                }

                HtmlDocument doc2 = new HtmlDocument();

                doc2.LoadHtml(text);
                hrefList = doc2.DocumentNode.SelectNodes("//a")
                                  .Select(p => p.GetAttributeValue("href", "not found")).Where(p => p.Contains("/aktualnosci/")).GroupBy(p => p.ToString())
                                  .ToList();

                var imgList = doc2.DocumentNode.SelectNodes("//img")
                                  .Select(p => p.GetAttributeValue("src", "not found"))
                                  .ToList();

                var titleList = doc2.DocumentNode.SelectNodes("//h2").Select(p => p.InnerText)
                                  .ToList();

                response.Close();
                readStream.Close();

                int index = 5;

                //       DataTable dt = GetWiadomosciPilka();

                if (newUser == true)
                {

                    //    if (.Count > 0)
                    //    {
                    //        List<int> deleteList = new List<int>();
                    //        var listTemp = new List<System.Linq.IGrouping<string, string>>();
                    //        var imageListTemp = new List<string>();
                    //        var titleListTemp = new List<string>();

                    //        for (int i = 0; i < hrefList.Count; i++)
                    //        {
                    //            if (dt.Rows[dt.Rows.Count - 1]["Wiadomosc1"].ToString() != hrefList[i].Key && dt.Rows[dt.Rows.Count - 1]["Wiadomosc2"].ToString() != hrefList[i].Key &&
                    //                dt.Rows[dt.Rows.Count - 1]["Wiadomosc3"].ToString() != hrefList[i].Key && dt.Rows[dt.Rows.Count - 1]["Wiadomosc4"].ToString() != hrefList[i].Key && dt.Rows[dt.Rows.Count - 1]["Wiadomosc5"].ToString() != hrefList[i].Key
                    //            )
                    //            {
                    //                listTemp.Add(hrefList[i]);
                    //                imageListTemp.Add("http://www.gkskatowice.eu" + imgList[i]);
                    //                titleListTemp.Add(titleList[i].Replace("&quot;", ""));
                    //            }
                    //            listTemp2.Add(hrefList[i]);
                    //        }
                    //        hrefList = listTemp;
                    //        index = hrefList.Count;
                    //        imgList = imageListTemp;
                    //        titleList = titleListTemp;
                    //        //   AddWiadomosc(listTemp2);
                    //    }
                    //    else
                    //    {
                    //        index = 5;
                    //        //   AddWiadomosc(hrefList);
                    //    }
                    //}

                    for (int i = 0; i < index; i++)
                    {
                        string link = "";
                        if (hrefList[i].Key.Contains("http"))
                        {
                            link = hrefList[i].Key;
                        }
                        else
                        {
                            link = "http://www.gkskatowice.eu" + hrefList[i].Key;
                            //link = "http://www.orlenliga.pl/" + hrefList[i].Key;
                        }

                        if (link.Contains("video"))
                        {
                            list.Add(GetHeroCard(
                            titleList[i].ToString(), "", "",
                            new CardImage(url: imgList[i]),
                            new CardAction(ActionTypes.OpenUrl, "Oglądaj video", value: link),
                            new CardAction(ActionTypes.OpenUrl, "Udostępnij", value: "https://www.facebook.com/sharer/sharer.php?u=" + link))
                            );
                        }
                        else
                            if (link.Contains("gallery"))
                        {
                            list.Add(GetHeroCard(
                            titleList[i].ToString(), "", "",
                            new CardImage(url: imgList[i]),
                            new CardAction(ActionTypes.OpenUrl, "Przeglądaj galerie", value: link),
                            new CardAction(ActionTypes.OpenUrl, "Udostępnij", value: "https://www.facebook.com/sharer/sharer.php?u=" + link))
                            );
                        }
                        else
                        {
                            list.Add(GetHeroCard(
                            titleList[i].ToString().Replace("&#8211;", "-"), "", "",
                            new CardImage(url: imgList[i]),
                            new CardAction(ActionTypes.OpenUrl, "Więcej", value: link),
                            new CardAction(ActionTypes.OpenUrl, "Udostępnij", value: "https://www.facebook.com/sharer/sharer.php?u=" + link))
                            );
                        }

                        //  list.Add(new Microsoft.Bot.Connector.VideoCard(titleList[i], "", "",null)
                    }
                }
                if (listTemp2.Count > 0)
                {
                    hrefList = listTemp2;
                }
            }
            return list;

        }
        public static IList<Attachment> GetCardsAttachmentsSport(ref List<IGrouping<string, string>> hrefList, bool newUser = false)
        {
            List<Attachment> list = new List<Attachment>();

            string urlAddress = "https://www.kleszczow.pl/sport-komunikaty/";
            // string urlAddress = "http://www.orlenliga.pl/";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
            request.Accept = "text/html";
            request.Method = "GET";
            request.UserAgent = ".NET Framework Test Client";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            var listTemp2 = new List<System.Linq.IGrouping<string, string>>();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if (response.CharacterSet == null)
                {
                    readStream = new StreamReader(receiveStream);
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                }

                string data = readStream.ReadToEnd();

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(data);

                string matchResultDivId = "media";
                string xpath = String.Format("//div[@class='{0}']", matchResultDivId);
                var people = doc.DocumentNode.SelectNodes(xpath).Select(p => p.InnerHtml);
                string text = "";
                foreach (var person in people)
                {
                    text += person;
                }

                HtmlDocument doc2 = new HtmlDocument();

                doc2.LoadHtml(text);
                hrefList = doc2.DocumentNode.SelectNodes("//a")
                                  .Select(p => p.GetAttributeValue("href", "not found")).Where(p => p.Contains("/aktualnosci/")).GroupBy(p => p.ToString())
                                  .ToList();

                var imgList = doc2.DocumentNode.SelectNodes("//img")
                                  .Select(p => p.GetAttributeValue("src", "not found"))
                                  .ToList();

                var titleList = doc2.DocumentNode.SelectNodes("//h2").Select(p => p.InnerText)
                                  .ToList();

                response.Close();
                readStream.Close();

                int index = 5;

                //       DataTable dt = GetWiadomosciPilka();

                if (newUser == true)
                {

                    //    if (.Count > 0)
                    //    {
                    //        List<int> deleteList = new List<int>();
                    //        var listTemp = new List<System.Linq.IGrouping<string, string>>();
                    //        var imageListTemp = new List<string>();
                    //        var titleListTemp = new List<string>();

                    //        for (int i = 0; i < hrefList.Count; i++)
                    //        {
                    //            if (dt.Rows[dt.Rows.Count - 1]["Wiadomosc1"].ToString() != hrefList[i].Key && dt.Rows[dt.Rows.Count - 1]["Wiadomosc2"].ToString() != hrefList[i].Key &&
                    //                dt.Rows[dt.Rows.Count - 1]["Wiadomosc3"].ToString() != hrefList[i].Key && dt.Rows[dt.Rows.Count - 1]["Wiadomosc4"].ToString() != hrefList[i].Key && dt.Rows[dt.Rows.Count - 1]["Wiadomosc5"].ToString() != hrefList[i].Key
                    //            )
                    //            {
                    //                listTemp.Add(hrefList[i]);
                    //                imageListTemp.Add("http://www.gkskatowice.eu" + imgList[i]);
                    //                titleListTemp.Add(titleList[i].Replace("&quot;", ""));
                    //            }
                    //            listTemp2.Add(hrefList[i]);
                    //        }
                    //        hrefList = listTemp;
                    //        index = hrefList.Count;
                    //        imgList = imageListTemp;
                    //        titleList = titleListTemp;
                    //        //   AddWiadomosc(listTemp2);
                    //    }
                    //    else
                    //    {
                    //        index = 5;
                    //        //   AddWiadomosc(hrefList);
                    //    }
                    //}

                    for (int i = 0; i < index; i++)
                    {
                        string link = "";
                        if (hrefList[i].Key.Contains("http"))
                        {
                            link = hrefList[i].Key;
                        }
                        else
                        {
                            link = "http://www.gkskatowice.eu" + hrefList[i].Key;
                            //link = "http://www.orlenliga.pl/" + hrefList[i].Key;
                        }

                        if (link.Contains("video"))
                        {
                            list.Add(GetHeroCard(
                            titleList[i].ToString(), "", "",
                            new CardImage(url: imgList[i]),
                            new CardAction(ActionTypes.OpenUrl, "Oglądaj video", value: link),
                            new CardAction(ActionTypes.OpenUrl, "Udostępnij", value: "https://www.facebook.com/sharer/sharer.php?u=" + link))
                            );
                        }
                        else
                            if (link.Contains("gallery"))
                        {
                            list.Add(GetHeroCard(
                            titleList[i].ToString(), "", "",
                            new CardImage(url: imgList[i]),
                            new CardAction(ActionTypes.OpenUrl, "Przeglądaj galerie", value: link),
                            new CardAction(ActionTypes.OpenUrl, "Udostępnij", value: "https://www.facebook.com/sharer/sharer.php?u=" + link))
                            );
                        }
                        else
                        {
                            list.Add(GetHeroCard(
                            titleList[i].ToString().Replace("&#8211;", "-"), "", "",
                            new CardImage(url: imgList[i]),
                            new CardAction(ActionTypes.OpenUrl, "Więcej", value: link),
                            new CardAction(ActionTypes.OpenUrl, "Udostępnij", value: "https://www.facebook.com/sharer/sharer.php?u=" + link))
                            );
                        }

                        //  list.Add(new Microsoft.Bot.Connector.VideoCard(titleList[i], "", "",null)
                    }
                }
                if (listTemp2.Count > 0)
                {
                    hrefList = listTemp2;
                }
            }
            return list;

        }

        public static IList<Attachment> GetCardsAttachmentsInwestycje(ref List<IGrouping<string, string>> hrefList, bool newUser = false)
        {
            List<Attachment> list = new List<Attachment>();

            string urlAddress = "https://www.kleszczow.pl/inwestycje-2/";
            // string urlAddress = "http://www.orlenliga.pl/";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
            request.Accept = "text/html";
            request.Method = "GET";
            request.UserAgent = ".NET Framework Test Client";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            var listTemp2 = new List<System.Linq.IGrouping<string, string>>();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if (response.CharacterSet == null)
                {
                    readStream = new StreamReader(receiveStream);
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                }

                string data = readStream.ReadToEnd();

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(data);

                string matchResultDivId = "media";
                string xpath = String.Format("//div[@class='{0}']", matchResultDivId);
                var people = doc.DocumentNode.SelectNodes(xpath).Select(p => p.InnerHtml);
                string text = "";
                foreach (var person in people)
                {
                    text += person;
                }

                HtmlDocument doc2 = new HtmlDocument();

                doc2.LoadHtml(text);
                hrefList = doc2.DocumentNode.SelectNodes("//a")
                                  .Select(p => p.GetAttributeValue("href", "not found")).Where(p => p.Contains("/aktualnosci/")).GroupBy(p => p.ToString())
                                  .ToList();

                var imgList = doc2.DocumentNode.SelectNodes("//img")
                                  .Select(p => p.GetAttributeValue("src", "not found"))
                                  .ToList();

                var titleList = doc2.DocumentNode.SelectNodes("//h2").Select(p => p.InnerText)
                                  .ToList();

                response.Close();
                readStream.Close();

                int index = 5;

                //       DataTable dt = GetWiadomosciPilka();

                if (newUser == true)
                {

                    //    if (.Count > 0)
                    //    {
                    //        List<int> deleteList = new List<int>();
                    //        var listTemp = new List<System.Linq.IGrouping<string, string>>();
                    //        var imageListTemp = new List<string>();
                    //        var titleListTemp = new List<string>();

                    //        for (int i = 0; i < hrefList.Count; i++)
                    //        {
                    //            if (dt.Rows[dt.Rows.Count - 1]["Wiadomosc1"].ToString() != hrefList[i].Key && dt.Rows[dt.Rows.Count - 1]["Wiadomosc2"].ToString() != hrefList[i].Key &&
                    //                dt.Rows[dt.Rows.Count - 1]["Wiadomosc3"].ToString() != hrefList[i].Key && dt.Rows[dt.Rows.Count - 1]["Wiadomosc4"].ToString() != hrefList[i].Key && dt.Rows[dt.Rows.Count - 1]["Wiadomosc5"].ToString() != hrefList[i].Key
                    //            )
                    //            {
                    //                listTemp.Add(hrefList[i]);
                    //                imageListTemp.Add("http://www.gkskatowice.eu" + imgList[i]);
                    //                titleListTemp.Add(titleList[i].Replace("&quot;", ""));
                    //            }
                    //            listTemp2.Add(hrefList[i]);
                    //        }
                    //        hrefList = listTemp;
                    //        index = hrefList.Count;
                    //        imgList = imageListTemp;
                    //        titleList = titleListTemp;
                    //        //   AddWiadomosc(listTemp2);
                    //    }
                    //    else
                    //    {
                    //        index = 5;
                    //        //   AddWiadomosc(hrefList);
                    //    }
                    //}

                    for (int i = 0; i < index; i++)
                    {
                        string link = "";
                        if (hrefList[i].Key.Contains("http"))
                        {
                            link = hrefList[i].Key;
                        }
                        else
                        {
                            link = "http://www.gkskatowice.eu" + hrefList[i].Key;
                            //link = "http://www.orlenliga.pl/" + hrefList[i].Key;
                        }

                        if (link.Contains("video"))
                        {
                            list.Add(GetHeroCard(
                            titleList[i].ToString(), "", "",
                            new CardImage(url: imgList[i]),
                            new CardAction(ActionTypes.OpenUrl, "Oglądaj video", value: link),
                            new CardAction(ActionTypes.OpenUrl, "Udostępnij", value: "https://www.facebook.com/sharer/sharer.php?u=" + link))
                            );
                        }
                        else
                            if (link.Contains("gallery"))
                        {
                            list.Add(GetHeroCard(
                            titleList[i].ToString(), "", "",
                            new CardImage(url: imgList[i]),
                            new CardAction(ActionTypes.OpenUrl, "Przeglądaj galerie", value: link),
                            new CardAction(ActionTypes.OpenUrl, "Udostępnij", value: "https://www.facebook.com/sharer/sharer.php?u=" + link))
                            );
                        }
                        else
                        {
                            list.Add(GetHeroCard(
                            titleList[i].ToString().Replace("&#8211;", "-"), "", "",
                            new CardImage(url: imgList[i]),
                            new CardAction(ActionTypes.OpenUrl, "Więcej", value: link),
                            new CardAction(ActionTypes.OpenUrl, "Udostępnij", value: "https://www.facebook.com/sharer/sharer.php?u=" + link))
                            );
                        }

                        //  list.Add(new Microsoft.Bot.Connector.VideoCard(titleList[i], "", "",null)
                    }
                }
                if (listTemp2.Count > 0)
                {
                    hrefList = listTemp2;
                }
            }
            return list;

        }

        private static Attachment GetHeroCard(string title, string subtitle, string text, CardImage cardImage, CardAction cardAction, CardAction cardAction2)
        {
            if (cardAction2 != null)
            {
                var heroCard = new HeroCard
                {
                    Title = title,
                    Subtitle = subtitle,
                    Text = text,
                    Images = new List<CardImage>() { cardImage },
                    Buttons = new List<CardAction>() { cardAction, cardAction2 },
                };

                return heroCard.ToAttachment();
            }
            else
            {
                var heroCard = new HeroCard
                {
                    Title = title,
                    Subtitle = subtitle,
                    Text = text,
                    Images = new List<CardImage>() { cardImage },
                    Buttons = new List<CardAction>() { cardAction },
                };

                return heroCard.ToAttachment();
            }
        }

  
        public static DataTable GetWiadomosci()
        {
            DataTable dt = new DataTable();

            try
            {
                SqlConnection sqlConnection1 = new SqlConnection("Server=tcp:plps.database.windows.net,1433;Initial Catalog=PLPS;Persist Security Info=False;User ID=tomasoft;Password=Tomason18,;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
                SqlCommand cmd = new SqlCommand();

                cmd.CommandText = "SELECT * FROM [dbo].[Wiadomosci" + BaseDB.appName + "]";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = sqlConnection1;

                sqlConnection1.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                sqlConnection1.Close();
                return dt;
            }
            catch
            {
                BaseDB.AddToLog("Błąd pobierania wiadomości");
                return null;
            }
        }


        public static DataTable GetUser()
        {
            DataTable dt = new DataTable();

            try
            {
                SqlConnection sqlConnection1 = new SqlConnection("Server=tcp:plps.database.windows.net,1433;Initial Catalog=PLPS;Persist Security Info=False;User ID=tomasoft;Password=Tomason18,;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
                SqlCommand cmd = new SqlCommand();

                cmd.CommandText = "SELECT * FROM [dbo].[User" + BaseDB.appName + "] where flgDeleted=0";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = sqlConnection1;

                sqlConnection1.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                sqlConnection1.Close();
                return dt;
            }
            catch
            {
                BaseDB.AddToLog("Błąd pobierania użytkowników");
                return null;
            }
        }


    }
}