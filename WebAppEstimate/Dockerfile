FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["WebAppEstimate.csproj", "./"]
RUN dotnet restore "WebAppEstimate.csproj"

COPY . .
RUN dotnet publish "WebAppEstimate.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "WebAppEstimate.dll"]