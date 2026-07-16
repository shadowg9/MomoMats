// Partial backend configuration. Do NOT commit backend access keys or storage account keys.
// The backend requires storage_account_name and container_name to be supplied at
// `terraform init -backend-config` or configured in your local environment.

terraform {
  backend "azurerm" {
    key = "momomats/dev/terraform.tfstate"
    // storage_account_name = "<supply-at-init>"
    // container_name       = "<supply-at-init>"
    // access_key           = "<supply-at-init-or-use-managed-identity>"
  }
}
