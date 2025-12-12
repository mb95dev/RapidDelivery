# Kubernetes Deployment Manifests

This directory contains Kubernetes manifests for deploying the RapidDelivery microservices.

## Prerequisites

- Kubernetes cluster (local: minikube, Docker Desktop, or cloud: AKS, EKS, GKE)
- kubectl configured to connect to your cluster
- Docker images built and pushed to a container registry

## Deployment Steps

### 1. Create Namespace

```bash
kubectl create namespace rapiddelivery
```

### 2. Create Secrets

```bash
# Copy the example and edit with actual values
cp orders-secrets.yaml.example orders-secrets.yaml
# Edit orders-secrets.yaml with your actual database connection string
kubectl apply -f orders-secrets.yaml -n rapiddelivery
```

### 3. Create ConfigMap

```bash
kubectl apply -f orders-configmap.yaml -n rapiddelivery
```

### 4. Deploy Orders API

```bash
kubectl apply -f orders-api-deployment.yaml -n rapiddelivery
```

### 5. Verify Deployment

```bash
# Check pods
kubectl get pods -n rapiddelivery

# Check services
kubectl get services -n rapiddelivery

# Check HPA
kubectl get hpa -n rapiddelivery

# View logs
kubectl logs -f deployment/orders-api -n rapiddelivery
```

## Key Features Demonstrated

### Health Checks
- **Liveness Probe**: Detects if container is running
- **Readiness Probe**: Detects if container is ready to serve traffic

### Resource Management
- **Resource Requests**: Minimum resources guaranteed
- **Resource Limits**: Maximum resources allowed
- **HPA**: Automatic scaling based on CPU/memory usage

### High Availability
- **Replicas**: Multiple pod instances for redundancy
- **Rolling Updates**: Zero-downtime deployments
- **Service Discovery**: ClusterIP service for internal communication

## Interview Talking Points

- **Resource Management**: Requests and limits prevent resource starvation
- **Health Checks**: Ensure only healthy pods receive traffic
- **Auto-scaling**: HPA automatically adjusts replicas based on load
- **Service Discovery**: Kubernetes DNS enables service-to-service communication
- **Secrets Management**: Sensitive data stored securely in Kubernetes secrets
- **ConfigMaps**: Non-sensitive configuration externalized from code

## Production Considerations

1. **Ingress Controller**: Add Ingress for external access
2. **Network Policies**: Restrict pod-to-pod communication
3. **Pod Disruption Budgets**: Ensure minimum availability during updates
4. **Monitoring**: Integrate with Prometheus/Grafana
5. **Logging**: Centralized logging with ELK stack or similar
6. **TLS**: Enable HTTPS with cert-manager

