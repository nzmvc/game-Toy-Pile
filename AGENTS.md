# GameFactoryChassis — Ajan Kuralları

## Proje
Hyper/hybrid-casual mobil oyun şasisi + ilk oyun "Toy Pile" (tap-triple-match,
fizik yığını, şok dalgası twist'i). Unity LTS, URP, yalnızca portre, tek parmak.

## Mimari Kurallar
- Klasörler: Assets/_Chassis (oyundan bağımsız çekirdek), Assets/_Game
  (aktif mekanik modülü), Assets/_Content (level data).
- Assembly definitions: Chassis.Core, Chassis.Juice, Chassis.UI, Game.TileMatch.
- Sistemler arası iletişim tip güvenli merkezi EventBus (struct event). God object yasak.
- Üçüncü parti SDK'lar interface arkasında: IAdsProvider, IAnalyticsProvider.
  v1'de YALNIZCA Dummy/Console implementasyonları; gerçek SDK v2.
- IGameMechanic sözleşmesi: Initialize(LevelData), StartLevel(), Tick(float dt),
  EndLevel(LevelResult).
- Tüm dengeleme değerleri GameConfig ScriptableObject'te; koda sabit gömme.
- Analytics event isimleri snake_case; docs/EventSchema.md'de olmayan event gönderilemez.

## Çalışma Şekli
- Her prompt öncesi kısa plan sun, onayımı bekle. Onaysız geniş refactor yok.
- Bir prompt = bir iş paketi. Sonunda değişen dosyaları listele + commit mesajı öner.
- Unity Editor'de yapılacakları "İNSAN GÖREVİ" başlığıyla ayrı listele.
- Derleme hatalarını Unity Console'dan ben yapıştıracağım; minimal diff'le düzelt.

## Yasaklar
- Multiplayer, meta katman, gerçek reklam/IAP SDK'sı, sunucu (v2).
- Editor dışında çalışmayan kod bırakma; her adım Android build'iyle uyumlu.