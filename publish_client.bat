@echo off

dotnet publish Defsite/Defsite.csproj -p:PublishSingleFile=true;PublishTrimmed=true -r win-x64 -c Release --self-contained true

echo "Copying files to the final directory."
xcopy Defsite\bin\Release\win-x64\publish\Defsite.exe Defsite\bin\Defsite\ /I /Y
xcopy Defsite\bin\Release\win-x64\Assets\ Defsite\bin\Defsite\Assets\ /S /I /Y

rem Copy essential DLLs (this might not be needed in the final release of .net 5)
xcopy Defsite\bin\Release\win-x64\coreclr.dll Defsite\bin\Defsite\ /I /Y
xcopy Defsite\bin\Release\win-x64\clrjit.dll Defsite\bin\Defsite\ /I /Y
xcopy Defsite\bin\Release\win-x64\clrcompression.dll Defsite\bin\Defsite\ /I /Y
xcopy Defsite\bin\Release\win-x64\mscordaccore.dll Defsite\bin\Defsite\ /I /Y

rem Should copy redistributables and also maybe generate the installer

pause