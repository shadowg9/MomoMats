resource "random_id" "suffix" {
  byte_length = 3
  keepers = {
    prefix = var.name_prefix
  }
}

locals {
  suffix   = length(trimspace(var.unique_suffix)) > 0 ? lower(trimspace(var.unique_suffix)) : lower(random_id.suffix.hex)
  location = var.location
}

// Resource Group
resource "azurerm_resource_group" "rg" {
  name     = "rg-${var.name_prefix}-dev"
  location = local.location
  tags     = var.tags
}

// Azure Container Registry (Basic SKU)
resource "azurerm_container_registry" "acr" {
  name                = "acr${var.name_prefix}${local.suffix}"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  sku                 = "Basic"
  admin_enabled       = false
  tags                = var.tags
}

// App Service Plan (Linux)
resource "azurerm_service_plan" "asp" {
  name                = "sp-${var.name_prefix}-dev"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name

  os_type = "Linux"

  sku_name = var.app_service_sku_size

  tags = var.tags
}

// Linux Web App (infrastructure shell)
resource "azurerm_linux_web_app" "web" {
  name                = "wa-${var.name_prefix}-dev"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  service_plan_id     = azurerm_service_plan.asp.id

  identity {
    type = "SystemAssigned"
  }

  site_config {
    always_on           = false
    ftps_state          = "Disabled"
    minimum_tls_version = "1.2"
  }

  https_only = true

  tags = var.tags
}

// Role definition lookup for AcrPull
data "azurerm_role_definition" "acr_pull" {
  name = "AcrPull"
}

// Assign AcrPull to the Web App managed identity scoped to the ACR
resource "azurerm_role_assignment" "acr_pull_assign" {
  scope              = azurerm_container_registry.acr.id
  role_definition_id = data.azurerm_role_definition.acr_pull.id
  principal_id       = azurerm_linux_web_app.web.identity[0].principal_id
  depends_on         = [azurerm_linux_web_app.web]
}

// Generate a random password for MySQL administrator (stored only in state)
resource "random_password" "mysql_administrator" {
  length           = 24
  special          = true
  min_upper        = 1
  min_lower        = 1
  min_numeric      = 1
  min_special      = 1
  override_special = "!#$%&*()-_=+[]{}:?"
}

// MySQL Flexible Server
resource "azurerm_mysql_flexible_server" "mysql" {
  name                = "mysql-${var.name_prefix}-${local.suffix}"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location

  sku_name = var.mysql_sku_name
  version  = var.mysql_version

  administrator_login    = var.mysql_administrator_login
  administrator_password = random_password.mysql_administrator.result

  tags = var.tags
}

// Ensure require_secure_transport is enabled via server configuration
resource "azurerm_mysql_flexible_server_configuration" "require_secure_transport" {
  name                = "require_secure_transport"
  resource_group_name = azurerm_resource_group.rg.name
  server_name         = azurerm_mysql_flexible_server.mysql.name
  value               = "ON"
}

// Create an empty database named momomats
resource "azurerm_mysql_flexible_database" "momomats_db" {
  name                = "momomats"
  resource_group_name = azurerm_resource_group.rg.name
  server_name         = azurerm_mysql_flexible_server.mysql.name
  charset             = "utf8mb4"
  collation           = "utf8mb4_general_ci"
}

// No firewall rules are created by this configuration. Operators must supply allowed_client_ips via a follow-up change or use private networking.

