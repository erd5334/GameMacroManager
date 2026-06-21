# 🎮 Game Macro Manager

Oyun makrolarını kolayca yönetmenizi sağlayan profesyonel bir C# WPF uygulaması.

## 📋 İçindekiler

- [Özellikler](#özellikler)
- [Kurulum](#kurulum)
- [Kullanım Kılavuzu](#kullanım-kılavuzu)
- [Proje Yapısı](#proje-yapısı)
- [Geliştirme Notları](#geliştirme-notları)

---

## ✨ Özellikler

| Özellik | Açıklama |
|---------|----------|
| 🎮 **Çoklu Oyun Desteği** | Farklı oyunlar için ayrı profiller oluşturun |
| 👤 **Karakter Yönetimi** | Her oyun için birden fazla karakter tanımlayın |
| ⚡ **Combo Editörü** | Görsel olarak tuş kombinasyonları oluşturun |
| 🔑 **Global Hotkey** | Oyun açıkken bile çalışan kısayol tuşları |
| 💾 **Otomatik Kaydetme** | Tüm veriler JSON formatında saklanır |
| 🌙 **Modern Karanlık Tema** | Göz yormayan modern arayüz |

---

## 🚀 Kurulum

### Gereksinimler
- Windows 10/11
- .NET 8.0 Runtime

### Çalıştırma

```bash
# Proje dizinine git
cd GameMacroManager

# Derle ve çalıştır
dotnet run

# Veya sadece exe'yi çalıştır
.\bin\Debug\net8.0-windows\GameMacroManager.exe
```

---

## 📖 Kullanım Kılavuzu

### 1️⃣ Oyun Ekleme

1. Sol panelde **"OYUNLAR"** bölümünü bulun
2. **"+ Ekle"** butonuna tıklayın
3. Yeni oyunun adını düzenleyin
4. Oyun renk temasını değiştirebilirsiniz

### 2️⃣ Karakter Ekleme

1. Önce bir oyun seçin
2. Ortadaki **"KARAKTERLER"** panelinde **"+ Ekle"** butonuna tıklayın
3. Karakter adını girin
4. Her karakter için farklı renk seçebilirsiniz

### 3️⃣ Combo Oluşturma

1. Bir karakter seçin
2. Sağ panelde **"Yeni Combo"** butonuna tıklayın
3. Combo'ya bir isim verin (örn: "Hadouken")
4. **Hotkey** alanına kısayol tuşu girin (örn: "N", "F1", "G")
5. Kategori belirleyin (Saldırı, Savunma, Özel, vb.)

### 4️⃣ Tuş Aksiyonları Ekleme

1. Combo seçiliyken **"+ Tuş Ekle"** butonuna tıklayın
2. Her tuş için:
   - **Tuş**: Hangi tuşa basılacak (A, B, Left, Right, Space, vb.)
   - **Aksiyon Tipi**:
     - `Tap` = Bas ve bırak
     - `Press` = Sadece bas (basılı tut)
     - `Release` = Sadece bırak
   - **Gecikme**: Tuştan sonra beklenecek süre (ms)
   - **Basılı**: Tuşun basılı tutulma süresi (ms)

### 5️⃣ Combo Ayarları

| Ayar | Açıklama |
|------|----------|
| **Basılı tutulunca tekrarla** | Hotkey basılı tutulduğu sürece combo tekrarlanır |
| **Tekrar gecikmesi** | Tekrarlar arası bekleme süresi (ms) |

### 6️⃣ Makroları Kullanma

1. Sağ üstteki **toggle switch** ile makroları aktif edin
2. Yeşil **"AKTİF"** yazısını görün
3. Oyununuzu açın
4. Belirlediğiniz hotkey'e basın!

### 7️⃣ Test Etme

1. Bir combo seçin
2. **"Test Et (2sn)"** butonuna tıklayın
3. 2 saniye içinde istediğiniz yere tıklayın
4. Combo otomatik olarak çalışacaktır

---

## 📁 Veri Konumu

Tüm veriler şu konuma kaydedilir:
```
%APPDATA%\GameMacroManager\
├── games.json      # Oyun, karakter ve combo verileri
└── settings.json   # Uygulama ayarları
```

**Veri klasörünü açmak için:** Alt durum çubuğundaki 📁 ikonuna tıklayın.

---

## ⌨️ Kısayol Tuşları

| Tuş | İşlev |
|-----|-------|
| **F9** | Makroları Aç/Kapa |
| **F10** | Tüm Makroları Durdur |

---

## 🎮 Örnek Combo: Street Fighter - Blanka

**Blitz Yumruk Combo:**
```
Hotkey: N
Tekrar: Açık

Aksiyonlar:
1. [Z] Tap - 50ms gecikme
2. [D] Tap - 50ms gecikme
3. [D] Tap - 50ms gecikme
4. [D] Tap - 50ms gecikme
```

**Kullanım:** N tuşuna basılı tuttuğunuzda, sürekli Z-D-D-D kombinasyonu tekrarlanır.

---

## ⚠️ Önemli Notlar

1. **Yönetici Olarak Çalıştırın** - Bazı oyunlarda global hotkey çalışması için gerekebilir
2. **Anti-Cheat Uyarısı** - Online oyunlarda makro kullanımı yasak olabilir
3. **Performans** - Çok kısa gecikme süreleri bazı oyunlarda çalışmayabilir
4. **Test Edin** - Yeni comboları oyun dışında test edin

---

## 🔧 Sorun Giderme

### Makrolar Çalışmıyor
- Sağ üstte "AKTİF" yazdığından emin olun
- Hotkey'in başka bir programa atanmadığından emin olun
- Uygulamayı yönetici olarak çalıştırın

### Tuşlar Çok Hızlı/Yavaş
- Gecikme sürelerini ayarlayın (30-100ms arası önerilir)
- Her oyunun farklı tepki süresi olabilir

### Uygulama Açılmıyor
- .NET 8.0 Runtime yüklü olduğundan emin olun
- `dotnet --version` komutuyla kontrol edin

---

## 📞 Destek

Sorularınız için GitHub Issues kullanabilirsiniz.

---

## 📄 Lisans

Bu proje kişisel kullanım için geliştirilmiştir.
