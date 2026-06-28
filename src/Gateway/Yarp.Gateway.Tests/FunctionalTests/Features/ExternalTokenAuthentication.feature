Feature: External token authentication
  Gateway 應能將自訂 token 交由外部服務驗證，且不誤處理 JWT-shaped Bearer token。

  Background:
    Given Gateway 已啟用 ExternalToken authentication

  Scenario: 有效的外部 token 可以存取受保護路由
    When 使用者以 ExternalToken "valid-external-token" 要求外部驗證路由
    Then HTTP 狀態碼應為 200
    And 回應應包含外部使用者 id "external-user-001"
    And 外部驗證服務應收到 token "valid-external-token"
    And 下游不應收到外部 access token

  Scenario: 有效的外部 key 交換 access token 後轉送至下游
    When 使用者以 ExternalKey "valid-external-key" 要求外部驗證路由
    Then HTTP 狀態碼應為 200
    And 回應應包含外部使用者 id "external-user-001"
    And 外部驗證服務應收到 key "valid-external-key"
    And 下游應收到外部 access token "access-token-from-key-exchange"

  Scenario: 無效的外部 token 被拒絕
    When 使用者以 ExternalToken "invalid-external-token" 要求外部驗證路由
    Then HTTP 狀態碼應為 401
    And 外部驗證服務應收到 token "invalid-external-token"

  Scenario: JWT-shaped Bearer token 不交給外部驗證服務
    When 使用者以 JWT-shaped Bearer token 要求外部驗證路由
    Then HTTP 狀態碼應為 401
    And 外部驗證服務不應被呼叫