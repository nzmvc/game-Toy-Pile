Oyun: "Toy Pile" (çalışma adı) — GDD-lite
Amaç: 3D fizik kutusuna yığılmış objelerden aynı türden 3'ünü bulup dokunarak toplamak, kutuyu tamamen boşaltmak.
Core loop (2–4 dk/seviye): Bak → aynı olan 3'ü bul → tap (obje bara uçar) → 3'lü tamamlanınca pop (partikül+ses+haptik) → pop şok dalgası yığını hafif sarsar, altta gizli objeler yüzeye çıkar (sürpriz = değişken ödül) → kombo → kutu boşalır → win.
Twist (diferansiyasyon): "Şok dalgası kaskadı" — her patlama fiziksel impulse ile yığını yeniden düzenler. Hem satisfying, hem reklam kreatifinin ilk 3 saniyesinde görünür bir fark.
Kontrol: Yalnızca tap. Portre. Sıfır metin.
Fail / near-miss: Toplama barı 7 slot; bar 7/7 dolar ve eşleşme yoksa fail. Bar 6/7'deyken UI gerilim verir (titreme + ses) — near-miss tasarımı doğal olarak mekaniğin içinde.
Zorluk eğrisi kuralları: Parametreler: toplam obje N (12→60+), tür sayısı T (4→12), istif derinliği, "tuzak çifti" oranı. Her 4–5 seviyede bir nefes (kolay) seviyesi. Süre baskısı yok (zen); timer v2 işi.
Seviye üretim algoritması: seed(level_id) → T tür seç → her türden 3k obje → fizik kutusuna deterministik düşür → greedy solver botu çözemezse seviyeyi reddet ve yeniden üret. Tamamen algoritmik, elle seviye tasarımı sıfır.
Zeigarnik: Level sayacı + günlük streak alevi (v1'de bu kadar; harita v2).
Monetizasyon yerleşimleri (v1'de hepsi dummy/stub): Rewarded: +1 slot (fail kurtarma), shuffle, win'de 2x ödül. Interstitial: level sonu, frequency cap config'ten. IAP v2.
Ekranlar, aksiyonlar, araçlar (Makine 4 lite)
Ekran akışı: Boot → MainMenu (Play, streak, ayarlar) → Gameplay (fizik kutusu + 7'li bar + level no + 2 booster butonu) → Win (coin count-up, 2x rewarded, Next) → Fail (rewarded ile devam / Retry) → Settings (ses/haptik, gizlilik linki).
Asset listesi: 10–12 low-poly obje (meyve/oyuncak seti — Asset Store'dan hazır paket al, eksikleri Meshy/Tripo ile üret), bar UI, pastel gradyan arka plan, 2 partikül (pop, konfeti), 6–8 SFX (tap, uçuş, pop, kombo, win, fail), haptik preset'ler. Stil: pastel + yüksek kontrast objeler; okunabilirlik > gerçekçilik.
Araçlar: Ekran akışı için Figma'da tek sayfa yeterli (aşırı tasarım yapma, bu hyper-casual); UI Unity uGUI ile; ikon/store görselleri sonra — CPI testinden önce store asset'ine zaman harcamak scope israfı.
Juice checklist: tap'te squash-punch · bara uçuşta trail · pop'ta partikül+ses+haptik · kombo zincirinde SFX pitch kademeli artar · şok dalgasında mikro kamera shake · win'de konfeti · near-miss'te bar titremesi.