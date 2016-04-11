$folder1 = 'D:/Workspace/10-Front/Services/Stock/Dev/Dev-Nav/Src/Stock'
$folder2 = 'D:\Workspace\10-Front\services\Pricing\dev\Dev-Nav\Src\'

$Dir = get-childitem $folder2 -recurse
# $Dir |get-member
$List = $Dir | where {$_.extension -eq ".csproj"} | % {
    $f = (Get-Content $_.FullName).Replace('<TargetFrameworkVersion>v4.5</TargetFrameworkVersion>','<TargetFrameworkVersion>v4.0</TargetFrameworkVersion>') | Set-Content -Force $_.FullName
}