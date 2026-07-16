---
name: momomats-azure-terraform
description: Generates, reviews, validates, tests, and plans lightweight Microsoft Azure Terraform infrastructure for the MomoMats ASP.NET Core container application. Use when creating or modifying files in terraform-azure, preparing Azure CI/CD, configuring Azure App Service, Azure Container Registry, Azure Database for MySQL, remote Terraform state, or GitHub Actions for Azure.
---

# MomoMats Azure Terraform

Create and maintain the Azure infrastructure for the MomoMats application.

## Required context

Before generating Terraform:

1. Inspect `MomoMats.csproj`.
2. Inspect `Program.cs`.
3. Inspect `Dockerfile`.
4. Inspect `.github/workflows/container-ci.yml`.
5. Inspect existing files under `terraform-azure/`.
6. Read `references/architecture.md`.
7. Read `references/security-rules.md`.

## Scope

Only create or modify:

- `terraform-azure/**`
- `.github/workflows/azure-terraform.yml`
- `.github/workflows/azure-container-deploy.yml`
- Azure-related documentation when requested

Do not modify:

- `terraform/**`
- `argocd/**`
- Existing AWS infrastructure
- Existing Kubernetes manifests
- Application source code unless the user explicitly requests it

## Terraform structure

Prefer this structure:

- `versions.tf` for Terraform and provider versions
- `providers.tf` for provider configuration
- `backend.tf` for the Azure Storage backend declaration
- `main.tf` for lightweight Azure resources
- `variables.tf` for input variables
- `outputs.tf` for useful nonsensitive outputs
- `terraform.tfvars.example` for safe examples
- `tests/momomats.tftest.hcl` for native Terraform tests

Keep the initial implementation small. Do not introduce modules unless repetition or additional environments justify them.

## Target infrastructure

Generate Terraform for:

1. Azure Resource Group
2. Azure Container Registry
3. Linux App Service Plan
4. Linux Web App for Containers
5. Azure Database for MySQL Flexible Server
6. MySQL database
7. Application Insights when requested
8. Required identities and minimum role assignments

Do not create AKS, Argo CD, bastion hosts, virtual machines, NAT gateways, or complex hub-and-spoke networking unless explicitly requested.

## Workflow

Follow this order:

1. Explain the intended changes briefly.
2. Inspect the repository.
3. Create or update the Terraform files.
4. Run `terraform fmt -recursive`.
5. Run `terraform init`.
6. Run `terraform validate`.
7. Run `terraform test` when tests exist.
8. Run `terraform plan`.
9. Summarize resources to add, change, and destroy.
10. Stop and request explicit approval before deployment.

If validation or planning fails, inspect the error, correct the configuration, and rerun the failed command.

## Approval boundary

Never run any of the following without explicit user approval in the current conversation:

- `terraform apply`
- `terraform destroy`
- `terraform import`
- `terraform state rm`
- `terraform state mv`
- Azure CLI commands that create, update, or delete cloud resources
- Commands that trigger a deployment workflow

A request to generate, validate, test, or plan infrastructure does not authorize applying it.

## Output expectations

At completion, report:

- Files created or changed
- Validation result
- Test result
- Terraform plan summary
- Estimated cost-sensitive resources
- Any manual configuration still required
- Whether deployment approval is required