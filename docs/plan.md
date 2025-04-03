## 🚀 ПЛАН РАЗРАБОТКИ BLACKCANDLE — **СТАДИЯМИ, С ГОЛОВОЙ**

---

### 📦 **Стадия 1: Архитектура и API**

🔹 Цель: запустить проект, чтобы хоть что-то работало и не стыдно было коммитить.

**Этапы:**
1. [✅] Создание решения и всех проектов (`API`, `Application`, `Infrastructure`, `Domain` и т.д.)
2. [✅] Базовые `Domain.Entities`: `Ticker`, `PortfolioAsset`
3. [✅] ValueObject: `Portfolio`, `PriceHistoryPoint`
4. [✅] Исключения: `EmptyPortfolioException`, `InvalidTickerException`, `MissingApiKeyException`
5. [✅] Интерфейсы: `IPortfolioService`, `IDataStorage`, `ILoggerService`
6. [✅] Реализации: `InMemoryDataStorage`, `ConsoleLogger`
7. [✅] DTO + API-контроллеры (`PortfolioController`, `BaseController`)
8. [✅] Swagger, конфиги, middleware
9. [✅] Unit-тесты на `PortfolioService`, `InMemoryStorage`

---

### 📊 **Стадия 2: Получение рыночных данных**

🔹 Цель: получать исторические котировки для анализа.

**Изменения:**
- **MOEX — в топку.**
- Используем:
    - `TinkoffClient` — для цен
    - `SmartLabScraper` — для фундаменталки (P/E, P/B, дивы, капитализация и т.д.)

**Этапы:**
1. [✅] Интерфейсы: `ITinkoffClient`, `ISmartLabClient`
2. [✅] Компонент: `MarketDataAggregator`
3. [✅] Тесты с моками

---

### 📈 **Стадия 3: Технический анализ (индикаторы)**

🔹 Цель: оборачивать `Skender.Stock.Indicators` и не сойти с ума.

**Этапы:**
1. [✅] ValueObject: `IndicatorResult`
2. [✅] Enum: `IndicatorType`
3. [✅] Интерфейс: `ISkenderIndicatorEngine`
4. [✅] Реализация: `SkenderWrapper`
5. [✅] Сервис: `IndicatorService.CalculateIndicators()`
6. [✅] Тесты: `SkenderWrapperTests`, `IndicatorServiceTests`

---

### 📉 **Стадия 4: Генерация торговых сигналов**

🔹 Цель: простая логика Buy/Sell/Hold на основе индикаторов.

**Этапы:**
1. [✅] Enums: `TradeAction`, `SignalReason`
2. [✅] Entity: `TradeSignal`
3. [✅] Интерфейс: `ISignalService`
4. [✅] Реализация: `SignalService`
5. [✅] Простые правила сигналов:
    - RSI < 30 + EMA пробит → Buy
    - RSI > 70 + MACD разворот → Sell
6. [✅] Тесты

---

### 🧠 **Стадия 5: Фундаментальный анализ**

🔹 Цель: получить метрики с Smart-Lab и использовать их в скоринге.

**Этапы:**
1. [✅] Entity: `FundamentalData`
2. [✅] Интерфейс: `IFundamentalDataService`
3. [✅] Реализация: `SmartLabScraper`
4. [✅] Сервис: `FundamentalScoringService`
5. [✅] Интеграция в `SignalService` (дополнительные фильтры)
6. [✅] Тесты

---

### 🤖 **Стадия 6: Автоматическое исполнение сделок**

🔹 Цель: бот сам торгует, если ему разрешили.

**Этапы:**
1. [✅] Интерфейс: `IBrokerTradeClient`
    - `PlaceMarketOrder()`, `GetAvailableCash()`
2. [✅] Реализация: `FakeTinkoffTradeClient` (для начала)
3. [✅] Сервис: `TradeExecutionService`
4. [✅] Флаг `BotSettings.EnableAutoTrading`
5. [✅] Обновление портфеля после сделок
6. [✅] Тесты

---

### 📨 **Стадия 7: Telegram-уведомления**

🔹 Цель: сообщать пользователю, что он нищий, но держится.

**Этапы:**
1. [✅] DTO: `TelegramMessage`, `TelegramSettings`
2. [✅] Интерфейс: `ITelegramService`
3. [✅] Реализация: `TelegramNotifier`
4. [✅] Метод: `TelegramService.SendAnalysisSummary()`
5. [🔲] (Опц.) Команды: `/status`, `/report`, `/autotrade_on`, `/autotrade_off`

---

### ⏲️ **Стадия 8: Планировщик**

🔹 Цель: запускать всё по расписанию.

**Этапы:**
1. [✅] Интерфейс: `ISchedulerAdapter`
2. [✅] Реализация: `QuartzScheduler` или `HangfireScheduler`
3. [✅] ValueObject: `CronSchedule`, `BotStatus`
4. [✅] Джоба: `PortfolioAnalysisJob`
5. [✅] API: `SchedulerController.TriggerNow()`

---

### 🧠 **Стадия 9: UseCase и сборка пайплайна**

🔹 Цель: весь анализ в одном сценарии.

**Этапы:**
1. [✅] UseCase: `RunPortfolioAnalysisUseCase`
2. [✅] Сервис: `AnalysisResultRepository`
3. [✅] ValueObject: `PortfolioAnalysisResult`
4. [✅] Интеграция всех сервисов
5. [✅] Логгирование этапов: `LoggerService.LogStep()`

---

### 🧪 **Стадия 10: Тесты, фикстуры, стабилизация**

🔹 Цель: чтобы ничего не сломалось при первой перезагрузке.

**Этапы:**
1. [✅] Моки: `FakeTinkoffClient`, `MockStorage`, `FakeTelegram`
2. [✅] Fixtures: тикеры, индикаторы, портфели
3. [✅] Покрытие: `Application`, `Infrastructure`, `Scheduler`, `Telegram`

---

### 📊 **Стадия 11: (Опц.) Визуализация**

🔹 Цель: чтоб показать на митапе или маме.

**Этапы:**
1. [🔲] Мини-фронт: React + Chart.js
2. [🔲] API: графики, логи
3. [🔲] Swagger и документация

---

## ✅ Правила реализации

- Каждая задача — с `XML` документацией
- Только нормальное именование
- Деление по слоям (`Domain`, `Application`, `Infrastructure`)
- 0% хардкода, 100% — боль и чистота

---

