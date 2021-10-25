cd %~dp0

SET minorVersion="8.54-beta"
SET majorVersion="0"

SET projectID=Viki.LoadRunner.Tools.Legacy
SET project1="..\\..\src\\%projectID%\\%projectID%.csproj"

SET StudioType=Community
IF NOT "%1"=="" (
SET StudioType=%1
)

"C:\Program Files (x86)\Microsoft Visual Studio\2019\%StudioType%\MSBuild\Current\Bin\MSBuild.exe" "%project1%" /verbosity:m /target:Rebuild /p:GenerateBuildInfoConfigFile=false /p:VisualStudioVersion=16.0 /p:platform=AnyCPU /p:TargetFramework=net471 /p:Configuration=Release /p:OutputPath="%cd%\pack\lib\net471" /p:DebugSymbols=false /p:DebugType=none /P:SignAssembly=False /p:DocumentationFile=%projectID%.xml

%windir%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe EditNuspec.msbuild /p:File="%projectID%.nuspec" /p:projectID="%projectID%" /p:majorVersion=%majorVersion% /p:minorVersion=%minorVersion%

echo Try to clean up
del %projectID%.nupkg
echo Clean up finished

echo Try to remove necessary dll's

del pack\lib\net471\Newtonsoft.Json.dll
del pack\lib\net471\Newtonsoft.Json.xml
del pack\lib\net471\Newtonsoft.Json.pdb
del pack\lib\net471\Viki.LoadRunner.Engine.dll
del pack\lib\net471\Viki.LoadRunner.Engine.deps.json
del pack\lib\net471\System.Data.Common.dll
del pack\lib\net471\System.Diagnostics.StackTrace.dll
del pack\lib\net471\System.Diagnostics.Tracing.dll
del pack\lib\net471\System.Globalization.Extensions.dll
del pack\lib\net471\System.IO.Compression.dll
del pack\lib\net471\System.Net.Http.dll
del pack\lib\net471\System.Net.Sockets.dll
del pack\lib\net471\System.Runtime.Serialization.Primitives.dll
del pack\lib\net471\System.Security.Cryptography.Algorithms.dll
del pack\lib\net471\System.Security.SecureString.dll
del pack\lib\net471\System.Threading.Overlapped.dll
del pack\lib\net471\System.Xml.XPath.XDocument.dll

echo Necessary dll's removed

echo Creating package
..\NuGet.exe pack .\pack\%projectID%.nuspec

ren %projectID%.%majorVersion%.%minorVersion%.nupkg %projectID%.nupkg
echo Packet renamed to %projectID%.nupkg

echo Try to remove 'pack' folder
rd pack /S /Q
echo 'pack' folder removed

pause