## 🧱 Архитектура монолита (Solution structure, но по-умному)

Монолитный проект в .NET — это не значит "один проект на все случаи". Это значит "всё в одном решении, но с головой". Вот структура:

```
/BlackCandle.sln
  |-- BlackCandle.API             // ASP.NET Core Web API
  |-- BlackCandle.Application     // Бизнес-логика (сервисы, интерфейсы)
  |-- BlackCandle.Infrastructure  // Интеграции: API, базы, логи
  |-- BlackCandle.Domain          // Модели, абстракции, enums
  |-- BlackCandle.Telegram        // Работа с Telegram-ботом
  |-- BlackCandle.Scheduler       // Планировщик (Hangfire/Quartz)
  |-- BlackCandle.Tests           // xUnit + Moq
```

---

Окей, по-серьёзке. Сейчас не время пихать код, сейчас время **строить бетонный фундамент**, чтобы потом не бегать с криками "а куда вот это положить?".

Разберём **по каждому проекту**, что там должно быть, **какие классы**, **что они делают**, и **зачем вообще существуют**. Всё по уму, без философии, строго и по делу.

---

## 📁 `BlackCandle.API` — ASP.NET Core Web API

👉 **Назначение**: слой общения с внешним миром — фронтом, Postman’ом, чем угодно. Только контроллеры и DTO.

### Содержит:
- **Controllers/**
  - `PortfolioController` – работа с портфелем (добавить тикер, получить список).
  - `AnalysisController` – запустить анализ вручную, получить результаты.
  - `SchedulerController` – управление кроном (вкл/выкл, редактирование).
  - `LogsController` – получить последние логи.
- **DTO/**
  - `AddTickerRequest`, `PortfolioViewModel`, `AnalysisResultResponse` – то, что приходит и уходит.
- **Middleware/**
  - `ErrorHandlingMiddleware` – глобальный try-catch.
- **Configuration/**
  - `DependencyInjectionConfig`, `AppSettings`, `SwaggerSetup`.

---

## 📁 `BlackCandle.Application` — бизнес-логика

👉 **Назначение**: мозг проекта. Координирует доменные сущности, orchestrates всё веселье.

### Содержит:

- **Services/**
  - `IAnalysisService` / `AnalysisService` – расчёт индикаторов и генерация сигналов.
  - `IPortfolioService` / `PortfolioService` – управление портфелем (CRUD, проверка лимитов).
  - `ISignalService` / `SignalService` – интерпретация результатов анализа.
  - `ILoggerService` – универсальный логгер.
  - `ITelegramService` – отправка уведомлений.
- **UseCases/**
  - `RunPortfolioAnalysisUseCase` – агрегирует всё: тянет данные, считает, шлёт.
  - `UpdatePortfolioUseCase`, `GetPortfolioUseCase`.
- **Interfaces/**
  - Абстракции для инфраструктурных клиентов: `ITinkoffClient`, `IMoexClient`, `IDataStorage`, `ISchedulerAdapter`.
- **Mappers/**
  - `IndicatorMapper`, `SignalMapper` – перевод доменных моделей в ответные DTO.

---

## 📁 `BlackCandle.Infrastructure` — реализация зависимостей

👉 **Назначение**: грязная работа. Подключение к API, база, логгирование.

### Содержит:

- **APIClients/**
  - `TinkoffClient` – получение рыночных данных и котировок.
  - `MoexClient` – парсинг MOEX XML/JSON.
- **Indicators/**
  - `SkenderWrapper` – обёртка над Skender.Stock.Indicators, нормализует данные.
- **Persistence/**
  - `SupabaseStorage` – работа с БД (через EF или REST, зависит от реализации).
  - `EntityMapper`, `DbContext` (если EF).
- **Logging/**
  - `LogtailLogger`, `FileLogger`, `ConsoleLogger` – кастомные логгеры.
- **Adapters/**
  - `SchedulerAdapter` – связка с Hangfire/Quartz (через интерфейс `ISchedulerAdapter`).

---

## 📁 `BlackCandle.Domain` — доменная модель

👉 **Назначение**: ядро. Здесь всё, что относится к бизнес-сущностям.

### Содержит:

- **Entities/**
  - `Ticker`, `PortfolioAsset`, `TradeSignal`, `IndicatorResult`, `PortfolioAnalysisResult`.
- **Enums/**
  - `TradeAction`, `IndicatorType`, `ExchangeType`, `SignalReason`.
- **ValueObjects/**
  - `PriceHistoryPoint`, `ConfidenceLevel`, `CronSchedule`, `BotStatus`.
- **Exceptions/**
  - `InvalidTickerException`, `PortfolioLimitExceededException`.

Никакой зависимости от других проектов. Только бизнес и больше ничего.

---

## 📁 `BlackCandle.Telegram` — Telegram-интеграция

👉 **Назначение**: бот, уведомления, обратная связь.

### Содержит:

- **Services/**
  - `TelegramNotifier` – отправка сообщений.
  - `TelegramCommandHandler` – (опц.) обработка команд типа /status, /report.
- **Models/**
  - `TelegramMessage`, `TelegramUser`, `TelegramSettings`.
- **Interfaces/**
  - `ITelegramService` – абстракция, реализуется в Application.

---

## 📁 `BlackCandle.Scheduler` — планировщик

👉 **Назначение**: крутить cron-задания, запускать пайплайн, уметь отрубаться.

### Содержит:

- **Jobs/**
  - `PortfolioAnalysisJob` – основной запуск анализа.
  - `RetryFailedJob` – (опц.) повтор неудачных вызовов API.
- **Scheduler/**
  - `HangfireScheduler`, `QuartzScheduler` – реализация `ISchedulerAdapter`.
  - `ScheduleConfiguration` – считывание расписания.
- **Utils/**
  - `JobContextLogger` – писать шаги в лог.

---

## 📁 `BlackCandle.Tests` — юнит и интеграционные тесты

👉 **Назначение**: проверка логики, стабильности, возврата из ада после рефакторинга.

### Содержит:

- **Application/**
  - `AnalysisServiceTests`, `PortfolioServiceTests`, `SignalServiceTests`.
- **Infrastructure/**
  - `TinkoffClientTests`, `MoexClientTests`, `SkenderWrapperTests`.
- **Telegram/**
  - `TelegramNotifierTests`.
- **Mocking/**
  - `FakeTinkoffClient`, `MockSupabaseStorage`.
- **Fixtures/**
  - Заранее подготовленные портфели, индикаторы и сигналы.

---

## 🔁 **Пайплайн бота (RunPortfolioAnalysisUseCase)**  
📅 Запускается либо:  
— автоматически по расписанию (через `SchedulerAdapter`)  
— вручную через API (`AnalysisController.TriggerNow()`)

---

## 🔷 Шаг 1: Получение актуального портфеля
### Класс: `PortfolioService.GetCurrentPortfolio()`
- Получает список активов пользователя (тикеры, количество, цена покупки).
- Вариант: из Supabase, либо локально.
- Проверяет наличие хотя бы одного актива, иначе бросает `EmptyPortfolioException`.

> 🔗 Зависимость: `IDataStorage` (реализация — `SupabaseStorage`)

---

## 🔷 Шаг 2: Получение исторических данных по каждому тикеру
### Класс: `MarketDataAggregator.FetchHistoricalData(IEnumerable<Ticker>)`
- Использует `TinkoffClient` и/или `MoexClient` (в зависимости от биржи).
- Загружает OHLCV данные (Open, High, Low, Close, Volume) за заданный период (например, 1 год).
- Преобразует в `List<PriceHistoryPoint>`

> 🔗 Зависимости:
> - `ITinkoffClient`
> - `IMoexClient`

---

## 🔷 Шаг 3: Расчёт технических индикаторов
### Класс: `IndicatorService.CalculateIndicators()`
- Принимает `List<PriceHistoryPoint>` на вход.
- Вызывает `SkenderWrapper` для расчёта:
  - `SMA`, `EMA`, `RSI`, `MACD`, `ADX`, и т. д.
- Возвращает `List<IndicatorResult>` по каждому тикеру.

> 🔗 Зависимость: `ISkenderIndicatorEngine`

---

## 🔷 Шаг 4: Генерация торговых сигналов
### Класс: `SignalService.GenerateSignals(indicators, portfolio)`
- Анализирует индикаторы и текущую позицию в портфеле.
- Выдаёт сигналы типа:
  - `Buy`, если RSI < 30 и цена пробила EMA
  - `Sell`, если RSI > 70 и MACD разворачивается
  - `Hold` — во всех остальных случаях
- Возвращает `List<TradeSignal>`

> 🔗 Зависимость: `ISignalPolicyEngine` (если хочешь отделить логику сигналов в отдельную стратегию)

---

## 🔷 Шаг 5: Сохранение результатов анализа
### Класс: `AnalysisResultRepository.SaveAnalysisResult()`
- Сохраняет `PortfolioAnalysisResult` и `TradeSignals` в базу.
- Может быть использовано для отображения в логах и фронте.

> 🔗 Зависимость: `IDataStorage`  

---

## 🔷 Шаг 6: Уведомление через Telegram
### Класс: `TelegramService.SendAnalysisSummary(signals)`
- Формирует короткий текст: какие тикеры покупать, продавать, держать.
- Отправляет через `TelegramNotifier`.

Пример:
```
📈 Актуальный сигнал:
- 🔴 Продать: GAZP (RSI=79, MACD ⬇)
- 🟢 Купить: YNDX (RSI=22, EMA пробит)
- 🟡 Держать: SBER
```

> 🔗 Зависимость: `ITelegramService`

---

## 🔷 Шаг 7: Логгирование пайплайна
### Класс: `LoggerService.LogStep()`
- В каждый ключевой момент (до/после каждого шага) пишет лог:
  - `StartAnalysis`
  - `FetchedData`
  - `CalculatedIndicators`
  - `GeneratedSignals`
  - `SentToTelegram`
- Сохраняет как локально, так и в Logtail/базу (по конфигурации).

> 🔗 Зависимость: `ILoggerService`

---

## 🔁 Как всё собирается: `RunPortfolioAnalysisUseCase`

```plaintext
Scheduler → RunPortfolioAnalysisUseCase
           |
           ├─ PortfolioService.GetCurrentPortfolio()
           ├─ MarketDataAggregator.FetchHistoricalData()
           ├─ IndicatorService.CalculateIndicators()
           ├─ SignalService.GenerateSignals()
           ├─ AnalysisResultRepository.SaveAnalysisResult()
           ├─ TelegramService.SendAnalysisSummary()
           └─ LoggerService.LogStep()
```

---

## 🔒 Возможные “стоп-краны”

- **Ключей нет** → `MissingApiKeyException` (обработка и лог).
- **MOEX не отвечает** → Retry + лог.
- **Индикаторы не рассчитались** → `IndicatorCalculationException`
- **Пустой портфель** → skip анализа.

---

## ⏲️ Управление запуском (BlackCandle.Scheduler)

- **QuartzScheduler / HangfireScheduler**
  - Регает `PortfolioAnalysisJob`
  - Подгружает `CronSchedule` из БД/конфига
- Позволяет:
  - Включать/выключать бота (флаг `BotStatus`)
  - Менять интервал запуска
  - Форсить выполнение вручную

---
