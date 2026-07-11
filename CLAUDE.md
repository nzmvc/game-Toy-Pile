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