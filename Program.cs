using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace NotDefteri
{
    /// <summary>
    /// Uygulamanın giriş noktası
    /// Ana formu başlatır
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Uygulamanın ana giriş noktası.
        /// Komut satırı argümanlarını kontrol eder ve dosya yolu varsa açar
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // CodePages encoding provider'ı kaydet (Windows-1254, Windows-1252 gibi encoding'ler için gerekli)
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            
            // Windows Forms uygulaması için gerekli ayarlar
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Ana formu oluştur

            Form1 form;
            
            // Eğer komut satırından dosya yolu verilmişse, formu dosya yolu ile oluştur
            if (args.Length > 0 && !string.IsNullOrEmpty(args[0]))
            {
                string dosyaYolu = args[0];
                // Dosya yolu tırnak içindeyse temizle
                if (dosyaYolu.StartsWith("\"") && dosyaYolu.EndsWith("\""))
                {
                    dosyaYolu = dosyaYolu.Substring(1, dosyaYolu.Length - 2);
                }
                
                // Dosya varsa formu dosya yolu ile oluştur (form yüklendiğinde açılacak)
                if (File.Exists(dosyaYolu))
                {
                    form = new Form1(dosyaYolu);
                }
                else
                {
                    form = new Form1();
                }
            }
            else
            {
                form = new Form1();
            }
            
            // Formu başlat
            Application.Run(form);
        }
    }
}

