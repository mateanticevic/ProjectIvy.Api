Import-Module WebAdministration

"Stopping the WebSite"
Stop-WebSite "api2.anticevic.net"

"Stopping the AppPool"
Stop-WebAppPool "api2.anticevic.net"