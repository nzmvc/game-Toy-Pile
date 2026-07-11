# AOS_ORCHESTRATOR.py - OpenAI Agents SDK Altyapısı
# Gerekli kurulum: pip install "openai-agents>=0.14.0"

import asyncio
import json
from openai import OpenAI
from agents import Agent, Runner

# OpenAI API istemcisinin başlatılması
client = OpenAI()

# --- SİMÜLE EDİLMİŞ CANLI VERİ SAĞLAYICI (TOOL) ---
def fetch_live_market_data() -> str:
    """AppMagic ve TikTok trendlerini simüle eden veri aracı."""
    trends = [
        {
            "genre": "Screw Puzzle", 
            "mechanic": "Unscrewing bolts, sorting by color into matching boxes", 
            "estimated_cpi": 0.35, 
            "estimated_dev_weeks": 2, 
            "monetization": "Hybrid (IAP + Rewarded Ads)"
        },
        {
            "genre": "Classic Match-3 Clone", 
            "mechanic": "Traditional match-3 with royal decoration story", 
            "estimated_cpi": 3.20, 
            "estimated_dev_weeks": 16, 
            "monetization": "IAP-Heavy"
        },
        {
            "genre": "ASMR Sand Slicing", 
            "mechanic": "Satisfying slicing of kinetic sand with haptic patterns", 
            "estimated_cpi": 0.18, 
            "estimated_dev_weeks": 1, 
            "monetization": "Ad-Heavy"
        }
    ]
    return json.dumps(trends, ensure_ascii=False, indent=2)

def save_gatekeeper_decision(decision_data_json: str) -> str:
    """Seçim Kapısı'nın onayladığı kararları diske kaydeder."""
    try:
        data = json.loads(decision_data_json)
        filename = f"gatekeeper_decision_{data.get('genre', 'unknown').replace(' ', '_')}.json"
        with open(filename, "w", encoding="utf-8") as f:
            json.dump(data, f, ensure_ascii=False, indent=2)
        return f"BAŞARILI: Karar ve analiz '{filename}' dosyasına kaydedildi."
    except Exception as e:
        return f"HATA: Dosya kaydedilemedi: {str(e)}"

# --- AGENT HANDOFF (KONTROL DEVİR) FONKSİYONU ---
def handoff_to_gatekeeper(selected_trend_json: str):
    """Machine 1'den Machine 2'ye geçişi ve veri aktarımını sağlar."""
    print(f"\n[AOS Handoff]: Pazar Radarı görevi tamamladı. Seçim Kapısı'na devrediliyor...")
    return gatekeeper_agent, f"Lütfen şu seçilen trendi analiz et ve skorla: {selected_trend_json}"

# --- MACHINE 1: PAZAR RADARI ---
market_radar_agent = Agent(
    name="Machine 1 - Pazar Radarı",
    model="gpt-4o",
    instructions="""
    Sen otonom oyun fabrikasının ilk makinesisin.
    1. 'fetch_live_market_data' aracıyla pazar trendlerini çek.
    2. Solo bir geliştirici için en mantıklı (geliştirme süresi kısa, satisfying alt türünde) konsepti seç.
    3. Seçtiğin trendin JSON verisini 'handoff_to_gatekeeper' aracıyla Seçim Kapısı'na göndererek kontrolü devret.
    """,
    tools=[fetch_live_market_data, handoff_to_gatekeeper]
)

# --- MACHINE 2: SEÇİM KAPISI ---
gatekeeper_agent = Agent(
    name="Machine 2 - Seçim Kapısı",
    model="gpt-4o",
    instructions="""
    Sen otonom oyun fabrikasının en kritik filtresisin.
    Gelen aday oyunu şu kriterlerle skorla:
    1. Dev Time: Solo için en fazla 4 hafta olmalı. 4 haftayı aşan her hafta için skordan 2 puan kır.
    2. Tahmini CPI: Android için hedef $0.40'ın altıdır. $0.40 altındaysa +3 puan, $1.50 ve üstündeyse oyunu doğrudan ele (Skor: 0).
    3. Monetizasyon: Hibrit-Casual (IAP + Ad) ise +2 puan ekle [cite: 8].
    
    Toplam skoru 10 üzerinden hesapla. Skor >= 7.5 ise Approved (YEŞİL IŞIK) ver ve 'save_gatekeeper_decision' aracıyla diske kaydet.
    Skor < 7.5 ise Rejected (ÖLDÜR) kararı ver.
    """,
    tools=[save_gatekeeper_decision]
)

# --- WORKSPACE ORKESTRASYONU ---
async def start_pipeline():
    print("AOS Otonom Üretim Bandı Başlatılıyor...\n")
    run_result = await Runner.run(
        agent=market_radar_agent,
        prompt="Pazar analizini başlat, en mantıklı trendi seçip Seçim Kapısı'na gönder."
    )
    print("\n--- ÜRETİM BANDI NİHAİ ÇIKTISI ---")
    print(run_result.final_output)

if __name__ == "__main__":
    asyncio.run(start_pipeline())
