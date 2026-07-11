# Makine 0 — Şasi (Unity Template) Kurulum Promptları

**Araç:** Claude Code · **Önerilen model:** Claude Opus 4.8 (mimari adımlar 0.1–0.2), Claude Sonnet 4.6 (rutin adımlar)
**Çalışma şekli:** Promptlar sıralıdır; her adım bitip derlenip test edilmeden sonrakine geçme. Her adım sonunda commit at.

---

## Ön Koşullar (insan işi — agent'a devredilmez)

1. Unity Hub + güncel LTS sürümü kur, **URP + Mobile** şablonuyla boş proje aç (proje adı: `GameFactoryChassis`).
2. Hesaplar: Google Play Console, Apple Developer, Firebase projesi, reklam mediation hesabı (Unity LevelPlay **veya** AppLovin MAX — birini seç, ikisini birden kurma).
3. Git repo oluştur, Unity için `.gitignore` ekle.
4. Claude Code'u repo kökünde başlat ve önce aşağıdaki `CLAUDE.md` dosyasını koy.

---

## CLAUDE.md (repo köküne yapıştır)

```markdown
# GameFactoryChassis — Proje Bağlamı

## Amaç
Hyper/hybrid-casual mobil oyunlar için yeniden kullanılabilir Unity şasisi.
Her yeni oyun = bu şasi + tek bir "mekanik modülü" + görsel kaplama.
Şasinin kendisi oyun DEĞİLDİR; oyundan bağımsız her şeyi (reklam, analytics,
IAP, kayıt, UI, juice) barındırır.

## Teknik Kurallar
- Unity LTS, URP, yalnızca portre mod, tek parmak input varsayımı.
- Klasör yapısı: Assets/_Chassis (dokunulmaz çekirdek), Assets/_Game (aktif
  mekanik modülü), Assets/_Content (level data, ScriptableObject'ler).
- Her ana sistem kendi assembly definition'ında (Chassis.Core, Chassis.Ads,
  Chassis.Analytics, Chassis.IAP, Chassis.Juice, Chassis.UI).
- God object yasak. Sistemler arası iletişim merkezi EventBus üzerinden.
- Üçüncü parti SDK'lar her zaman bir interface arkasına sarılır
  (IAdsProvider, IAnalyticsProvider) — SDK değişimi tek dosya işi olmalı.
- Tüm dengeleme değerleri (interstitial sıklığı, zorluk eğrisi katsayıları)
  RemoteConfig'ten okunur, koda gömülmez.
- Analytics event isimleri snake_case ve EventSchema.md'de kayıtlı olmayan
  event gönderilemez.

## Tanımlar
- IGameMechanic: her oyunun uyguladığı yaşam döngüsü sözleşmesi
  (Initialize, StartLevel, Tick, EndLevel).
- JuicePreset: partikül + ses + haptik üçlüsünü tek varlıkta toplayan
  ScriptableObject.

## Yasaklar
- Multiplayer, sunucu tarafı, meta katman, turnuva sistemi (v2 işi).
- Banner reklam varsayılan kapalı.
- Editor dışında çalışmayan kod bırakma; her adım cihaz build'iyle uyumlu olmalı.
```

---

## Prompt 0.1 — Mimari İskelet

```
Bu Unity projesinde CLAUDE.md'deki kurallara göre şasinin mimari iskeletini kur:

1. Klasör yapısını ve assembly definition'ları oluştur (Chassis.Core, Chassis.Ads,
   Chassis.Analytics, Chassis.IAP, Chassis.Juice, Chassis.UI + Game.Mechanic).
2. Chassis.Core içinde: hafif bir ServiceLocator, merkezi EventBus
   (tip güvenli, struct event'ler), GameManager (durum makinesi:
   Boot → MainMenu → Playing → LevelEnd) ve uygulama yaşam döngüsü yönetimi.
3. IGameMechanic interface'ini tanımla: Initialize(LevelData),
   StartLevel(), Tick(float dt), EndLevel(LevelResult) + mekanik→şasi
   event'leri (LevelCompleted, LevelFailed, ProgressChanged).
4. LevelData ve LevelResult veri tiplerini tanımla (ScriptableObject +
   JSON serileştirilebilir).
5. Boot sahnesi + sahne yükleme akışı kur.

Kod yazmadan önce bana kısa bir mimari plan sun ve onayımı al.
Bittiğinde: derlenen proje + mimariyi anlatan docs/ARCHITECTURE.md.
```

## Prompt 0.2 — Analytics Katmanı

```
Analytics katmanını kur:

1. IAnalyticsProvider interface'i + FirebaseAnalyticsProvider implementasyonu
   (Firebase SDK entegrasyon adımlarını bana talimat olarak ver, kurulum sonrası
   kodu bağla) + geliştirme için ConsoleAnalyticsProvider.
2. docs/EventSchema.md oluştur ve şu standart şemayı yaz; kodda bu şemayı tip
   güvenli sabitlere dönüştür:
   - session_start, session_end {duration}
   - level_start {level_id, mechanic_id, attempt_no}
   - level_complete {level_id, duration, moves}
   - level_fail {level_id, duration, fail_reason}
   - ad_offer {placement}, ad_shown {placement, type}, ad_reward {placement}
   - iap_initiated {sku}, iap_purchase {sku, price_usd}
   - rate_us_shown, rate_us_accepted
3. Şemada olmayan event gönderimini derleme hatasıyla engelle.
4. Firebase DebugView ile doğrulama adımlarını docs/QA.md'ye yaz.
```

## Prompt 0.3 — Reklam Katmanı

```
Reklam katmanını kur (mediation: [LevelPlay/MAX — seçtiğini yaz]):

1. IAdsProvider interface'i + seçilen mediation implementasyonu + test/dummy provider.
2. AdsManager: interstitial gösterimi frequency capping ile (minimum saniye
   aralığı + minimum level aralığı — ikisi de RemoteConfig'ten), rewarded
   placement registry (string placement id ile kayıt/talep), reklam yüklenme
   durumu event'leri.
3. remove_ads satın alınmışsa interstitial'lar tamamen kapanır, rewarded açık kalır.
4. ATT (iOS) ve GDPR/UMP consent akışını boot sırasına ekle — consent alınmadan
   reklam init edilmez.
5. Her reklam olayı EventSchema'daki event'leri tetikler.
SDK kurulum adımlarını bana insan-talimatı olarak listele; kodu kurulumdan sonra bağla.
```

## Prompt 0.4 — IAP + Kayıt Sistemi

```
1. Unity IAP entegrasyonu: remove_ads (non-consumable) + coin_pack_s/m/l
   (consumable) şablon SKU'ları, restore purchases (iOS).
2. SaveSystem: JSON tabanlı, versiyonlanabilir, atomik yazma; PlayerPrefs
   yalnızca hafif ayarlar için. Kaydedilenler: ilerleme, coin, remove_ads
   durumu, ayarlar.
3. CurrencyService: coin kazanma/harcama tek noktadan, her hareket analytics'e düşer.
4. docs/QA.md'ye IAP sandbox test adımlarını ekle.
```

## Prompt 0.5 — Juice Toolkit

```
Chassis.Juice altında:

1. JuicePreset ScriptableObject: partikül prefab + SFX klip + haptik şiddeti
   tek varlıkta; JuicePlayer.Play(preset, position) API'si.
2. Object pool'lu partikül sistemi (Instantiate/Destroy yasak).
3. Haptik wrapper (iOS/Android, ayarlardan kapatılabilir).
4. SFX manager: ses havuzu, pitch varyasyonu (±%5), müzik ducking.
5. Hafif tween yardımcıları: scale-punch, shake, move/fade (DOTween varsa onu
   sar, yoksa minimal kendi implementasyonun).
6. Kamera shake servisi.
7. Demo sahnesi: bir küpe dokununca preset'in tüm etkileri çalışsın.
```

## Prompt 0.6 — Level Loader + Mekanik Soketi

```
1. LevelLoader: Assets/_Content'ten LevelData yükler, IGameMechanic'e verir,
   sonuçları GameManager'a raporlar.
2. DifficultyService: level numarasına göre zorluk katsayıları üretir; eğri
   parametreleri RemoteConfig'ten. Kural: her 4-5 seviyede bir "nefes"
   (kolay seviye) — near-miss ve akış için.
3. Örnek mekanik modülü "TapToClear": ekranda beliren şekillere dokununca
   juice preset'iyle patlarlar, hepsi bitince level_complete. Amaç oyun değil,
   soketin uçtan uca çalıştığını kanıtlamak.
4. 20 adet TapToClear seviyesi üreten basit bir editor aracı yaz
   (level generator'ların şablonu olacak).
```

## Prompt 0.7 — UI Çerçevesi

```
1. Screen-stack tabanlı UI: MainMenu, Gameplay HUD, Win, Fail, Settings,
   Shop ekranları; safe-area uyumlu, portre.
2. Win ekranı akışı: ödül → (koşullar uygunsa) interstitial → sonraki seviye.
   Fail ekranı: rewarded ile devam et teklifi + retry.
3. Rate-us mantığı: X başarılı seviye sonrası, oturum başına en fazla 1 kez,
   native in-app review API'siyle.
4. Settings: ses/haptik/müzik toggle, restore purchases, gizlilik politikası linki.
5. Tüm UI event'leri EventSchema'ya bağlı.
```

## Prompt 0.8 — Build Pipeline + QA

```
1. Android (AAB, IL2CPP, ARM64) ve iOS build ayarlarını yapılandır;
   sürümleme şeması kur.
2. docs/RELEASE_CHECKLIST.md üret: consent akışı, reklam test modu kapalı mı,
   analytics DebugView doğrulaması, IAP sandbox, cihazda 10 dk oynanış testi,
   store metadata kontrolü.
3. Şasiden yeni oyun türetme rehberi yaz: docs/NEW_GAME_FROM_CHASSIS.md
   (hedef: yeni mekanik = _Game klasörü + 1 mekanik modülü + level generator,
   şasiye dokunmadan).
```

---

## Kabul Kriterleri — Şasi "Bitti" Tanımı

- [ ] Dummy mekanikle uçtan uca akış: aç → seviye oyna → win → interstitial → sonraki seviye
- [ ] Rewarded video "devam et" akışı çalışıyor
- [ ] Tüm event'ler Firebase DebugView'da görünüyor
- [ ] remove_ads satın alımı interstitial'ları kapatıyor, restore çalışıyor
- [ ] Consent (ATT/GDPR) akışı boot'ta doğru sırada
- [ ] Juice demo sahnesi cihazda "hissettiriyor" (senin onayın)
- [ ] Yeni mekanik eklemek = yalnızca `_Game` klasörü + level data; şasi koduna dokunulmuyor
- [ ] Gerçek Android cihazda temiz kurulumdan 10 dk sorunsuz oynanış

> Mentor notu: 0.1 ve 0.2'de acele etme — şasinin ilk iki adımı fabrikanın temelidir. Sonraki her oyun bu temelin faizini öder.
