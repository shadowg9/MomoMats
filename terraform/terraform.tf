terraform {
  required_version = ">= 1.5.7, < 2.0.0"

  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 6.37"
    }

    http = {
      source  = "hashicorp/http"
      version = "~> 3.6"
    }

    tls = {
      source  = "hashicorp/tls"
      version = "~> 4.3"
    }

    local = {
      source  = "hashicorp/local"
      version = "~> 2.9"
    }
  }
}


provider "aws" {
  region = "us-east-1"
}