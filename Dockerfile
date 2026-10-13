# Imagen del Servidor CAD (Render Web Service, ADR-05)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY global.json Directory.Build.props ./
COPY src/Cad.Contratos/Cad.Contratos.csproj src/Cad.Contratos/
COPY src/Cad.Server/Cad.Server.csproj src/Cad.Server/
RUN dotnet restore src/Cad.Server/Cad.Server.csproj
COPY src/Cad.Contratos/ src/Cad.Contratos/
COPY src/Cad.Server/ src/Cad.Server/
RUN dotnet publish src/Cad.Server/Cad.Server.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
USER $APP_UID
ENTRYPOINT ["dotnet", "Cad.Server.dll"]
