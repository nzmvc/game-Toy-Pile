# Kullanıcı ve QA Test Protokolü — "Toy Pile" (v1.0)

Bu doküman, oyunun mobil cihazlarda (Android/iOS) veya Unity Editor'de manuel olarak test edilmesi esnasında takip edilecek **Temiz Kurulum** ve **10 Dakikalık Oynanış Kontrol Listesi**ni (Checklist) içerir.

---

## 📲 1. Temiz Kurulum Kontrolü (Clean Install)
Amacımız, oyun ilk kez yüklendiğinde varsayılan ayarların ve verilerin temiz olarak başladığını doğrulamaktır.

- [ ] **Eski Sürümün Silinmesi:** Cihazda (veya simülatörde) yüklü olan önceki tüm test sürümlerini tamamen kaldırın (Uninstall).
- [ ] **Açılış Durumu (Default State):** Oyunu ilk kez açtığınızda:
  - `PlayerPrefs` verilerinin sıfırlanmış olduğunu,
  - Ana menüdeki **Streak (Seri galibiyet)** sayacının `0` olarak başladığını,
  - Settings (Ayarlar) menüsünde **Sound (Ses)** ve **Haptics (Titreşim)** seçeneklerinin varsayılan olarak **Açık (Enabled)** geldiğini doğrulayın.

---

## 🕹️ 2. 10 Dakikalık Oynanış Kontrol Listesi (Gameplay Checklist)

### A. Performans ve Akıcılık (Performance & FPS)
- [ ] **Kare Hızı (FPS Check):** Oyunun en az 60 FPS akıcılığında çalıştığını doğrulayın.
- [ ] **Fizik Yığını Kasması:** Seviye ilk başladığında yığına dökülen 30+ adet fizik objesinin çarpışma ve dökülme anında hiçbir kasılma (frame-drop) veya takılmaya sebep olmadığını gözlemleyin.
- [ ] **Cihaz Isınması:** 10 dakikalık kesintisiz oynanış sonrasında cihazda aşırı ısınma veya bataryada hızlı tükenme gibi performans sızıntısı belirtileri olmadığını kontrol edin.

### B. Giriş ve Kontrol Hassasiyeti (Input Latency & Flight)
- [ ] **Dokunma Tepki Süresi:** Sahnedeki bir objeye dokunulduğu an (raycast gecikmesi olmadan) objenin anında tepki verdiğini ve uçuşa geçtiğini doğrulayın.
- [ ] **Dokunma Juiciness:** Dokunulan her objede:
  - Hafifçe içe doğru esneyip basılma hissi veren animasyonun (`TweenHelper.PunchScale`),
  - Tıklama sesinin (`Click SFX`),
  - Hafif bir haptik titreşimin (mobil cihazda) tetiklendiğini hissedin.
- [ ] **Uçuş İzi (Flight Trail):** Nesnelerin bar yuvasına uçarken arkalarında kendi renklerinde süzülen bir iz (`TrailRenderer`) bıraktığını ve slota ulaştıkları an bu izin düzgünce söndüğünü gözlemleyin.

### C. Kombo ve Patlama Geri Bildirimleri (Combo Juice)
- [ ] **Pitch Kombo Zinciri:** 2 saniye aralıklarla ardı ardına 3'lü eşleştirmeler yapın. Her patlamada çıkan sesin perdesinin (pitch) kademeli olarak yükseldiğini (`1.0 -> 1.08 -> 1.16 ...`) işitin. 2 saniye bekledikten sonra yapılan ilk patlamada sesin tekrar normal (`1.0`) tonuna sıfırlandığını gözlemleyin.
- [ ] **Şok Dalgası ve Sarsıntı:** Eşleşen 3 nesne patladığı an:
  - Çevreye fiziksel bir itme kuvveti yayıldığını ve diğer objelerin kenara savrulduğunu (`TriggerShockwave`),
  - Kamerada anlık mikro bir sarsıntı (`ShakePosition`) oluştuğunu doğrulayın.

### D. Near-Miss Hissi (Kıl Payı Kaybetme Gerilimi)
- [ ] **Tansiyon Sarsıntısı:** Toplama barına bilerek 6 adet obje yerleştirin (son 1 boş yuva kalana kadar). 6. obje yerleştiği an:
  - Alt taraftaki bar panelinin fiziksel olarak titrediğini (`TweenHelper.ShakePosition`),
  - Gerilim ses efektinin çaldığını,
  - Mobil cihazda uyarıcı bir titreşimin tetiklendiğini gözlemleyin.

### E. Reklam ve Booster Akışları (Dummy Ads Verification)
- [ ] **Shuffle (Karıştırma) Testi:** Oynanış esnasında yığın çok tıkandığında HUD'daki **Shuffle** butonuna basın:
  - Konsolda `[Ads] Showing Rewarded mock-up for placement: shuffle_booster` ve ad_offer/ad_shown event loglarının düştüğünü,
  - Reklam simülasyonu bittiğinde sahadaki tüm objelerin havaya savrulup karışılarak yeniden düştüğünü doğrulayın.
- [ ] **+1 Slot Testi:** HUD'daki **+1 Slot** butonuna basın:
  - Konsolda rewarded ad loglarının çıktığını,
  - Toplama barının kapasitesinin geçici olarak 8'e genişlediğini doğrulayın.
- [ ] **Fail & Devam Et (Fail Continue):** Barı 7/7 doldurarak seviyeyi kaybedin. Fail ekranında "+1 Slot ile Devam Et" butonuna tıklayın:
  - Reklam izlendikten sonra bar kapasitesinin 8'e çıktığını ve oyunun kaldığı yerden devam ettiğini doğrulayın.
- [ ] **Interstitial Sıklık Sınırı (Frequency Cap):** Seviyeyi kazanıp Win ekranına ulaşın.
  - İlk seviyede "Next" dediğinizde geçiş reklamı logunun çıktığını doğrulayın.
  - İkinci seviyeyi 30 saniyeden kısa sürede bitirip "Next" dediğinizde reklam logunun **basılmadığını** (frequency cap engeli),
  - 30 saniyeden fazla bekleyip Next dediğinizde ise geçiş reklamının başarıyla gösterildiğini doğrulayın.
