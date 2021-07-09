resource "aws_dynamodb_table" "activityhistoryapi_dynamodb_table" {
    name                  = "ActivityHistory"
    billing_mode          = "PROVISIONED"
    read_capacity         = 10
    write_capacity        = 10
    hash_key              = "targetId"
    range_key             = "id"

    attribute {
        name              = "id"
        type              = "S"
    }

    attribute {
        name              = "targetId"
        type              = "S"
    }

    attribute {
        name              = "createdAt"
        type              = "S"
    }

    local_secondary_index {
        name              = "ActivityHistoryByCreatedAt"
        range_key         = "createdAt"
        projection_type   = "ALL"
    }

    tags = {
        Name              = "activity-history-api-${var.environment_name}"
        Environment       = var.environment_name
        terraform-managed = true
        project_name      = var.project_name
    }

    point_in_time_recovery {
        enabled           = true
    }
}
