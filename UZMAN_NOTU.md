# İÜC İzin Takip — Yazılım Uzmanı Devir Notu

**Hazırlanma tarihi:** 4 Temmuz 2026 (son güncelleme: 21 Temmuz 2026)  
**Devir eden:** Levent (levent@irm.net.tr)  
**Amaç:** Projenin mevcut durumunu, teknik altyapısını ve yapılacakları aktarmak.

---

## 1. Projeye Genel Bakış

İstanbul Üniversitesi-Cerrahpaşa Proje Geliştirme Yönetimi birimi çalışanlarının izin taleplerini mobil üzerinden yapmasını ve yöneticilerin bu talepleri Google Sheets üzerinden yönetmesini sağlayan bir uygulamadır.

---

## 2. Teknoloji Yığını

| Katman | Teknoloji |
|---|---|
| Mobil uygulama | .NET MAUI 9 (Android hedef) |
| MVVM | CommunityToolkit.Mvvm 8.4.2 |
| UI araçları | CommunityToolkit.Maui 7.0.1 |
| C# UI Markup | CommunityToolkit.Maui.Markup 4.0.0 |
| Backend | Google Apps Script (Web App olarak deploy edilmiş) |
| Veritabanı | Google Sheets |
| İletişim | HTTP POST → JSON (Google Apps Script redirect akışı) |

**Minimum Android sürümü:** API 21 (Android 5.0)

---

## 3. Mimari

Proje standart MVVM yapısındadır:

```
Views/          → Saf C# sayfaları (XAML yok, CommunityToolkit.Maui.Markup ile)
ViewModels/     → İş mantığı, CommunityToolkit.Mvvm [ObservableProperty] / [RelayCommand]
Models/         → LeaveRequest, User, ApiResponse
Services/       → ApiService (HTTP), AuthService (session yönetimi)
Converters/     → InvertedBoolConverter, StatusMessageColorConverter, StatusColorConverter, StatusBgConverter,
                  IsNotNullOrEmptyConverter, DateDisplayConverter, PastLeaveOpacityConverter
AppColors.cs    → Tüm tema renkleri statik C# sınıfı olarak tanımlı
Resources/      → Renkler (Colors.xaml), stiller (Styles.xaml), fontlar, görseller
```

**UI Yaklaşımı:** Tüm View sayfaları XAML yerine saf C# ile yazılmıştır.
`CommunityToolkit.Maui.Markup` paketi `.Bind()`, `.Row()`, `.Column()` gibi fluent extension metodları sağlar.
AppShell ve App altyapı dosyaları XAML olarak kalmaktadır (navigasyon tanımları).

---

## 4. Kullanıcı Akışı (7 Ekran)

1. **EmailVerificationPage** — Kullanıcı iş e-postasını girer; sistem Sheet'te arar, 6 haneli kod gönderir
2. **LoginPage (OTP)** — Doğrulanmış kullanıcı giriş için OTP ister
3. **LeaveRequestPage** — İzin formu (tür, tarih aralığı, açıklama)
4. **Tarih seçimi** — LeaveRequestPage içinde DatePicker entegrasyonu
5. **LeaveSummaryPage** — Özet ekranı, kullanıcı onaylar
6. **SuccessPage** — Başarı bildirimi
7. **LeaveListPage** — Kullanıcının geçmiş/aktif izin talepleri

---

## 5. Backend — Google Apps Script

**Canlı endpoint URL (güncel, 20 Temmuz 2026'dan itibaren):**
```
https://script.google.com/macros/s/AKfycby0JE-uoihEsZvm_01fCXWohuQEmn5fzmdAVZg93_bSbiS3-HMQ3CMNArF6ovJCrk1c/exec
```
Bu URL, uygulama kodunda [Services/ApiService.cs](Services/ApiService.cs) içindeki `BaseUrl` sabitinde tanımlıdır.

**Deploy hesabı (güncel):** pgyizin@iuc.edu.tr — e-postaların bu adresten gönderilmesi istendiği için script **tamamen yeni bir Apps Script projesi** olarak bu hesapta oluşturuldu (eski koddan kopyalanarak). Eski proje/deployment silinmedi ama artık kullanılmıyor:

> **Eski (yedek, kullanımda değil) endpoint:** `https://script.google.com/macros/s/AKfycbyYt5_sQOitUZxofPDbwVhBJ5mf7k2Oe0oxs4DcdOck26vXDHz_pslZYLwbT1SGwAmy/exec` (eski hesap: levent@iuc.edu.tr)

**Önemli:** Bundan sonra `Code.gs` üzerinde yapılacak TÜM değişiklikler pgyizin@iuc.edu.tr hesabındaki YENİ Apps Script projesinde yapılmalı ve oradan deploy edilmelidir. Bu repodaki `GoogleAppsScript/Code.gs` dosyası sadece yerel bir referans/yedek kopyadır, canlı script'i otomatik güncellemez — her değişiklik iki tarafa da (yerel dosya + Apps Script editörü) elle yansıtılmalıdır.

Yeni hesabın Google Sheet'e (aşağıdaki ID) **Düzenleyen** erişimi verilmiştir; e-posta gönderimi ve Sheet yazımı 20 Temmuz 2026'da canlı ortamda test edilip doğrulanmıştır (OTP e-postası pgyizin@iuc.edu.tr'den geldi, Sheet'e satır yazıldı).

**Google Sheet ID:** `1vy5P2Gt8a9uQ7Exi81_g78HnpJUzWl5VsHf53OnUJdc`  
**Sheet adı:** `IzinTakip_DB`

**Sayfalar:**
- `Kullanicilar` — E-posta, Ad Soyad, Rol, Doğrulandı, DoğrulamaKodu, Expiry, OTP, OTPExpiry
- `IzinTalepleri` — ID, Email, AdSoyad, Birim, IzinSuresi, BaslangicTarihi, BitisTarihi, IseDonus, Aciklama, OlusturmaTarihi, Durum

**Desteklenen aksiyonlar (action parametresi):**
| Aksiyon | Açıklama |
|---|---|
| `verifyEmail` | E-posta doğrulama kodu gönderir |
| `confirmCode` | Doğrulama kodunu onaylar |
| `loginOTP` | Giriş OTP'si gönderir |
| `confirmOTP` | OTP'yi doğrular, kullanıcı bilgilerini döner |
| `submitLeave` | İzin talebini Sheet'e yazar |
| `getLeaves` | Kullanıcının taleplerini döner |

**Kurulum fonksiyonu:** `setupDurumDropdown()` — Apps Script editöründen elle bir kez çalıştırılır. `IzinTalepleri` sayfasındaki `Durum` sütununu (K, 1000 satıra kadar) "Beklemede / Onaylandı / Reddedildi" seçenekli açılır listeye (data validation) çevirir.

**Önemli not:** Google Apps Script, POST isteklerine 302 redirect ile yanıt verir. `ApiService.cs` bunu özel olarak yönetir (önce redirect'i yakalar, sonra GET ile takip eder). Bu davranış değiştirilemez; Google'ın altyapı kısıtıdır.

**Deploy hatırlatması:** Code.gs içeriği değiştirildiğinde Apps Script editöründe kaydetmek yetmez — `Dağıt > Dağıtımları yönet > (kalem ikonu) > Sürüm: Yeni sürüm > Dağıt` adımıyla yeni bir sürüm yayınlanmalıdır, aksi halde canlı URL eski koda göre çalışmaya devam eder. Web App URL'i bu işlemde değişmez.

---

## 6. Build Ortamı

**Android SDK dizini:** `C:\Users\levent02\Android`  
**JDK gerçek yolu:** `C:\Users\levent02\Android\jdk-21.0.7+6` (Program Files altında DEĞİL — Android SDK klasörünün içinde)

**Build komutu (Release APK, doğrulanmış):**
```powershell
$env:JAVA_HOME = "C:\Users\levent02\Android\jdk-21.0.7+6"
dotnet publish -f net9.0-android -c Release `
  -p:AndroidSdkDirectory="C:\Users\levent02\Android" `
  -p:JavaSdkDirectory="C:\Users\levent02\Android\jdk-21.0.7+6"
```

**Çıktı:** `bin\Release\net9.0-android\publish\com.companyname.izintakip-Signed.apk`

---

## 7. Tamamlanan İşler

- [x] 7 ekranlı tam akış (e-posta doğrulama → OTP → izin formu → özet → başarı → liste)
- [x] Google Apps Script backend (tüm endpoint'ler çalışıyor)
- [x] Google Sheets entegrasyonu
- [x] Çakışan tarih kontrolü (aynı kullanıcı için çakışan izin engellenir)
- [x] MVVM mimarisi (CommunityToolkit.Mvvm)
- [x] Premium koyu tema (lacivert zemin, altın aksanlar)
- [x] Android APK build alınıyor
- [x] Uygulama başlığı "Proje Geliştirme Yönetimi Birimi İzin Sistemi" olarak güncellendi (LoginPage + EmailVerificationPage, ortalı hizalama)
- [x] Yeni İzin Talebi formundan Birim seçimi kaldırıldı; Birim artık sabit "Proje Geliştirme Yönetimi" olarak otomatik gönderiliyor
- [x] İzin Süresi seçimi 1-50 gün aralığına genişletildi (yarım gün seçenekleri kaldırıldı)
- [x] Tüm tarih gösterimleri (Talep Özeti, İzin Listesi, Başarı ekranı) "gün ay yıl" formatında, Türkiye saat dilimine göre doğru gösteriliyor (`DateDisplayConverter`)
- [x] İzin Listesi ekranına Yeni İzin Talebi'ne dönüş butonu ("+") ve daha net bir çıkış ikonu (🚪) eklendi
- [x] Sheet'teki Durum sütunu açılır listeye (dropdown) çevrildi; uygulama kodu ile Sheet değerleri ("Onaylandı") tutarlı hale getirildi
- [x] İzin talebi oluşturma zaman damgası artık ham UTC ISO yerine Türkiye saatiyle okunabilir formatta ("dd.MM.yyyy HH:mm")
- [x] Uygulama ikonu ve splash ekranı İÜC Cerrahpaşa / PGY kimliğine göre yenilendi (`Resources/AppIcon/appicon.svg`, `Resources/Splash/splash.png`)
- [x] Giriş ve e-posta doğrulama ekranlarında durum mesajları artık başarı/hata durumuna göre yeşil/kırmızı renkleniyor (`StatusMessageColorConverter`, `StatusIsError` özelliği)
- [x] "Kodu tekrar gönder" özelliği eklendi — 60 saniyelik geri sayım sonrası aktif olan bir link (`CanResend`, `ResendCountdownText`, `SendOtpCommand`/`SendVerificationCodeCommand` üzerinde `CanExecute`)
- [x] Ana sekmeler yeniden düzenlendi: **İzin Taleplerim** solda (varsayılan açılış sekmesi), **Yeni İzin Talebi** sağda; ikisi için de özel SVG ikonlar eklendi (`izin_taleplerim.svg`, `yeni_talep.svg`)
- [x] Yeni İzin Talebi ekranındaki geri ok artık oturumu kapatmak yerine İzin Taleplerim listesine dönüyor (`GoToLeaveListCommand`)
- [x] Global `BoxView` stilinde (`Styles.xaml`) `BackgroundColor` yerine yanlışlıkla ayarlanmış özellik `Color` olarak düzeltildi — Yeni İzin Talebi formunda Kaydet butonunun üstünde görünen istenmeyen gri dikdörtgen giderildi
- [x] İzin Listesi ekranındaki "+" butonu, tasarıma uygun şekilde sağ altta sabit duran yuvarlak, gölgeli bir Floating Action Button (FAB) olarak yeniden tasarlandı
- [x] Çıkış ikonu net bir "logout" ikonuna (`cikis_yap.svg`) çevrildi; çıkışa basınca artık "Emin misiniz?" onay diyaloğu (`DisplayAlert`) çıkıyor
- [x] `ApiService.cs`'de sunucudan JSON yerine beklenmeyen bir yanıt (ör. HTML) geldiğinde artık kullanıcıya ham yanıt gösterilmiyor; genel "Sunucu yanıtı işlenemedi..." mesajı gösteriliyor, teknik detay sadece `Debug.WriteLine` ile loglanıyor
- [x] Uygulama görünen adı **"PGY İzin"** olarak değiştirildi (`ApplicationTitle` — `ApplicationId` bilinçli olarak `com.companyname.izintakip` kaldı, değiştirilseydi cihazlarda mevcut kurulumun üzerine güncelleme değil ayrı bir uygulama olarak kurulurdu)
- [x] Backend Google hesabı `pgyizin@iuc.edu.tr`'ye taşındı (bkz. Bölüm 5) — e-postalar artık bu adresten gönderiliyor
- [x] İzin Listesi artık en son eklenen talebi en üstte gösteriyor (client tarafında ters sıralama, `LeaveListViewModel.LoadLeaves`)
- [x] Bitiş tarihi geçmiş olan izin talepleri listede %55 opaklıkla, daha soluk gösteriliyor (`PastLeaveOpacityConverter`)

---

## 8. Yapılacaklar

### 8.1 Kullanıcı Revizeleri (7 Temmuz 2026 oturumu — tamamlandı)
Bu revizelerin tamamı uygulanmış ve build ile doğrulanmıştır:

1. Başlık metni "Proje Geliştirme Yönetimi Birimi İzin Sistemi" olarak değiştirildi, ortalı hizalandı
2. Yeni İzin Talebi formundan Birim Picker'ı kaldırıldı (sabit değerle otomatik dolduruluyor)
3. İzin Süresi seçimi 1-50 gün olarak genişletildi
4. Talep Özeti ekranındaki taşan uyarı metni satır sonu yapacak şekilde düzeltildi
5. Tüm ekranlardaki tarih formatları Türkiye saatine göre "gün ay yıl" olarak düzeltildi
6. İzin Listesi ekranına yeni talep butonu ve daha anlaşılır çıkış ikonu eklendi
7. Google Sheet Durum sütunu dropdown yapıldı + oluşturma zaman damgası formatı düzeltildi (Code.gs, yeniden deploy edilip test edildi)

**Ayrıca bu oturumda düzeltilen pre-existing hata:** `MauiProgram.cs` içinde eksik `using CommunityToolkit.Maui.Markup;` satırı vardı, proje bu oturumdan önce hiç derlenmiyordu.

### 8.1b Kullanıcı Revizeleri (17-21 Temmuz 2026 oturumları — tamamlandı)
Bu revizelerin tamamı uygulanmış ve build ile doğrulanmıştır (detaylar Bölüm 7'de):

1. Uygulama ikonu ve splash ekranı yenilendi (PGY logosu + İÜC Cerrahpaşa arma)
2. Durum mesajları (kod gönderildi / hata vb.) artık yeşil/kırmızı ayrımıyla gösteriliyor
3. "Kodu tekrar gönder" — 60 saniye geri sayımlı, süre dolunca aktifleşen link
4. Ana sekmeler yeniden sıralandı ve yeni özel ikonlar eklendi (İzin Taleplerim / Yeni İzin Talebi)
5. Yeni İzin Talebi ekranındaki geri ok artık listeye dönüyor (önceden yanlışlıkla oturumu kapatıyordu)
6. Global `BoxView` stil hatası düzeltildi (Kaydet butonu üstündeki istenmeyen gri kutu)
7. "+" butonu tasarıma uygun yüzen aksiyon butonuna (FAB) çevrildi
8. Çıkış ikonu netleştirildi, onay diyaloğu eklendi
9. Sunucu hata mesajları sadeleştirildi, ham teknik detay kullanıcıya gösterilmiyor
10. Uygulama adı "PGY İzin" olarak değiştirildi
11. **Backend Google hesabı `pgyizin@iuc.edu.tr`'ye taşındı, yeni Apps Script projesi + yeni URL** (bkz. Bölüm 5 — bundan sonraki Code.gs değişiklikleri bu yeni projede yapılmalı)
12. İzin Listesi en son eklenen talebi üstte gösterecek şekilde sıralandı, geçmiş tarihli talepler soluklaştırıldı

> *(Yeni revizeler geldikçe bu bölüm güncellenecektir)*

### 8.2 Yönetici Modülü
Mevcut uygulamada yönetici modülü yoktur. Şu an talepler Sheet'teki Durum sütunundan (artık dropdown) elle onaylanıp reddediliyor. Kullanıcı mevcut akışı test ettikten sonra yönetici modülüne nasıl geçileceğine karar verecek. Önerilen yön: Apps Script tabanlı basit bir web arayüzü (HTML Service) — backend zaten Apps Script+Sheets olduğu için ek altyapı gerektirmiyor; alternatif olarak MAUI içine ikinci bir "Yönetici" rolü/ekranı eklenebilir (daha çok iş: rol bazlı giriş + yeni endpoint'ler).

Eklenmesi gereken işlevler (karar sonrası):
- Yönetici girişi (rol kontrolü: `Kullanicilar` sheet'inde `Rol` sütunu mevcut, değer: `yonetici`)
- Tüm bekleyen taleplerin listelenmesi
- Talep onaylama / reddetme (Sheet'teki `Durum` sütununu günceller: `Beklemede` → `Onaylandı` / `Reddedildi`)
- Apps Script'e `getAllLeaves` ve `updateLeaveStatus` endpoint'leri eklenmesi gerekir

### 8.3 Font Entegrasyonu
Tasarımda Montserrat + Mulish fontları belirtilmiştir. Şu an OpenSans (varsayılan) kullanılıyor.
- Font dosyalarını `Resources/Fonts/` altına ekle
- `MauiProgram.cs`'de `.ConfigureFonts()` içinde kaydet
- `Resources/Styles/Styles.xaml`'de ilgili stilleri güncelle

### 8.4 Uygulama Kimliği
Şu an `com.companyname.izintakip` olarak ayarlı. Canlıya geçmeden önce `com.iuc.izintakip` gibi kuruma özgü bir kimliğe güncellenmeli (`IzinTakip.csproj`'da `ApplicationId`).

---

## 9. Tasarım Referansı

`design_handoff_izin_sistemi/` klasöründe:
- `designs/` — Etkileşimli tasarım tahtaları (.dc.html dosyaları, tarayıcıda açılır)
- `screenshots/` — 7 ekranın PNG görselleri
- `assets/` — İÜC logo ve arma dosyaları
- `README.md` — Tasarım tokenları ve renk paleti

---

## 10. Bilinen Sınırlamalar

- Google Apps Script ücretsiz katmanda günlük e-posta gönderme limiti vardır (100 e-posta/gün). Kullanıcı sayısı artarsa ücretli Google Workspace veya harici SMTP'ye geçiş değerlendirilmeli.
- Apps Script cold start gecikmesi yaşanabilir (ilk istek 2-3 saniye sürebilir).
- Şu an sadece Android hedeflenmektedir; iOS için Mac ve Apple Developer hesabı gerekir.
