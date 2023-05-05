#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
#EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS prebuild
WORKDIR /src
COPY ["CCFinal.csproj", "CCFinal/"]
RUN dotnet restore "CCFinal/CCFinal.csproj"
WORKDIR "/src/CCFinal"
COPY . .

FROM prebuild AS build
RUN dotnet build "CCFinal.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CCFinal.csproj" -c Release -o /app/publish --no-restore #/p:UseAppHost=false

FROM base AS final
RUN apt update
RUN apt install --yes curl
HEALTHCHECK CMD curl --fail http://localhost/healthcheck | grep -E '^{\"status\":\"Healthy\",' || exit 1
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CCFinal.dll", "--urls", "http://0.0.0.0:80"]