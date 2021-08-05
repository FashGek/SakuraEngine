import os

if __name__ == '__main__':
    # Init Dapr On Kubernetes
    os.system("helm upgrade --install dapr dapr/dapr \
        --version=1.3 \
        --namespace dapr-system \
        --create-namespace \
        --set global.ha.enabled=true \
        --wait")
    # Install K8 prequencies
    os.system("helm repo add bitnami https://charts.bitnami.com/bitnami")
    os.system("helm repo add dapr https://dapr.github.io/helm-charts/")
    os.system("helm repo update")
    os.system("helm install redis bitnami/redis")
    # K8 Install Components
    os.system("kubectl apply -f dapr-components/redis-state.yaml")
    os.system("kubectl apply -f dapr-components/redis-pubsub.yaml")
    # Update Control-Plane
    #os.system("dapr upgrade -k --runtime-version=1.3.0")
    # Verify Installation
    os.system("kubectl get pods --namespace dapr-system")
