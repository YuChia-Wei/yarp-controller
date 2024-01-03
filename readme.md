# Yarp Controller

:::info
development pending (controller)
:::

## 系統用途、目的

- 管理 Yarp 設定檔案
- 管理使用 Yarp 所建立的服務

## 技術與架構

此專案預期利用以下技術、技能進行建置與開發，核心目標是在開發初期讓我練習這些內容。

1. 六角架構(Hexagonal architecture) / 乾淨架構(Clean architecture)
2. DDD
3. .net 8
4. RESTful WebApi Design
5. MQTT (or RabbitMQ and other message queue system)

## 雜談

我也預期利用這個專案來建立團隊合作時可能會需要的設定檔案，包括但不限於 editorconfig

## docker testing

run keylock

```bash
docker run -d -p 8080:8080 -p 8443:8443 -e KEYCLOAK_ADMIN=user -e KEYCLOAK_ADMIN_PASSWORD=password quay.io/keycloak/keycloak start-dev
```

client id testing json : [link](./doc/test_client.json)