# AOS - Otonom Mobil Oyun Üretim Fabrikası Mimari Dosyası

## 1. Pazar Gerçekleri ve Finansal Arbitraj

Günümüz mobil oyun pazarı indirme sayıları üzerinden değil, mevcut oyuncu tabanından elde edilen Ömür Boyu Değer (LTV) optimizasyonu üzerinden büyümektedir. Sürdürülebilir bir karlılık yakalamanın yegane formülü, bir kullanıcıdan kazanılan paranın kullanıcıyı edinme maliyetinden yüksek olması ($LTV > CPI$) şartına bağlıdır. Solo bir geliştiricinin kısıtlı zaman diliminde bu rekabette ayakta kalmasının tek yolu, "hızlı prototipleme ve erken pazarlanabilirlik testi" odaklı otonom bir üretim bandı kurmaktır.

## 2. 9 Makineli Üretim Hattı Şeması

Oyun fabrikası, girdisi pazar verileri, çıktısı ise doğrulanmış ve yayınlanmış mobil oyunlar olan otonom bir banttır:

| Makine Kimliği | Makine Adı | Birincil İşlevi | Girdi Parametresi | Çıktı Parametresi | Ardıl Makine |
| --- | --- | --- | --- | --- | --- |
| **Machine 0** | Şasi Platformu | Yeniden kullanılabilir altyapı şablonunu hazır tutar. | Unity 6 Engine, SDK Gereksinimleri | Temel Altyapı Şablonu (Prefab'lar) | Machine 5 |
| **Machine 1** | Pazar Radarı | Trend mekanikleri ve yüksek bütçeli reklam kreatiflerini tarar. | Top Charts, TikTok/Meta Ad Libraries. | Haftalık Trend ve Boşluk Analiz Raporu | Machine 2 |
| **Machine 2** | Seçim Kapısı | Aday fikirleri belirlenmiş filtrelerle skorlar ve eler. | Radar Adayları, Üretim Maliyet Verileri | Yeşil Işıklı Prototip Konsepti | Machine 3 |
| **Machine 3** | Mekanik/GDD | Mekanik kuralları ve seviye tasarım matematiğini yazar. | Seçilen Konsept, Psikolojik Hedefler | 3 Sayfalık Yapılandırılmış GDD (JSON) | Machine 4 & 5 |
| **Machine 4** | Görsel/UI | Ekran tasarımlarını ve arayüz ağacını oluşturur. | GDD, Stil Teması, Asset Kütüphaneleri | Design Spec JSON, Mockup Dosyaları | Machine 5 |
| **Machine 5** | Kod (Claude) | Şasi üzerine sadece yeni mekanik kodunu yazar. | GDD, Design Spec, Şasi Kod Altyapısı | Oynanabilir APK/Test Sürümü | Machine 6 |
| **Machine 6** | Pazarlanabilirlik | Kod yazılmadan önce veya sonra reklam performansını ölçer. | Prototip Gameplay Videosu, Yaratıcı Reklamlar | CTR, IPM ve Erken CPI Verileri. | Machine 7 |
| **Machine 7** | Veri & İnfaz | Test sonuçlarına göre projenin devamlılığına karar verir. | CPI Test Raporu, Kohort Retention Verileri. | Devam (Scale) / İyileştirme / İnfaz (Kill) | Machine 8 |
| **Machine 8** | ASO & Yayın | Mağaza optimizasyonunu ve yerelleştirmeyi tamamlar. | Onaylanmış Oyun Verisi, TR/EN Metin Girişleri | Mağaza Yayını, Yayınlanmış Store Sayfası | Pazar Sonu |

## 3. "Satisfying" ve Puzzle Oyunlarının Psikolojik Dinamikleri

Makinelerin tasarlayacağı oyunlarda uygulanacak temel psikolojik kaldıraçlar şunlardır:

- **Sıfır Bilişsel Yük:** Portre modunda, tek parmakla oynanan ve anında geri bildirim veren mekanikler.
- **Değişken Ödül ve Deformasyon:** Kinetik kum kesme, vida sökme veya renk eşleştirme gibi öngörülemeyen fiziksel ve görsel kaskadlar.
- **Zeigarnik Etkisi:** Düzensiz bir sahneyi mükemmel bir harmoniye kavuşturma arzusu (TidyUp, Sort oyunları).
- **Near-Miss (Kıl Payı Kaybetme):** Son hamlede kaybetme hissi uyandırarak rewarded reklam ve can satın alım dönüşümlerini maksimize etmek.

## 4. Akıllı LLM Yönlendirme (Smart Routing) Matrisi

Fabrikanın operasyonel maliyetlerini düşürmek için her makineye en uygun bilişsel kapasitedeki LLM atanır:

- **Machine 1 (Radar):** Gemini 2.5 Pro / 3.1 Pro (Geniş bağlam penceresi ile binlerce kullanıcı yorumunu taramak için).
- **Machine 2 (Kapı):** DeepSeek R1 (Rasyonel ve katı kurallı akıl yürütme için).
- **Machine 3 (GDD):** Claude Sonnet 4.6 (Hatasız ve yapılandırılmış JSON spec dosyaları için).
- **Machine 4 (Görsel):** GPT-4o / Claude Sonnet 4.6 (Multimodal arayüz ve UI gözlemleri için).
- **Machine 5 (Kod):** Claude Sonnet 4.5 / 4.6 (C# ve Unity ScriptableObject mimarisine üst düzey hakimiyet için).
- **Machine 7 (İnfaz):** DeepSeek R1 (Erken dönem verilerini ve bütçe eşiklerini katı matematiksel kurallarla süzmek için).
