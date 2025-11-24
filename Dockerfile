FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY *.csproj .
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app --self-contained true -r linux-x64

FROM debian:11-slim AS runtime
WORKDIR /app
COPY --from=build /app .

CMD ["./BotProject"]
