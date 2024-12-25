################################################################################
# ПЕРЕМЕННЫЕ (примерные)
################################################################################

# Список директорий с микросервисами
SERVICES = ServiceA ServiceB ServiceC

# Корневая папка, где лежат директории сервисов
SERVICES_PATH = ./services

# Конфигурация .NET по умолчанию
CONFIGURATION = Debug

# Тег для локальных Docker-образов (например: myapp, myregistry/myapp)
DOCKER_TAG = myapp

################################################################################
# Команды для ВСЕХ микросервисов (.NET)
################################################################################

## build-all
## Собрать все микросервисы (по умолчанию Debug; используйте CONFIGURATION=Release)
build-all:
	@echo "Собираем все сервисы в $(CONFIGURATION) конфигурации..."
	@for s in $(SERVICES); do \
		echo " -> Сборка $$s..."; \
		dotnet build $(SERVICES_PATH)/$$s -c $(CONFIGURATION); \
	done

## run-all
## Запустить все микросервисы ПАРАЛЛЕЛЬНО (обычный режим)
run-all:
	@echo "Запускаем все микросервисы ПАРАЛЛЕЛЬНО (обычный режим)..."
	@echo "Подсказка: для продакшена обычно используют Docker/K8s/docker-compose и т.д."
	@$(foreach s, $(SERVICES), \
		( dotnet run --project $(SERVICES_PATH)/$(s) -c $(CONFIGURATION) & ) \
		;)
	@echo "Все процессы запущены. Для остановки используйте Ctrl+C или kill."

## watch-all
## Запускает все сервисы последовательно в режиме hot reload
## (На практике hot reload для множества сервисов редко удобен; обычно для одного)
watch-all:
	@echo "Запускаем все сервисы ПОСЛЕДОВАТЕЛЬНО (hot reload)..."
	@for s in $(SERVICES); do \
		echo " -> Hot reload для $$s..."; \
		dotnet watch --project $(SERVICES_PATH)/$$s run; \
	done

## debug-all
## Запустить все микросервисы в режиме отладки (Debug)
debug-all:
	@echo "Запускаем все микросервисы в режиме отладки..."
	@$(foreach s, $(SERVICES), \
		( dotnet run --project $(SERVICES_PATH)/$(s) -c Debug & ) \
		;)
	@echo "Все процессы в Debug. Подключайтесь из IDE к каждому порту/процессу."

################################################################################
# Команды для ОДНОГО микросервиса (указывать: SERVICE=<ИмяПапки>)
################################################################################

## build
## Сборка конкретного микросервиса: make build SERVICE=ServiceA
build:
ifndef SERVICE
	$(error Необходимо указать SERVICE=<ИмяПапкиСервиса>)
endif
	@echo "Собираем сервис: $(SERVICE) (конфигурация: $(CONFIGURATION))"
	dotnet build $(SERVICES_PATH)/$(SERVICE) -c $(CONFIGURATION)

## run
## Запуск конкретного микросервиса: make run SERVICE=ServiceA
run:
ifndef SERVICE
	$(error Необходимо указать SERVICE=<ИмяПапкиСервиса>)
endif
	@echo "Запускаем сервис: $(SERVICE) в конфигурации $(CONFIGURATION)"
	dotnet run --project $(SERVICES_PATH)/$(SERVICE) -c $(CONFIGURATION)

## watch
## Запуск конкретного микросервиса с hot reload: make watch SERVICE=ServiceA
watch:
ifndef SERVICE
	$(error Необходимо указать SERVICE=<ИмяПапкиСервиса>)
endif
	@echo "Hot reload для сервиса: $(SERVICE)"
	dotnet watch --project $(SERVICES_PATH)/$(SERVICE) run

## debug
## Запуск конкретного сервиса в режиме отладки: make debug SERVICE=ServiceA
debug:
ifndef SERVICE
	$(error Необходимо указать SERVICE=<ИмяПапкиСервиса>)
endif
	@echo "Запуск $(SERVICE) в режиме Debug..."
	dotnet run --project $(SERVICES_PATH)/$(SERVICE) -c Debug

################################################################################
# Создание нового микросервиса (шаблон webapi)
################################################################################

## create-service
## Пример: make create-service NAME=SomeNewService
## При желании используйте любой шаблон (webapi, worker и т.д.)
create-service:
ifndef NAME
	$(error Укажите имя нового сервиса: make create-service NAME=MyNewService)
endif
	@echo "Создаем новый сервис: $(NAME)"
	dotnet new webapi -n $(NAME) -o $(SERVICES_PATH)/$(NAME)
	@echo "Готово! Не забудьте добавить $(NAME) в список SERVICES в Makefile."
	@echo "А также создать Dockerfile в папке $(SERVICES_PATH)/$(NAME), если нужно."

################################################################################
# Docker / Docker Compose
################################################################################

## docker-build-all
## Собрать Docker-образы для всех микросервисов (локально)
docker-build-all:
	@echo "Собираем Docker-образы для всех сервисов..."
	@for s in $(SERVICES); do \
		echo " -> Docker build для $$s..."; \
		docker build \
			-t $(DOCKER_TAG)/$$s:latest \
			$(SERVICES_PATH)/$$s; \
	done
	@echo "Все образы собраны локально (с тегом: $(DOCKER_TAG)/<Service>:latest)."

## docker-build
## Собрать Docker-образ для конкретного сервиса: make docker-build SERVICE=ServiceA
docker-build:
ifndef SERVICE
	$(error Укажите SERVICE=<ИмяПапкиСервиса>)
endif
	@echo "Собираем Docker-образ для сервиса: $(SERVICE)"
	docker build \
		-t $(DOCKER_TAG)/$(SERVICE):latest \
		$(SERVICES_PATH)/$(SERVICE)

## docker-up
## Запуск всего стека через docker-compose (фоново, -d)
docker-up:
	@echo "Запуск docker-compose up (в фоновом режиме)..."
	docker compose up -d --build
	@echo "Все сервисы запущены в контейнерах. Смотрите: docker ps"

## docker-down
## Остановить и удалить все контейнеры docker-compose
docker-down:
	@echo "Останавливаем и удаляем все контейнеры docker-compose..."
	docker compose down

## docker-stop
## Остановить все контейнеры (но НЕ удалять)
docker-stop:
	@echo "Останавливаем все контейнеры (без удаления)..."
	docker compose stop

## docker-logs
## Просмотр логов всех сервисов (docker-compose logs -f)
docker-logs:
	@echo "Показываем логи всех контейнеров (нажмите Ctrl+C для выхода)..."
	docker compose logs -f

## docker-push
## Пример команды для пуша локальных образов в регистр
docker-push:
	@echo "Публикуем собранные Docker-образы в регистр (пример)"
	@for s in $(SERVICES); do \
		echo " -> push $(DOCKER_TAG)/$$s:latest"; \
		docker push $(DOCKER_TAG)/$$s:latest; \
	done

## docker-pull
## Пример команды для скачивания образов из регистра
docker-pull:
	@echo "Тянем образы из регистра (пример)"
	@for s in $(SERVICES); do \
		echo " -> pull $(DOCKER_TAG)/$$s:latest"; \
		docker pull $(DOCKER_TAG)/$$s:latest; \
	done
