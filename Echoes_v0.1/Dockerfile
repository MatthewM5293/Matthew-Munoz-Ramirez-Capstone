FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Echoes_v0.1/Echoes_v0.1.csproj", "Echoes_v0.1/"]
RUN dotnet restore "Echoes_v0.1/Echoes_v0.1.csproj"
COPY . .
WORKDIR "/src/Echoes_v0.1"
RUN dotnet build "Echoes_v0.1.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Echoes_v0.1.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Echoes_v0.1.dll"]
