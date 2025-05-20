resource "aws_dynamodb_table" "activityhistoryapi_dynamodb_table" {
  name         = "ActivityHistory"
  billing_mode = "PAY_PER_REQUEST"
  hash_key     = "targetId"
  range_key    = "id"

  attribute {
    name = "id"
    type = "S"
  }

  attribute {
    name = "targetId"
    type = "S"
  }

  attribute {
    name = "createdAt"
    type = "S"
  }

  ttl {
    attribute_name = "TimetoLiveForRecord"
    enabled        = true
  }

  local_secondary_index {
    name            = "ActivityHistoryByCreatedAt"
    range_key       = "createdAt"
    projection_type = "ALL"
  }


  tags = merge(
    local.default_tags,
    { BackupPolicy = "Dev", Backup = false, Confidentiality = "Internal" }
  )

  point_in_time_recovery {
    enabled = false
  }
}
