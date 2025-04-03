## 🚀 ПЛАН РАЗРАБОТКИ — **ПОШАГОВО, СТАДИЯМИ**

---

### 📦 **Стадия 1: Базовая архитектура и каркас**

🔹 Цель: поднять всё решение и минимально живой API, чтобы можно было тестировать на ходу.

**Этапы:**

1. [✅] Создание solution и всех проектов
2. [✅] Создание `Domain.Entities`: `Ticker`, `PortfolioAsset`
3. [✅] ValueObject: `Portfolio`
4. [✅] Исключения: `EmptyPortfolioException`, `InvalidTickerException`, `MissingApiKeyException`
5. [✅] Интерфейсы: `IPortfolioService`, `IDataStorage`, `ILoggerService`
6. [✅] Сервис: `PortfolioService`
7. [✅] Реализация: `InMemoryDataStorage`
8. [✅] Реализация: `ConsoleLogger`
9. [✅] Middleware: `ErrorHandlingMiddleware`
10. [✅] DTO: `AddTickerRequest`, `PortfolioViewModel`
11. [✅] Базовый контроллер: `BaseController` с `OperationResult<T>`
12. [✅] Контроллер: `PortfolioController`
13. [✅] Swagger и конфиги
14. [✅] Unit-тесты для `PortfolioService` и `InMemoryDataStorage`

---

### 📊 **Стадия 2: Интеграция рыночных данных (Tinkoff, Smart-Lab)**

🔹 Цель: тянуть исторические данные по тикерам, чтобы строить индикаторы.

**Этапы:**

1. [✅] ValueObject: `PriceHistoryPoint`
2. [✅] Интерфейсы: `ITinkoffClient`, `ISmartLabClient`
3. [✅] Компонент: `MarketDataAggregator.FetchHistoricalData()`
4. [✅] Тесты на клиентов с моками

---

### 📈 **Стадия 3: Расчёт индикаторов**

🔹 Цель: преобразовать исторические данные в технические сигналы.

**Этапы:**

1. [🔲] ValueObject: `IndicatorResult`
2. [🔲] Интерфейс: `ISkenderIndicatorEngine`
3. [🔲] Реализация: `SkenderWrapper`
4. [🔲] Сервис: `IndicatorService.CalculateIndicators()`
5. [🔲] Тесты: `SkenderWrapperTests`, `IndicatorServiceTests`

---

### ⚖️ **Стадия 4: Генерация торговых сигналов**

🔹 Цель: преобразовать индикаторы в понятные сигналы `Buy/Sell/Hold`.

**Этапы:**

1. [🔲] Enum: `TradeAction`, `SignalReason`, `IndicatorType`
2. [🔲] Entity: `TradeSignal`
3. [🔲] Интерфейс: `ISignalService`
4. [🔲] Реализация: `SignalService`
5. [🔲] Логика: правила сигналов (RSI < 30 = Buy, RSI > 70 = Sell и т.п.)
6. [🔲] Тесты: `SignalServiceTests`

---

### 🧾 **Стадия 5: Отчёты и Telegram**

🔹 Цель: отправлять сигналы в Telegram, красиво и по делу.

**Этапы:**

1. [🔲] DTO: `TelegramMessage`, `TelegramSettings`
2. [🔲] Интерфейс: `ITelegramService`
3. [🔲] Реализация: `TelegramNotifier`
4. [🔲] Метод: `TelegramService.SendAnalysisSummary()`
5. [🔲] Опционально: команды бота (/status, /report)

---

### 📅 **Стадия 6: Планировщик и запуск анализа**

🔹 Цель: запускать весь пайплайн по расписанию и вручную.

**Этапы:**

1. [🔲] Интерфейс: `ISchedulerAdapter`
2. [🔲] Реализация: `HangfireScheduler` или `QuartzScheduler`
3. [🔲] Cron-модель: `CronSchedule`
4. [🔲] Сервис: `PortfolioAnalysisJob`
5. [🔲] Контроллер: `SchedulerController.TriggerNow()`

---

### 🧠 **Стадия 7: UseCases и агрегация**

🔹 Цель: собрать все части пайплайна в единую цепочку.

**Этапы:**

1. [🔲] UseCase: `RunPortfolioAnalysisUseCase`
2. [🔲] Сохранение: `AnalysisResultRepository.SaveAnalysisResult()`
3. [🔲] ValueObject: `PortfolioAnalysisResult`
4. [🔲] Логгирование этапов: `LoggerService.LogStep()`

---

### 🧪 **Стадия 8: Тесты и обёртки**

🔹 Цель: обеспечить стабильность, откат и читаемость кода.

**Этапы:**

1. [🔲] Моки: `FakeTinkoffClient`, `MockSupabaseStorage`
2. [🔲] Fixtures: примеры тикеров, портфелей, индикаторов
3. [🔲] Test coverage: `Application`, `Infrastructure`, `Scheduler`, `Telegram`

---

### 📊 **Стадия 9: Визуализация (опц.)**

🔹 Цель: если очень захочется — можно добавить фронт

**Этапы:**

1. [🔲] Фронт: React + Chart.js или Recharts
2. [🔲] Эндпоинты для графиков
3. [🔲] Swagger-документация и OpenAPI

---

## ✅ Формат задач

Каждую задачу на следующем шаге мы делаем:
- Сразу с `XML` документацией
- С правильным именованием и намёками на паттерны
- С чёткой привязкой к `Domain` / `Application` / `Infrastructure`
- С минимумом хардкода

---

## 📌 Итого

| Стадия | Название                             | Прогресс |
|--------|--------------------------------------|---------|
| 1      | Архитектура и портфель               | ✅ 100% |
| 2      | Исторические данные                  | ✅ 100% |
| 3      | Индикаторы                           | ⏳       |
| 4      | Сигналы                              | ⏳       |
| 5      | Telegram                             | ⏳       |
| 6      | Планировщик                          | ⏳       |
| 7      | Пайплайн use case                    | ⏳       |
| 8      | Юнит-тесты и стабилизация            | ⏳       |
| 9      | Визуализация (если понадобится)      | ⏳       |

---