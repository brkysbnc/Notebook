using System;
using System.IO;
using System.Windows.Forms;

namespace NotDefteri
{
    /// <summary>
    /// Ana form sınıfı
    /// Not defteri uygulamasının tüm işlevselliğini içerir
    /// </summary>
    public partial class Form1 : Form
    {
        // Mevcut dosya yolu - kaydedilmiş dosyanın konumunu tutar
        private string dosyaYolu = string.Empty;
        
        // Dosyada değişiklik yapılıp yapılmadığını takip eder
        private bool degisiklikVar = false;
        
        // Komut satırından gelen dosya yolu (form yüklendikten sonra açılacak)
        private string baslangicDosyaYolu = string.Empty;

        /// <summary>
        /// Form yapıcı metodu
        /// Form bileşenlerini başlatır
        /// </summary>
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Form yapıcı metodu - komut satırından dosya yolu ile
        /// </summary>
        /// <param name="baslangicDosyaYolu">Açılacak dosyanın yolu</param>
        public Form1(string baslangicDosyaYolu) : this()
        {
            this.baslangicDosyaYolu = baslangicDosyaYolu;
            // Shown event'i form tamamen gösterildikten sonra çağrılır (Load'dan sonra)
            this.Shown += Form1_Shown;
        }

        /// <summary>
        /// Form gösterildiğinde çağrılır (Load'dan sonra, form tamamen hazır olduğunda)
        /// Komut satırından dosya yolu verilmişse dosyayı açar
        /// </summary>
        private void Form1_Shown(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(baslangicDosyaYolu))
            {
                // BeginInvoke ile asenkron olarak çağır, form tamamen hazır olsun
                this.BeginInvoke(new Action(() =>
                {
                    DosyayiAc(baslangicDosyaYolu);
                }));
            }
        }

        /// <summary>
        /// Metin kutusu içeriği değiştiğinde çağrılır
        /// Değişiklik durumunu işaretler
        /// </summary>
        private void txtNot_TextChanged(object sender, EventArgs e)
        {
            // Metin değiştiğinde değişiklik bayrağını aktif et
            degisiklikVar = true;
            
            // Başlık çubuğuna dosya adını ve değişiklik durumunu göster
            string baslik = "Not Defteri";
            if (!string.IsNullOrEmpty(dosyaYolu))
            {
                baslik = Path.GetFileName(dosyaYolu) + " - Not Defteri";
            }
            if (degisiklikVar)
            {
                baslik += " *";
            }
            this.Text = baslik;
        }

        /// <summary>
        /// Yeni dosya oluşturma işlemi
        /// Mevcut değişiklikleri kontrol eder
        /// </summary>
        private void yeniToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Eğer değişiklik varsa kullanıcıya sor
            if (degisiklikVar)
            {
                DialogResult sonuc = MessageBox.Show(
                    "Kaydedilmemiş değişiklikler var. Kaydetmek ister misiniz?",
                    "Not Defteri",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (sonuc == DialogResult.Yes)
                {
                    // Kullanıcı kaydetmek istiyor
                    if (Kaydet())
                    {
                        // Başarıyla kaydedildiyse yeni dosya oluştur
                        YeniDosyaOlustur();
                    }
                    // İptal edildiyse hiçbir şey yapma
                }
                else if (sonuc == DialogResult.No)
                {
                    // Kullanıcı kaydetmek istemiyor, direkt yeni dosya oluştur
                    YeniDosyaOlustur();
                }
                // Cancel durumunda hiçbir şey yapma
            }
            else
            {
                // Değişiklik yoksa direkt yeni dosya oluştur
                YeniDosyaOlustur();
            }
        }

        /// <summary>
        /// Yeni boş bir dosya oluşturur
        /// </summary>
        private void YeniDosyaOlustur()
        {
            txtNot.Clear();
            dosyaYolu = string.Empty;
            degisiklikVar = false;
            this.Text = "Not Defteri";
        }

        /// <summary>
        /// Dosya açma işlemi
        /// Mevcut değişiklikleri kontrol eder
        /// </summary>
        private void acToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Eğer değişiklik varsa kullanıcıya sor
            if (degisiklikVar)
            {
                DialogResult sonuc = MessageBox.Show(
                    "Kaydedilmemiş değişiklikler var. Kaydetmek ister misiniz?",
                    "Not Defteri",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (sonuc == DialogResult.Yes)
                {
                    // Kullanıcı kaydetmek istiyor
                    if (!Kaydet())
                    {
                        // Kaydetme iptal edildiyse dosya açma işlemini durdur
                        return;
                    }
                }
                else if (sonuc == DialogResult.Cancel)
                {
                    // İptal edildiyse dosya açma işlemini durdur
                    return;
                }
                // No durumunda kaydetmeden devam et
            }

            // Dosya açma dialogunu göster
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                DosyayiAc(openFileDialog1.FileName);
            }
        }

        /// <summary>
        /// Belirtilen dosya yolundaki dosyayı açar ve içeriğini yükler
        /// Komut satırından veya menüden çağrılabilir
        /// </summary>
        /// <param name="dosyaYolu">Açılacak dosyanın tam yolu</param>
        public void DosyayiAc(string dosyaYolu)
        {
            try
            {
                // Dosya var mı kontrol et
                if (!File.Exists(dosyaYolu))
                {
                    MessageBox.Show(
                        "Dosya bulunamadı: " + dosyaYolu,
                        "Hata",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                string icerik = string.Empty;
                
                // Dosya boyutunu kontrol et
                FileInfo dosyaBilgisi = new FileInfo(dosyaYolu);
                
                if (dosyaBilgisi.Length == 0)
                {
                    // Dosya boşsa direkt boş içerik yükle
                    icerik = string.Empty;
                }
                else
                {
                    // Encoding listesi - bizim uygulama UTF-8 ile kaydediyor, o yüzden önce UTF-8 dene
                    // Encoding'leri güvenli bir şekilde al
                    System.Text.Encoding?[] encodings = new System.Text.Encoding?[]
                    {
                        System.Text.Encoding.UTF8,                    // UTF-8 (bizim uygulama bununla kaydediyor)
                        null,                                         // Windows-1254 (Türkçe ANSI) - güvenli alınacak
                        null,                                         // Windows-1252 (Batı Avrupa) - güvenli alınacak
                        System.Text.Encoding.Default,                  // Sistem varsayılan encoding
                        new System.Text.UTF8Encoding(true)              // UTF-8 BOM ile
                    };
                    
                    // Windows-1254 encoding'ini güvenli bir şekilde al
                    try
                    {
                        encodings[1] = System.Text.Encoding.GetEncoding(1254);
                    }
                    catch
                    {
                        encodings[1] = null; // Encoding mevcut değilse null bırak
                    }
                    
                    // Windows-1252 encoding'ini güvenli bir şekilde al
                    try
                    {
                        encodings[2] = System.Text.Encoding.GetEncoding(1252);
                    }
                    catch
                    {
                        encodings[2] = null; // Encoding mevcut değilse null bırak
                    }
                    
                    bool okundu = false;
                    foreach (var enc in encodings)
                    {
                        // Null encoding'leri atla
                        if (enc == null)
                            continue;
                            
                        try
                        {
                            icerik = File.ReadAllText(dosyaYolu, enc);
                            // Dosya başarıyla okundu (içerik boş olsa bile)
                            okundu = true;
                            break;
                        }
                        catch
                        {
                            // Bu encoding ile okunamadı, bir sonrakini dene
                            continue;
                        }
                    }
                    
                    if (!okundu)
                    {
                        // Hiçbir encoding ile okunamadı, hata göster
                        MessageBox.Show(
                            "Dosya okunamadı. Dosya bozuk olabilir.",
                            "Hata",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return;
                    }
                }
                
                // İçeriği yükle - TextChanged event'ini geçici olarak devre dışı bırak
                txtNot.TextChanged -= txtNot_TextChanged;
                txtNot.Text = icerik;
                this.dosyaYolu = dosyaYolu;
                degisiklikVar = false;
                this.Text = Path.GetFileName(dosyaYolu) + " - Not Defteri";
                // TextChanged event'ini tekrar aktif et
                txtNot.TextChanged += txtNot_TextChanged;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Dosya açılırken hata oluştu: " + ex.Message,
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Dosya kaydetme işlemi
        /// Eğer dosya yolu yoksa "Farklı Kaydet" dialogunu açar
        /// </summary>
        private void kaydetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Kaydet();
        }

        /// <summary>
        /// Dosya kaydetme mantığı
        /// Dosya yolu varsa direkt kaydeder, yoksa "Farklı Kaydet" dialogunu açar
        /// </summary>
        /// <returns>Kaydetme başarılı olduysa true, iptal edildiyse false</returns>
        private bool Kaydet()
        {
            // Eğer dosya yolu yoksa "Farklı Kaydet" dialogunu aç
            if (string.IsNullOrEmpty(dosyaYolu))
            {
                return FarkliKaydet();
            }
            else
            {
                // Dosya yolu varsa direkt kaydet (UTF-8 encoding ile)
                try
                {
                    File.WriteAllText(dosyaYolu, txtNot.Text, System.Text.Encoding.UTF8);
                    degisiklikVar = false;
                    this.Text = Path.GetFileName(dosyaYolu) + " - Not Defteri";
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Dosya kaydedilirken hata oluştu: " + ex.Message,
                        "Hata",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return false;
                }
            }
        }

        /// <summary>
        /// Farklı kaydetme işlemi
        /// Dosya gezgininden konum seçtirir
        /// </summary>
        private void farkliKaydetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FarkliKaydet();
        }

        /// <summary>
        /// Farklı kaydetme mantığı
        /// Dosya gezgininden konum seçtirir ve dosyayı kaydeder
        /// </summary>
        /// <returns>Kaydetme başarılı olduysa true, iptal edildiyse false</returns>
        private bool FarkliKaydet()
        {
            // Dosya kaydetme dialogunu göster
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Seçilen konuma dosyayı kaydet (UTF-8 encoding ile)
                    File.WriteAllText(saveFileDialog1.FileName, txtNot.Text, System.Text.Encoding.UTF8);
                    dosyaYolu = saveFileDialog1.FileName;
                    degisiklikVar = false;
                    this.Text = Path.GetFileName(dosyaYolu) + " - Not Defteri";
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Dosya kaydedilirken hata oluştu: " + ex.Message,
                        "Hata",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return false;
                }
            }
            return false; // Kullanıcı iptal etti
        }

        /// <summary>
        /// Çıkış işlemi
        /// Mevcut değişiklikleri kontrol eder
        /// </summary>
        private void cikisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close(); // FormClosing event'i kontrol edecek
        }

        /// <summary>
        /// Form kapatılırken çağrılır
        /// Kaydedilmemiş değişiklikleri kontrol eder
        /// </summary>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Eğer değişiklik varsa kullanıcıya sor
            if (degisiklikVar)
            {
                DialogResult sonuc = MessageBox.Show(
                    "Kaydedilmemiş değişiklikler var. Kaydetmek ister misiniz?",
                    "Not Defteri",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (sonuc == DialogResult.Yes)
                {
                    // Kullanıcı kaydetmek istiyor
                    if (Kaydet())
                    {
                        // Başarıyla kaydedildiyse formu kapat
                        e.Cancel = false;
                    }
                    else
                    {
                        // Kaydetme iptal edildiyse formu kapatma
                        e.Cancel = true;
                    }
                }
                else if (sonuc == DialogResult.No)
                {
                    // Kullanıcı kaydetmek istemiyor, formu kapat
                    e.Cancel = false;
                }
                else
                {
                    // Kullanıcı iptal etti, formu kapatma
                    e.Cancel = true;
                }
            }
        }
    }
}

