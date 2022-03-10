FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ShipmentService/ShipmentService.csproj", "ShipmentService/"]
RUN dotnet restore "ShipmentService/ShipmentService.csproj"
COPY . .
WORKDIR "/src/ShipmentService"
RUN dotnet build "ShipmentService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ShipmentService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ShipmentService.dll"]
