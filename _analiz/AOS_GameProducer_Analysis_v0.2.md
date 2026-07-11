# AI Native Game Studio OS
# ChatGPT Session Analysis
## Agent 01 – Game Producer
### Version: 0.2
### Date: 2026-07-11

---

# 1. Session Goal

Bu oturumun amacı AI Native Game Studio OS'in ilk ajanı olan **Game Producer**'ı tasarlamak ve çalışan ilk sürümünü oluşturmaktır.

Bu çalışma sonunda Game Producer'ın görev tanımı, sistem içindeki konumu, davranış kuralları ve Machine0 ile olan veri sözleşmesi belirlenmiştir.

---

# 2. Temel Mimari

```
Human
    │
    ▼
Game Producer
    │
GameBrief.json
    │
    ▼
Machine 0
(Studio Architect)
    │
Project Blueprint
```

Karar:

Machine0 hiçbir zaman kullanıcı ile konuşmaz.

İnsan ile iletişim kuran tek ajan Game Producer'dır.

---

# 3. Agent Responsibility

Game Producer;

İnsanlardan gelen fikirleri

↓

Standartlaştırılmış

↓

GameBrief.json

haline dönüştürür.

Yapmayacağı işler

- Kod üretmek
- Unity mimarisi oluşturmak
- Sprint planlamak
- Analytics hazırlamak
- Teknik öneriler oluşturmak

---

# 4. Problem Analizi

İlk test başarılı olmasına rağmen aşağıdaki problemler tespit edilmiştir.

## Problem 1

Agent her cevapta GameBrief üretmeye çalışıyor.

Karar

GameBrief yalnızca workflow sonunda üretilecek.

---

## Problem 2

"devam et"

"ok"

"next"

gibi mesajları yeni bilgi olarak algılıyor.

Karar

User Input Validation eklenecek.

---

## Problem 3

Workflow unutuluyor.

Agent tekrar başa dönüyor.

Karar

Conversation State yönetilecek.

---

## Problem 4

Agent sekreter gibi davranıyor.

Karar

Executive Producer gibi davranacak.

Ürün geliştirecek.

Risk gösterecek.

Öneri sunacak.

---

# 5. Yeni Workflow

```
State 0

WaitingForIdea

↓

State 1

Discovery

↓

State 2

CollectInformation

↓

State 3

Validation

↓

State 4

Generate GameBrief

↓

Finished
```

---

# 6. Discovery Mode

Bu modda

Agent

- fikri anlamaya çalışır

- eksik bilgileri bulur

- riskleri belirler

- gereksiz soru sormaz

Bu modda

JSON ÜRETMEZ.

---

# 7. Brief Generation Mode

Bu moda yalnızca

Completion Score >= 90

olduğunda geçilir.

Bu modda

tek seferde

GameBrief.json

oluşturulur.

---

# 8. Conversation Rules

Agent

workflow'u unutmaz.

Kullanıcı

"devam et"

yazarsa

Workflow devam eder.

Tekrar başlamaz.

---

# 9. User Input Validation

Her mesaj önce analiz edilir.

Yeni bilgi içeriyor mu?

Eğer

Hayır

ise

Workflow ilerlemez.

Yalnızca bekleyen sorular hatırlatılır.

---

# 10. Completion Score

0-30

Idea Stage

31-60

Discovery

61-89

Validation

90+

Ready for Machine0

---

# 11. Quality Gate

Completion Score

90 altında ise

Machine0

çalıştırılmaz.

---

# 12. Output Rules

Discovery Mode

```
{
status,

completion_score,

remaining_questions
}
```

Brief Generation Mode

```
GameBrief.json
```

Başka çıktı oluşturulmaz.

---

# 13. Machine0 Contract

Game Producer'ın çıktısı

Machine0'ın girdisidir.

Bu nedenle

GameBrief

AI Native Game Studio OS'in

ortak veri sözleşmesidir.

---

# 14. Agent Personality

Agent

Executive Producer gibi davranacaktır.

Davranış özellikleri

- Mentor

- Product Manager

- Mobile Game Expert

- Market Aware

- Risk Oriented

- Business Focused

Asla

sekreter gibi davranmayacaktır.

---

# 15. Agent Modes

Mode 1

Discovery

Mode 2

Validation

Mode 3

GameBrief Generation

---

# 16. Agent Inputs

Serbest metin

veya

önceden hazırlanmış Game Idea

Örnek

```
I want to build a relaxing mobile game.

Players restore abandoned islands.
```

---

# 17. Agent Outputs

Discovery

```
status

completion_score

remaining_questions
```

Final

```
GameBrief.json
```

---

# 18. Başarı Kriterleri

Başarılı bir Game Producer;

✓ Aynı soruyu tekrar sormaz

✓ Workflow'u unutmaz

✓ Kullanıcının verdiği bilgileri kaybetmez

✓ Gereksiz JSON üretmez

✓ Machine0 için yeterli veri üretir

✓ Mentor gibi davranır

---

# 19. Machine0 Öncesi Yapılacaklar

Tamamlanacak işler

[ ] Instruction yeniden yazılacak

[ ] Conversation State eklenecek

[ ] User Input Validation eklenecek

[ ] Discovery Mode eklenecek

[ ] Brief Generation Mode eklenecek

[ ] Output Rules sadeleştirilecek

[ ] Completion Score algoritması geliştirilecek

[ ] GameBrief Schema v1 hazırlanacak

---

# 20. Machine0 İçin Gereken Dokümanlar

Machine0 geliştirilmeden önce aşağıdaki dokümanlar hazırlanacaktır.

## 1

GameBrief Schema v1

Amaç

Machine0 giriş formatı

---

## 2

Machine0 PRD

Görev tanımı

Sorumluluklar

Çıktılar

---

## 3

Machine0 Agent Specification

Instruction

State Machine

Workflow

Memory

Tools

Output

---

## 4

AOS Message Protocol

Agentlar arası iletişim standardı

---

## 5

AOS Agent Manifest Standard

Her agent için ortak tanım

---

# 21. Son Kararlar

Bu oturum sonunda aşağıdaki mimari kararlar alınmıştır.

1.

Game Producer sistemin tek giriş noktasıdır.

2.

Machine0 kullanıcı ile konuşmaz.

3.

Game Producer yalnızca GameBrief üretir.

4.

GameBrief sistemin ortak veri sözleşmesidir.

5.

Discovery ve Brief Generation ayrı modlardır.

6.

Conversation State zorunludur.

7.

Workflow unutulamaz.

8.

Completion Score kalite kapısıdır.

9.

Completion Score >=90 olmadan Machine0 çalıştırılmaz.

10.

Tüm sonraki agentlar bu standart üzerine inşa edilecektir.

---

# Next Session Roadmap

Bir sonraki oturumda aşağıdaki sırayla ilerlenmelidir.

1.

GameBrief Schema v1

↓

2.

Machine0 PRD

↓

3.

Machine0 Instruction

↓

4.

Machine0 Test

↓

5.

İlk çalışan AI Native Game Studio OS