@echo off

dotnet publish Client/Client.csproj -p:PublishSingleFile=true;PublishTrimmed=true -r win-x64 -c Release --self-contained true

echo "Copying files to the final directory."
xcopy Client\bin\Release\win-x64\publish\Client.exe Client\bin\Defsite\ /I
xcopy Client\bin\Release\win-x64\Assets\ Client\bin\Defsite\Assets\ /S /I

rem Copy essential DLLs (this might not be needed in the final release of .net 5)
xcopy Client\bin\Release\win-x64\coreclr.dll Client\bin\Defsite\ /I
xcopy Client\bin\Release\win-x64\clrjit.dll Client\bin\Defsite\ /I
xcopy Client\bin\Release\win-x64\clrcompression.dll Client\bin\Defsite\ /I
xcopy Client\bin\Release\win-x64\mscordaccore.dll Client\bin\Defsite\ /I

rem Should copy redistributables and also maybe generate the installer

pause