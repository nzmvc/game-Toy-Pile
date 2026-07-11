# AI Native Game Studio OS (AOS) - Eksiksiz Analiz ve Dokümantasyon Dizini

Bu rapor; otonom oyun üretim fabrikasının yapısını, psikolojik oyun dinamiklerini, Google Antigravity ve OpenAI Agents SDK entegrasyonlarını ve ilk ajan olan AOS Game Producer (v0.2) spesifikasyonunu içeren modüler dokümanlardan oluşmaktadır.

Aşağıdaki dosyalar projeniz için bağımsız olarak bu dizinde oluşturulmuştur:
1. [SYSTEM_ARCHITECTURE.md](file:///Users/nazimavci/dev/GitHub/game-toy-pile/_analiz/gemini/SYSTEM_ARCHITECTURE.md): 9 makineli otonom üretim hattının genel mantığı, pazar metrikleri ve model dağıtım matrisi.
2. [GAME_PRODUCER_SPEC.md](file:///Users/nazimavci/dev/GitHub/game-toy-pile/_analiz/gemini/GAME_PRODUCER_SPEC.md): İnsan ile sistem arasındaki tek giriş noktası olan Game Producer ajanın spesifikasyonları, durum makinesi (state machine) ve OpenAI GPT/Assistant için doğrudan kullanılabilecek sistem talimatı.
3. [CLAUDE.md](file:///Users/nazimavci/dev/GitHub/game-toy-pile/_analiz/gemini/CLAUDE.md): Claude Projects ve Antigravity IDE üzerinde kod üreten yapay zekanın uymak zorunda olduğu kurallar (Project Constitution).
4. [AOS_ORCHESTRATOR.py](file:///Users/nazimavci/dev/GitHub/game-toy-pile/_analiz/gemini/AOS_ORCHESTRATOR.py): Machine 1 (Pazar Radarı) ve Machine 2 (Seçim Kapısı) ajanlarının OpenAI Agents SDK kullanarak birbiriyle konuşmasını, otonom veri aktarmasını (handoff) ve rasyonel skorlama kararlarını yerel diske kaydetmesini sağlayan çalışır Python kodu.

## Nasıl Kullanacaksınız?

1. **AOS Game Producer (v0.2)** kurgusunu kod yazmadan denemek için **[GAME_PRODUCER_SPEC.md](file:///Users/nazimavci/dev/GitHub/game-toy-pile/_analiz/gemini/GAME_PRODUCER_SPEC.md)** içerisindeki prompt şablonunu doğrudan ChatGPT Custom GPT veya OpenAI Assistant Instructions alanına yapıştırın.
2. Yerel bilgisayarınızda otonom bir test bandı çalıştırmak istiyorsanız, bir Python dosyası (`main.py` veya `AOS_ORCHESTRATOR.py`) oluşturup **[AOS_ORCHESTRATOR.py](file:///Users/nazimavci/dev/GitHub/game-toy-pile/_analiz/gemini/AOS_ORCHESTRATOR.py)** kodunu yapıştırarak `python AOS_ORCHESTRATOR.py` komutuyla otonom pazar analizi ve karar alma süreçlerini terminalinizde canlı olarak izleyin [cite: 26, 27].
3. Unity projenizi kod üretici ajanlarla (Claude Code veya Antigravity) entegre edeceğiniz zaman, projenizin ana dizinine **[CLAUDE.md](file:///Users/nazimavci/dev/GitHub/game-toy-pile/_analiz/gemini/CLAUDE.md)** dosyasını yerleştirerek ajanların monolitik olmayan temiz yapılar kurmasını sağlayın [cite: 22, 24].
