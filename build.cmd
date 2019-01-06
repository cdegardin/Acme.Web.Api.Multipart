@echo Off
set config=%1
if "%config%" == "" (
   set config=Release
)
 
set version=1.0.0
if not "%PackageVersion%" == "" (
   set version=%PackageVersion%
)

%nuget% restore
"%MsBuildExe%" Acme.Web.Api.Multipart\Acme.Web.Api.Multipart.csproj /t:pack /p:Configuration=Release