terraform {
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 3.0"
    }
  }
}

provider "aws" {
  region = "eu-west-2"
}

locals {
  default_tags = {
    Name              = "activity-history-api-${var.environment_name}"
    Environment       = var.environment_name
    terraform-managed = true
    project_name      = var.project_name
    Application       = "MTFH Housing Pre-Production"
    TeamEmail         = "developementteam@hackney.gov.uk"
  }
}

terraform {
  backend "s3" {
    bucket         = "housing-pre-production-terraform-state"
    encrypt        = true
    region         = "eu-west-2"
    key            = "services/activity-history-api/state"
    dynamodb_table = "housing-pre-production-terraform-state-lock"
  }
}
