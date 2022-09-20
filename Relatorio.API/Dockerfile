#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Relatorio.API/Relatorio.csproj", "src/Relatorio.API/"]
COPY ["src/Relatorio.Core/Relatorio.Core.csproj", "src/Relatorio.Core/"]
COPY ["src/Relatorio.Data/Relatorio.Data.csproj", "src/Relatorio.Data/"]
RUN dotnet restore "src/Relatorio.API/Relatorio.csproj"
COPY . .
WORKDIR "/src/src/Relatorio.API"
RUN dotnet build "Relatorio.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Relatorio.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Relatorio.dll"]