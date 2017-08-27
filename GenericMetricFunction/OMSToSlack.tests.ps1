$port = 7071
$uriBase = "http://localhost:$port/api"
$cpu = Get-Content cpu-payload.json
$cpu2 = Get-Content cpu-server2-payload.json
$memory = Get-Content memory-payload.json
$drive = Get-Content drive-payload.json

Describe "OMS to Slack Function" {
    It "Should return a 200 for a CPU payload" {
        (Invoke-WebRequest -Uri "$uriBase/OMSMetricToSlack" -Body $cpu -Method Post -ContentType "text/json").StatusCode | Should Be 200
    }

    It "Should return a 200 for a Memory payload" {
        (Invoke-WebRequest -Uri "$uriBase/OMSMetricToSlack" -Body $memory -Method Post -ContentType "text/json").StatusCode | Should Be 200
    }

    It "Should return a 200 for a Drive payload" {
        (Invoke-WebRequest -Uri "$uriBase/OMSMetricToSlack " -Body $drive -Method Post -ContentType "text/json").StatusCode | Should Be 200
    }

        It "Should return a 200 for a CPU server 2 payload" {
        (Invoke-WebRequest -Uri "$uriBase/OMSMetricToSlack" -Body $cpu2 -Method Post -ContentType "text/json").StatusCode | Should Be 200
    }
}