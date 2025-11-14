########################################
# Provider Docker
########################################
terraform {
  required_providers {
    docker = {
      source  = "kreuzwerker/docker"
      version = "~> 3.0"
    }
  }
}

provider "docker" {}

########################################
# Rede Docker
########################################
resource "docker_network" "sql_network" {
  name = "sqlserver_network"
}

########################################
# Volume para armazenamento persistente
########################################
resource "docker_volume" "sql_data" {
  name = "sqlserver_data"

  # OPCIONAL: para mapear para uma pasta local, descomentar isto:
  # driver_opts = {
  #   type   = "none"
  #   device = "/caminho/no/host/sql_data"
  #   o      = "bind"
  # }
}

########################################
# Container SQL Server
########################################
resource "docker_container" "sqlserver" {
  name  = "sqlserver"
  image = "microsoft/mssql-server"

  # Variáveis de ambiente obrigatórias
  env = [
    "ACCEPT_EULA=Y",
    "SA_PASSWORD=Your_password123",  # TROCA ISTO
    "MSSQL_PID=Express"
  ]

  # Mapeamento da porta SQL
  ports {
    internal = 1433
    external = 1433
  }

  # Limite de memória (2GB)
  memory = 2048

  # Volume para armazenar os dados SQL
  mounts {
    target = "/var/opt/mssql"
    source = docker_volume.sql_data.name
    type   = "volume"
  }

  networks_advanced {
    name = docker_network.sql_network.name
  }

  # Healthcheck para garantir que o SQL está operacional
  healthcheck {
    test = [
      "CMD",
      "/opt/mssql-tools/bin/sqlcmd",
      "-S", "localhost",
      "-U", "sa",
      "-P", "Your_password123",
      "-Q", "SELECT 1"
    ]
    interval = "30s"
    timeout  = "10s"
    retries  = 3
  }
}
