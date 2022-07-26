#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["FinancialChat/FinancialChat.csproj", "FinancialChat/"]
COPY ["FinancialChat.Domain/FinancialChat.Domain.csproj", "FinancialChat.Domain/"]
COPY ["StockBot.Messages/StockBot.Messages.csproj", "StockBot.Messages/"]
RUN dotnet restore "FinancialChat/FinancialChat.csproj"
COPY . .
WORKDIR "/src/FinancialChat"
RUN dotnet build "FinancialChat.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FinancialChat.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FinancialChat.dll"]