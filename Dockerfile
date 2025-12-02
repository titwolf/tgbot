# Build stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Копируем csproj и восстанавливаем зависимости
COPY *.csproj ./
RUN dotnet restore

# Копируем всё и публикуем
COPY . ./
RUN dotnet publish -c Release -o /app

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app

COPY --from=build /app ./

# Устанавливаем системные зависимости (для libgdiplus и SSL)
RUN apt-get update && \
    apt-get install -y --no-install-recommends \
      libgdiplus \
      libicu72 \
      libssl3 \
    && rm -rf /var/lib/apt/lists/*

# Порт по умолчанию (Render подставляет свои настройки)
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

ENTRYPOINT ["dotnet", "BotProject.dll"]
