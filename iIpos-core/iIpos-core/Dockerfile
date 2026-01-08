FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY iIpos-core.csproj .
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
EXPOSE 10000

COPY --from=build /out .
ENV ASPNETCORE_URLS=http://0.0.0.0:10000
ENTRYPOINT ["dotnet", "iIpos-core.dll"]
