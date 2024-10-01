using HtmlAgilityPack;
using ScrapySharp.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace WBECrawler
{
    class Program
    {

        //https://github.com/AngleSharp/AngleSharp

        static void Main(string[] args)
        {

            var form = $"PHPSESSID=bad72bdd4645b9bb1c6ffc7ed692cfe9&wf=wbbsuche&wbbsuche%5B5%5D=01.+Allgemeinmedizin&wbbsuche%5B2%5D=--+keine+Auswahl+--&wbbsuche%5B4%5D=--+keine+Auswahl+--&wbbsuche%5B1%5D=--+keine+Auswahl+--&steps=10&wbbsuche%5B0%5D=&wbbsuche%5B3%5D=&action_formular_showResult=Suchen";
            //var form = $"PHPSESSID=bad72bdd4645b9bb1c6ffc7ed692cfe9&wf=wbbsuche&wbbsuche%5B5%5D=--+keine+Auswahl+--&wbbsuche%5B2%5D=--+keine+Auswahl+--&wbbsuche%5B4%5D=--+keine+Auswahl+--&wbbsuche%5B1%5D=--+keine+Auswahl+--&steps=10000&wbbsuche%5B0%5D=&wbbsuche%5B3%5D=&action_formular_showResult=Suchen";


            CookieCollection Cookies = new CookieCollection();
            var web = new HtmlWeb();
            web.OverrideEncoding = Encoding.Default;
            web.UseCookies = true;
            web.PreRequest += (request) =>
            {
                if (request.Method == "POST")
                {
                    string payload = form; // request.Address.Query;
                    byte[] buff = Encoding.UTF8.GetBytes(payload.ToCharArray());
                    request.ContentLength = buff.Length;
                    request.ContentType = "application/x-www-form-urlencoded";
                    System.IO.Stream reqStream = request.GetRequestStream();
                    reqStream.Write(buff, 0, buff.Length);
                }

                request.CookieContainer.Add(Cookies);

                return true;
            };

            web.PostResponse += (request, response) =>
            {
                if (request.CookieContainer.Count > 0 || response.Cookies.Count > 0)
                {
                    Cookies.Add(response.Cookies);
                }
            };



            var url = "https://www.slaek.de/de/01/weiterbildung/wbb-listen02.php";



            var result = web.Load(url, "POST");


            /*
             <!--/UdmComment--><div class="block php"><form name="showResult" action="/de/01/weiterbildung/wbb-listen02.php" method="POST">
<input type="hidden" name="PHPSESSID" value="bad72bdd4645b9bb1c6ffc7ed692cfe9" />
<input type="hidden" name="wf" value="wbbsuche" />
<h2>Weiterbildungsbefugte (aktuell) - Suchergebnisse</h2><h4>
			Ihre Suche ergab: 735 Treffer
		</h4><table cellspacing="10"  border="0"><tr><td><a href="?phpfile=formular&amp;fname=start&amp;wf=wbbsuche&amp;PHPSESSID=bad72bdd4645b9bb1c6ffc7ed692cfe9&amp;dom=slaek-neu.aek-service.de&amp;antzformto=/de/01/weiterbildung/wbb-listen02.php&amp;rserv=www.slaek.de">zurück zur Suche</a></td></tr></table><br><br><table width="100%" align="center" cellpadding="2" cellspacing="2"  border="0" rules="groups">
<thead><tr>
<th align="left" width="30%">
						Fachrichtung<br>
</th>
<th align="left" width="30%">befugte(r) Ärztin/Arzt</th>
<th align="left" width="40%">Weiterbildungsstätte</th>
</tr></thead>
<tr >
<td align="left" valign="top"><b>Allgemeinmedizin</b></td>
<td align="left" valign="top"><b>Dr. med. Jeannette Baumann-Walther</b></td>
<td align="left" valign="top">
<b>Praxis</b><br><br>
</td>
</tr>
<tr >
<td>
	
             */


            //if (web.Load(url) is HtmlDocument document)
            if (result is HtmlDocument document)
            {

                HtmlNode table = document.DocumentNode.SelectSingleNode("//table[2]");

                List<HtmlNode> list = new List<HtmlNode>();

                foreach (var cn in table.ChildNodes)
                {
                    if (cn.Name.Equals("thead")) list.Add(cn);
                    if (cn.Name.Equals("#text")) list.Add(cn);
                }

                foreach (var cn in list)
                {
                    table.RemoveChild(cn);
                }

                //var nodes = document.DocumentNode.CssSelect("#item-search-results li").ToList();

                foreach (var node in table.SelectNodes(".//tr"))
                {
                    //Console.WriteLine(" " + node.CssSelect("h2 a").Single().InnerText);

                    if (node.InnerHtml.ToLower().Contains("befugnis"))
                    {
                        var split = node.InnerHtml.Split("<br><br>");
                        if (split[0].ToLower().Contains("befugnis"))
                            Console.WriteLine(RemoveHTML(split[0]).Trim());
                        if (split[1].ToLower().Contains("wbo 2"))
                            Console.WriteLine(RemoveHTML(split[1]).Trim());
                    }

                    //Console.WriteLine(" " + node.SelectSingleNode("//td[2]").InnerText);
                }
            }
            Console.ReadLine();
        }

        public static string RemoveHTML(string text)
        {
            text = text.Replace("&nbsp;", " ").Replace("\t", "").Replace("<br>", "").Replace("\n", "");
            var oRegEx = new System.Text.RegularExpressions.Regex("<[^>]+>");
            return oRegEx.Replace(text, string.Empty);
        }

    }
}
