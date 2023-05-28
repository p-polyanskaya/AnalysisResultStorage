FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR .
COPY ["Endpoint/Endpoint.csproj", "Endpoint/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Consumers/Consumers.csproj", "Consumers/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["GrpcServices/GrpcServices.csproj", "GrpcServices/"]
COPY ["Options/Options.csproj", "Options/"]
COPY ["Metrics/Metrics.csproj", "Metrics/"]
RUN dotnet restore "Endpoint/Endpoint.csproj"
COPY . .
RUN dotnet build "Endpoint/Endpoint.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Endpoint/Endpoint.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Endpoint.dll"]