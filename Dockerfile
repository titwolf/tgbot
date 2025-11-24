
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY *.csproj ./
RUN dotnet restore
COPY . ./
RUN dotnet publish -c Release -o /app --self-contained true -r linux-x64
FROM debian:11-slim
WORKDIR /app
COPY --from=build /app ./
RUN apt-get update && \
    apt-get install -y libicu67 libssl3 libgdiplus && \
    rm -rf /var/lib/apt/lists/*
CMD ["./BotProject"]
