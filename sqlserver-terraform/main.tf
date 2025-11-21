terraform {
  required_providers {
    docker = {
      source  = "kreuzwerker/docker"
      version = "~> 3.0"
    }
  }
}

provider "docker" {}

# ------------------------------
#  SQL Server Image
# ------------------------------
resource "docker_image" "sqlserver" {
  name = "mcr.microsoft.com/mssql/server:2022-latest"
}

# ------------------------------
#  Persistent Volume
# ------------------------------
resource "docker_volume" "sql_data" {
  name = "sqlserver_data"
}

# ------------------------------
#  SQL Server Container
# ------------------------------
resource "docker_container" "sqlserver" {
  name  = "sqlserver"
  image = docker_image.sqlserver.name

  env = [
    "ACCEPT_EULA=Y",
    "MSSQL_PID=Developer",
    "SA_PASSWORD=${var.sa_password}"
  ]

  ports {
    internal = 1433
    external = 1433
  }

  # Attach volume
  volumes {
    volume_name    = docker_volume.sql_data.name
    container_path = "/var/opt/mssql"
  }
}

# ------------------------------
#  Outputs
# ------------------------------
output "sql_connection_string" {
  value     = "Server=localhost,1433;Database=master;User Id=sa;Password=${var.sa_password};TrustServerCertificate=True;"
  sensitive = true
}
