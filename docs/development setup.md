# Developer Environment Setup

## Software requirements

## k3d environment

### create simple dev cluster

```bash
k3d cluster create dev \
  --api-port 6443 \
  --port 443:443@loadbalancer \
  --port 9443:9443@loadbalancer \
  --port 30306:30306@loadbalancer \
  --port 30431:30431@loadbalancer
```

install local certificate trust:
mkcert <https://github.com/FiloSottile/mkcert>

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

## dashboard

<https://docs.portainer.io/start/install/server/kubernetes/baremetal>
