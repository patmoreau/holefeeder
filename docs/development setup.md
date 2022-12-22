# Developer Environment Setup

## Software requirements

## k3d environment

### create simple dev cluster

```bash
k3d cluster create dev \
  --api-port 6550 \
  --port 443:443@loadbalancer \
  --port 30306:30306@loadbalancer \
  --servers 3 \
  --agents 3
```

### create a simple prd cluster

```bash
k3d cluster create prd \
  --port 80:80@loadbalancer \
  --port 443:443@loadbalancer \
  --api-port 147.182.156.172:6550 \
  --k3s-arg '--tls-san=147.182.156.172@loadbalancer'
```

## Merge kubeconfig

```bash
cp ~/.kube/config ~/.kube/config.bak && KUBECONFIG=~/.kube/config:./holefeeder-cluster-config.yaml kubectl config view --flatten > /tmp/config && mv /tmp/config ~/.kube/config
```

## Cert-manager

```bash
helm repo add jetstack https://charts.jetstack.io
helm repo update
kubectl create namespace cert-manager
helm install cert-manager jetstack/cert-manager \
    --namespace cert-manager \
    --version v1.9.1 \
    --set installCRDs=true --wait --debug
kubectl -n cert-manager rollout status deploy/cert-manager
```

```bash
kubectl apply -f https://github.com/cert-manager/cert-manager/releases/download/v1.10.1/cert-manager.yaml
```

## dashboard

### 4. Install the helm repos for rancher

```bash
helm repo add rancher-latest https://releases.rancher.com/server-charts/latest
helm repo update
kubectl create namespace cattle-system
helm install rancher rancher-latest/rancher \
    --namespace cattle-system \
    --version=2.6.1 \
    --set hostname=rancher.localhost \
    --set bootstrapPassword=congratsthanandayme \
    --wait --debug
kubectl -n cattle-system rollout status deploy/rancher
kubectl -n cattle-system get all,ing
```

```bash
GITHUB_URL=https://github.com/kubernetes/dashboard/releases
VERSION_KUBE_DASHBOARD=$(curl -w '%{url_effective}' -I -L -s -S ${GITHUB_URL}/latest -o /dev/null | sed -e 's|.*/||')
kubectl create -f https://raw.githubusercontent.com/kubernetes/dashboard/${VERSION_KUBE_DASHBOARD}/aio/deploy/recommended.yaml
```

### Dashboard RBAC Configuration

```bash
kubectl create -f dashboard.admin-user.yml -f dashboard.admin-user-role.yml
```

### Obtain bearer token

```bash
kubectl -n kubernetes-dashboard describe secret admin-user-token | grep '^token'
```

### Local Access to the Dashboard

```bash
kubectl proxy
```

<http://localhost:8001/api/v1/namespaces/kubernetes-dashboard/services/https:kubernetes-dashboard:/proxy/>
