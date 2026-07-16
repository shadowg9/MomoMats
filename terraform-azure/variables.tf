variable "location" {
  description = "Azure location for all resources"
  type        = string
  default     = "eastus"
}

variable "name_prefix" {
  description = "Human-friendly prefix for resource names"
  type        = string
  default     = "momomats"
}

variable "unique_suffix" {
  description = "Optional stable suffix to ensure global uniqueness. If empty, a random suffix will be generated."
  type        = string
  default     = ""
}

variable "app_service_sku_tier" {
  description = "App Service plan SKU tier"
  type        = string
  default     = "Basic"
}

variable "app_service_sku_size" {
  description = "App Service plan SKU size"
  type        = string
  default     = "B1"
}

variable "mysql_administrator_login" {
  description = "MySQL administrator username"
  type        = string
  default     = "momoadmin"
}

variable "mysql_sku_name" {
  description = "MySQL Flexible Server SKU name (e.g. B_Standard_B1ms)"
  type        = string
  default     = "B_Standard_B1ms"
}

variable "mysql_version" {
  description = "MySQL major version"
  type        = string
  default     = "8.0.21"
}

variable "mysql_storage_gb" {
  description = "MySQL storage size in GB"
  type        = number
  default     = 32
}

variable "allowed_client_ips" {
  description = "List of CIDR addresses allowed to access the MySQL server. Leave empty to require operator to supply values."
  type        = list(string)
  default     = []
}

variable "tags" {
  description = "Tags applied to all resources"
  type        = map(string)
  default = {
    project     = "MomoMats"
    environment = "dev"
    managed-by  = "Terraform"
    purpose     = "portfolio"
  }
}
