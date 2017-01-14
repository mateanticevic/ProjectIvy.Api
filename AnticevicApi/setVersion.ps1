param([string]$version)

$jsons = Get-ChildItem -Recurse project.json

Foreach($json in $jsons)
{
    $file = Get-Content $json.FullName
    $file = $file -replace '"(version)": "([^"]+)",', ('"$1": "' + $version + '",')
    $file = $file -replace '"(AnticevicApi.[^"]+)": "([^"]+)"', ('"$1": "' + $version + '"')

    $Utf8NoBomEncoding = New-Object System.Text.UTF8Encoding $False
    [System.IO.File]::WriteAllLines($json.FullName, $file, $Utf8NoBomEncoding)
}