@rem --- MS VS 2010 command prompt ---
@rem %comspec% /k ""C:\Program Files\Microsoft Visual Studio 10.0\VC\vcvarsall.bat"" x86

"%windir%\Microsoft.NET\Framework\v4.0.30319\InstallUtil" /ServiceName=SH.Service "%~dp0\SH.Service.exe"

@pause
