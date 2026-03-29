FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# copy everything
COPY . .

# run restore using project file explicitly
RUN dotnet restore PrintModuleApp.csproj

# publish
RUN dotnet publish PrintModuleApp.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/out .

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "PrintModuleApp.dll"]