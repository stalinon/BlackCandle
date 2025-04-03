# 🧱 МОДЕЛЬ ДАННЫХ ДЛЯ BLACKCANDLE

Минимальный, но достаточный набор сущностей и value-объектов для работы аналитического и торгового бота.

---

## 📂 1. PortfolioAsset

Актив в портфеле пользователя.

| Поле | Тип | Описание |
|------|-----|----------|
| `Ticker` | `string` | Символ (например, `SBER`) |
| `Quantity` | `decimal` | Количество акций |
| `BuyPrice` | `decimal` | Цена покупки |
| `BuyDate` | `DateTime` | Дата покупки |

---

## 📂 2. Ticker

Базовая информация о тикере (нормализованная).

| Поле | Тип | Описание |
|------|-----|----------|
| `Symbol` | `string` | Символ бумаги |
| `Exchange` | `ExchangeType` | Тип биржи (`TINKOFF`, `SMARTLAB`) |
| `Currency` | `string` | Валюта (`RUB`, `USD`) |
| `Sector` | `string` | Отрасль (по возможности) |

---

## 📂 3. PriceHistoryPoint

Историческая цена бумаги (OHLCV).

| Поле | Тип | Описание |
|------|-----|----------|
| `Date` | `DateTime` | Дата |
| `Open` | `decimal` | Цена открытия |
| `High` | `decimal` | Макс |
| `Low` | `decimal` | Мин |
| `Close` | `decimal` | Закрытие |
| `Volume` | `long` | Объём |

---

## 📂 4. IndicatorResult

Результат теханализа по тикеру и индикатору.

| Поле | Тип | Описание |
|------|-----|----------|
| `Ticker` | `string` | Символ |
| `IndicatorType` | `enum` | RSI, EMA, SMA, MACD и т.д. |
| `Date` | `DateTime` | Дата значения |
| `Value` | `decimal` | Основное значение |
| `ExtraData` | `Dictionary<string, decimal>` | Доп. поля (MACD Signal, EMA Fast и т.д.) |

---

## 📂 5. FundamentalData

Фундаментальные метрики с Smart-Lab.

| Поле | Тип | Описание |
|------|-----|----------|
| `Ticker` | `string` | Символ |
| `PERatio` | `decimal?` | P/E |
| `PBRatio` | `decimal?` | P/B |
| `DividendYield` | `decimal?` | Дивдоходность |
| `MarketCap` | `decimal?` | Капитализация |
| `ROE` | `decimal?` | Рентабельность капитала |

---

## 📂 6. TradeSignal

Сигнал к действию по бумаге.

| Поле | Тип | Описание |
|------|-----|----------|
| `Ticker` | `string` | Символ |
| `Action` | `TradeAction` | Buy / Sell / Hold |
| `Reason` | `SignalReason` | Причина сигнала (RSI < 30 и т.п.) |
| `Confidence` | `ConfidenceLevel` | Уверенность |
| `Date` | `DateTime` | Время генерации |

---

## 📂 7. PortfolioAnalysisResult

Результат полного анализа по портфелю.

| Поле | Тип | Описание |
|------|-----|----------|
| `Date` | `DateTime` | Когда анализ проводился |
| `Portfolio` | `List<PortfolioAsset>` | Состояние портфеля на момент анализа |
| `Signals` | `List<TradeSignal>` | Список рекомендаций |
| `FundamentalsUsed` | `bool` | Использовались ли фундаментальные метрики |
| `Commentary` | `string` | Краткое описание результата анализа |

---

## 📂 8. ExecutedTrade

Факт исполнения сделки (для истории).

| Поле | Тип | Описание |
|------|-----|----------|
| `Ticker` | `string` | Бумага |
| `Side` | `TradeAction` | Buy / Sell |
| `Quantity` | `decimal` | Количество |
| `Price` | `decimal` | Цена исполнения |
| `ExecutedAt` | `DateTime` | Время |
| `Status` | `string` | Выполнено / Ошибка / Отказ |

---

## 📂 9. BotSettings

Текущие настройки бота.

| Поле | Тип | Описание |
|------|-----|----------|
| `EnableAutoTrading` | `bool` | Разрешено ли исполнять сделки |
| `MaxPositionPerTickerPercent` | `decimal` | Ограничение на тикер |
| `MinTradeAmount` | `decimal` | Минимум на сделку |
| `Schedule` | `CronSchedule` | Расписание |
| `BotStatus` | `enum` | Enabled / Disabled / Maintenance |

---

## 📂 10. CronSchedule

Планировщик задач.

| Поле | Тип | Описание |
|------|-----|----------|
| `Expression` | `string` | cron-строка |
| `Timezone` | `string` | Часовой пояс |
| `NextRun` | `DateTime` | Следующий запуск |

---

## 📂 11. LogEntry

Лог работы пайплайна.

| Поле | Тип | Описание |
|------|-----|----------|
| `Timestamp` | `DateTime` | Когда |
| `Step` | `string` | Название этапа |
| `Message` | `string` | Что произошло |
| `Level` | `string` | Info / Warning / Error |

---

## 📂 Enums

| Имя | Значения |
|-----|----------|
| `TradeAction` | `Buy`, `Sell`, `Hold` |
| `IndicatorType` | `RSI`, `SMA`, `EMA`, `MACD`, `ADX`, ... |
| `SignalReason` | `RSI_LOW`, `RSI_HIGH`, `MACD_CROSS`, `FUNDAMENTALS_GOOD`, ... |
| `ExchangeType` | `TINKOFF`, `SMARTLAB` |
| `ConfidenceLevel` | `Low`, `Medium`, `High` |
| `BotStatus` | `Enabled`, `Disabled`, `Maintenance` |

---
