# Custom Joins Testing Guide

## Manual Testing Steps

### 1. Start the Application
```bash
dotnet run --urls "http://localhost:5000"
```

### 2. Test Admin Endpoints

#### Get Available Entities
```bash
curl http://localhost:5000/api/admin/schema/entities
```

Expected output:
```json
[
    "ContextProfile",
    "PdfTemplate",
    "SampleInvoice",
    "SampleInvoiceItem"
]
```

#### Get Fields for SampleInvoice
```bash
curl http://localhost:5000/api/admin/schema/entities/SampleInvoice/fields
```

Expected output:
```json
[
    "CustomerName",
    "Date",
    "Id",
    "Number"
]
```

### 3. Test Custom Join Validation

#### Valid Join Configuration
```bash
curl -X POST http://localhost:5000/api/admin/validate-joins \
  -H "Content-Type: application/json" \
  -d '{
    "contextName": "invoice",
    "joins": [
      {
        "alias": "ExternalCustomer",
        "targetEntity": "Customer",
        "joinType": "LEFT",
        "condition": {
          "leftField": "CustomerCode",
          "rightField": "ExternalId",
          "operator": "="
        },
        "nestedJoins": []
      }
    ]
  }'
```

Expected output:
```json
{
  "errors": [],
  "isValid": true
}
```

#### Invalid Join Configuration (SQL Injection Attempt)
```bash
curl -X POST http://localhost:5000/api/admin/validate-joins \
  -H "Content-Type: application/json" \
  -d '{
    "contextName": "invoice",
    "joins": [
      {
        "alias": "Customer; DROP TABLE Users--",
        "targetEntity": "Customer",
        "joinType": "LEFT",
        "condition": {
          "leftField": "CustomerCode",
          "rightField": "ExternalId",
          "operator": "="
        },
        "nestedJoins": []
      }
    ]
  }'
```

Expected output:
```json
{
  "errors": [
    "Invalid alias 'Customer; DROP TABLE Users--'. Use only letters, numbers, and underscores."
  ],
  "isValid": false
}
```

#### Invalid Operator
```bash
curl -X POST http://localhost:5000/api/admin/validate-joins \
  -H "Content-Type: application/json" \
  -d '{
    "contextName": "invoice",
    "joins": [
      {
        "alias": "Customer",
        "targetEntity": "Customer",
        "joinType": "LEFT",
        "condition": {
          "leftField": "CustomerCode",
          "rightField": "ExternalId",
          "operator": "OR 1=1--"
        },
        "nestedJoins": []
      }
    ]
  }'
```

Expected output:
```json
{
  "errors": [
    "Invalid operator 'OR 1=1--'"
  ],
  "isValid": false
}
```

### 4. Test Existing PDF Functionality (Without Custom Joins)

#### Preview HTML
```bash
curl http://localhost:5000/api/pdf/preview/invoice/11111111-1111-1111-1111-111111111111
```

This should return HTML with invoice data.

#### Generate PDF
```bash
curl -o invoice.pdf http://localhost:5000/api/pdf/generate/invoice/11111111-1111-1111-1111-111111111111
```

This should download a PDF file.

## Integration Test Scenarios

### Scenario 1: Simple Custom Join
**Use Case**: Join Invoice with external Customer table on CustomerCode field

**Context Profile**:
```json
{
  "contextName": "invoice-with-customer",
  "rootEntity": "SampleInvoice",
  "customJoins": [
    {
      "alias": "Customer",
      "targetEntity": "Customer",
      "joinType": "LEFT",
      "condition": {
        "leftField": "CustomerCode",
        "rightField": "ExternalId",
        "operator": "="
      }
    }
  ],
  "allowedFields": [
    "Number",
    "Date",
    "Customer.Name",
    "Customer.Email"
  ]
}
```

### Scenario 2: Nested Custom Joins
**Use Case**: Join Invoice -> Customer -> CustomerAddress

**Context Profile**:
```json
{
  "contextName": "invoice-with-customer-address",
  "rootEntity": "SampleInvoice",
  "customJoins": [
    {
      "alias": "Customer",
      "targetEntity": "Customer",
      "joinType": "LEFT",
      "condition": {
        "leftField": "CustomerCode",
        "rightField": "ExternalId",
        "operator": "="
      },
      "nestedJoins": [
        {
          "alias": "Address",
          "targetEntity": "Address",
          "joinType": "LEFT",
          "condition": {
            "leftField": "AddressId",
            "rightField": "Id",
            "operator": "="
          }
        }
      ]
    }
  ]
}
```

### Scenario 3: Mix of EF Includes and Custom Joins
**Use Case**: Use EF for Items (FK relationship) and custom join for Customer

**Context Profile**:
```json
{
  "contextName": "invoice-mixed",
  "rootEntity": "SampleInvoice",
  "includePaths": ["Items"],
  "customJoins": [
    {
      "alias": "Customer",
      "targetEntity": "Customer",
      "joinType": "LEFT",
      "condition": {
        "leftField": "CustomerCode",
        "rightField": "ExternalId",
        "operator": "="
      }
    }
  ],
  "allowedFields": [
    "Number",
    "Date",
    "Customer.Name",
    "Items.Description",
    "Items.Quantity"
  ]
}
```

## Security Test Cases

### Test 1: SQL Injection via Alias
**Input**: `"alias": "test'; DROP TABLE Users--"`
**Expected**: Validation error

### Test 2: SQL Injection via Operator
**Input**: `"operator": "= OR 1=1"`
**Expected**: Validation error

### Test 3: SQL Injection via Field Names
**Input**: `"leftField": "Id; DELETE FROM Users"`
**Expected**: Validation error

### Test 4: Join Depth Limit
**Input**: 4 levels of nested joins
**Expected**: Validation error (max depth is 3)

### Test 5: Join Count Limit
**Input**: 11 custom joins
**Expected**: Validation error (max is 10)

## Performance Tests

### Test 1: Simple Join Performance
- Create context with 1 custom join
- Fetch 100 records
- Expected: < 5 seconds

### Test 2: Complex Join Performance
- Create context with 3 nested joins
- Fetch 100 records
- Expected: < 10 seconds

### Test 3: Mixed EF + Custom Joins
- Create context with 2 EF includes + 2 custom joins
- Fetch 100 records
- Expected: < 8 seconds
