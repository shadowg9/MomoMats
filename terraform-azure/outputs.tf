output "resource_group_name" {
  description = "Name of the resource group"
  value       = azurerm_resource_group.rg.name
}

output "container_registry_login_server" {
  description = "ACR login server (do not include credentials)"
  value       = azurerm_container_registry.acr.login_server
}

output "web_app_default_hostname" {
  description = "Default hostname of the MomoMats Azure Web App."
  value       = azurerm_linux_web_app.web.default_hostname
}

output "mysql_server_fqdn" {
  description = "MySQL server FQDN"
  value       = azurerm_mysql_flexible_server.mysql.fqdn
}

// Do NOT output administrator password or full connection strings

