#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["StockBot.Service/StockBot.Service.csproj", "StockBot.Service/"]
COPY ["FinancialChat.Domain/FinancialChat.Domain.csproj", "FinancialChat.Domain/"]
COPY ["StockBot.Messages/StockBot.Messages.csproj", "StockBot.Messages/"]
RUN dotnet restore "StockBot.Service/StockBot.Service.csproj"
COPY . .
WORKDIR "/src/StockBot.Service"
RUN dotnet build "StockBot.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StockBot.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StockBot.Service.dll"]