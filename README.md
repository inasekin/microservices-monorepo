﻿# Microservices Monorepo

![CI](https://github.com/inasekin/microservices-monorepo/workflows/CI/badge.svg)
[![Maintainability](https://api.codeclimate.com/v1/badges/abebd1eb315cbe567ca0/maintainability)](https://codeclimate.com/github/inasekin/microservices-monorepo/maintainability)

## О проекте
Этот репозиторий представляет собой монорепозиторий для микросервисного проекта на .NET. Включает несколько микросервисов, инфраструктурные компоненты, базу данных и фронтенд-часть.

## Структура репозитория

- `src/` — исходный код микросервисов и компонентов.
- `frontend/` — исходный код фронтенд-приложения.
- `docker/` — Docker-файлы и вспомогательные скрипты.
- `Makefile` — удобные команды для управления проектом.
- `docs/` — документация к проекту (архитектура, ERD и тд).
- `tests/` — тесты к приложению.

## Работа с подмодулями

Git-модули позволяют добавлять другие Git-репозитории как связанные подмодули. Файл .gitmodules содержит информацию о пути и URL для каждого подмодуля:

```text
   [submodule "frontend"]
   path = frontend
   url = https://github.com/inasekin/bugtracker-frontend
```

### Добавление подмодуля

#### Добавьте подмодуль:
```bash
git submodule add <URL> <путь>
```
#### Инициализируйте и скачайте подмодуль:
```bash
git submodule update --init --recursive
```
#### Обновление изменений в подмодуле
Зайдите в директорию подмодуля:
```bash
cd <путь>
```
Подтяните изменения:
```bash
git pull origin main
```
Вернитесь в главную директорию и коммитьте измененный ссылочный комит:
```bash
cd ../
git add .
git commit -m "feat: update submodule <имя>”
```

## Требования

### Инструменты

1. **Docker и Docker Compose**:
   - Установить [Docker](https://docs.docker.com/get-docker/).
   - Установить [Docker Compose](https://docs.docker.com/compose/install/).

2. **Make**:
   - Make — это инструмент автоматизации. Используется для упрощения запуска команд.
   - Установка на macOS и Linux:
     ```bash
     sudo apt update && sudo apt install make -y # Для Ubuntu/Debian
     brew install make                           # Для macOS
     ```
   - Установка на Windows:
      - Установить [Make для Windows](http://gnuwin32.sourceforge.net/packages/make.htm).
      - Добавить `make` в переменную окружения PATH.

3. **.NET SDK 8.0**:
   - Скачать и установить с официального сайта: [.NET SDK](https://dotnet.microsoft.com/download).

## Шаги запуска

### Запуск инфраструктуры (БД, RabbitMQ и др.)

1. Соберите инфраструктуру:
   ```bash
   make infra-up
   ```
2. Остановите инфраструктуру:
   ```bash
   make infra-down
   ```

### Запуск микросервисов
1. Соберите и запустите микросервисы:
   ```bash
   make services-up
   ```
2. Перезапустите отдельный сервис:
   ```bash
   make restart-service SERVICE=<имя_сервиса>
   ```
3. Остановите микросервисы:
   ```bash
   make services-down
   ```

### Запуск frontend
1. Соберите и запустите фронтенд:
   ```bash
   make frontend-up
   ```
2. Остановите фронтенд:
   ```bash
   make frontend-down
   ```

### Стартовые миграции

#### AuthService:
   ```bash
   dotnet ef migrations add InitAuthDb \
   --project src/Services/AuthService/AuthService.Infrastructure/AuthService.Infrastructure.csproj \
   --startup-project src/Services/AuthService/AuthService.Api/AuthService.Api.csproj \
   --output-dir Migrations
   ```
#### UserService:
   ```bash
   dotnet ef migrations add InitUserDb \
   --project src/Services/UserService/UserService.Infrastructure/UserService.Infrastructure.csproj \
   --startup-project src/Services/UserService/UserService.Api/UserService.Api.csproj \
   --output-dir Migrations
   ```
#### ProjectService:
   ```bash
   dotnet ef migrations add InitProjectDb \
   --project src/Services/ProjectService/ProjectService.Infrastructure/ProjectService.Infrastructure.csproj \
   --startup-project src/Services/ProjectService/ProjectService.Api/ProjectService.Api.csproj \
   --output-dir Migrations
   ```

### Запуск и дебаг отдельных сервисов

При запуске отдельного сервиса swagger будет доступен по адресу - http://localhost:{порт из Properties/launchSettings.json}/api/v1/{префикс сервиса}/swagger/index.html

Пример - http://localhost:6002/api/v1/user/swagger/index.html

Если запуск первый раз, то нужно будет выполнить миграции

1. Билд проекта:
   ```bash
   make build SERVICE=UserService
   ```
2. Запуск проекта:
   ```bash
   make run SERVICE=UserService
   ```
3. Watch проекта:
   ```bash
   make watch SERVICE=UserService
   ```
4. Debug проекта:
   ```bash
   make debug SERVICE=UserService
   ```