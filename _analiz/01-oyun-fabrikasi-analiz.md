# Oyun Üretim Fabrikası — Oturum Analizi ve Strateji Dokümanı

**Tarih:** 10 Temmuz 2026 · **Sürüm:** v1.0 · **Sahip:** Nazım
**Durum:** Onaylı strateji — Sprint 1 (Makine 0 + Makine 1) başlatılabilir

---

## 1. Vizyon (tek cümle)

İnsan onaylı karar kapıları olan, agent (LLM) destekli bir **hyper/hybrid-casual oyun üretim hattı** kurmak; her oyunu bir "ürün denemesi" olarak ele alıp veriyle **öldürmek ya da büyütmek**.

Temel içgörü: Hyper-casual sektörü (Voodoo, Rollic, SayGames, Homa) zaten fabrika modeliyle çalışıyor — fikir → hızlı prototip → CPI testi → öldür/büyüt. Bu projenin farkı, hattaki makinelerin çoğunu agent'lara devretmek ve tek kişiyle stüdyo çıktısı üretmek.

---

## 2. Karar Günlüğü

| Konu | Karar | Gerekçe |
|---|---|---|
| İş modeli | Kendi yayın (self-publish), portföy/fabrika yaklaşımı | %100 gelir kontrolü; tek oyuna bağımlılık yok; kill disiplini uygulanabilir |
| Üretim Hattı 1 kategorisi | "Düşünmeden, parmak hareketiyle" oynanan **satisfying hyper/hybrid-casual** (sand sort, screw puzzle, goods sorting, slice/ASMR tarzı mekanikler) | Kısa geliştirme süresi, algoritmik içerik, rewarded-ad dostu, tek kişilik ekiple üretilebilir |
| Hedef DIŞI | Match-3 (Candy Crush/Royal Match tarzı) ile doğrudan rekabet | Milyar dolarlık UA savaşı, binlerce elle tasarlanmış seviye, LiveOps ekip işi — fabrika ürünü değil mega-ürün |
| Reddedilen niş | Kelime/bulmaca oyunları | Kullanıcı tercihi |
| Zaman bütçesi | Haftada 10–20 saat | Birincil kural: scope disiplini. MVP/prototip başına ≤ 2 hafta geliştirme sınırı |
| Teknoloji | Unity / C# + yeniden kullanılabilir şasi mimarisi | Parmak fiziği, juice ve reklam SDK ekosistemi Unity'de; React Native bu hat için uygun değil |
| Otomasyon ilkesi | **Önce üret, sonra otomatikleştir** | Sekiz makineyi ürün çıkarmadan inşa etmek klasik mühendis tuzağı; darboğazı üretim gösterir |

---

## 3. Pazar Gerçekleri (özet)

- Mobil oyun işi bir "oyun yapma" işi değil, **kullanıcı edinme (UA) işi**dir. Tek denklem: **LTV > CPI**. Organik keşif fiilen ölü.
- Türkiye ekosistem avantajı: Peak (1,8 milyar $ exit), Dream Games (Royal Match), Rollic, Spyke, Ace Games. Bilgi, topluluk ve örnek bolluğu.
- Bu bir **sayı oyunu**: hit öncesi 10–20 prototip normaldir. %80–90 kill oranı sağlıksızlık değil, sağlık göstergesidir.
- Test hiyerarşisi: **oyunu değil, önce reklamı test et.** Kod yazmadan (sahte gameplay videosuyla bile) CPI ölçülebilir; fikri kod yazmadan öldürmek fabrikanın en kârlı işlemidir.

---

## 4. Psikoloji Spesifikasyonu (kalite standardı)

Bu oyunlar "aptal" değil, hassas mühendisliktir. Her prototip bu tabloya karşı denetlenir:

| İlke | Uygulama | Ölçüldüğü yer |
|---|---|---|
| Değişken ödül (variable ratio) | Kaskadlar, rastgele bonuslar, sürpriz ödüller — kumar makinesi matematiği | Oturum sayısı/gün, retry oranı |
| 3 saniye kuralı | Mekanik hem oyunda hem **reklam kreatifinde** ilk 3 saniyede anlaşılır | Reklam CTR, CPI |
| Sıfır bilişsel yük | Tek parmak, portre mod, okuma gerektirmez; oyun kendini oynatarak öğretir | Tutorial tamamlama oranı |
| Juice üründür | Her etkileşimde partikül + ses + haptik titreşim; "satisfying" his oyunun süsü değil, ta kendisi | Playtime |
| Near-miss | Son hamlede kaybetme tasarımı — kazanmaktan çok "bir daha" oynatır | Retry oranı |
| Zeigarnik etkisi | Yarım kalan şey kaşınır: ilerleme çubukları, haritalar, streak'ler | D1 dönüş |
| Oturum ritmi | 2–5 dakikalık bölümler (kuyruk/metro/mola anları) | Ortalama oturum süresi |

---

## 5. Fabrika Mimarisi — Makine Kataloğu

Her makine: **Girdi → Süreç → Çıktı → İnsan kapısı**

| # | Makine | Girdi | Çıktı | İnsan kapısı |
|---|---|---|---|---|
| 0 | **Şasi** (tek seferlik) | — | Yeniden kullanılabilir Unity template: reklam mediation, analytics, IAP, juice toolkit, level loader, mekanik soketi | Mimari onay |
| 1 | **Pazar Radarı** | Top chart'lar, AppMagic, Meta Ad Library, TikTok Creative Center | Haftalık "yükselen mekanikler" raporu, 10 aday kartı | — |
| 2 | **Seçim Kapısı** | Radar aday kartları | Skorlanmış 1–2 yeşil ışıklı konsept | ✔ Yeşil ışık onayı |
| 3 | **Mekanik/GDD** | Onaylı konsept | 2–3 sayfalık tasarım dokümanı: core loop, kontrol, zorluk eğrisi, seviye üretim algoritması | GDD onayı |
| 4 | **Görsel/Ekran** | GDD | Stil rehberi, ekran akışı, asset listesi, juice checklist | Stil onayı |
| 5 | **Kod** (Claude Code) | GDD + Şasi | Oynanabilir prototip (yalnızca mekanik modülü yazılır) | Cihazda **bizzat oynama** — game feel devredilemez |
| 6 | **Pazarlanabilirlik Testi** | Prototip veya sahte gameplay videosu | 3–5 reklam kreatifi + $100–200'lık TikTok/Meta testi → CPI, CTR | ✔ Bütçe onayı |
| 7 | **Veri & İnfaz** | CPI testi + soft launch verisi | ÖLDÜR / DÜZELT / BÜYÜT kararı, gerekçeli | ✔ Nihai karar |
| 8 | **ASO & Yayın** | Büyüt kararı | Store metni, keyword seti, ekran görüntüleri, lokalizasyon | Yayın onayı |

**Fabrika müdürünün (insanın) devredilemez üç görevi:** yeşil ışık onayları, para harcama kararları ve *game feel* — juice'un tuttuğu ancak telefonda parmakla hissedilir.

---

## 6. Metrik Eşikleri (İnfaz Kuralları)

| Metrik | Eşik | Kaynak/aşama |
|---|---|---|
| CPI (test) | < $0.40 hedef; < $0.30 mükemmel | Makine 6, TikTok/Meta testi |
| Reklam CTR | > %1.5–2 | Makine 6 |
| D1 retention | > %45 (hyper-casual referansı) | Soft launch, Firebase |
| D7 retention | > %15–20 | Soft launch |
| İlk gün toplam playtime | > 15–20 dk (hybrid hedeflerde daha yüksek) | Soft launch |
| Karar mantığı | Tüm metrikler yeşil → **BÜYÜT** · tek zayıf metrik → **DÜZELT** (1 iterasyon hakkı) · çoklu kırmızı → **ÖLDÜR** | Makine 7 |

Kural: Karar eşikleri **koda/tabloya yazılır**, tartışılmaz. Oyuna âşık olmak fabrikaların bir numaralı ölüm nedenidir.

---

## 7. LLM / Model Eşleştirme (Makine → Motor)

**Katmanlama ilkesi:** Pahalı ve güçlü modeller *düşük hacimli, yüksek değerli* kararlara (mimari, seçim, kod); ucuz ve hızlı modeller *yüksek hacimli* taramalara (radar, lokalizasyon). Görsel/video üretimi LLM işi değildir — difüzyon/video modelleri ayrı sütundur.

| Makine | Görev tipi | Birincil öneri | Ucuz/alternatif | Not |
|---|---|---|---|---|
| 0 Şasi | Unity/C# mimari + kod | **Claude Code + Claude Opus 4.8** | Rutin adımlarda Claude Sonnet 4.6 | Mimari kararlar en güçlü modelle; şasi tek seferlik yatırım, kalitesinden kısılmaz |
| 1 Radar | Web tarama + özetleme + yapılandırılmış çıktı | **Claude Sonnet 4.6 (web search açık)** | Günlük hızlı tarama: Claude Haiku 4.5 veya Gemini Flash | Hacim yüksek → maliyet önemli; haftalık derin tarama + günlük hafif tarama ikilisi |
| 2 Seçim Kapısı | Muhakeme / çok kriterli skorlama | **Claude Fable 5 / Opus 4.8** (extended thinking) | GPT-5 sınıfı reasoning modu | Haftada 1 çalışır, fabrikanın en kritik kararı — asla ucuz modele verilmez |
| 3 GDD | Yapılandırılmış tasarım yazımı | Claude Sonnet 4.6 (Proje bağlamıyla) | — | Proje talimatı + analiz dosyası bağlam olarak yüklü olmalı |
| 4 Görsel | Konsept görsel + UI/ikon + 3D asset | Midjourney/Flux (konsept), Recraft (UI/ikon/vektör), Meshy veya Tripo (3D asset) | Gemini görsel düzenleme (Nano Banana sınıfı) hızlı revizyonlar | LLM'in rolü: prompt yazımı, stil rehberi, asset listesi (Claude). Production asset = asset store + AI karışımı |
| 5 Kod | Mekanik modül geliştirme | **Claude Code (Opus 4.8 / Fable 5)** | İterasyon ve küçük düzeltmelerde Sonnet 4.6 | Şasi sayesinde kapsam dar: tek modül + level generator |
| 6 Test Kreatifleri | Video üretim + hook metni | Veo / Sora / Runway sınıfı video modelleri; hook/senaryo metni Claude | CapCut şablonları + gerçek gameplay kaydı (çoğu zaman yeterli ve daha ucuz) | 3 saniye kuralı kreatifin kalbidir; ilk kare mekaniği göstermeli |
| 7 Veri & İnfaz | SQL/analiz + yorum | **Deterministik kod + eşik tablosu**; yorum katmanı Claude Sonnet 4.6 | — | Karar kuralı LLM'e bırakılmaz — kodda yaşar; LLM yalnızca "neden" raporunu yazar |
| 8 ASO | Store metni + çok dilli lokalizasyon | Claude Sonnet 4.6 | Toplu lokalizasyon: Claude Haiku 4.5 | TR + EN minimum; ucuz modelle 10+ dile ölçeklenebilir |

**Dürüstlük notu:** Bu tabloyu bir Anthropic modeli (Claude) yazdı; kod tarafında Claude gerçekten sektör standardı olsa da, kendi iş yükünde rakipleri (GPT-5 sınıfı, Gemini) A/B testinden geçirmen sağlıklıdır. Model isimleri ve fiyatlar hızla eskir — harcama kararından önce güncel sürüm ve fiyatları doğrula: https://docs.claude.com/en/api/overview

---

## 8. Yol Haritası

| Sprint | Hafta | İş | Çıktı |
|---|---|---|---|
| 1 | 1–3 | **Makine 0** şasi kurulumu + **Makine 1** yarı-manuel radar (haftalık 60–90 dk rutin) | Çalışan template + ilk 10 aday kartı |
| 2 | 4–6 | Makine 2 ile ilk seçim → Makine 3 GDD → Makine 5 ile **Prototip #1** | Cihazda oynanabilir prototip |
| 3 | 7–8 | Makine 6 CPI testi ($100–200 bütçe) → Makine 7 kararı | ÖLDÜR/DÜZELT/BÜYÜT + öğrenilenler |
| 4+ | 9+ | Darboğaz neredeyse o makineyi otomatikleştir; hatta yeni prototip sok | Ayda 1–2 prototip ritmi |

---

## 9. Riskler ve Mentor Uyarıları

1. **Fabrika tuzağı:** Ürün çıkarmadan sekiz makineyi inşa etmek. Panzehir: Sprint planına sadakat, "önce üret sonra otomatikleştir".
2. **Oyuna âşık olma:** Kill kararını geciktirmek. Panzehir: eşikler Bölüm 6'da yazılı; Makine 7'nin çıktısı tartışılmaz, en fazla 1 "DÜZELT" hakkı.
3. **Match-3 sürüklenmesi:** "Biraz meta ekleyelim, biraz seviye tasarımı..." derken kapsam patlar. Panzehir: 2 haftalık geliştirme tavanı.
4. **Game feel devri:** Agent'lar juice'u ölçemez. Her prototip cihazda bizzat oynanır.
5. **Platform/SDK riski:** Reklam SDK'ları, store politikaları ve gizlilik kuralları (ATT/GDPR) değişir. Şasi güncel tutulur; consent akışı birinci sınıf vatandaştır.
6. **Zaman bütçesi:** 10–20 saat/hafta gerçeğiyle prototip başına takvim süresi 2–4 haftadır (efor 2 hafta). Takvimle eforu karıştırma.

---

*Bu doküman Claude Projesi'nin bilgi tabanına (Project knowledge) yüklenmelidir — bkz. 04-claude-proje-talimati.md*
