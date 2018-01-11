$branch = @{ $true = $env:APPVEYOR_REPO_BRANCH; $false = $(git symbolic-ref --short -q HEAD) }[$env:APPVEYOR_REPO_BRANCH -ne $NULL];
$revision = @{ $true = "{0:00000}" -f [convert]::ToInt32("0" + $env:APPVEYOR_BUILD_NUMBER, 10); $false = "local" }[$env:APPVEYOR_BUILD_NUMBER -ne $NULL];
$suffix = @{ $true = ""; $false = "$($branch.Substring(0, [math]::Min(10,$branch.Length)))-$revision"}[$branch -eq "master" -and $revision -ne "local"]

echo "build: Version suffix is $suffix"

Push-Location $PSScriptRoot

if(Test-Path .\artifacts) {
	echo "build: Cleaning .\artifacts"
	Remove-Item .\artifacts -Force -Recurse
}
# run restore on all *.csproj files in the src folder including 2>1 to redirect stderr to stdout for badly behaved tools
Get-ChildItem -Path .\src -Filter *.csproj -Recurse | ForEach-Object { 
	& dotnet restore $_.FullName --no-cache
	if($LASTEXITCODE -ne 0) { exit 1 }
}

# run pack on all *.csproj files in the src folder including 2>1 to redirect stderr to stdout for badly behaved tools
Get-ChildItem -Path .\src -Filter *.csproj -Recurse | ForEach-Object {
	if ($suffix) { 
		& dotnet pack $_.FullName -c Release -o ..\..\artifacts --version-suffix=$suffix
	} else {
		& dotnet pack $_.FullName -c Release -o ..\..\artifacts
	}
	if($LASTEXITCODE -ne 0) { exit 1 }
}

# run tests
Get-ChildItem -Path .\test -Filter *.csproj -Recurse | ForEach-Object {
	 & dotnet test -c Release $_.FullName
	 if($LASTEXITCODE -ne 0) { exit 1 }
}
