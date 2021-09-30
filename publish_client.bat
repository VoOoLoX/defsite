@echo off

dotnet publish Defsite/Defsite.csproj -p:PublishSingleFile=true -r win-x64 -c Release --self-contained true

echo "Copying files to the final directory"
xcopy Defsite\bin\Release\win-x64\publish\ Defsite\bin\Defsite\ /S /I /Y

echo "Making .dlls hidden"
attrib +h Defsite\bin\Defsite\cimgui.dll
attrib +h Defsite\bin\Defsite\cimguizmo.dll
attrib +h Defsite\bin\Defsite\glfw3.dll

rem Should copy redistributables and also maybe generate the installer

pause