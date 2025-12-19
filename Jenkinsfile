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
        COMPOSE_FILE = 'docker-compose.app.yaml'
        COMPOSE_PROJECT_NAME = "products-${env.BRANCH_NAME}"
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

        stage('Deploy to Dev') {
            when {
                branch 'development'
            }
            steps {
                sh '''
                    docker compose -f ${COMPOSE_FILE} up -d
                '''
            }
        }

        stage('Deploy to Prod') {
            when {
                branch 'main'
            }
            steps {
                sh '''
                    docker compose -f ${COMPOSE_FILE} up -d
                '''
            }
        }
    }
}
