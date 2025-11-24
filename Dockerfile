# 1. Сборка приложения
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Копируем csproj и восстанавливаем зависимости
COPY *.csproj ./
RUN dotnet restore

# Копируем весь проект и публикуем в папку /app
COPY . ./
RUN dotnet publish -c Release -o /app --self-contained true -r linux-x64

# 2. Минимальный runtime
FROM debian:12-slim AS runtime
WORKDIR /app

# Копируем готовое приложение из сборки
COPY --from=build /app ./

# Устанавливаем необходимые библиотеки
RUN apt-get update && \
    apt-get install -y libicu70 libssl3 libgdiplus && \
    rm -rf /var/lib/apt/lists/*

# Запуск бота
CMD ["./BotProject"]
