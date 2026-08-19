# AcademiaX Web API — Render (Docker) deploy image.
# Multi-stage: SDK image restores/builds/publishes, runtime image only ships the
# published output so the final image stays small and has no build tools in it.

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy csproj files first and restore separately so Docker can cache the restore
# layer across builds where only source (not dependencies) changed.
COPY AcademiaX/AcademiaX.csproj AcademiaX/
COPY AcademiaX_Business/AcademiaX_Business.csproj AcademiaX_Business/
COPY AcademiaX_Core/AcademiaX_Core.csproj AcademiaX_Core/
COPY AcademiaX_Data_Access/AcademiaX_Data_Access.csproj AcademiaX_Data_Access/
RUN dotnet restore AcademiaX/AcademiaX.csproj

COPY . .
RUN dotnet publish AcademiaX/AcademiaX.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
# Render sets PORT at runtime; Program.cs reads it and binds Kestrel accordingly.
EXPOSE 8080

ENTRYPOINT ["dotnet", "AcademiaX.dll"]
