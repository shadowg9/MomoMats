# ---------------------------------------------------------
# RDS SUBNET GROUP
# Places RDS inside the existing private VPC subnets.
# ---------------------------------------------------------
resource "aws_db_subnet_group" "momomats" {
  name        = "momomats-db-subnet-group"
  description = "Private subnet group for the MomoMats MySQL database"
  subnet_ids  = module.vpc.private_subnets

  tags = {
    Name        = "momomats-db-subnet-group"
    Project     = "MomoMats"
    Environment = "dev"
    Terraform   = "true"
  }
}

# ---------------------------------------------------------
# RDS SECURITY GROUP
# Allows MySQL only from the EKS worker nodes.
# ---------------------------------------------------------
resource "aws_security_group" "rds" {
  name        = "momomats-rds-sg"
  description = "Allow MySQL connections from MomoMats EKS nodes"
  vpc_id      = module.vpc.vpc_id

  ingress {
    description     = "MySQL from EKS worker nodes"
    from_port       = 3306
    to_port         = 3306
    protocol        = "tcp"
    security_groups = [module.eks.node_security_group_id]
  }

  egress {
    description = "Allow outbound traffic"
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = {
    Name        = "momomats-rds-sg"
    Project     = "MomoMats"
    Environment = "dev"
    Terraform   = "true"
  }
}

# ---------------------------------------------------------
# RDS MYSQL INSTANCE
# Development-sized, private, encrypted MySQL database.
# ---------------------------------------------------------
resource "aws_db_instance" "momomats" {
  identifier = "momomats-mysql"

  engine         = "mysql"
  instance_class = "db.t4g.micro"

  allocated_storage     = 20
  max_allocated_storage = 50
  storage_type          = "gp3"
  storage_encrypted     = true

  db_name  = "momomats"
  username = "momomatsadmin"
  port     = 3306

  # AWS generates and stores the master password in Secrets Manager.
  manage_master_user_password = true

  db_subnet_group_name   = aws_db_subnet_group.momomats.name
  vpc_security_group_ids = [aws_security_group.rds.id]

  publicly_accessible = false
  multi_az            = false

  backup_retention_period    = 1
  auto_minor_version_upgrade = true
  apply_immediately          = true

  deletion_protection = false
  skip_final_snapshot = true

  tags = {
    Name        = "momomats-mysql"
    Project     = "MomoMats"
    Environment = "dev"
    Terraform   = "true"
  }
}