# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/Orders/Orders.API/Orders.API.csproj", "Services/Orders/Orders.API/"]
COPY ["Common/Common/Common.csproj", "Common/Common/"]
COPY ["Services/Orders/Orders.Application/Orders.Application/Orders.Application.csproj", "Services/Orders/Orders.Application/Orders.Application/"]
COPY ["Common/Messaging/Common.Messaging/Common.Messaging.csproj", "Common/Messaging/Common.Messaging/"]
COPY ["Services/Orders/Orders.Domain/Orders.Domain/Orders.Core.csproj", "Services/Orders/Orders.Domain/Orders.Domain/"]
COPY ["Services/Orders/Orders.Infrastructure/Orders.Infrastructure/Orders.Infrastructure.csproj", "Services/Orders/Orders.Infrastructure/Orders.Infrastructure/"]
RUN dotnet restore "./Services/Orders/Orders.API/Orders.API.csproj"
COPY . .
WORKDIR "/src/Services/Orders/Orders.API"
RUN dotnet build "./Orders.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Orders.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Orders.API.dll"]