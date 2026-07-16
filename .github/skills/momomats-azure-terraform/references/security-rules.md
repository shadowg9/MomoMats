# MomoMats Azure Terraform security rules

## Secrets

Never place real secrets in:

- Terraform source files
- `terraform.tfvars.example`
- GitHub workflow files
- Application source code
- `appsettings.json`
- Agent prompts
- Documentation
- Git commits

Treat the following as sensitive:

- MySQL administrator password
- Database connection string
- Azure credentials
- Container registry credentials
- Terraform backend credentials

Mark sensitive Terraform variables and outputs with:

`sensitive = true`

Do not output complete connection strings.

## Authentication

For local Terraform commands, use the authenticated Azure CLI session.

For GitHub Actions, prefer OpenID Connect workload identity federation.

Do not create a long-lived Azure client secret unless OIDC is unavailable and the user explicitly approves the fallback.

Do not enable the Azure Container Registry administrator account.

Use managed identities and narrowly scoped Azure role assignments where possible.

## Terraform state

Terraform state can contain sensitive values.

Requirements:

- Store state in the approved Azure Storage backend
- Keep the state container private
- Do not commit local state
- Do not commit `.terraform/`
- Do not commit plan files
- Do not print sensitive state values
- Do not use local state for CI/CD deployments

Confirm `.gitignore` excludes:

- `.terraform/`
- `*.tfstate`
- `*.tfstate.*`
- `*.tfplan`
- `crash.log`
- Real `*.tfvars` files

Allow `terraform.tfvars.example`.

## Network access

Require HTTPS for the web application.

Require TLS for MySQL.

Do not create unrestricted MySQL firewall rules such as all IPv4 addresses.

Restrict database access to the smallest practical set of outbound application addresses and explicitly approved developer addresses.

Do not expose administrative services publicly.

## Terraform safety

Always run:

1. `terraform fmt -check -recursive`
2. `terraform init`
3. `terraform validate`
4. `terraform test` when tests exist
5. `terraform plan`

Before apply, summarize:

- Resources to add
- Resources to change
- Resources to destroy
- Identity and role changes
- Public network exposure
- Cost-sensitive resources

Never automatically approve an apply.

Never use:

- `terraform apply -auto-approve`
- `terraform destroy -auto-approve`
- `terraform state rm`
- `terraform force-unlock`

unless the user explicitly understands and approves the operation.

## CI/CD permissions

Use least-privilege GitHub Actions permissions.

For OIDC authentication, request only the permissions needed by the workflow, typically:

- `contents: read`
- `id-token: write`

Do not give deployment workflows unnecessary write access to repository contents, issues, packages, or pull requests.

Use a protected GitHub environment for deployment approval when available.

## Cost control

Flag these resources before deployment:

- Azure Database for MySQL Flexible Server
- App Service plans
- Application Insights ingestion
- Container Registry storage
- Public IP addresses
- Private endpoints
- Log Analytics workspaces

Use development-sized resources and avoid high availability, zone redundancy, Premium SKUs, and excessive retention unless explicitly requested.