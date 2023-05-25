# Start with the .NET SDK image.
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
# Set the working directory.
WORKDIR /app
# Copy the source code.
COPY ./src .
# Restore dependencies.
RUN dotnet restore
# Build the application.
RUN dotnet build -c Release --no-restore
# Publish the application.
RUN dotnet publish -c Release --no-build -o /app/publish
# Start with a smaller runtime image.
FROM mcr.microsoft.com/dotnet/runtime:7.0 AS runtime
# Set the working directory.
WORKDIR /app
# Copy the published output from the build stage.
COPY --from=build /app/publish .
# Set the entry point for the application
ENTRYPOINT ["dotnet", "FrostAura.Intelligence.Iluvatar.Telegram.dll"]