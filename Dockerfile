FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443


FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["PhotoShare/Server/PhotoShare.Server.csproj", "PhotoShare/Server/"]
COPY ["PhotoShare/Client/PhotoShare.Client.csproj", "PhotoShare/Client/"]
COPY ["PhotoShare/Shared/PhotoShare.Shared.csproj", "PhotoShare/Shared/"]
#COPY PhotoShare/Server/nuget.config ./PhotoShare/Server/nuget.config
RUN dotnet restore "PhotoShare/Server/PhotoShare.Server.csproj"
COPY . .
WORKDIR "/src/PhotoShare/Server"
RUN dotnet build "PhotoShare.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PhotoShare.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PhotoShare.Server.dll"]