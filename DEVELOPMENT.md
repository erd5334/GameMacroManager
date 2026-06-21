# 🛠️ Game Macro Manager - Geliştirme Günlüğü

Bu dosya, projenin geliştirilme sürecini ve yapılan tüm işlemleri içerir.

---

## 📅 Proje Başlangıcı: 05 Ocak 2026

### 🎯 Amaç
Mevcut Python makro scriptini (macro.py) profesyonel bir C# WPF uygulamasına dönüştürmek.

---

## 📝 Yapılan İşlemler

### Faz 1: Proje Oluşturma

#### 1.1 WPF Projesi Oluşturuldu
```bash
dotnet new wpf -n GameMacroManager -o GameMacroManager --framework net8.0
```

#### 1.2 NuGet Paketleri Eklendi
```bash
dotnet add package InputSimulatorStandard    # Tuş simülasyonu
dotnet add package Newtonsoft.Json           # JSON işlemleri
dotnet add package CommunityToolkit.Mvvm     # MVVM framework
dotnet add package MaterialDesignThemes      # Modern UI
dotnet add package MaterialDesignColors      # Renk paleti
```

#### 1.3 Klasör Yapısı Oluşturuldu
```
Models/
ViewModels/
Views/
Services/
Helpers/
Data/
Assets/
```

---

### Faz 2: Model Sınıfları

#### 2.1 KeyAction.cs
- Tek bir tuş aksiyonunu temsil eder
- Tuş kodu, aksiyon tipi (Press/Release/Tap), gecikme süreleri
- Clone() metodu ile kopyalama desteği

#### 2.2 Combo.cs
- Birden fazla tuş aksiyonundan oluşan kombo
- Hotkey, kategori, tekrarlama ayarları
- Görsel temsil ve tahmini süre hesaplama

#### 2.3 Character.cs
- Oyun karakterini temsil eder
- İkon, renk, kombolar listesi

#### 2.4 Game.cs
- Oyunu temsil eder
- Tema rengi, banner, karakterler listesi

#### 2.5 AppSettings.cs
- Uygulama ayarları
- Son seçilen oyun/karakter, tema, hotkey'ler

---

### Faz 3: Servis Katmanı

#### 3.1 DataService.cs
- JSON tabanlı veri kalıcılığı
- %APPDATA%\GameMacroManager\ konumuna kaydetme
- Örnek veri oluşturma (ilk çalıştırma)
- Import/Export desteği

#### 3.2 MacroService.cs
- InputSimulatorStandard kullanarak tuş simülasyonu
- Async combo çalıştırma
- Başlatma/durdurma kontrolü
- Event sistemi (MacroStarted, MacroStopped)

#### 3.3 HotkeyService.cs
- Windows Low-Level Keyboard Hook
- Global hotkey dinleme (oyun açıkken bile çalışır)
- Karakter bazlı hotkey kayıt sistemi

---

### Faz 4: ViewModel (MVVM)

#### 4.1 MainViewModel.cs
- CommunityToolkit.Mvvm kullanımı
- ObservableProperty'ler: Games, SelectedGame, SelectedCharacter, SelectedCombo
- RelayCommand'lar: AddGame, DeleteGame, AddCharacter, AddCombo, AddKeyAction, vb.
- Otomatik kaydetme
- Status mesajları

---

### Faz 5: Kullanıcı Arayüzü

#### 5.1 App.xaml
- Material Design tema entegrasyonu
- Karanlık tema (Dark mode)
- Özel renkler: AccentPurple, AccentBlue, AccentGreen
- Gradient brush'lar
- DarkCard ve ListItemStyle stilleri

#### 5.2 MainWindow.xaml
- 3 panel düzeni: Oyunlar | Karakterler | Combo Editörü
- Header: Logo, başlık, aktif/pasif toggle
- ListBox'lar: Oyun, karakter, combo listeleri
- Combo editörü: Ad, kategori, hotkey, tuş aksiyonları
- Status bar: Durum mesajı, veri klasörü butonu

#### 5.3 Helpers/Converters.cs
- NullableToVisibilityConverter
- InverseBooleanConverter
- BooleanToVisibilityConverter
- StringNotEmptyToVisibilityConverter

---

### Faz 6: Bug Fix'ler

#### 6.1 ItemsSource Duplicate Hatası (MC3024)
**Sorun:** ComboBox'ta ItemsSource hem attribute hem element olarak tanımlanmış
**Çözüm:** Attribute kaldırıldı, sadece element bırakıldı

---

## 📊 Kullanılan Teknolojiler

| Teknoloji | Versiyon | Kullanım Amacı |
|-----------|----------|----------------|
| .NET | 8.0 | Ana framework |
| WPF | - | UI framework |
| CommunityToolkit.Mvvm | Latest | MVVM pattern |
| MaterialDesignThemes | Latest | Modern UI |
| InputSimulatorStandard | Latest | Tuş simülasyonu |
| Newtonsoft.Json | Latest | JSON serialization |

---

## 📁 Dosya Listesi

```
GameMacroManager/
├── App.xaml                    # Uygulama kaynakları ve stiller
├── App.xaml.cs                 # Uygulama başlangıç
├── MainWindow.xaml             # Ana pencere UI
├── MainWindow.xaml.cs          # Ana pencere code-behind
├── GameMacroManager.csproj     # Proje dosyası
│
├── Models/
│   ├── KeyAction.cs            # Tuş aksiyonu modeli
│   ├── Combo.cs                # Combo modeli
│   ├── Character.cs            # Karakter modeli
│   ├── Game.cs                 # Oyun modeli
│   └── AppSettings.cs          # Ayarlar modeli
│
├── Services/
│   ├── DataService.cs          # Veri kaydetme/yükleme
│   ├── MacroService.cs         # Makro çalıştırma
│   └── HotkeyService.cs        # Hotkey dinleme
│
├── ViewModels/
│   └── MainViewModel.cs        # Ana ViewModel
│
├── Helpers/
│   └── Converters.cs           # Value converters
│
├── README.md                   # Kullanım kılavuzu
└── DEVELOPMENT.md              # Bu dosya
```

---

## 🔮 Gelecek Geliştirmeler

### Öncelikli
- [ ] Tuş kayıt modu (basılan tuşları otomatik kaydet)
- [ ] System tray desteği
- [ ] Ses efektleri

### Planlanan
- [ ] Makro import/export (dosyaya)
- [ ] Profil değiştirme hotkey'i
- [ ] Oyun içi overlay
- [ ] Otomatik güncelleme

### İsteğe Bağlı
- [ ] Çoklu dil desteği
- [ ] Tema özelleştirme
- [ ] Makro kayıt & playback

---

## 🐛 Bilinen Sorunlar

1. **Bazı oyunlarda hotkey çalışmayabilir** - Yönetici olarak çalıştırma gerekebilir
2. **Anti-cheat yazılımları engelleyebilir** - Bu beklenen bir davranış

---

## 📈 Versiyon Geçmişi

| Versiyon | Tarih | Değişiklikler |
|----------|-------|---------------|
| 0.1.0 | 05.01.2026 | İlk çalışan sürüm |

---

## 👨‍💻 Geliştirici Notları

- MVVM pattern kullanıldı
- Async/await ile non-blocking işlemler
- Thread-safe hotkey dinleme
- Modüler servis yapısı

---

*Bu dosya, projenin gelişim sürecini takip etmek için otomatik olarak oluşturulmuştur.*
