FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["SpotFinder.VenueService.API/SpotFinder.VenueService.API.csproj", "SpotFinder.VenueService.API/"]
COPY ["SpotFinder.VenueService.Business/SpotFinder.VenueService.Business.csproj", "SpotFinder.VenueService.Business/"]
COPY ["SpotFinder.VenueService.Data/SpotFinder.VenueService.Data.csproj", "SpotFinder.VenueService.Data/"]
RUN dotnet restore "SpotFinder.VenueService.API/SpotFinder.VenueService.API.csproj"
COPY . .
WORKDIR "/src/SpotFinder.VenueService.API"
RUN dotnet build "SpotFinder.VenueService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SpotFinder.VenueService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SpotFinder.VenueService.API.dll"]
