# 文件說明

此套件是依據以下範本產生後重新調整的套件，移除 istio 相關的內容；因為 yarp 可承擔 ingress 與分流等行為。

* https://github.com/salesforce/helm-starter
* https://github.com/salesforce/helm-starter-istio

## Port 的設定

### Service Port 額外加入的東西
於 service 中的 port 設定因為這個套件是要開放給外部使用的，所以有加入 NodePort 的控制，另外也因為適用於分流，因此有設定 type 為 LoadBalancer

### Values 檔案的設定

Values 中的 Port 設定為了 OAuth 的關係，名稱必須改為 https，且 port 為 8443，TargetPort 因為是 Container 的 Port 所以不動，這樣子的設定才能讓 AppGateway 的 OAuth 正常運作，不然進到 pod 的時候會被認為是 http 連線，導致 OAuth 的轉址設定錯誤。

# 原始說明

## Installation

```sh
> helm template --namespace=[namespace] [chartname] | kubectl apply -f -
```

## Values.yaml

All configuration for this installation is managed in `values.yaml`. Configuration
values can be overriden individually at installation using Helm's `--set` command
line option.

### Service Identity

These three values control the names of generated Kubernetes and Istio objects,
and are used to ensure commont Kubernetes labeling. These values are used to populate
labels that allow for selecting all components of a particular system or service.

* `system`, `service`, `version` - These values describe _what_ this service and
  what it should be named. For example: `my-website`, `web-server`, `2`.

### Container Values

These settings control from where and how your service's docker image is acquired.

* `image.repository` - The docker repo and image to pull.
* `image.tag` - The docker image tag to pull.
* `image.imagePullPolicy` - Kubernetes image pull policy.

### Service Account Values

Istio request authorization requires that each service have a unique service account
identity to functuion correctly.

* `serviceAccount.name` - The Kubernetes service account your service will run under.
* `serviceAccount.create` - Optionally, this chart can generate the service account.
  If false, the service's service account must be pre-existing.

### Replica Values

These settings control service replicas, disruption budgets, and autoscaling.

* `replicaCount` - The initial number of replicas to start after installing this
  chart.
* `maxUnavailable` - The maximum number of intentionally unavailable pods as
  controlled by a `PodDisruptionBudget`.
* `autoscaling.minReplicas` - The minimum number of replicas to run under the
  control of a `HorizontalPodAutoscaler`.
* `autoscaling.maxReplicas` - The maximum number of replicas to run under the
  control of a `HorizontalPodAutoscaler`.
* `autoscaling.targetAverageCpuUtilization` - The CPU utilization target
  used by the `HorizontalPodAutoscaler` to make autoscale decisions.

### Kubernetes Pod Values

These settings configure your service's resource constraints and health check
probes. They ensure your service is a well behaved consumer of shared Kubernetes
resources.

* `resources.*` - Kubernetes resource request and limit configuration. See
  [Kubernetes resource documentation](https://kubernetes.io/docs/concepts/configuration/manage-resources-containers/) for values.
* `probes.*` - Kubernetes probe configuration. See [Kubernetes probe documentation](https://kubernetes.io/docs/tasks/configure-pod-container/configure-liveness-readiness-startup-probes/) for values.

### ConfigMap Values

These optional settings are used to populate and mount a configmap for your
service. When the generated config map changes, the associated service is automatically
resterted using a rolling restart. Generating the configmap from Helm chart values
is useful because it allows you to modify config map values durring installation
using Helm `--set` directives.

* `configMap.mountPath` - The directory inside your pod to mount the config map.
* `configMap.fileName` - The file name of the config map, when mounted in the pod.
* `configMap.content.*` - YAML keys and values under `content` are copied verbatim
  into the configmap's content.
