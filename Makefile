################################################################################
# ПЕРЕМЕННЫЕ
################################################################################

# Предполагаем, что есть 3 сервиса: AuthService, ProjectService, UserService.
SERVICES = AuthService ProjectService UserService

# Папка, где лежат эти сервисы
SERVICES_PATH = ./src/Services

# Фронтенд
FRONTEND_PATH = ./frontend

# Конфигурация .NET (Debug/Release)
CONFIGURATION = Debug

# Тег для Docker-образов
DOCKER_TAG = myapp

# Папка для Docker Compose файлов
DOCKER_FOLDER = ./docker

# Docker Compose команда
DOCKER_COMPOSE = docker compose

################################################################################
# Команды для ВСЕХ микросервисов (сборка/запуск)
################################################################################

## build-all
## Собрать все сервисы (используя .Api-проекты)
build-all:
	@echo "Собираем все сервисы..."
	@for s in $(SERVICES); do \
		echo " -> Сборка $$s..."; \
		dotnet build \
			$(SERVICES_PATH)/$$s/$$s.Api/$$s.Api.csproj \
			-c $(CONFIGURATION); \
	done

## run-all
## Запустить все сервисы ПАРАЛЛЕЛЬНО
run-all:
	@echo "Запускаем все микросервисы ПАРАЛЛЕЛЬНО..."
	@$(foreach s, $(SERVICES), \
		( dotnet run \
			--project $(SERVICES_PATH)/$(s)/$(s).Api/$(s).Api.csproj \
			-c $(CONFIGURATION) & ) \
	;)
	@echo "Все процессы запущены. Остановка: Ctrl+C."

## watch-all
## Запускает все сервисы ПОСЛЕДОВАТЕЛЬНО в hot reload
watch-all:
	@echo "Запускаем все сервисы ПОСЛЕДОВАТЕЛЬНО (hot reload)..."
	@for s in $(SERVICES); do \
		echo " -> Hot reload для $$s..."; \
		dotnet watch \
			--project $(SERVICES_PATH)/$$s/$$s.Api/$$s.Api.csproj run; \
	done

## debug-all
## Запустить все сервисы в режиме Debug
debug-all:
	@echo "Запускаем все микросервисы в режиме отладки..."
	@$(foreach s, $(SERVICES), \
		( dotnet run \
			--project $(SERVICES_PATH)/$(s)/$(s).Api/$(s).Api.csproj \
			-c Debug & ) \
	;)
	@echo "Все процессы в Debug. Подключайтесь из IDE к каждому порту/процессу."

################################################################################
# Команды для ОДНОГО микросервиса (указывать SERVICE=AuthService и т.д.)
################################################################################

## build
build:
ifndef SERVICE
	$(error "Укажите SERVICE=<ИмяСервиса> (AuthService, ProjectService, ...)")
endif
	@echo "Собираем сервис: $(SERVICE)"
	dotnet build --launch-profile "$(SERVICE)" $(SERVICES_PATH)/$(SERVICE)/$(SERVICE).Api/$(SERVICE).Api.csproj -c $(CONFIGURATION)

## run
run:
ifndef SERVICE
	$(error "Укажите SERVICE=<ИмяСервиса>")
endif
	@echo "Запускаем сервис: $(SERVICE)"
	dotnet run --launch-profile "$(SERVICE)" --project $(SERVICES_PATH)/$(SERVICE)/$(SERVICE).Api/$(SERVICE).Api.csproj -c $(CONFIGURATION)

## watch
watch:
ifndef SERVICE
	$(error "Укажите SERVICE=<ИмяСервиса>")
endif
	@echo "Hot reload для сервиса: $(SERVICE)"
	dotnet watch --launch-profile "$(SERVICE)" --project $(SERVICES_PATH)/$(SERVICE)/$(SERVICE).Api/$(SERVICE).Api.csproj run

## debug
debug:
ifndef SERVICE
	$(error "Укажите SERVICE=<ИмяСервиса>")
endif
	@echo "Запуск сервиса: $(SERVICE) в Debug"
	dotnet run --launch-profile "$(SERVICE)" --project $(SERVICES_PATH)/$(SERVICE)/$(SERVICE).Api/$(SERVICE).Api.csproj -c Debug

################################################################################
# MIGRATE-ALL
################################################################################

## migrate-all
## Выполняем EF миграции для каждого сервиса
migrate-all:
	@for s in $(SERVICES); do \
		echo "-> Выполняем EF миграции для $$s"; \
		dotnet ef database update \
		  --project $(SERVICES_PATH)/$$s/$$s.Infrastructure/$$s.Infrastructure.csproj \
		  --startup-project $(SERVICES_PATH)/$$s/$$s.Api/$$s.Api.csproj \
		  || true; \
	done

################################################################################
# Docker Compose (разделённые)
################################################################################

## infra-up
## Запускает инфраструктурные сервисы (БД, RabbitMQ, pgAdmin)
infra-up:
	$(DOCKER_COMPOSE) -f $(DOCKER_FOLDER)/docker-compose.infra.yml up -d --build

## infra-down
## Останавливает инфраструктурные сервисы
infra-down:
	$(DOCKER_COMPOSE) -f $(DOCKER_FOLDER)/docker-compose.infra.yml down

## infra-restart
## Перезапускает инфраструктурные сервисы
infra-restart:
	$(MAKE) infra-down
	$(MAKE) infra-up

## services-up
## Запускает микросервисы
services-up:
	$(DOCKER_COMPOSE) -f $(DOCKER_FOLDER)/docker-compose.services.yml up -d --build

## services-down
## Останавливает микросервисы
services-down:
	$(DOCKER_COMPOSE) -f $(DOCKER_FOLDER)/docker-compose.services.yml down

## services-restart
## Перезапускает микросервисы
services-restart:
	$(MAKE) services-down
	$(MAKE) services-up

## restart-service
## Перезапускает конкретный микросервис
restart-service:
ifndef SERVICE
	$(error "Укажите SERVICE=<ИмяСервиса> для перезапуска")
endif
	$(DOCKER_COMPOSE) -f $(DOCKER_FOLDER)/docker-compose.services.yml restart $(SERVICE)

################################################################################
# Фронтенд
################################################################################

## frontend-up
## Запускает фронтенд в режиме разработки
frontend-up:
	@echo "Запускаем фронтенд на порту 3000 (режим разработки)..."
	$(DOCKER_COMPOSE) -f ./docker/docker-compose.frontend.yml up --build -d
	
## frontend-up
## Запускает фронтенд в режиме разработки
frontend-up-dev:
	@echo "Запускаем фронтенд на порту 3000 (режим разработки)..."
	$(DOCKER_COMPOSE) -f ./docker/docker-compose.frontend.yml up --build

## frontend-down
## Останавливает фронтенд
frontend-down:
	@echo "Останавливаем фронтенд..."
	$(DOCKER_COMPOSE) -f ./docker/docker-compose.frontend.yml down -v

################################################################################
# Все вместе
################################################################################

## all-up
## Запускает инфраструктуру, микросервисы и фронтенд
all-up:
	$(MAKE) infra-up
	$(MAKE) services-up
	$(MAKE) frontend-up

## all-down
## Останавливает все (инфраструктуру, микросервисы и фронтенд)
all-down:
	$(MAKE) frontend-down
	$(MAKE) services-down
	$(MAKE) infra-down
