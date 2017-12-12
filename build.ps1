#!/usr/bin/env powershell
#requires -version 4
# CSG Build Script
# Copyright 2017 Cornerstone Solutions Group
Param(
	[alias("c")][string]
	$Configuration = "Release",
	[string]
	$BuildToolsVersion = "0.9.11-preview",
	[switch]
	$NoTest
)

$Solution=".\Csg.Data.Dapper.sln"
$TestProjects = @(
	#".\src\<TEST_PROJECT_NAME>\<TEST_PROJECT_NAME>.csproj"
)
$OutputPackages = @(
	".\Csg.Data.Dapper\Csg.Data.Dapper.csproj"
)

Write-Host "=============================================================================="
Write-Host "The Build Script"
Write-Host "=============================================================================="

try {
	. "$PSScriptRoot/bootstrap.ps1"	
	Get-BuildTools -Version $BuildToolsVersion | Out-Null

	# RESTORE
	Write-Host "Restoring Packages..." -ForegroundColor Magenta
	dotnet restore $SOLUTION
	if ($LASTEXITCODE -ne 0) {
		throw "Package restore failed with exit code $LASTEXITCODE."
	}

	# BUILD SOLUTION
	Write-Host "Performing build..." -ForegroundColor Magenta	
	dotnet build $SOLUTION --configuration $Configuration
	if ($LASTEXITCODE -ne 0) {
		throw "Build failed with exit code $LASTEXITCODE."
	}

	# RUN TESTS
	if ( !($NoTest.IsPresent) -and $TestProjects.Length -gt 0 ) {
		Write-Host "Performing tests..." -ForegroundColor Magenta
		foreach ($test_proj in $TestProjects) {
			Write-Host "Testing $test_proj"			
			#Note: The --logger parameter is for specifically for mstest to make it output test results
			dotnet test $test_proj --no-build --configuration $Configuration --logger "trx;logfilename=TEST-out.xml"
			if ($LASTEXITCODE -ne 0) {
				throw "Test failed with code $LASTEXITCODE"
			}
		}
	}

	# CREATE NUGET PACKAGES
	if ( $OutputPackages.Length -gt 0 ) {
		Write-Host "Packaging..."  -ForegroundColor Magenta
		foreach ($pack_proj in $OutputPackages){
			Write-Host "Packing $pack_proj"
			dotnet pack $pack_proj --no-build --configuration $Configuration
			if ($LASTEXITCODE -ne 0) {
				throw "Pack failed with code $result"
			}
		}
	}

	Write-Host "All Done. This build is great!" -ForegroundColor Green
	exit 0
} catch {
	Write-Host "ERROR: An error occurred and the build was aborted." -ForegroundColor White -BackgroundColor Red
	Write-Error $_	
	exit 3
}