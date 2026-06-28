Feature: MySSO authentication
  Gateway 應只在互動式路由轉導 MySSO，並透過 session Cookie 管理平台權杖。

  Background:
    Given Gateway 已啟用 MySSO authentication

  Scenario: 一般受保護路由未登入時回傳 401
    When 未登入使用者要求一般受保護路由
    Then HTTP 狀態碼應為 401
    And 回應不應包含 MySSO 轉導位置

  Scenario: 一般受保護路由收到無效 session Cookie 時將其刪除
    When 使用者以無效 MySSO session Cookie 要求一般受保護路由
    Then HTTP 狀態碼應為 401
    And 回應應刪除 MySSO session Cookie

  Scenario: 互動式路由未登入時轉導 MySSO
    When 未登入使用者要求互動式路由
    Then HTTP 狀態碼應為 302
    And 回應應轉導至 MySSO 並包含 callback 與 state
    And 回應應建立 correlation Cookie

  Scenario: MySSO form-post callback 建立可用的 Gateway session
    When 未登入使用者要求互動式路由
    And 瀏覽器以有效單次 token 提交 MySSO form-post callback
    Then HTTP 狀態碼應為 302
    And 回應應建立 MySSO session Cookie
    When 瀏覽器使用 MySSO session Cookie 要求一般受保護路由
    Then HTTP 狀態碼應為 200
    And 回應應包含 customer id "customer-001"
    And 回應應包含 access token "access-token-initial"

  Scenario: 已登入使用者查詢 session 到期資訊
    Given 使用者已完成 MySSO 登入
    When 瀏覽器查詢 MySSO session
    Then HTTP 狀態碼應為 200
    And session 回應應顯示已驗證及 1800 秒閒置時間
    And 回應應包含 session 到期 header

  Scenario: 已登入使用者主動更新平台權杖
    Given 使用者已完成 MySSO 登入
    When 瀏覽器要求更新 MySSO session
    Then HTTP 狀態碼應為 200
    And Auth Server 應收到既有 refresh token "refresh-token-initial"
    And 回應應建立 MySSO session Cookie
    When 瀏覽器使用 MySSO session Cookie 要求一般受保護路由
    Then 回應應包含 access token "access-token-refreshed"

  Scenario: refresh token 更新失敗時強制登出
    Given 使用者已完成 MySSO 登入
    And Auth Server 將拒絕 refresh token
    When 瀏覽器要求更新 MySSO session
    Then HTTP 狀態碼應為 401
    And 回應應刪除 MySSO session Cookie