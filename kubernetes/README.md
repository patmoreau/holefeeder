# Setting up Kubernetes cluster on DigitalOcean

In order to pull images from the GitHub Packages repository, a GitHub Personal Access Token (PAT) needs to be created and saved as a secret in kubernetes. This allows the cluster to pull the packages.

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
      "auth": "<<secret>>"
    }
  }
}
```

```bash
echo -n '{"auths":{"ghcr.io":{"auth":"<<secret>>"}}}' | base64
```
