# 1. Берём официальный .NET SDK для сборки
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

WORKDIR /app

# 2. Копируем файл проекта и восстанавливаем зависимости
COPY *.csproj ./
RUN dotnet restore

# 3. Копируем остальные файлы и публикуем проект
COPY . ./
RUN dotnet publish -c Release -o out

# 4. Используем минимальный runtime для запуска приложения
FROM mcr.microsoft.com/dotnet/runtime:7.0
WORKDIR /app
COPY --from=build /app/out .

# 5. Команда для запуска бота
CMD ["dotnet", "BotProject.dll"]
