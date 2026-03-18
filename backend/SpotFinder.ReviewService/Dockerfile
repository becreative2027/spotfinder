FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["SpotFinder.ReviewService.API/SpotFinder.ReviewService.API.csproj", "SpotFinder.ReviewService.API/"]
COPY ["SpotFinder.ReviewService.Business/SpotFinder.ReviewService.Business.csproj", "SpotFinder.ReviewService.Business/"]
COPY ["SpotFinder.ReviewService.Data/SpotFinder.ReviewService.Data.csproj", "SpotFinder.ReviewService.Data/"]
RUN dotnet restore "SpotFinder.ReviewService.API/SpotFinder.ReviewService.API.csproj"
COPY . .
WORKDIR "/src/SpotFinder.ReviewService.API"
RUN dotnet build "SpotFinder.ReviewService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SpotFinder.ReviewService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SpotFinder.ReviewService.API.dll"]
