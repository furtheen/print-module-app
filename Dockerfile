FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj first
COPY PrintModuleApp/PrintModuleApp.csproj PrintModuleApp/
WORKDIR /src/PrintModuleApp
RUN dotnet restore

# Copy everything else
WORKDIR /src
COPY . .
WORKDIR /src/PrintModuleApp

RUN dotnet publish -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "PrintModuleApp.dll"]