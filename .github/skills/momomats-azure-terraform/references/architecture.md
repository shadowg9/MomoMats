# MomoMats Azure architecture

## Application profile

MomoMats is:

- An ASP.NET Core application targeting .NET 10
- Packaged as a Linux container
- Configured to listen on container port 8080
- Dependent on MySQL through Entity Framework Core
- Responsible for applying EF Core migrations during startup
- Currently built into a container through the repository Dockerfile

## Deployment goal

Create a lightweight Azure counterpart to the existing AWS project.

The Azure implementation should demonstrate:

- AI-assisted infrastructure generation
- Infrastructure as Code
- Container deployment
- Remote Terraform state
- CI/CD
- Secret-conscious configuration
- Validation and approval controls

It should not reproduce the complexity of the AWS EKS and Argo CD architecture.

## Target resources

### Resource group

Place the MomoMats Azure resources in a dedicated resource group.

Suggested development name:

`rg-momomats-dev`

### Azure Container Registry

Use Azure Container Registry to store the MomoMats container image.

Suggested naming pattern:

`acrmomomats<unique-suffix>`

Use the Basic SKU for the development demonstration.

Disable the registry administrator account. Prefer managed identity and Azure role assignments.

### App Service plan

Use a Linux App Service plan suitable for development.

Keep the SKU configurable so it can be reduced, increased, or disabled according to Azure subscription availability.

### Linux Web App

Run the MomoMats container using Azure App Service for Containers.

Requirements:

- Linux container
- Container port 8080
- HTTPS-only access
- System-assigned managed identity
- Always On only when supported by the selected SKU
- Application settings configured through Terraform
- ACR image access through managed identity when supported

### Azure Database for MySQL

Use Azure Database for MySQL Flexible Server.

Requirements:

- MySQL 8
- Small development SKU
- Configurable administrator username
- Sensitive administrator password variable
- TLS required
- Public access restricted through firewall rules
- No hard-coded credentials
- A dedicated `momomats` database

The MomoMats connection string must be supplied as:

`ConnectionStrings__DefaultConnection`

Do not commit the real value to the repository.

### Application Insights

Application Insights is optional during the first deployment. Add it after the core application deployment works unless the user explicitly requests it initially.

## State storage

Use the Azure Storage backend created separately during the Terraform bootstrap process.

The backend should use:

- An Azure resource group
- A globally unique storage account
- A private blob container
- Blob versioning where appropriate
- A state key dedicated to the MomoMats development environment

Example state key:

`momomats/dev/terraform.tfstate`

Do not place backend access keys in repository files.

## Environments

Begin with one environment:

`dev`

Do not introduce staging and production directories until the development deployment works.

## Naming and tags

Use predictable lowercase Azure-compatible names.

Apply these tags when supported:

- `project = "MomoMats"`
- `environment = "dev"`
- `managed-by = "Terraform"`
- `purpose = "portfolio"`

## CI/CD separation

Use two workflows:

1. Terraform workflow:
   - Format check
   - Initialization
   - Validation
   - Tests
   - Plan on pull requests
   - Approval-gated apply

2. Application workflow:
   - Build container
   - Scan container
   - Authenticate to Azure using OIDC
   - Push image to ACR
   - Update or restart the Azure Web App

Prefer GitHub OIDC federation instead of permanent Azure client secrets.