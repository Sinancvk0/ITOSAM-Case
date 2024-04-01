using System;
using System.Collections.Generic;
using System.Net.Http;
using HtmlAgilityPack;

class Program
{
    static async System.Threading.Tasks.Task Main(string[] args)
    {
        var httpClient = new HttpClient();

        Dictionary<string, string> ilceListesi = new Dictionary<string, string>
        {
            { "5785", "Arnavutköy" }, { "434", "Avcılar" }, { "437", "Bağcılar" }, { "438", "Bahçelievler" },
            { "435", "Bakırköy" }, { "5786", "Başakşehir" }, { "436", "Bayrampaşa" }, { "439", "Beşiktaş" },
            { "1077", "Beylikdüzü" }, { "440", "Beyoğlu" }, {"441","Büyükçekmece" }, {"442","Çatalca" },
            {"445","Eminönü" }, {"443","Esenler" }, {"5787","Esenyurt" }, { "444","Eyüp" }, { "446","Fatih" },
            { "447","Gaziosmanpaşa" }, { "448","Güngören" }, { "449","Kağıthane" }, { "450","Küçükçekmece" },
            { "5677","MimarSinan/Büyükçekmece" }, { "451","Sarıyer" }, { "452","Silivri" }, { "5765","Sultangazi" },
            { "453","Şişli" }, { "454","Zeytinburnu" } , {"455","Adalar" }, { "1722","Ataşehir"}, { "456","Beykoz" },
            { "1244","Çekmeköy" }, {"457","Kadıköy" }, { "458","Kartal" }, { "459","Maltepe" }, { "460","Pendik" },
            { "5788","Sancaktepe" }, { "461","Sultanbeyli" }, { "462","Şile" }, { "463","Tuzla" }, { "464","Ümraniye" },
            { "465","Üsküdar" }
        };

        foreach (var kvp in ilceListesi)
        {
            string ilceNumarasi = kvp.Key;
            string ilceIsmi = kvp.Value;

            int toplamIlanSayisi = 0;
            int toplamBasvuruSayisi = 0;

            string baseUrl = $"https://www.kariyer.net/is-ilanlari/{ilceIsmi}?ct=34,82&tt={ilceNumarasi}&date=3g";

            var html = await httpClient.GetStringAsync(baseUrl);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var sayfaNumarasiElement = htmlDocument.DocumentNode.SelectSingleNode("//button[@class='page-link' and @aria-checked='true']");
            int sayfaSayisi = 1;
            if (sayfaNumarasiElement != null && int.TryParse(sayfaNumarasiElement.GetAttributeValue("aria-setsize", "1"), out int parsedSayfaSayisi))
            {
                sayfaSayisi = parsedSayfaSayisi;
            }

            for (int sayfaNo = 1; sayfaNo <= sayfaSayisi; sayfaNo++)
            {
                string pageUrl = $"{baseUrl}&cp={sayfaNo}";
                var pageHtml = await httpClient.GetStringAsync(pageUrl);
                var pageHtmlDocument = new HtmlDocument();
                pageHtmlDocument.LoadHtml(pageHtml);

                var ilanKartlari = pageHtmlDocument.DocumentNode.SelectNodes("//a[@class='k-ad-card']");

                if (ilanKartlari != null)
                {
                    int ilanSayisi = ilanKartlari.Count;
                    toplamIlanSayisi += ilanSayisi;

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

                            int basvuruSayisiInt;
                            if (int.TryParse(basvuruSayisiSadeceRakamlar, out basvuruSayisiInt))
                            {
                                toplamBasvuruSayisi += basvuruSayisiInt;
                            }
                        }
                    }
                }
            }

            Console.WriteLine($"İlçe İsmi: {ilceIsmi}");
            Console.WriteLine($"Toplam İlan Sayısı: {toplamIlanSayisi}");
            Console.WriteLine($"Toplam Başvuru Sayısı: {toplamBasvuruSayisi}");
            Console.WriteLine();
        }
    }
}
