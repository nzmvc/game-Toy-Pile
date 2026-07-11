# CLAUDE.md - AOS Proje Anayasası (v2.0)

## 1. Mimari Sınırlar ve Kurallar

- **ScriptableObject Bağımlılığı:** Singleton veya statik referans kullanımı kesinlikle yasaktır. Tüm sistemler arası haberleşme ve veri akışı ScriptableObject olay kanalları (`VoidGameEventSO`, `IntGameEventSO`) üzerinden loose-coupled olarak yürütülecektir [cite: 9, 23].
- **Monolitik Kod Yasağı:** Yazılan hiçbir MonoBehaviour sınıfı 150 satırı geçemez. Bu kısıtlama, otonom alt ajanların kod tabanını regresyon hatası üretmeden refaktör edebilmesi için zorunludur [cite: 9, 24].
- **GC ve Performans:** `Update()` döngüsü içerisinde bellek tahsis eden (GC allocation) yapılar, `Instantiate`, `Find` veya `GetComponent` çağrıları kesinlikle yasaktır [cite: 9, 25]. Obje havuzu (Object Pooling) kullanılacaktır [cite: 9, 25].

## 2. Antigravity IDE Kodlama Standartları

- IntelliSense ve autocomplete kararlılığı için harici DotRush/Roslyn dil sunucusu çıktılarını takip et [cite: 2].
- C# kodlarında Unity Inspector ile eşleşen tüm private değişkenlerin başına alt çizgi konulmalı (`_variableName`) ve `[SerializeField]` özniteliğiyle deklare edilmelidir.
- Tüm harici referanslar Inspector üzerinden çözülecek; kod içerisinde `GameObject.Find()` veya `FindObjectOfType()` araması yapılmayacaktır.

## 3. Klasör Düzeni

- `Assets/_Project/Scripts/Core/` - Çekirdek şasi motoru (yükleyiciler, genel durum yönetimi).
- `Assets/_Project/Scripts/Mechanics/` - İzole mekanik modülleri.
- `Assets/_Project/Scripts/SO_Architecture/` - Event kanalları ve veri SO'ları [cite: 23].
