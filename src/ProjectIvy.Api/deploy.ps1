Import-Module WebAdministration

"Sleeping for 5 seconds."
Start-Sleep -s 5

"Starting the AppPool"
Start-WebAppPool "api2.anticevic.net"

"Starting the WebSite"
Start-WebSite "api2.anticevic.net"