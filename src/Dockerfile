FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Tc.Backend.S4.Payment.Gateway/Tc.Backend.S4.Payment.Gateway.csproj", "Tc.Backend.S4.Payment.Gateway/"]
RUN dotnet restore "Tc.Backend.S4.Payment.Gateway/Tc.Backend.S4.Payment.Gateway.csproj"
COPY . .
WORKDIR "/src/Tc.Backend.S4.Payment.Gateway"
RUN dotnet build "Tc.Backend.S4.Payment.Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Tc.Backend.S4.Payment.Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tc.Backend.S4.Payment.Gateway.dll"]
