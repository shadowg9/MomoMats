# Generate an SSH key for the bastion host and register its public key in AWS.
resource "tls_private_key" "bastion_key" {
  algorithm = "RSA"
  rsa_bits  = 4096
}

resource "aws_key_pair" "bastion_keypair" {
  key_name   = "momomats-bastion-key"
  public_key = tls_private_key.bastion_key.public_key_openssh
}

# Save the private key locally. Keep this file out of Git.
resource "local_file" "bastion_private_key" {
  content         = tls_private_key.bastion_key.private_key_pem
  filename        = "momomats-bastion-key.pem"
  file_permission = "0400"
}

resource "aws_security_group" "bastion_sg" {
  name        = "momomats-bastion-sg"
  description = "Allow SSH to the MomoMats bastion host from the current public IP"
  vpc_id      = module.vpc.vpc_id

  ingress {
    description = "SSH from current public IP"
    from_port   = 22
    to_port     = 22
    protocol    = "tcp"
    cidr_blocks = ["${chomp(data.http.my_ip.response_body)}/32"]
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = {
    Name        = "momomats-bastion-sg"
    Project     = "MomoMats"
    Terraform   = "true"
    Environment = "dev"
  }
}

module "bastion_host" {
  source  = "terraform-aws-modules/ec2-instance/aws"
  version = "6.4.0"

  name          = "momomats-bastion-host"
  ami           = "ami-0d28727121d5d4a3c"
  instance_type = "t3.micro"
  key_name      = aws_key_pair.bastion_keypair.key_name
  monitoring    = true

  subnet_id              = element(module.vpc.public_subnets, 0)
  vpc_security_group_ids = [aws_security_group.bastion_sg.id]

  associate_public_ip_address = true

  tags = {
    Project     = "MomoMats"
    Terraform   = "true"
    Environment = "dev"
    Role        = "bastion"
  }
}
