FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["CryptoPortfolio.csproj", "."]
RUN dotnet restore "./CryptoPortfolio.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "CryptoPortfolio.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CryptoPortfolio.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CryptoPortfolio.dll"]
