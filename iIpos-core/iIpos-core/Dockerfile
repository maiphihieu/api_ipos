# ===== Build stage =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy csproj đúng đường dẫn
COPY ipos-core/ipos-core/ipos-core.csproj ./ipos-core/
WORKDIR /src/ipos-core
RUN dotnet restore

# copy toàn bộ source
WORKDIR /src
COPY . .

WORKDIR /src/ipos-core/ipos-core
RUN dotnet publish -c Release -o /app/publish

# ===== Runtime stage =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 10000

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:10000
ENTRYPOINT ["dotnet", "ipos-core.dll"]
