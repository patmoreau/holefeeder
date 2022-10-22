# Setting up Kubernetes cluster on DigitalOcean

Before you start you need the following Helm repos:

```bash
helm repo add ingress-nginx https://kubernetes.github.io/ingress-nginx
helm repo add bitnami https://charts.bitnami.com/bitnami
helm repo add jetstack https://charts.jetstack.io
helm repo update
```

## Setting up the cluster using no LoadBalancer

1. DO's firewall for your cluster doesn't have 80/443 inbound open by default.

```bash
doctl compute firewall create \
--inbound-rules="protocol:tcp,ports:80,address:0.0.0.0/0,address:::/0 protocol:tcp,ports:443,address:0.0.0.0/0,address:::/0" \
--tag-names=k8s:CLUSTER_UUID \
--name=k8s-extra-mycluster
```

(Get the CLUSTER_UUID value from the dashboard or the ID column from doctl kubernetes cluster list)

2. Create the nginx ingress using the host network.

```bash
helm install custom-ingress ingress-nginx/ingress-nginx -f custom-ingress.values.yaml
```

3. You should be able to access the cluster on :80 and :443 via any worker node IP and it'll route traffic to your ingress.

4. Since node IPs can & do change, look at deploying external-dns to manage DNS entries to point to your worker nodes. Again, using the helm chart and assuming your DNS domain is hosted by DigitalOcean (though any supported DNS provider will work):

```bash
helm install external-dns bitnami/external-dns -f external-dns.values.yaml
```

5. After a minute or so you should see the DNS records appear and be resolvable:

```bash
dig adminer.drifterapps.app             # should return worker IP address
curl -v http://adminer.drifterapps.app  # should send the request through the Ingress to your backend service
```

## Setting up a certificate manager

1. Before installing Cert-Manager to your cluster via Helm, you’ll manually create a namespace for it by running the following command:

```bash
kubectl create namespace cert-manager
```

2. Finally, install Cert-Manager into the cert-manager namespace by running the following command:

```bash
helm install cert-manager jetstack/cert-manager --namespace cert-manager --version v1.5.4 --set installCRDs=true
kubectl apply -f https://github.com/jetstack/cert-manager/releases/download/v1.6.1/cert-manager.yaml
```

## creating github registry secret for kubernetes

Encode github username and PAT

```bash
echo -n "username:123123adsfasdf123123" | base64
```

Create and encode .dockerconfigjson

```json
{
  "auths": {
    "ghcr.io": {
      "auth": "dXNlcm5hbWU6MTIzMTIzYWRzZmFzZGYxMjMxMjM="
    }
  }
}
```

```bash
echo -n '{"auths":{"ghcr.io":{"auth":"dXNlcm5hbWU6MTIzMTIzYWRzZmFzZGYxMjMxMjM="}}}' | base64
```
