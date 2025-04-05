## 🚦 CI & Статусы качества репы

| Проверка         | Статус |
|------------------|--------|
| 💅 Code Style     | ![Style](https://github.com/stalinon/BlackCandle/actions/workflows/ci.yml/badge.svg?branch=main&label=Style) |
| 🏗 Build          | ![Build](https://github.com/stalinon/BlackCandle/actions/workflows/ci.yml/badge.svg?branch=main&label=Build) |
| 🧪 Tests          | ![Tests](https://github.com/stalinon/BlackCandle/actions/workflows/ci.yml/badge.svg?branch=main&label=Tests) |
| 📊 Coverage       | ![Coverage](https://coveralls.io/repos/github/stalinon/BlackCandle/badge.svg?branch=main) |

---

![Maintainability](https://img.shields.io/badge/code--quality-strict-critical?color=black)
![Last Commit](https://img.shields.io/github/last-commit/stalinon/BlackCandle)
![Open Issues](https://img.shields.io/github/issues/stalinon/BlackCandle)
![Repo Size](https://img.shields.io/github/repo-size/stalinon/BlackCandle)

## Общая цель

Создать монолитное приложение для анализа инвестиционного портфеля, получения рыночных данных, расчёта индикаторов, генерации торговых сигналов и уведомлений через Telegram. Система должна запускаться как по расписанию, так и вручную, предоставлять отчётность, быть расширяемой и пригодной для демонстрации в портфолио.

---

## Функциональные требования

### 1. Управление портфелем
- Добавление и удаление активов в портфеле (тикер, количество, цена, дата покупки)
- Получение текущего состава портфеля через API
- Хранение истории портфеля (по желанию)

### 2. Интеграция с источниками данных
- Получение исторических котировок через Tinkoff Invest API и/или MOEX API
- Поддержка обработки OHLCV-данных (open/high/low/close/volume)
- Поддержка как российских, так и иностранных тикеров (если возможно)

### 3. Технический анализ
- Расчёт следующих индикаторов (через Skender.Stock.Indicators):
  - RSI
  - SMA/EMA (разные периоды)
  - MACD
  - ADX
  - (в перспективе — BBands, CCI, Stochastic и др.)
- Построение скоринговой модели по метрикам
- Генерация торговых сигналов на основе анализа

### 4. Генерация торговых рекомендаций
- Формирование списка:
  - Что продать (на основе сигналов, лимитов и просадок)
  - Что купить (на основе ранжирования)
  - Что удерживать
- Учёт ограничений:
  - Лимит на долю одного тикера в портфеле
  - Лимит на отрасль (если доступна классификация)
  - Минимальная сумма сделки (если задана)
- Формирование текстового отчёта в Telegram

### 5. Расписание и планирование
- Запуск анализа по расписанию (cron)
- Возможность запуска вручную через API
- Хранение конфигурации расписания
- Возможность временно отключить бота

### 6. Telegram-уведомления
- Отправка отчётов о текущем состоянии портфеля
- Отправка торговых рекомендаций
- (опционально) Возможность получения отчётов по команде (бот-интерфейс)

### 7. Логирование и аудит
- Логирование всех этапов пайплайна:
  - Получение портфеля
  - Загрузка данных
  - Расчёт индикаторов
  - Генерация сигналов
  - Отправка уведомлений
- Отображение логов на фронтенде
- Хранение логов локально или через Logtail

---

## Нефункциональные требования

### 1. Производительность
- Асинхронные вызовы ко всем внешним API
- Быстрая агрегация результатов анализа

### 2. Масштабируемость
- Возможность разнести части системы при необходимости:
  - Бэкенд → Azure / Railway
  - БД → внешний Postgres
  - Логи → ELK/Logtail

### 3. Безопасность
- Хранение API-ключей в зашифрованном виде или через переменные окружения
- Не хранить чувствительные данные в публичных репозиториях

### 4. Надёжность
- Обработка ошибок API (Retry, Timeout, Circuit Breaker)
- Валидация данных
- Фолбэки при отсутствии данных

### 5. Тестируемость
- Покрытие юнит-тестами сервисов и пайплайнов
- Использование Moq, xUnit
- Имитация внешних сервисов (Tinkoff, MOEX, Telegram)

---

## Потенциальные улучшения

- Визуализация портфеля и сигналов на фронте (React + Chart.js / Recharts)
- Поддержка симулятора сделок для тестирования стратегий
- Расширение набора индикаторов и фундаментальных метрик
- Автосинхронизация с брокерским счётом (если API это позволит)
- Подключение более умных стратегий (на основе AI/ML или генетических алгоритмов — для мазохистов)

---

## Архитектура монолита (Solution structure, но по-умному)

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
