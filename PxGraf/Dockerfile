## Multi-stage Dockerfile for PxGraf (.NET 8)
## Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy solution and restore as distinct layers
COPY PxGraf.csproj .

RUN dotnet restore PxGraf.csproj

# Copy the rest of the source
COPY . .

# Build
# RUN dotnet build PxGraf.csproj -c Release -f net8.0 -o /app/build

# Publish
RUN dotnet publish PxGraf.csproj -c Release -o  /app/publish /p:UseAppHost=false

## Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Expose default ASP.NET Core port
EXPOSE 5000

# Copy published output
COPY --from=build /app/publish .

# Set ASP.NET Core URLs to 0.0.0.0:8080 by default
ENV ASPNETCORE_URLS=http://+:5000

ENTRYPOINT ["dotnet", "PxGraf.dll"]


