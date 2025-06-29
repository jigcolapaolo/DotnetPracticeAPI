FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY DotnetPracticeAPI/DotnetPracticeAPI.csproj DotnetPracticeAPI/
WORKDIR /src/DotnetPracticeAPI
RUN dotnet restore

WORKDIR /src
COPY . .

WORKDIR /src/DotnetPracticeAPI
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "DotnetPracticeAPI.dll"]
