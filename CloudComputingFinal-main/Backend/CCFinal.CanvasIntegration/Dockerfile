#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0-jammy AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY "CCFinal.CanvasIntegration.csproj" .
RUN dotnet restore "CCFinal.CanvasIntegration.csproj"
COPY . .
RUN dotnet build "CCFinal.CanvasIntegration.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CCFinal.CanvasIntegration.csproj" -c Release -o /app/publish #/p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CCFinal.CanvasIntegration.dll"]