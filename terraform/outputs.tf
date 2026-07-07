output "cluster_name" {
  description = "Name of the MomoMats EKS cluster"
  value       = module.eks.cluster_name
}

output "bastion_public_ip" {
  description = "Public IP address of the bastion host"
  value       = module.bastion_host.public_ip
}

output "vpc_id" {
  description = "ID of the MomoMats VPC"
  value       = module.vpc.vpc_id
}

output "private_subnet_ids" {
  description = "Private subnet IDs used by EKS"
  value       = module.vpc.private_subnets
}

output "public_subnet_ids" {
  description = "Public subnet IDs used for internet-facing resources and the bastion host"
  value       = module.vpc.public_subnets
}
