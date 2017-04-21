if "%1"=="-h" goto _help  
if "%1"=="" set dir="."   
goto _lab  
set dir="%1"  
:_lab  
for /R %dir% %%i in (vc60.pdb vc90.pdb *.exp *.obj *.pch *.idb *.ilk *.sbr *.tmp *.res *.dcu *.opt *.ncb *.plg *.bsc *.aps *.exp *.log *.wrn *.mac *.xml *.vcxproj.user) do @del /f /s /q "%%i"  
for /R %dir% %%i in (Debug Release)do @rd /q "%%i"  
goto _end  
:_help  
:_end  