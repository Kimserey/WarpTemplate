@echo off
cls

tools\paket.bootstrapper.exe
if errorlevel 1 (
  exit /b %errorlevel%
)

tools\paket.exe restore
if errorlevel 1 (
  exit /b %errorlevel%
)