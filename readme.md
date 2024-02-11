# Application Gateway & Routing Controller

本專案包含以 Yarp 建立的 Gateway 服務，以及正在開發中的管理器。

管理器部分仍在開發中，Gateway 部分已經大致完成，相關說明後述。

## key clock prepare

由於整個系統都基於 OpenId Connect 進行身分認證，因此這邊需要準備一套 keyclock 來協助開發與身分認證相關的測試。

### run keyclock in docker

```bash
docker run -d -p 8080:8080 -p 8443:8443 \
    -e KEYCLOAK_ADMIN=user \
    -e KEYCLOAK_ADMIN_PASSWORD=password \
    quay.io/keycloak/keycloak start-dev
```

### import test client

client id testing json : [link](./doc/test_client.json)

## Yarp Gateway

### Authentication

#### Support

- OpenIdConnect
- JWT

#### Config Auth

我在 Gateway 中，有把 Auth 的設定資料獨立到專用的 json 檔案，只需要在部署前定義好資料就可以使用指定的 Auth 設定作為預設身分認證方法。

1. Deploy in docker
   部署在 docker 時，可以利用
   1. 環境參數的方式定義身分認證資料 (不建議，因為資料有點多)
   2. 掛載 volume 替換 Auth 設定檔

2. Deploy in Kubernetes
   部署在 kubernetes 時，如果有使用我在本 repo 中附屬的 helm package 的話，可以使用 values 調整設定。

### Config Yarp

因為 Yarp 預設在 Json 有異動的時候會主動更新路由，所以分別在以下兩種部署情境上都可以實現自動更新。

#### 1. Deploy in docker

在 docker run 的時候，將 Yarp 設定檔的資料夾所在位置 `/app/Configuration/ReverseProxy` 設定掛載外部 Volume，這時候只需要異動相關檔案即可實現路由更新。

```shell
docker run -d -p 8080:8080 -v /config-path:/app/Configuration/ReverseProxy ghcr.io/yuchia-wei/yarp-gateway:v0.0.1
```

#### 2. Deploy in Kubernetes

在本 Repo 中也有放上可搭配 YarpGateway 的 Kubernetes helm charts package ，使用此 package 就可以部署基本的 Gateway 進入 kubernetes (須搭配 istio)。

這套 helm package 設定會將 yarp 設定存放於 config map 中，只要更新 config map 就可以更新路由設定，不會不清楚原因為何，更新的有點慢甚至偶爾不會更新。