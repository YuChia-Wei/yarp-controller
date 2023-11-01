FROM ghcr.io/yuchia-wei/otel-dotnet-auto-instrumentation:7.0-booworm-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0-booworm-slim AS build
ARG BuildConfiguration=Debug
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Yarp.ControlPlant.WebApi/Yarp.ControlPlant.WebApi.csproj", "Yarp.ControlPlant.WebApi/"]
RUN dotnet restore "Yarp.ControlPlant/Yarp.ControlPlant.WebApi.csproj"
COPY . .
WORKDIR "/src/Yarp.ControlPlant.WebApi"
RUN dotnet build "Yarp.ControlPlant.WebApi.csproj" -c $BuildConfiguration -o /app/build

FROM build AS publish
ARG BuildConfiguration=Debug
RUN dotnet publish "Yarp.ControlPlant.WebApi.csproj" -c $BuildConfiguration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Yarp.ControlPlant.WebApi.dll"]
