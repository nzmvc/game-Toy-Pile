# Makine 1 — Pazar Radarı Kurulum Promptları

**Araç:** Claude (web search açık) · **Model:** Haftalık derin tarama → Claude Sonnet 4.6 · Günlük hızlı tarama → Claude Haiku 4.5
**Çalışma şekli v1 (yarı-manuel):** Haftada 1 kez, 60–90 dakikalık rutin. Otomasyona (v2) ancak hat ürün çıkarmaya başladıktan sonra geçilir.

---

## Kaynak Listesi (radar neyi tarar)

| Kaynak | Ne için | Not |
|---|---|---|
| Google Play / App Store ücretsiz oyun top listeleri (US + TR) | Son 30 günde üst sıralara yeni giren oyunlar | En güçlü "para kazanıyor" sinyali |
| AppMagic (ücretsiz tier) | Yükselen oyunlar, indirme tahminleri | Haftalık trend listesi |
| Meta Ad Library | Publisher'ların agresif reklam verdiği oyunlar | Reklam harcaması = ölçekleme sinyali |
| TikTok Creative Center | En çok izlenen oyun reklamları | Kreatif formatları da öğrenirsin |
| PocketGamer.biz, Deconstructor of Fun, publisher blogları | Trend mekanik analizleri, "ne arıyoruz" sayfaları | Sektörün kendi radarı |

**Takip edilecek publisher'lar:** Voodoo, Rollic, SayGames, Homa, Supersonic, Azur Games, Kwalee, Lion Studios.

---

## Haftalık Tarama Promptu (kopyala–yapıştır)

```
Sen "Oyun Fabrikası"nın Pazar Radarı agent'ısın. Görevin: hyper/hybrid-casual
pazarında yükselen, tek parmakla oynanan, "satisfying" mekanikleri tespit edip
aday kartlarına dönüştürmek. Web search kullan.

TARAMA KAYNAKLARI:
1. ABD ve Türkiye Google Play / App Store ücretsiz oyun listelerine son 30
   günde giren yeni oyunlar
2. AppMagic / Sensor Tower yükselen oyun analizleri ve blog yazıları
3. Şu publisher'ların yeni çıkardığı veya yoğun reklamını yaptığı oyunlar:
   Voodoo, Rollic, SayGames, Homa, Supersonic, Azur Games, Kwalee
4. PocketGamer.biz ve benzeri sektör kaynaklarındaki trend mekanik analizleri

ARADIĞIN PROFİL:
- Tek parmak + portre modda oynanabilir
- Mekanik, videoyu izleyen birine 3 saniyede kendini anlatır
- İçerik algoritmik üretilebilir (elle seviye tasarımı gerektirmez)
- Şasi üzerine tek modülle tahmini geliştirme ≤ 2 hafta

ELEME (hiç listeye alma):
- Match-3 ve meta-ağırlıklı türler
- Multiplayer, hikâye tabanlı, elle yoğun içerik isteyen oyunlar
- Lisanslı IP gerektiren konseptler

ÇIKTI: Tam 10 aday, her biri aşağıdaki aday kartı şablonuyla. Kanıtsız aday
yazma — her kartın "kanit" alanında chart pozisyonu, reklam yoğunluğu gözlemi
veya indirme tahmini gibi somut ve kaynaklı bir sinyal olmalı. Emin olmadığın
alana "doğrulanamadı" yaz; asla uydurma. Sonda tek paragraf: bu haftanın en
güçlü 2 adayı ve nedeni.
```

---

## Aday Kartı Şablonu (Makine 2'ye teslim formatı)

```yaml
id: R-2026-07-11-01
tarih: 2026-07-11
mekanik_adi: ""              # örn. "screw puzzle / vida sökme"
referans_oyunlar: []          # 1-3 örnek oyun
kanit: ""                     # chart pozisyonu / reklam yoğunluğu / indirme tahmini + kaynak
kontrol_semasi: ""            # tap / drag / swipe / hold
icerik_uretimi: ""            # algoritmik / yarı-elle / elle
uc_sn_okunabilirlik: 0        # 1-5
gelistirme_maliyeti: 0        # 1-5 (5 = çok ucuz, ≤1 hafta)
juice_potansiyeli: 0          # 1-5
reklam_izlenebilirlik: 0      # 1-5 (reklamı izlemek keyifli mi?)
rekabet_yogunlugu: ""         # düşük / orta / yüksek
trend_ivmesi: ""              # yükseliyor / plato / düşüşte
twist_fikri: ""               # kanıtlanmış mekaniğe eklenecek 1 fark
notlar: ""
```

---

## Günlük Hızlı Tarama Promptu (opsiyonel — Haiku 4.5)

```
Görev: 10 dakikalık hızlı radar. Son 48 saatte mobil hyper-casual pazarında
dikkat çeken yeni oyun, mekanik veya chart hareketi var mı, web'de tara
(sektör haberleri + chart değişimleri). Çıktı: en fazla 3 madde, her biri tek
cümle + kaynak linki. Kayda değer sinyal yoksa yalnızca "sinyal yok" yaz.
Aday kartı üretme — o haftalık taramanın işi.
```

---

## Makine 2 Önizleme — Skorlama Cetveli

Radar kartları bu cetvelle puanlanır (Seçim Kapısı'nın işi, format uyumu için burada):

| Kriter | Ağırlık | Dönüşüm |
|---|---|---|
| 3 saniye okunabilirlik | ×3 | 1–5 puan |
| Geliştirme maliyeti | ×3 | 1–5 (ucuz = yüksek puan) |
| Juice potansiyeli | ×2 | 1–5 |
| Reklam izlenebilirlik | ×2 | 1–5 |
| Rekabet yoğunluğu | ×2 | düşük=5 · orta=3 · yüksek=1 |
| Trend ivmesi | ×2 | yükseliyor=5 · plato=3 · düşüşte=1 |

**Maksimum: 70 puan.** Yeşil ışık kuralı: **toplam ≥ 50** VE hiçbir kriter 1–2 puan değil (kritik zayıflık vetosu). Haftada en fazla 1–2 aday yeşil ışık alır; sıfır aday da geçerli bir sonuçtur.

---

## İnsan İşi (devredilmez)

1. Yeşil ışık adaylarının referans oyunlarını **indir ve 10'ar dakika oyna** — his, rapordan okunmaz.
2. TikTok Creative Center'da o oyunların reklamlarını **izle**: hangi an "izlettiriyor", not al (Makine 6'nın girdisi).
3. Skoru sen onaylamadan hiçbir aday Makine 3'e (GDD) geçmez.

---

## v2 Otomasyon Notları (şimdilik yapma)

- Haftalık taramayı cron/n8n + Claude API ile zamanlanmış işe çevir; kartları bir sheet/Notion veritabanına yaz.
- Veri çekerken resmî API'leri tercih et; sitelerin kullanım şartlarına aykırı scraping'e girme.
- Otomasyon tetikleyicisi: radar rutini üst üste 3 hafta elle aksarsa VEYA hat ayda 2+ prototip ritmine ulaştıysa.
