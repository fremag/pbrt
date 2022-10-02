FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["pbrt-runner/pbrt-runner.csproj", "pbrt-runner/"]
RUN dotnet restore "pbrt-runner/pbrt-runner.csproj"
COPY . .
WORKDIR "/src/pbrt-runner"
RUN dotnet build "pbrt-runner.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "pbrt-runner.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "pbrt-runner.dll"]
