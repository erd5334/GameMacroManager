# ⏱️ Timing Calibration - Kullanım Kılavuzu

## 📋 Genel Bakış

**Timing Calibration** özelliği, tuş basma hızınızı ve tuşlar arası gecikme sürelerinizi ölçerek, combo'larınız için en uygun `HoldDurationMs` ve `DelayAfterMs` değerlerini belirlemenize yardımcı olur.

---

## 🎯 Nasıl Kullanılır?

### **1. Pencereyi Açın**
Ana pencerede sağ üstteki **"⏱️ Timing Calibration"** butonuna tıklayın.

### **2. Testi Başlatın**
- **"▶️ Başlat"** butonuna tıklayın
- Pencere aktif olacak ve tuş basımlarınızı dinlemeye başlayacak

### **3. Tuşlara Basın**
- **Herhangi bir tuşa** (harf, sayı, ok tuşları vb.) **10 kez** basın
- **Normal oyun hızınızda** basın - çok hızlı veya yavaş olmayın
- Sistem her tuş basımını otomatik olarak kaydedecek

### **4. Sonuçları İnceleyin**
Test tamamlandığında sağ panelde şu bilgileri göreceksiniz:

#### **⏱️ Basılı Tutma Süresi (HoldDurationMs)**
- **Ortalama**: Tuşları ortalama ne kadar basılı tuttuğunuz
- **Minimum**: En hızlı basımınız
- **Maximum**: En yavaş basımınız
- **Önerilen**: Combo'larda kullanmanız gereken değer

#### **⏳ Tuşlar Arası Gecikme (DelayAfterMs)**
- **Ortalama**: Tuşlar arasındaki ortalama süre
- **Minimum**: En hızlı geçişiniz
- **Maximum**: En yavaş geçişiniz
- **Önerilen**: Combo'larda kullanmanız gereken değer

### **5. Önerileri Okuyun**
Sistem size özel öneriler sunacak:
- ⚡ Çok hızlı mı basıyorsunuz?
- 🐌 Çok yavaş mı basıyorsunuz?
- ✅ İdeal hızda mısınız?
- 🎮 Farklı combo tipleri için öneriler

### **6. Değerleri Kopyalayın**
**"📋 Değerleri Kopyala"** butonuna tıklayarak sonuçları panoya kopyalayabilirsiniz.

---

## 💡 Önerilen Değerler

### **Combo Tiplerine Göre:**

| Combo Tipi | HoldDurationMs | DelayAfterMs | Kullanım |
|------------|----------------|--------------|----------|
| **Hızlı Combo** | Önerilen - 10 | Önerilen - 20 | Rapid fire, quick links |
| **Normal Combo** | Önerilen | Önerilen | Standart combo'lar |
| **Yavaş Combo** | Önerilen + 10 | Önerilen + 30 | Charged attacks, heavy hits |
| **Charged Attack** | 0 (Manuel) | Önerilen × 3 | Drive Parry, Denjin Charge |

---

## 🎮 Örnek Kullanım

### **Test Sonucu:**
```
Ortalama Hold: 45 ms
Önerilen Hold: 40 ms

Ortalama Delay: 95 ms
Önerilen Delay: 100 ms
```

### **Combo'da Kullanım:**
```json
{
    "Key": "M",
    "ActionType": "Tap",
    "HoldDurationMs": 40,    // Önerilen değer
    "DelayAfterMs": 100      // Önerilen değer
}
```

---

## ⚠️ Önemli Notlar

### **✅ Yapılması Gerekenler:**
1. ✅ Normal oyun hızınızda basın
2. ✅ Aynı ritimde basın
3. ✅ Rahat bir pozisyonda olun
4. ✅ Gerçek oyun senaryosunu düşünün

### **❌ Yapılmaması Gerekenler:**
1. ❌ Çok hızlı veya yavaş basmayın
2. ❌ Düzensiz ritimde basmayın
3. ❌ Farklı tuşlar kullanmayın (bir tuşa odaklanın)
4. ❌ Test sırasında dikkatiniz dağılmasın

---

## 🔧 Sorun Giderme

### **"Değerler çok yüksek/düşük çıkıyor"**
- Testi tekrar yapın
- Daha rahat bir pozisyonda deneyin
- Normal oyun hızınızı düşünün

### **"Sonuçlar tutarsız"**
- Aynı ritimde basmaya çalışın
- Daha yavaş ve kontrollü basın
- Birkaç kez test yapıp ortalama alın

### **"Hangi değeri kullanmalıyım?"**
- **Önerilen** değerle başlayın
- Oyunda test edin
- Gerekirse ±10-20ms ayarlayın

---

## 📊 Değerlendirme Kriterleri

### **HoldDurationMs:**
- **< 30ms**: Çok hızlı - Tuşlar düzgün algılanmayabilir
- **30-60ms**: İdeal - Çoğu oyun için uygun
- **> 100ms**: Yavaş - Combo timing'i bozulabilir

### **DelayAfterMs:**
- **< 50ms**: Çok hızlı - Oyun input'ları işleyemeyebilir
- **50-150ms**: İdeal - Çoğu combo için uygun
- **> 200ms**: Yavaş - Combo kesilir

---

## 🎯 İpuçları

1. **Birden fazla test yapın**: İlk test her zaman en doğru olmayabilir
2. **Farklı tuşlar deneyin**: Bazı tuşlar farklı hissedebilir
3. **Oyun içinde test edin**: Gerçek senaryoda deneyin
4. **İnce ayar yapın**: Önerilen değerler başlangıç noktasıdır
5. **Kaydedin**: Sonuçları not alın veya kopyalayın

---

## 📝 Örnek Senaryo

### **Senaryo: Street Fighter 6 Combo**

1. **Test Yap**: Timing Calibration ile ölç
2. **Sonuç Al**: Hold=40ms, Delay=100ms
3. **Combo Oluştur**: Bu değerlerle başla
4. **Oyunda Test Et**: Training mode'da dene
5. **İnce Ayar**: Gerekirse ±10-20ms ayarla
6. **Kaydet**: Final değerleri kaydet

---

## 🚀 Gelişmiş Kullanım

### **Farklı Karakterler İçin:**
- Her karakter için ayrı test yapın
- Farklı combo tipleri için farklı değerler kullanın
- Hızlı karakterler: Daha düşük değerler
- Yavaş karakterler: Daha yüksek değerler

### **Farklı Oyunlar İçin:**
- **SF6**: Orta hız (40-50ms hold, 80-120ms delay)
- **FC26**: Hızlı (20-30ms hold, 50-80ms delay)
- **Tekken**: Yavaş (50-60ms hold, 100-150ms delay)

---

## ❓ SSS

**S: Kaç kez test yapmalıyım?**
C: En az 2-3 kez yapıp ortalama alın.

**S: Hangi tuşa basmalıyım?**
C: Herhangi bir tuş olabilir, önemli olan ritim.

**S: Değerler her zaman aynı mı olmalı?**
C: Hayır, combo tipine göre değişebilir.

**S: Önerilen değer çalışmazsa?**
C: ±10-20ms ayarlayıp tekrar deneyin.

---

## 📞 Destek

Sorunuz veya öneriniz varsa:
- GitHub Issues
- Discord Community
- Email Support

---

**💡 Hatırlatma**: Bu değerler kişiseldir! Herkesin tuş basma hızı farklıdır. Kendi değerlerinizi bulun ve kullanın! 🎮🔥
