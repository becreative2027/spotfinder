FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["SpotFinder.SearchService.API/SpotFinder.SearchService.API.csproj", "SpotFinder.SearchService.API/"]
COPY ["SpotFinder.SearchService.Business/SpotFinder.SearchService.Business.csproj", "SpotFinder.SearchService.Business/"]
COPY ["SpotFinder.SearchService.Data/SpotFinder.SearchService.Data.csproj", "SpotFinder.SearchService.Data/"]
RUN dotnet restore "SpotFinder.SearchService.API/SpotFinder.SearchService.API.csproj"
COPY . .
WORKDIR "/src/SpotFinder.SearchService.API"
RUN dotnet build "SpotFinder.SearchService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SpotFinder.SearchService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SpotFinder.SearchService.API.dll"]
