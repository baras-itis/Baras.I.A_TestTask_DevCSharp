# Vulnerability Scanner API 

REST API сервис на C# для управления уязвимостями программного обеспечения и выполнения асинхронных задач сканирования.

## Запуск

1. Visual Studio 
-  Установить .NET SDK 
-  Откройте решение в Visual Studio 
-  Нажмите f5
-  В консоли появится ссылка на Swagger UI (https://localhost:7125/swagger). Перейдите по ней чтобы протестить API.

2. Запуск через консоль
- откройте решение через консоль 
- напишите `dotnet run`
- в консоли будет строчка `Now listening on: http://localhost:5290`. Перейдите по ссылку, чтобы открыть документацию
- Чтобы выйти `Ctr + C`


## Хранение файлов
- **Постоянного хранилища нету**, так как это требование в тз.
Все данные хранятся в **оперативной памяти** с использованием потокобезопасного `ConcurrentDictionary.
- При перезапуске все очищается
- логи выводятся в консоль

## Эндпоинты API

### 1. Добавление новой уязвимости
* **Метод:** `POST /api/vulnerabilities`
* **Тело запроса:**

```json
  {
    "name": "SQL Injection",
    "severity": "High"
  }
```

**Доступные уровни**: Low, Medium, High, Critical
**Ответ**:
```json
{ "id": "guid" }
```

### 2. Получение информации об уязвимости
- Метод: `GET /api/vulnerabilities/{id}`
- Ответ: Полная информация об уязвимости (ID, название, уровень, дата создания).

### 3. Запуск фонового сканирования
- Метод: `POST /api/scans`
- Тело запроса:
```json
{
  "repositoryUrl": "https://gitlab.com/test/repo"
}
```
- Ответ:
 ```json 
{ "taskId": "guid" }
```
-Особенности: Сканирование выполняется асинхронно, Задача возвращает ID, а обработка идет в `BackgroundService`

### 4. Получение статуса сканирования
- Метод: GET /api/scans/{id}
- Ответ:
```json
{
  "status": "Completed",
  "lastStatusChange": "2023-10-27T10:00:00Z",
  "result": "Анализ завершен. Уязвимостей не обнаружено."
}
```
- Статусы: Pending, Running, Completed, Error

## Тестирование

- В Visual Studio откройте Тест (Test) -> Обозреватель тестов (Test Explorer).
- Нажмите «Запустить все тесты» (Run All).
- Все тесты (6 штук) должны быть зелеными.

## Архитектр Проекта

- **Models**: Cущности `Vulnerability`, `ScanTask` и перечисления `SeverityLevel`, `ScanStatus`.
- **Services**: 
    - `InMemoryStore` - хранилище в памяти
    - `ScanQueue` - очередь задач
    - `ScanBackgroundWorker` - фоновый сервис, забирающий задачи из очереди и  имитирующий сканирование   
- **Controllers**:
    - `VulnerabilitiesController`
    - `ScansController` 
- **MiddleWare**:
    - `GlobalExceptionHandler` - глобальных перехватчик ошибок, возвращает `ProblemDetails`
- **Tests*:
    - `Baras.Test.Tests` - использует `xUnit`, `moq`