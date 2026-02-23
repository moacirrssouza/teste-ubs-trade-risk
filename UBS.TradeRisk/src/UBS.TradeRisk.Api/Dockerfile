FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

COPY ["src/UBS.TradeRisk.Api/UBS.TradeRisk.Api.csproj", "src/UBS.TradeRisk.Api/"]
COPY ["src/UBS.TradeRisk.Application/UBS.TradeRisk.Application.csproj", "src/UBS.TradeRisk.Application/"]
COPY ["src/UBS.TradeRisk.Domain/UBS.TradeRisk.Domain.csproj", "src/UBS.TradeRisk.Domain/"]
COPY ["src/UBS.TradeRisk.Infra/UBS.TradeRisk.Infra.csproj", "src/UBS.TradeRisk.Infra/"]

RUN dotnet restore "src/UBS.TradeRisk.Api/UBS.TradeRisk.Api.csproj"

COPY . .
WORKDIR "/src/src/UBS.TradeRisk.Api"

ARG BUILD_CONFIGURATION=Release
RUN dotnet build "UBS.TradeRisk.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "UBS.TradeRisk.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app

RUN addgroup -g 1000 appuser && \
    adduser -D -u 1000 -G appuser appuser

COPY --from=publish --chown=appuser:appuser /app/publish .

EXPOSE 8080
EXPOSE 8081

USER appuser

HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD wget --no-verbose --tries=1 --spider http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "UBS.TradeRisk.Api.dll"]