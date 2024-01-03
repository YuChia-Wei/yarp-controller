FROM ghcr.io/yuchia-wei/otel-dotnet-auto-instrumentation:8.0-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 8080
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG BuildConfiguration=Debug
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Yarp.Gateway/Yarp.Gateway.csproj", "Yarp.Gateway/"]
RUN dotnet restore "Yarp.Gateway/Yarp.Gateway.csproj"
COPY . .
WORKDIR "/src/Yarp.Gateway"
RUN dotnet build "Yarp.Gateway.csproj" -c $BuildConfiguration -o /app/build

FROM build AS publish
ARG BuildConfiguration=Debug
RUN dotnet publish "Yarp.Gateway.csproj" -c $BuildConfiguration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Yarp.Gateway.dll"]
