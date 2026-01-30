


FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore 
RUN dotnet publish --configuration Release --output Release


FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /src/Release .
EXPOSE 8080
EXPOSE 443
ENTRYPOINT ["dotnet","PlatformService.dll"]

