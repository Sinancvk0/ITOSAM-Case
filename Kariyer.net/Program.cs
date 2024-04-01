using System;
using System.Collections.Generic;
using System.Net.Http;
using HtmlAgilityPack;
using OfficeOpenXml;

class Program
{
    static async System.Threading.Tasks.Task Main(string[] args)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        var httpClient = new HttpClient();
        string[] sektorAdlari = { "Bilişim", "İnşaat", "Eğitim", "Enerji", "Gıda", "Kimya",
            "Elektrik & Elektronik", "Güvenlik", "Maden ve Metal Sanayi", "Mobilya & Aksesuar",
            "Ev Eşyaları", "Orman Ürünleri", "Ofis / Büro Malzemeleri", "Otomotiv", "Sağlık",
            "Tarım / Ziraat", "Taşımacılık", "Tekstil", "Telekomünikasyon", "Turizm", "Yapı",
            "Topluluklar", "Hizmet", "Danışmanlık", "Reklam ve Tanıtım", "Finans - Ekonomi",
            "Ticaret", "Denizcilik", "Eğlence - Kültür - Sanat", "Basım - Yayın", "Medya",
            "Havacılık", "Hızlı Tüketim Malları", "Hayvancılık", "Sigortacılık",
            "Dayanıklı Tüketim Ürünleri", "Atık Yönetimi ve Geri Dönüşüm", "Arşiv Yönetimi ve Saklama", "Perakende",
            "Çevre", "İletişim Danışmanlığı", "Kaynak ve Kesme Ekipmanları", "Gemi Yan Sanayi", "Bina ve Site Yönetimi",
            "Sondaj", "Dental", "Organizasyon", "Otoyol, Tünel ve Köprü İşletmeciliği", "Diğer" };

        var sektorler = new List<Tuple<string, string>>();

        foreach (var sektorAdi in sektorAdlari)
        {
            var url = $"https://www.kariyer.net/is-ilanlari/{sektorAdi}-is-ilanlari?ct=34,82&date=15g";
            sektorler.Add(Tuple.Create(sektorAdi, url));
        }

        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("İlan Sayıları");

            worksheet.Cells[1, 1].Value = "Sektör";
            worksheet.Cells[1, 2].Value = "İlan Sayısı";

            int row = 2;

            foreach (var sektor in sektorler)
            {
                var html = await httpClient.GetStringAsync(sektor.Item2);
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);
                var ilanSayisiElementi = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='t-6 text-secondary mb-3 search-result-section']");

                string ilanSayisi = (ilanSayisiElementi != null) ? ilanSayisiElementi.InnerText.Trim() : "Bulunamadı";

                worksheet.Cells[row, 1].Value = sektor.Item1;
                worksheet.Cells[row, 2].Value = ilanSayisi;

                row++;
            }

           
            var excelFile = new FileInfo("SektörVerileri.xlsx");
            package.SaveAs(excelFile);
        }
            Console.WriteLine("Excel dosyası oluşturuldu.");
    }
}
