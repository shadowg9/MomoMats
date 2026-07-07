resource "aws_security_group" "eks_additional_sg" {
  name        = "momomats-eks-additional-sg"
  description = "Allow the MomoMats bastion host to reach the private EKS API endpoint"
  vpc_id      = module.vpc.vpc_id

  ingress {
    description     = "HTTPS from bastion host"
    from_port       = 443
    to_port         = 443
    protocol        = "tcp"
    security_groups = [aws_security_group.bastion_sg.id]
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = {
    Name        = "momomats-eks-additional-sg"
    Project     = "MomoMats"
    Terraform   = "true"
    Environment = "dev"
  }
}

module "eks" {
  source  = "terraform-aws-modules/eks/aws"
  version = "~> 21.0"

  name               = "momomats-eks-cluster"
  kubernetes_version = "1.34"

  addons = {
    coredns = {}
    eks-pod-identity-agent = {
      before_compute = true
    }
    kube-proxy = {}
    vpc-cni = {
      before_compute = true
    }
  }

  endpoint_public_access = false
  enable_cluster_creator_admin_permissions = true

  vpc_id                        = module.vpc.vpc_id
  subnet_ids                   = module.vpc.private_subnets
  additional_security_group_ids = [aws_security_group.eks_additional_sg.id]

  eks_managed_node_groups = {
    momomats_nodes = {
      ami_type       = "AL2023_x86_64_STANDARD"
      instance_types = ["t3.medium"]

      min_size     = 2
      max_size     = 4
      desired_size = 2
    }
  }

  tags = {
    Project     = "MomoMats"
    Terraform   = "true"
    Environment = "dev"
  }
}
