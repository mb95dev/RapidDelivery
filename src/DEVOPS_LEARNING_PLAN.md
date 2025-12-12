# 🚀 DevOps Learning Plan - Docker, Kubernetes, IaC & CI/CD

## Your Focused Learning Path

```
Week 1-2: Docker Deep Dive
    ↓
Week 3-4: GitHub Actions CI/CD
    ↓
Week 5-7: Kubernetes
    ↓
Week 8-9: Infrastructure as Code (Terraform)
    ↓
Week 10: Integration & Production Deployment
```

---

# 📦 PHASE 1: Docker Deep Dive (Week 1-2)

## Prerequisites
- [x] Docker Desktop installed
- [x] Basic docker-compose working (you have this!)

---

## Week 1: Docker Fundamentals

### Day 1-2: Understanding Docker Concepts

#### 🎯 Learning Goals:
- [ ] Understand Images vs Containers
- [ ] Learn Docker architecture (Client, Daemon, Registry)
- [ ] Master Dockerfile instructions

#### 📖 Theory: Docker Architecture
```
┌─────────────────────────────────────────────────────────────┐
│                        Docker Host                          │
│  ┌─────────────────────────────────────────────────────┐   │
│  │                   Docker Daemon                      │   │
│  │  ┌─────────┐  ┌─────────┐  ┌─────────┐             │   │
│  │  │Container│  │Container│  │Container│             │   │
│  │  │Orders   │  │Notif    │  │RabbitMQ │             │   │
│  │  └─────────┘  └─────────┘  └─────────┘             │   │
│  │                                                      │   │
│  │  Images: [orders-api] [notifications-api] [rabbitmq]│   │
│  └─────────────────────────────────────────────────────┘   │
│                           ▲                                 │
│                           │                                 │
│  ┌─────────────────────────────────────────────────────┐   │
│  │              Docker Client (CLI)                     │   │
│  │   docker build, docker run, docker-compose          │   │
│  └─────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
                            │
                            ▼
              ┌─────────────────────────┐
              │    Docker Registry      │
              │  (Docker Hub, GHCR)     │
              └─────────────────────────┘
```

#### 🛠️ Hands-on Tasks:

##### Task 1.1: Explore Your Current Setup
```powershell
# List all images
docker images

# List running containers
docker ps

# List all containers (including stopped)
docker ps -a

# Inspect your orders-api image
docker inspect ordersapi

# View image layers
docker history ordersapi
```

##### Task 1.2: Understand Dockerfile Instructions
Create a cheat sheet by analyzing your Dockerfile:

```dockerfile
# Base image - the starting point
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS base

# Set working directory inside container
WORKDIR /app

# Expose ports (documentation, doesn't actually publish)
EXPOSE 8080

# Copy files from build context to container
COPY . .

# Run commands during build
RUN dotnet build

# Set the startup command
ENTRYPOINT ["dotnet", "Orders.API.dll"]
```

| Instruction | Purpose | When Executed |
|-------------|---------|---------------|
| FROM | Base image | Build time |
| WORKDIR | Set directory | Build time |
| COPY | Copy files | Build time |
| RUN | Execute command | Build time |
| EXPOSE | Document ports | Build time (metadata) |
| ENV | Set environment var | Build time |
| ENTRYPOINT | Container startup | Run time |
| CMD | Default arguments | Run time |

---

### Day 3-4: Multi-Stage Builds & Optimization

#### 🎯 Learning Goals:
- [ ] Understand multi-stage builds
- [ ] Optimize image size
- [ ] Implement build caching

#### 🛠️ Task 1.3: Create Optimized Dockerfile

Create new optimized Dockerfile for Orders.API:

```dockerfile
# ===========================================
# Stage 1: Build
# ===========================================
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

# Copy only project files first (better caching)
COPY ["Services/Orders/Orders.API/Orders.API.csproj", "Services/Orders/Orders.API/"]
COPY ["Services/Orders/Orders.Application/Orders.Application/Orders.Application.csproj", "Services/Orders/Orders.Application/Orders.Application/"]
COPY ["Services/Orders/Orders.Domain/Orders.Domain/Orders.Core.csproj", "Services/Orders/Orders.Domain/Orders.Domain/"]
COPY ["Services/Orders/Orders.Infrastructure/Orders.Infrastructure/Orders.Infrastructure.csproj", "Services/Orders/Orders.Infrastructure/Orders.Infrastructure/"]
COPY ["Common/Common/Common.csproj", "Common/Common/"]
COPY ["Common/Messaging/Common.Messaging/Common.Messaging.csproj", "Common/Messaging/Common.Messaging/"]

# Restore dependencies (cached if .csproj unchanged)
RUN dotnet restore "Services/Orders/Orders.API/Orders.API.csproj"

# Copy everything else
COPY . .

# Build the application
WORKDIR "/src/Services/Orders/Orders.API"
RUN dotnet build "Orders.API.csproj" -c Release -o /app/build

# ===========================================
# Stage 2: Publish
# ===========================================
FROM build AS publish
RUN dotnet publish "Orders.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ===========================================
# Stage 3: Runtime (Final)
# ===========================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS final

# Security: Run as non-root user
USER $APP_UID

WORKDIR /app

# Copy only published output (small image!)
COPY --from=publish /app/publish .

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Expose port
EXPOSE 8080

# Start application
ENTRYPOINT ["dotnet", "Orders.API.dll"]
```

#### 🛠️ Task 1.4: Compare Image Sizes
```powershell
# Build original
docker build -t orders-api:original -f Services/Orders/Orders.API/Dockerfile.original .

# Build optimized
docker build -t orders-api:optimized -f Services/Orders/Orders.API/Dockerfile .

# Compare sizes
docker images | Select-String "orders-api"
```

---

### Day 5-7: Docker Compose Advanced

#### 🎯 Learning Goals:
- [ ] Master docker-compose.yml structure
- [ ] Implement multiple environments
- [ ] Use volumes, networks, and secrets

#### 🛠️ Task 1.5: Create Environment-Specific Compose Files

**File Structure:**
```
src/
├── docker-compose.yml           # Base configuration
├── docker-compose.override.yml  # Development (auto-loaded)
├── docker-compose.prod.yml      # Production
└── docker-compose.test.yml      # Testing
```

**Base Configuration (docker-compose.yml):**
```yaml
# Base configuration - shared across all environments
services:
  orders-api:
    image: ${DOCKER_REGISTRY:-}orders-api:${TAG:-latest}
    build:
      context: .
      dockerfile: Services/Orders/Orders.API/Dockerfile
    depends_on:
      ordersdb:
        condition: service_healthy
      messagebroker:
        condition: service_healthy
    networks:
      - rapiddelivery-network

  notifications-api:
    image: ${DOCKER_REGISTRY:-}notifications-api:${TAG:-latest}
    build:
      context: .
      dockerfile: Services/Notifications/Notifications.API/Dockerfile
    depends_on:
      messagebroker:
        condition: service_healthy
    networks:
      - rapiddelivery-network

  messagebroker:
    image: rabbitmq:3-management-alpine
    hostname: rabbitmq
    healthcheck:
      test: rabbitmq-diagnostics -q ping
      interval: 10s
      timeout: 5s
      retries: 5
    networks:
      - rapiddelivery-network

  ordersdb:
    image: mcr.microsoft.com/mssql/server:2022-latest
    healthcheck:
      test: /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$$SA_PASSWORD" -C -Q "SELECT 1" || exit 1
      interval: 10s
      timeout: 5s
      retries: 5
    networks:
      - rapiddelivery-network

networks:
  rapiddelivery-network:
    driver: bridge
```

**Development Override (docker-compose.override.yml):**
```yaml
# Development environment - loaded automatically
services:
  orders-api:
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ASPNETCORE_HTTP_PORTS=8080
      - ConnectionStrings__Database=Server=ordersdb;Database=OrdersDb;User Id=sa;Password=SwN12345678;Encrypt=false;TrustServerCertificate=True
      - MessageBroker__Host=amqp://rabbitmq:5672
      - MessageBroker__UserName=guest
      - MessageBroker__Password=guest
    ports:
      - "6000:8080"
    volumes:
      - ${APPDATA}/Microsoft/UserSecrets:/home/app/.microsoft/usersecrets:ro

  notifications-api:
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ASPNETCORE_HTTP_PORTS=8080
      - MessageBroker__Host=amqp://rabbitmq:5672
      - MessageBroker__UserName=guest
      - MessageBroker__Password=guest
    ports:
      - "6001:8080"

  messagebroker:
    ports:
      - "5672:5672"
      - "15672:15672"
    environment:
      - RABBITMQ_DEFAULT_USER=guest
      - RABBITMQ_DEFAULT_PASS=guest

  ordersdb:
    ports:
      - "1433:1433"
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=SwN12345678
    volumes:
      - ordersdb-data:/var/opt/mssql

volumes:
  ordersdb-data:
```

**Production Configuration (docker-compose.prod.yml):**
```yaml
# Production environment
services:
  orders-api:
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_HTTP_PORTS=8080
    deploy:
      replicas: 3
      resources:
        limits:
          cpus: '0.5'
          memory: 512M
        reservations:
          cpus: '0.25'
          memory: 256M
      restart_policy:
        condition: on-failure
        delay: 5s
        max_attempts: 3
    logging:
      driver: json-file
      options:
        max-size: "10m"
        max-file: "3"

  notifications-api:
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
    deploy:
      replicas: 2
      resources:
        limits:
          cpus: '0.25'
          memory: 256M

  messagebroker:
    deploy:
      resources:
        limits:
          cpus: '0.5'
          memory: 512M
    # No ports exposed in production!

  ordersdb:
    deploy:
      resources:
        limits:
          cpus: '1'
          memory: 2G
    # No ports exposed in production!
```

#### 🛠️ Task 1.6: Running Different Environments
```powershell
# Development (default)
docker-compose up -d

# Production
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d

# Testing
docker-compose -f docker-compose.yml -f docker-compose.test.yml up -d

# View logs
docker-compose logs -f orders-api

# Scale a service
docker-compose up -d --scale orders-api=3
```

---

## Week 1 Checklist
- [ ] Understand Docker architecture
- [ ] Know all Dockerfile instructions
- [ ] Created optimized multi-stage Dockerfile
- [ ] Implemented health checks
- [ ] Created environment-specific compose files
- [ ] Can run dev/prod configurations

---

## Week 2: Docker Registry & Advanced Patterns

### Day 1-2: Container Registry

#### 🎯 Learning Goals:
- [ ] Push images to GitHub Container Registry (GHCR)
- [ ] Understand image tagging strategies
- [ ] Implement image scanning

#### 🛠️ Task 2.1: Setup GitHub Container Registry

```powershell
# Login to GitHub Container Registry
echo $env:GITHUB_TOKEN | docker login ghcr.io -u YOUR_USERNAME --password-stdin

# Tag your image
docker tag orders-api:latest ghcr.io/YOUR_USERNAME/rapiddelivery/orders-api:latest
docker tag orders-api:latest ghcr.io/YOUR_USERNAME/rapiddelivery/orders-api:v1.0.0

# Push to registry
docker push ghcr.io/YOUR_USERNAME/rapiddelivery/orders-api:latest
docker push ghcr.io/YOUR_USERNAME/rapiddelivery/orders-api:v1.0.0
```

#### Image Tagging Strategy:
```
ghcr.io/username/orders-api:latest        # Latest build
ghcr.io/username/orders-api:v1.0.0        # Semantic version
ghcr.io/username/orders-api:main-abc123   # Branch + commit SHA
ghcr.io/username/orders-api:pr-42         # Pull request
```

---

### Day 3-4: Docker Networking Deep Dive

#### 🛠️ Task 2.2: Understand Docker Networks

```powershell
# List networks
docker network ls

# Inspect network
docker network inspect rapiddelivery-src_rapiddelivery-network

# Create custom network
docker network create --driver bridge my-network

# Connect container to network
docker network connect my-network orders-api
```

**Network Types:**
```
┌─────────────────────────────────────────────────────────────┐
│ Bridge Network (Default)                                    │
│ - Containers can communicate via container name             │
│ - Isolated from host network                                │
│ - Good for: Single-host development                         │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│ Host Network                                                │
│ - Container shares host's network                           │
│ - No network isolation                                      │
│ - Good for: Performance-critical apps                       │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│ Overlay Network                                             │
│ - Spans multiple Docker hosts                               │
│ - Used in Docker Swarm                                      │
│ - Good for: Multi-host deployments                          │
└─────────────────────────────────────────────────────────────┘
```

---

### Day 5-7: Docker Best Practices

#### 🛠️ Task 2.3: Security Best Practices Checklist

Create `.dockerignore`:
```
# .dockerignore
**/.git
**/.gitignore
**/bin
**/obj
**/.vs
**/.idea
**/node_modules
**/*.md
**/Dockerfile*
**/docker-compose*
**/.env
**/secrets
```

Security Checklist:
- [ ] Use specific image tags (not `latest` in production)
- [ ] Run as non-root user (`USER $APP_UID`)
- [ ] Don't store secrets in images
- [ ] Scan images for vulnerabilities
- [ ] Use multi-stage builds
- [ ] Minimize image layers
- [ ] Keep base images updated

---

## Week 2 Checklist
- [ ] Pushed images to GHCR
- [ ] Understand tagging strategies
- [ ] Know Docker network types
- [ ] Implemented security best practices
- [ ] Created .dockerignore

---

# 🔄 PHASE 2: GitHub Actions CI/CD (Week 3-4)

## Week 3: CI Pipeline

### Day 1-2: GitHub Actions Fundamentals

#### 🎯 Learning Goals:
- [ ] Understand GitHub Actions concepts
- [ ] Write workflow YAML files
- [ ] Use actions marketplace

#### 📖 Theory: GitHub Actions Concepts
```
┌─────────────────────────────────────────────────────────────┐
│                        Workflow                             │
│  ┌───────────────────────────────────────────────────────┐ │
│  │                      Job 1: Build                      │ │
│  │  ┌─────────┐  ┌─────────┐  ┌─────────┐  ┌─────────┐  │ │
│  │  │ Step 1  │→ │ Step 2  │→ │ Step 3  │→ │ Step 4  │  │ │
│  │  │Checkout │  │ Setup   │  │ Build   │  │ Test    │  │ │
│  │  └─────────┘  └─────────┘  └─────────┘  └─────────┘  │ │
│  └───────────────────────────────────────────────────────┘ │
│                            ↓                                │
│  ┌───────────────────────────────────────────────────────┐ │
│  │                      Job 2: Docker                     │ │
│  │  ┌─────────┐  ┌─────────┐  ┌─────────┐               │ │
│  │  │ Login   │→ │ Build   │→ │ Push    │               │ │
│  │  │ GHCR    │  │ Image   │  │ Image   │               │ │
│  │  └─────────┘  └─────────┘  └─────────┘               │ │
│  └───────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────┘
```

#### 🛠️ Task 3.1: Create CI Workflow

Create `.github/workflows/ci.yml`:

```yaml
name: CI Pipeline

on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main]

env:
  DOTNET_VERSION: '10.0.x'
  REGISTRY: ghcr.io
  IMAGE_NAME: ${{ github.repository }}

jobs:
  # ===========================================
  # Job 1: Build and Test
  # ===========================================
  build:
    name: Build & Test
    runs-on: ubuntu-latest
    defaults:
      run:
        working-directory: ./src

    steps:
      - name: 📥 Checkout code
        uses: actions/checkout@v4

      - name: 🔧 Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: ${{ env.DOTNET_VERSION }}

      - name: 📦 Restore dependencies
        run: dotnet restore rapid-delivery.sln

      - name: 🏗️ Build
        run: dotnet build rapid-delivery.sln --configuration Release --no-restore

      - name: 🧪 Run Unit Tests
        run: dotnet test rapid-delivery.sln --configuration Release --no-build --verbosity normal --collect:"XPlat Code Coverage" --results-directory ./coverage

      - name: 📊 Upload coverage reports
        uses: codecov/codecov-action@v4
        with:
          directory: ./src/coverage
          fail_ci_if_error: false

  # ===========================================
  # Job 2: Build Docker Images
  # ===========================================
  docker:
    name: Build Docker Images
    runs-on: ubuntu-latest
    needs: build
    permissions:
      contents: read
      packages: write

    strategy:
      matrix:
        include:
          - service: orders-api
            dockerfile: Services/Orders/Orders.API/Dockerfile
          - service: notifications-api
            dockerfile: Services/Notifications/Notifications.API/Dockerfile

    steps:
      - name: 📥 Checkout code
        uses: actions/checkout@v4

      - name: 🔧 Set up Docker Buildx
        uses: docker/setup-buildx-action@v3

      - name: 🔑 Log in to Container Registry
        uses: docker/login-action@v3
        with:
          registry: ${{ env.REGISTRY }}
          username: ${{ github.actor }}
          password: ${{ secrets.GITHUB_TOKEN }}

      - name: 📋 Extract metadata
        id: meta
        uses: docker/metadata-action@v5
        with:
          images: ${{ env.REGISTRY }}/${{ github.repository }}/${{ matrix.service }}
          tags: |
            type=ref,event=branch
            type=ref,event=pr
            type=sha,prefix=
            type=raw,value=latest,enable=${{ github.ref == 'refs/heads/main' }}

      - name: 🐳 Build and push Docker image
        uses: docker/build-push-action@v5
        with:
          context: ./src
          file: ./src/${{ matrix.dockerfile }}
          push: ${{ github.event_name != 'pull_request' }}
          tags: ${{ steps.meta.outputs.tags }}
          labels: ${{ steps.meta.outputs.labels }}
          cache-from: type=gha
          cache-to: type=gha,mode=max

  # ===========================================
  # Job 3: Security Scan
  # ===========================================
  security:
    name: Security Scan
    runs-on: ubuntu-latest
    needs: docker
    if: github.event_name != 'pull_request'
    
    steps:
      - name: 📥 Checkout code
        uses: actions/checkout@v4

      - name: 🔍 Run Trivy vulnerability scanner
        uses: aquasecurity/trivy-action@master
        with:
          image-ref: '${{ env.REGISTRY }}/${{ github.repository }}/orders-api:${{ github.sha }}'
          format: 'sarif'
          output: 'trivy-results.sarif'

      - name: 📤 Upload Trivy scan results
        uses: github/codeql-action/upload-sarif@v3
        with:
          sarif_file: 'trivy-results.sarif'
```

---

### Day 3-4: Workflow Features

#### 🛠️ Task 3.2: Add Code Quality Checks

Create `.github/workflows/code-quality.yml`:

```yaml
name: Code Quality

on:
  pull_request:
    branches: [main, develop]

jobs:
  lint:
    name: Lint & Format Check
    runs-on: ubuntu-latest
    defaults:
      run:
        working-directory: ./src

    steps:
      - uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'

      - name: Restore tools
        run: dotnet tool restore

      - name: Check formatting
        run: dotnet format --verify-no-changes --verbosity diagnostic

      - name: Run analyzers
        run: dotnet build --configuration Release /p:TreatWarningsAsErrors=true
```

---

### Day 5-7: Branch Protection & PR Workflows

#### 🛠️ Task 3.3: Setup Branch Protection

Go to GitHub Repository → Settings → Branches → Add rule:

```
Branch name pattern: main

✅ Require a pull request before merging
  ✅ Require approvals: 1
  ✅ Dismiss stale pull request approvals when new commits are pushed

✅ Require status checks to pass before merging
  ✅ Require branches to be up to date before merging
  Status checks:
    - Build & Test
    - Build Docker Images

✅ Require conversation resolution before merging
```

---

## Week 3 Checklist
- [ ] Created CI workflow
- [ ] Build and test on every push
- [ ] Docker images built and pushed
- [ ] Security scanning enabled
- [ ] Code quality checks added
- [ ] Branch protection configured

---

## Week 4: CD Pipeline

### Day 1-3: Deployment Workflow

#### 🛠️ Task 4.1: Create CD Workflow

Create `.github/workflows/cd.yml`:

```yaml
name: CD Pipeline

on:
  push:
    branches: [main]
  workflow_dispatch:
    inputs:
      environment:
        description: 'Environment to deploy to'
        required: true
        default: 'staging'
        type: choice
        options:
          - staging
          - production

env:
  REGISTRY: ghcr.io

jobs:
  # ===========================================
  # Deploy to Staging
  # ===========================================
  deploy-staging:
    name: Deploy to Staging
    runs-on: ubuntu-latest
    environment: staging
    if: github.event_name == 'push' || github.event.inputs.environment == 'staging'

    steps:
      - name: 📥 Checkout code
        uses: actions/checkout@v4

      - name: 🔑 Login to Azure
        uses: azure/login@v2
        with:
          creds: ${{ secrets.AZURE_CREDENTIALS }}

      - name: 🚀 Deploy to Azure Container Apps
        uses: azure/container-apps-deploy-action@v1
        with:
          resourceGroup: rg-rapiddelivery-staging
          containerAppName: orders-api
          imageToDeploy: ${{ env.REGISTRY }}/${{ github.repository }}/orders-api:${{ github.sha }}

      - name: 🧪 Run smoke tests
        run: |
          sleep 30
          curl -f https://orders-api-staging.azurecontainerapps.io/health || exit 1

  # ===========================================
  # Deploy to Production
  # ===========================================
  deploy-production:
    name: Deploy to Production
    runs-on: ubuntu-latest
    environment: production
    needs: deploy-staging
    if: github.event.inputs.environment == 'production'

    steps:
      - name: 📥 Checkout code
        uses: actions/checkout@v4

      - name: 🔑 Login to Azure
        uses: azure/login@v2
        with:
          creds: ${{ secrets.AZURE_CREDENTIALS }}

      - name: 🚀 Deploy to Azure Container Apps
        uses: azure/container-apps-deploy-action@v1
        with:
          resourceGroup: rg-rapiddelivery-prod
          containerAppName: orders-api
          imageToDeploy: ${{ env.REGISTRY }}/${{ github.repository }}/orders-api:${{ github.sha }}
```

---

### Day 4-5: Environment Secrets

#### 🛠️ Task 4.2: Configure GitHub Environments

Go to Repository → Settings → Environments:

**Create Staging Environment:**
```
Name: staging
Environment secrets:
  - AZURE_CREDENTIALS: {"clientId":"...","clientSecret":"..."}
  - DATABASE_CONNECTION_STRING: Server=...
  - RABBITMQ_CONNECTION: amqp://...
```

**Create Production Environment:**
```
Name: production
Required reviewers: [Your username]
Wait timer: 5 minutes
Environment secrets:
  - AZURE_CREDENTIALS: {"clientId":"...","clientSecret":"..."}
  - DATABASE_CONNECTION_STRING: Server=...
  - RABBITMQ_CONNECTION: amqp://...
```

---

### Day 6-7: Complete CI/CD Pipeline

#### 🛠️ Task 4.3: Unified Pipeline

```yaml
# .github/workflows/pipeline.yml
name: Complete Pipeline

on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main]
  release:
    types: [published]

jobs:
  build:
    # ... build job from ci.yml

  docker:
    needs: build
    # ... docker job from ci.yml

  deploy-staging:
    needs: docker
    if: github.ref == 'refs/heads/main'
    # ... staging deployment

  deploy-production:
    needs: deploy-staging
    if: github.event_name == 'release'
    # ... production deployment
```

---

## Week 4 Checklist
- [ ] CD workflow created
- [ ] Staging deployment automated
- [ ] Production deployment with approval
- [ ] Environment secrets configured
- [ ] Smoke tests after deployment
- [ ] Complete pipeline working

---

# ☸️ PHASE 3: Kubernetes (Week 5-7)

## Week 5: Kubernetes Fundamentals

### Day 1-2: Core Concepts

#### 🎯 Learning Goals:
- [ ] Understand Kubernetes architecture
- [ ] Know core objects (Pod, Service, Deployment)
- [ ] Use kubectl effectively

#### 📖 Theory: Kubernetes Architecture
```
┌─────────────────────────────────────────────────────────────────┐
│                     Kubernetes Cluster                          │
│  ┌───────────────────────────────────────────────────────────┐ │
│  │                    Control Plane                           │ │
│  │  ┌─────────┐ ┌─────────┐ ┌─────────┐ ┌─────────────────┐ │ │
│  │  │API      │ │etcd     │ │Scheduler│ │Controller       │ │ │
│  │  │Server   │ │         │ │         │ │Manager          │ │ │
│  │  └─────────┘ └─────────┘ └─────────┘ └─────────────────┘ │ │
│  └───────────────────────────────────────────────────────────┘ │
│                              │                                  │
│         ┌────────────────────┼────────────────────┐            │
│         ▼                    ▼                    ▼            │
│  ┌─────────────┐     ┌─────────────┐     ┌─────────────┐      │
│  │   Node 1    │     │   Node 2    │     │   Node 3    │      │
│  │ ┌─────────┐ │     │ ┌─────────┐ │     │ ┌─────────┐ │      │
│  │ │ kubelet │ │     │ │ kubelet │ │     │ │ kubelet │ │      │
│  │ └─────────┘ │     │ └─────────┘ │     │ └─────────┘ │      │
│  │ ┌─────────┐ │     │ ┌─────────┐ │     │ ┌─────────┐ │      │
│  │ │ Pod     │ │     │ │ Pod     │ │     │ │ Pod     │ │      │
│  │ │ Pod     │ │     │ │ Pod     │ │     │ │ Pod     │ │      │
│  │ └─────────┘ │     │ └─────────┘ │     │ └─────────┘ │      │
│  └─────────────┘     └─────────────┘     └─────────────┘      │
└─────────────────────────────────────────────────────────────────┘
```

#### 🛠️ Task 5.1: Setup Local Kubernetes

```powershell
# Option 1: Enable Kubernetes in Docker Desktop
# Docker Desktop → Settings → Kubernetes → Enable Kubernetes

# Option 2: Install minikube
choco install minikube
minikube start

# Verify installation
kubectl version
kubectl cluster-info
kubectl get nodes
```

---

### Day 3-4: Core Objects

#### 🛠️ Task 5.2: Create Kubernetes Manifests

Create folder structure:
```
k8s/
├── base/
│   ├── namespace.yaml
│   ├── orders-api/
│   │   ├── deployment.yaml
│   │   ├── service.yaml
│   │   ├── configmap.yaml
│   │   └── secret.yaml
│   ├── notifications-api/
│   │   ├── deployment.yaml
│   │   └── service.yaml
│   ├── rabbitmq/
│   │   ├── deployment.yaml
│   │   └── service.yaml
│   └── mssql/
│       ├── deployment.yaml
│       ├── service.yaml
│       └── pvc.yaml
└── overlays/
    ├── dev/
    │   └── kustomization.yaml
    └── prod/
        └── kustomization.yaml
```

**Namespace (k8s/base/namespace.yaml):**
```yaml
apiVersion: v1
kind: Namespace
metadata:
  name: rapiddelivery
  labels:
    app.kubernetes.io/name: rapiddelivery
```

**ConfigMap (k8s/base/orders-api/configmap.yaml):**
```yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: orders-api-config
  namespace: rapiddelivery
data:
  ASPNETCORE_ENVIRONMENT: "Production"
  MessageBroker__Host: "amqp://rabbitmq:5672"
  MessageBroker__UserName: "guest"
```

**Secret (k8s/base/orders-api/secret.yaml):**
```yaml
apiVersion: v1
kind: Secret
metadata:
  name: orders-api-secrets
  namespace: rapiddelivery
type: Opaque
stringData:
  ConnectionStrings__Database: "Server=mssql;Database=OrdersDb;User Id=sa;Password=SwN12345678;Encrypt=false;TrustServerCertificate=True"
  MessageBroker__Password: "guest"
```

**Deployment (k8s/base/orders-api/deployment.yaml):**
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: orders-api
  namespace: rapiddelivery
  labels:
    app: orders-api
spec:
  replicas: 2
  selector:
    matchLabels:
      app: orders-api
  template:
    metadata:
      labels:
        app: orders-api
    spec:
      containers:
        - name: orders-api
          image: ghcr.io/YOUR_USERNAME/rapiddelivery/orders-api:latest
          ports:
            - containerPort: 8080
          envFrom:
            - configMapRef:
                name: orders-api-config
            - secretRef:
                name: orders-api-secrets
          resources:
            requests:
              memory: "256Mi"
              cpu: "250m"
            limits:
              memory: "512Mi"
              cpu: "500m"
          livenessProbe:
            httpGet:
              path: /health
              port: 8080
            initialDelaySeconds: 15
            periodSeconds: 20
          readinessProbe:
            httpGet:
              path: /health/ready
              port: 8080
            initialDelaySeconds: 5
            periodSeconds: 10
```

**Service (k8s/base/orders-api/service.yaml):**
```yaml
apiVersion: v1
kind: Service
metadata:
  name: orders-api
  namespace: rapiddelivery
spec:
  selector:
    app: orders-api
  ports:
    - port: 80
      targetPort: 8080
  type: ClusterIP
```

---

### Day 5-7: Deploy to Kubernetes

#### 🛠️ Task 5.3: Deploy Application

```powershell
# Create namespace
kubectl apply -f k8s/base/namespace.yaml

# Apply all manifests
kubectl apply -f k8s/base/orders-api/

# Check deployment status
kubectl get pods -n rapiddelivery
kubectl get services -n rapiddelivery
kubectl get deployments -n rapiddelivery

# View logs
kubectl logs -f deployment/orders-api -n rapiddelivery

# Port forward to test locally
kubectl port-forward service/orders-api 8080:80 -n rapiddelivery

# Now access: http://localhost:8080
```

---

## Week 5 Checklist
- [ ] Local Kubernetes running
- [ ] Understand core objects
- [ ] Created namespace, configmap, secret
- [ ] Created deployment and service
- [ ] Deployed orders-api to Kubernetes
- [ ] Can view logs and port-forward

---

## Week 6: Kubernetes Advanced

### Day 1-2: Kustomize

#### 🛠️ Task 6.1: Setup Kustomize

**Base Kustomization (k8s/base/kustomization.yaml):**
```yaml
apiVersion: kustomize.config.k8s.io/v1beta1
kind: Kustomization

namespace: rapiddelivery

resources:
  - namespace.yaml
  - orders-api/deployment.yaml
  - orders-api/service.yaml
  - orders-api/configmap.yaml
  - notifications-api/deployment.yaml
  - notifications-api/service.yaml
  - rabbitmq/deployment.yaml
  - rabbitmq/service.yaml
  - mssql/deployment.yaml
  - mssql/service.yaml
  - mssql/pvc.yaml

commonLabels:
  app.kubernetes.io/managed-by: kustomize
```

**Dev Overlay (k8s/overlays/dev/kustomization.yaml):**
```yaml
apiVersion: kustomize.config.k8s.io/v1beta1
kind: Kustomization

namespace: rapiddelivery-dev

resources:
  - ../../base

namePrefix: dev-

patches:
  - patch: |-
      - op: replace
        path: /spec/replicas
        value: 1
    target:
      kind: Deployment

images:
  - name: ghcr.io/YOUR_USERNAME/rapiddelivery/orders-api
    newTag: develop
```

**Prod Overlay (k8s/overlays/prod/kustomization.yaml):**
```yaml
apiVersion: kustomize.config.k8s.io/v1beta1
kind: Kustomization

namespace: rapiddelivery-prod

resources:
  - ../../base

namePrefix: prod-

patches:
  - patch: |-
      - op: replace
        path: /spec/replicas
        value: 3
    target:
      kind: Deployment
      name: orders-api

images:
  - name: ghcr.io/YOUR_USERNAME/rapiddelivery/orders-api
    newTag: v1.0.0
```

```powershell
# Preview dev manifests
kubectl kustomize k8s/overlays/dev

# Apply dev environment
kubectl apply -k k8s/overlays/dev

# Apply prod environment
kubectl apply -k k8s/overlays/prod
```

---

### Day 3-4: Ingress Controller

#### 🛠️ Task 6.2: Setup Ingress

```powershell
# Install NGINX Ingress Controller
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.10.0/deploy/static/provider/cloud/deploy.yaml
```

**Ingress (k8s/base/ingress.yaml):**
```yaml
apiVersion: networking.k8s.io/v1
kind: Ingress
metadata:
  name: rapiddelivery-ingress
  namespace: rapiddelivery
  annotations:
    nginx.ingress.kubernetes.io/rewrite-target: /$2
spec:
  ingressClassName: nginx
  rules:
    - host: api.rapiddelivery.local
      http:
        paths:
          - path: /orders(/|$)(.*)
            pathType: ImplementationSpecific
            backend:
              service:
                name: orders-api
                port:
                  number: 80
          - path: /notifications(/|$)(.*)
            pathType: ImplementationSpecific
            backend:
              service:
                name: notifications-api
                port:
                  number: 80
```

Add to hosts file:
```
# C:\Windows\System32\drivers\etc\hosts
127.0.0.1 api.rapiddelivery.local
```

---

### Day 5-7: Helm Charts

#### 🛠️ Task 6.3: Create Helm Chart

```powershell
# Install Helm
choco install kubernetes-helm

# Create chart
helm create helm/rapiddelivery
```

**Chart Structure:**
```
helm/rapiddelivery/
├── Chart.yaml
├── values.yaml
├── templates/
│   ├── deployment.yaml
│   ├── service.yaml
│   ├── ingress.yaml
│   ├── configmap.yaml
│   ├── secret.yaml
│   └── _helpers.tpl
└── charts/
```

**values.yaml:**
```yaml
# Default values for rapiddelivery
replicaCount: 2

ordersApi:
  image:
    repository: ghcr.io/YOUR_USERNAME/rapiddelivery/orders-api
    tag: latest
    pullPolicy: IfNotPresent
  
  service:
    type: ClusterIP
    port: 80

  resources:
    limits:
      cpu: 500m
      memory: 512Mi
    requests:
      cpu: 250m
      memory: 256Mi

  config:
    aspnetcoreEnvironment: Production
    messageBrokerHost: amqp://rabbitmq:5672

notificationsApi:
  image:
    repository: ghcr.io/YOUR_USERNAME/rapiddelivery/notifications-api
    tag: latest

rabbitmq:
  enabled: true
  auth:
    username: guest
    password: guest

ingress:
  enabled: true
  className: nginx
  host: api.rapiddelivery.local
```

**templates/deployment.yaml:**
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: {{ include "rapiddelivery.fullname" . }}-orders-api
  labels:
    {{- include "rapiddelivery.labels" . | nindent 4 }}
spec:
  replicas: {{ .Values.replicaCount }}
  selector:
    matchLabels:
      app: orders-api
  template:
    metadata:
      labels:
        app: orders-api
    spec:
      containers:
        - name: orders-api
          image: "{{ .Values.ordersApi.image.repository }}:{{ .Values.ordersApi.image.tag }}"
          ports:
            - containerPort: 8080
          env:
            - name: ASPNETCORE_ENVIRONMENT
              value: {{ .Values.ordersApi.config.aspnetcoreEnvironment }}
          resources:
            {{- toYaml .Values.ordersApi.resources | nindent 12 }}
```

```powershell
# Install chart
helm install rapiddelivery ./helm/rapiddelivery -n rapiddelivery --create-namespace

# Upgrade
helm upgrade rapiddelivery ./helm/rapiddelivery -n rapiddelivery

# Uninstall
helm uninstall rapiddelivery -n rapiddelivery
```

---

## Week 6 Checklist
- [ ] Kustomize overlays working
- [ ] Ingress controller deployed
- [ ] Can access services via domain
- [ ] Helm chart created
- [ ] Can deploy with Helm

---

## Week 7: Production Kubernetes

### Day 1-3: Azure Kubernetes Service (AKS)

#### 🛠️ Task 7.1: Create AKS Cluster

```bash
# Login to Azure
az login

# Create resource group
az group create --name rg-rapiddelivery --location eastus

# Create AKS cluster
az aks create \
  --resource-group rg-rapiddelivery \
  --name aks-rapiddelivery \
  --node-count 3 \
  --enable-managed-identity \
  --generate-ssh-keys

# Get credentials
az aks get-credentials --resource-group rg-rapiddelivery --name aks-rapiddelivery

# Verify connection
kubectl get nodes
```

---

### Day 4-5: Deploy to AKS

#### 🛠️ Task 7.2: Update GitHub Actions for AKS

Add to `.github/workflows/cd.yml`:

```yaml
  deploy-aks:
    name: Deploy to AKS
    runs-on: ubuntu-latest
    needs: docker
    environment: production

    steps:
      - uses: actions/checkout@v4

      - name: Azure Login
        uses: azure/login@v2
        with:
          creds: ${{ secrets.AZURE_CREDENTIALS }}

      - name: Set AKS context
        uses: azure/aks-set-context@v3
        with:
          resource-group: rg-rapiddelivery
          cluster-name: aks-rapiddelivery

      - name: Deploy with Helm
        run: |
          helm upgrade --install rapiddelivery ./helm/rapiddelivery \
            --namespace rapiddelivery \
            --create-namespace \
            --set ordersApi.image.tag=${{ github.sha }} \
            --set notificationsApi.image.tag=${{ github.sha }}
```

---

### Day 6-7: Monitoring & Scaling

#### 🛠️ Task 7.3: Horizontal Pod Autoscaler

```yaml
# k8s/base/orders-api/hpa.yaml
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: orders-api-hpa
  namespace: rapiddelivery
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: orders-api
  minReplicas: 2
  maxReplicas: 10
  metrics:
    - type: Resource
      resource:
        name: cpu
        target:
          type: Utilization
          averageUtilization: 70
    - type: Resource
      resource:
        name: memory
        target:
          type: Utilization
          averageUtilization: 80
```

---

## Week 7 Checklist
- [ ] AKS cluster created
- [ ] Deployed to AKS
- [ ] GitHub Actions deploys to AKS
- [ ] HPA configured
- [ ] Monitoring enabled

---

# 🏗️ PHASE 4: Infrastructure as Code (Week 8-9)

## Week 8: Terraform Fundamentals

### Day 1-2: Terraform Basics

#### 🛠️ Task 8.1: Setup Terraform

```powershell
# Install Terraform
choco install terraform

# Verify
terraform version
```

Create folder structure:
```
infrastructure/
├── terraform/
│   ├── main.tf
│   ├── variables.tf
│   ├── outputs.tf
│   ├── providers.tf
│   ├── backend.tf
│   └── modules/
│       ├── aks/
│       ├── acr/
│       └── networking/
└── environments/
    ├── dev/
    │   └── terraform.tfvars
    └── prod/
        └── terraform.tfvars
```

---

### Day 3-5: Azure Resources with Terraform

**providers.tf:**
```hcl
terraform {
  required_version = ">= 1.0"
  
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0"
    }
  }
}

provider "azurerm" {
  features {}
}
```

**variables.tf:**
```hcl
variable "environment" {
  description = "Environment name"
  type        = string
}

variable "location" {
  description = "Azure region"
  type        = string
  default     = "eastus"
}

variable "node_count" {
  description = "Number of AKS nodes"
  type        = number
  default     = 3
}
```

**main.tf:**
```hcl
# Resource Group
resource "azurerm_resource_group" "main" {
  name     = "rg-rapiddelivery-${var.environment}"
  location = var.location
  
  tags = {
    Environment = var.environment
    Project     = "RapidDelivery"
    ManagedBy   = "Terraform"
  }
}

# Container Registry
resource "azurerm_container_registry" "main" {
  name                = "acrrapiddelivery${var.environment}"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  sku                 = "Standard"
  admin_enabled       = true
}

# AKS Cluster
resource "azurerm_kubernetes_cluster" "main" {
  name                = "aks-rapiddelivery-${var.environment}"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  dns_prefix          = "rapiddelivery-${var.environment}"

  default_node_pool {
    name       = "default"
    node_count = var.node_count
    vm_size    = "Standard_D2_v2"
  }

  identity {
    type = "SystemAssigned"
  }

  tags = {
    Environment = var.environment
  }
}

# Attach ACR to AKS
resource "azurerm_role_assignment" "aks_acr" {
  principal_id                     = azurerm_kubernetes_cluster.main.kubelet_identity[0].object_id
  role_definition_name             = "AcrPull"
  scope                            = azurerm_container_registry.main.id
  skip_service_principal_aad_check = true
}
```

**outputs.tf:**
```hcl
output "resource_group_name" {
  value = azurerm_resource_group.main.name
}

output "aks_cluster_name" {
  value = azurerm_kubernetes_cluster.main.name
}

output "acr_login_server" {
  value = azurerm_container_registry.main.login_server
}

output "kube_config" {
  value     = azurerm_kubernetes_cluster.main.kube_config_raw
  sensitive = true
}
```

**environments/dev/terraform.tfvars:**
```hcl
environment = "dev"
location    = "eastus"
node_count  = 2
```

---

### Day 6-7: Terraform Workflow

```powershell
cd infrastructure/terraform

# Initialize
terraform init

# Plan (preview changes)
terraform plan -var-file="../environments/dev/terraform.tfvars"

# Apply (create resources)
terraform apply -var-file="../environments/dev/terraform.tfvars"

# Destroy (cleanup)
terraform destroy -var-file="../environments/dev/terraform.tfvars"
```

---

## Week 8 Checklist
- [ ] Terraform installed
- [ ] Understand HCL syntax
- [ ] Created resource group
- [ ] Created AKS cluster
- [ ] Created ACR
- [ ] Can deploy infrastructure

---

## Week 9: Advanced IaC

### Day 1-3: Terraform Modules

#### 🛠️ Task 9.1: Create Reusable Modules

**modules/aks/main.tf:**
```hcl
resource "azurerm_kubernetes_cluster" "main" {
  name                = var.cluster_name
  location            = var.location
  resource_group_name = var.resource_group_name
  dns_prefix          = var.dns_prefix

  default_node_pool {
    name       = "default"
    node_count = var.node_count
    vm_size    = var.vm_size
  }

  identity {
    type = "SystemAssigned"
  }

  tags = var.tags
}
```

**modules/aks/variables.tf:**
```hcl
variable "cluster_name" {
  type = string
}

variable "location" {
  type = string
}

variable "resource_group_name" {
  type = string
}

variable "dns_prefix" {
  type = string
}

variable "node_count" {
  type    = number
  default = 3
}

variable "vm_size" {
  type    = string
  default = "Standard_D2_v2"
}

variable "tags" {
  type    = map(string)
  default = {}
}
```

**Using module in main.tf:**
```hcl
module "aks" {
  source = "./modules/aks"

  cluster_name        = "aks-rapiddelivery-${var.environment}"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  dns_prefix          = "rapiddelivery-${var.environment}"
  node_count          = var.node_count

  tags = {
    Environment = var.environment
    Project     = "RapidDelivery"
  }
}
```

---

### Day 4-5: Terraform State & Backend

**backend.tf:**
```hcl
terraform {
  backend "azurerm" {
    resource_group_name  = "rg-terraform-state"
    storage_account_name = "stterraformstate"
    container_name       = "tfstate"
    key                  = "rapiddelivery.tfstate"
  }
}
```

```powershell
# Create storage for state (one-time setup)
az group create --name rg-terraform-state --location eastus

az storage account create \
  --name stterraformstate \
  --resource-group rg-terraform-state \
  --sku Standard_LRS

az storage container create \
  --name tfstate \
  --account-name stterraformstate
```

---

### Day 6-7: Terraform in CI/CD

Add to `.github/workflows/infrastructure.yml`:

```yaml
name: Infrastructure

on:
  push:
    branches: [main]
    paths:
      - 'infrastructure/**'
  pull_request:
    branches: [main]
    paths:
      - 'infrastructure/**'

jobs:
  terraform:
    name: Terraform
    runs-on: ubuntu-latest
    defaults:
      run:
        working-directory: infrastructure/terraform

    steps:
      - uses: actions/checkout@v4

      - name: Setup Terraform
        uses: hashicorp/setup-terraform@v3
        with:
          terraform_version: 1.7.0

      - name: Azure Login
        uses: azure/login@v2
        with:
          creds: ${{ secrets.AZURE_CREDENTIALS }}

      - name: Terraform Init
        run: terraform init

      - name: Terraform Format Check
        run: terraform fmt -check

      - name: Terraform Plan
        run: terraform plan -var-file="../environments/dev/terraform.tfvars" -out=tfplan

      - name: Terraform Apply
        if: github.ref == 'refs/heads/main' && github.event_name == 'push'
        run: terraform apply -auto-approve tfplan
```

---

## Week 9 Checklist
- [ ] Created reusable modules
- [ ] Remote state configured
- [ ] Terraform in CI/CD
- [ ] Can deploy multiple environments
- [ ] Infrastructure as code complete!

---

# 🎯 Final Integration (Week 10)

## Complete Pipeline

```
┌─────────────────────────────────────────────────────────────────┐
│                        Developer                                │
│                           │                                     │
│                     git push                                    │
│                           ▼                                     │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │                  GitHub Actions                          │   │
│  │  ┌─────────┐  ┌─────────┐  ┌─────────┐  ┌─────────────┐ │   │
│  │  │ Build   │→ │ Test    │→ │ Docker  │→ │ Deploy      │ │   │
│  │  │ & Lint  │  │         │  │ Push    │  │ to AKS      │ │   │
│  │  └─────────┘  └─────────┘  └─────────┘  └─────────────┘ │   │
│  └─────────────────────────────────────────────────────────┘   │
│                           │                                     │
│                           ▼                                     │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │              Azure (Terraform Managed)                   │   │
│  │  ┌─────────────────────────────────────────────────┐    │   │
│  │  │                    AKS                           │    │   │
│  │  │  ┌─────────┐  ┌─────────┐  ┌─────────────────┐  │    │   │
│  │  │  │Orders   │  │Notif    │  │RabbitMQ/Redis   │  │    │   │
│  │  │  │API      │  │API      │  │                  │  │    │   │
│  │  │  └─────────┘  └─────────┘  └─────────────────┘  │    │   │
│  │  └─────────────────────────────────────────────────┘    │   │
│  └─────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
```

---

# 📚 Resources

## Docker
- 📺 [Docker Tutorial for Beginners](https://www.youtube.com/watch?v=fqMOX6JJhGo)
- 📖 [Docker Official Docs](https://docs.docker.com/)

## Kubernetes
- 📺 [Kubernetes Course - TechWorld with Nana](https://www.youtube.com/watch?v=X48VuDVv0do)
- 📖 [Kubernetes Official Docs](https://kubernetes.io/docs/)
- 🎮 [Kubernetes Interactive Tutorial](https://kubernetes.io/docs/tutorials/kubernetes-basics/)

## GitHub Actions
- 📖 [GitHub Actions Docs](https://docs.github.com/en/actions)
- 📺 [GitHub Actions Tutorial](https://www.youtube.com/watch?v=R8_veQiYBjI)

## Terraform
- 📺 [Terraform Course](https://www.youtube.com/watch?v=SLB_c_ayRMo)
- 📖 [Terraform Azure Provider](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs)
- 🎮 [HashiCorp Learn](https://learn.hashicorp.com/terraform)

---

# ✅ Progress Tracker

| Week | Topic | Status |
|------|-------|--------|
| 1 | Docker Fundamentals | ⬜ |
| 2 | Docker Advanced | ⬜ |
| 3 | GitHub Actions CI | ⬜ |
| 4 | GitHub Actions CD | ⬜ |
| 5 | Kubernetes Basics | ⬜ |
| 6 | Kubernetes Advanced | ⬜ |
| 7 | AKS Production | ⬜ |
| 8 | Terraform Basics | ⬜ |
| 9 | Terraform Advanced | ⬜ |
| 10 | Integration | ⬜ |

---

*Let's build production-ready infrastructure! 🚀*

