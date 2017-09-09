Import-Module WebAdministration
Start-Sleep -s 5
Start-WebAppPool "api2.anticevic.net"
Start-WebSite "api2.anticevic.net"