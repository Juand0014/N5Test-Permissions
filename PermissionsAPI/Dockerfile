FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY PermissionsAPI/PermissionsAPI.sln .

COPY PermissionsAPI/PermissionsAPI.csproj ./PermissionsAPI/
COPY Permissions.Tests/Permissions.Tests.csproj ./Permissions.Tests/

RUN dotnet restore ./PermissionsAPI

COPY PermissionsAPI/. ./PermissionsAPI/
COPY Permissions.Tests/. ./Permissions.Tests/

WORKDIR /src/PermissionsAPI
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "PermissionsAPI.dll"]
