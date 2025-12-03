$ErrorActionPreference = "Stop"

$apiUrl = "http://spasync.runasp.net/api/data/upload"
$apiKey = "SpaSyncAPI-2024-X7kM9pL2qR5tY8wZ3vN6"
$headers = @{ 
    "X-API-Key" = $apiKey
    "Content-Type" = "application/json" 
}

function Push-Entity {
    param($EntityName, $Query, $Endpoint)
    
    Write-Host "Pushing $EntityName..." -NoNewline
    $data = Invoke-Sqlcmd -ServerInstance "NIKOLA\SQLEXPRESS" -Database "spa_erp" -Query $Query
    
    if ($data.Count -eq 0) {
        Write-Host " (no data)"
        return
    }
    
    $json = $data | ConvertTo-Json -Compress
    if ($data.Count -eq 1) {
        $json = "[$json]"
    }
    
    try {
        $response = Invoke-WebRequest -Uri "$apiUrl/$Endpoint" -Method POST -Headers $headers -Body $json -UseBasicParsing
        $result = $response.Content | ConvertFrom-Json
        Write-Host " $($result.recordsProcessed) records"
    } catch {
        Write-Host " ERROR: $_"
    }
}

# Push Persons
Push-Entity "Persons" @"
SELECT person_id as PersonId, first_name as FirstName, last_name as LastName, 
       email as Email, phone as Phone, address as Address, dob as Dob, created_at as CreatedAt
FROM Person
"@ "persons"

# Push Employees  
Push-Entity "Employees" @"
SELECT employee_id as EmployeeId, person_id as PersonId, role_id as RoleId, 
       hire_date as HireDate, status as Status, note as Note, created_at as CreatedAt
FROM Employee
"@ "employees"

# Push Customers
Push-Entity "Customers" @"
SELECT customer_id as CustomerId, person_id as PersonId, customer_code as CustomerCode,
       loyalty_points as LoyaltyPoints, created_at as CreatedAt, is_archived as IsArchived
FROM Customer
"@ "customers"

# Push Appointments
Push-Entity "Appointments" @"
SELECT appointment_id as AppointmentId, customer_id as CustomerId, 
       scheduled_start as ScheduledStart, scheduled_end as ScheduledEnd,
       status as Status, notes as Notes, created_by_user_id as CreatedByUserId, created_at as CreatedAt
FROM Appointment
"@ "appointments"

# Push Sales
Push-Entity "Sales" @"
SELECT sale_id as SaleId, customer_id as CustomerId, created_by_user_id as CreatedByUserId,
       sale_number as SaleNumber, total_amount as TotalAmount, payment_status as PaymentStatus, created_at as CreatedAt
FROM Sale
"@ "sales"

# Push Payments
Push-Entity "Payments" @"
SELECT payment_id as PaymentId, sale_id as SaleId, payment_method as PaymentMethod,
       amount as Amount, paid_at as PaidAt, recorded_by_user_id as RecordedByUserId
FROM Payment
"@ "payments"

Write-Host "`nDone!"
