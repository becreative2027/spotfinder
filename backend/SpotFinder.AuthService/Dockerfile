FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["SpotFinder.AuthService.API/SpotFinder.AuthService.API.csproj", "SpotFinder.AuthService.API/"]
COPY ["SpotFinder.AuthService.Business/SpotFinder.AuthService.Business.csproj", "SpotFinder.AuthService.Business/"]
COPY ["SpotFinder.AuthService.Data/SpotFinder.AuthService.Data.csproj", "SpotFinder.AuthService.Data/"]
RUN dotnet restore "SpotFinder.AuthService.API/SpotFinder.AuthService.API.csproj"
COPY . .
WORKDIR "/src/SpotFinder.AuthService.API"
RUN dotnet build "SpotFinder.AuthService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SpotFinder.AuthService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SpotFinder.AuthService.API.dll"]
