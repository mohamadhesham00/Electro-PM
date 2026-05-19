FROM mcr.microsoft.com/dotnet/sdk:9.0 AS development
WORKDIR /app

# Install EF Core tools globally inside the image
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

# Copy everything from the root directory
COPY . .

# Explicitly restore all packages across all projects
RUN dotnet restore "API/API.csproj"

# Build the API in Release mode so the binaries are generated natively inside the container
RUN dotnet build "API/API.csproj" -c Release

# Give executable permissions to our migration entrypoint script
RUN chmod +x API/entrypoint.sh

# Point straight to our shell execution script
ENTRYPOINT ["./API/entrypoint.sh"]