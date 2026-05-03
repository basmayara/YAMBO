FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY YAMBO.ApiGateway/YAMBO.ApiGateway.csproj YAMBO.ApiGateway/
RUN dotnet restore YAMBO.ApiGateway/YAMBO.ApiGateway.csproj
COPY . .
RUN dotnet publish YAMBO.ApiGateway/YAMBO.ApiGateway.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
COPY YAMBO.ApiGateway/ocelot.json /app/ocelot.json
ENTRYPOINT ["dotnet", "YAMBO.ApiGateway.dll"]