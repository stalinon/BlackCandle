| CI / Code Style | Coverage | .NET | Code Quality | 🐞 Issues | 📦 Size | ⏱ Last Commit |
|-----------------|----------|------|---------------|-----------|---------|----------------|
| ![CI](https://github.com/stalinon/BlackCandle/actions/workflows/ci.yml/badge.svg?branch=main&label=CI+Build/Test) | [![Coverage](https://coveralls.io/repos/github/stalinon/BlackCandle/badge.svg?branch=main)](https://coveralls.io/github/stalinon/BlackCandle?branch=main) | ![.NET](https://img.shields.io/badge/.NET-8.0-blueviolet) | ![Maintainability](https://img.shields.io/badge/code--quality-strict-critical?color=black) | ![Issues](https://img.shields.io/github/issues/stalinon/BlackCandle) | ![Repo Size](https://img.shields.io/github/repo-size/stalinon/BlackCandle) | ![Last Commit](https://img.shields.io/github/last-commit/stalinon/BlackCandle) |

---

Система анализа инвестиционного портфеля с расчётом технических и фундаментальных метрик, генерацией торговых сигналов и доставкой уведомлений в Telegram. Запуск по расписанию или вручную через API. Подходит для автоматизации принятия инвестиционных решений.

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
