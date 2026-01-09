pipeline {
    agent {
        docker {
            image 'mcr.microsoft.com/dotnet/sdk:9.0'
            // Mount Docker socket/binaries so docker/compose work inside the build container
            args '-v /var/run/docker.sock:/var/run/docker.sock -v /usr/bin/docker:/usr/bin/docker -v /usr/lib/docker/cli-plugins:/usr/lib/docker/cli-plugins'
        }
    }

    environment {
        DOTNET_CLI_TELEMETRY_OPTOUT = '1'
        DOTNET_SKIP_FIRST_TIME_EXPERIENCE = '1'
        IMAGE_NAME = 'products-api'
        IMAGE_TAG = "${env.BRANCH_NAME}-${env.BUILD_NUMBER}"
        USE_ARGOCD = 'true'
        HELM_APP_CHART = 'helm/app'
        HELM_RELEASE_NAME = 'products-api'
        HELM_VALUES_STAGING = 'helm/app/values-staging.yaml'
        HELM_VALUES_PROD = 'helm/app/values-prod.yaml'
    }

    options {
        skipDefaultCheckout(true)
        timestamps()
    }

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Restore') {
            steps {
                sh 'dotnet restore ProductsAPI.sln'
            }
        }

        stage('Test (with coverage)') {
            steps {
                sh '''
                    set -e
                    dotnet test ProductsAPI.sln \
                      --configuration Release \
                      --results-directory ./TestResults \
                      --logger "trx;LogFileName=test_results.trx" \
                      --collect:"XPlat Code Coverage"
                '''

                sh '''
                    export PATH="$PATH:$HOME/.dotnet/tools"
                    dotnet tool install -g trx2junit || true
                    trx2junit TestResults/**/*.trx
                '''
            }
            post {
                always {
                    junit allowEmptyResults: true, testResults: 'TestResults/**/*.xml'
                    publishCoverage adapters: [coberturaAdapter(path: 'TestResults/**/coverage.cobertura.xml')], sourceFileResolver: sourceFiles('STORE_ALL_BUILD_SOURCES')
                    archiveArtifacts artifacts: 'TestResults/**/*', allowEmptyArchive: true
                }
            }
        }

        stage('Build DotNet') {
            steps {
                sh 'dotnet build ProductsAPI.sln --configuration Release --no-restore'
            }
        }

        stage('Build Docker Image') {
            steps {
                sh """
                    docker build -t ${IMAGE_NAME}:${IMAGE_TAG} -t ${IMAGE_NAME}:latest .
                """
            }
        }

        stage('Deploy to Staging (Helm)') {
            when {
                allOf {
                    branch 'development'
                    expression { env.USE_ARGOCD != 'true' }
                }
            }
            steps {
                sh '''
                    helm upgrade --install ${HELM_RELEASE_NAME} ${HELM_APP_CHART} \
                      -f ${HELM_VALUES_STAGING} \
                      --namespace products-staging \
                      --create-namespace
                '''
            }
        }

        stage('Deploy to Prod (Helm)') {
            when {
                allOf {
                    branch 'main'
                    expression { env.USE_ARGOCD != 'true' }
                }
            }
            steps {
                sh '''
                    helm upgrade --install ${HELM_RELEASE_NAME} ${HELM_APP_CHART} \
                      -f ${HELM_VALUES_PROD} \
                      --namespace products-prod \
                      --create-namespace
                '''
            }
        }

        stage('Deploy via ArgoCD') {
            when {
                expression { env.USE_ARGOCD == 'true' }
            }
            steps {
                sh '''
                    echo "ArgoCD enabled: build image only. Deployment handled by GitOps."
                '''
            }
        }
    }
}
