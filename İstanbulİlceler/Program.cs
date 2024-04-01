using System;
using System.Collections.Generic;
using System.Net.Http;
using HtmlAgilityPack;

class Program
{
    static async System.Threading.Tasks.Task Main(string[] args)
    {
        var httpClient = new HttpClient();

        // Verilen sayıları bir liste içerisine alalım
        List<string> ttValues = new List<string> { "463", "5785"};

        foreach (var ttValue in ttValues)
        {
            // URL'yi oluştururken tt parametresini güncelleyerek kullanalım
            string baseUrl = $"https://www.kariyer.net/is-ilanlari/istanbul+?ct=34,82&tt={ttValue}&date=15g";

            var html = await httpClient.GetStringAsync(baseUrl);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var ilanKartlari = htmlDocument.DocumentNode.SelectNodes("//a[@class='k-ad-card']");

            if (ilanKartlari != null)
            {
                foreach (var ilanKarti in ilanKartlari)
                {
                    string ilanDetayUrlRelative = ilanKarti.Attributes["href"].Value;

                    Uri baseUri = new Uri("https://www.kariyer.net");
                    Uri ilanDetayUri = new Uri(baseUri, ilanDetayUrlRelative);
                    string ilanDetayUrlAbsolute = ilanDetayUri.AbsoluteUri;

                    var ilanDetayHtml = await httpClient.GetStringAsync(ilanDetayUrlAbsolute);
                    var ilanDetayHtmlDocument = new HtmlDocument();
                    ilanDetayHtmlDocument.LoadHtml(ilanDetayHtml);

                    var basvuruSayisiTagi = ilanDetayHtmlDocument.DocumentNode.SelectSingleNode("//p[@data-v-1ec9e575 and contains(span, 'başvuru')]");

                    if (basvuruSayisiTagi != null)
                    {
                        string basvuruSayisi = basvuruSayisiTagi.InnerText.Trim();

                        string[] basvuruParts = basvuruSayisi.Split(" ");
                        string basvuruSayisiSadeceRakamlar = basvuruParts[0];

                        Console.WriteLine($"İlan Detay URL: {ilanDetayUrlAbsolute}");
                        Console.WriteLine($"Başvuru Sayısı: {basvuruSayisiSadeceRakamlar}");
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine($"İlan Detay URL: {ilanDetayUrlAbsolute}");
                        Console.WriteLine("Başvuru sayısı bulunamadı.");
                        Console.WriteLine();
                    }
                }
            }
            else
            {
                Console.WriteLine("İlan bulunamadı.");
            }
        }
    }
}
