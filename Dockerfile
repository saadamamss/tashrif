# Tashrif API — production image (.NET 8)
# Railway uses this automatically when present at the branch root.

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish "Application/tashrif.API/tashrif.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
# Railway injects $PORT; Program.cs listens on it when present,
# otherwise the app falls back to its local :5001 default.
ENTRYPOINT ["dotnet", "tashrif.API.dll"]
