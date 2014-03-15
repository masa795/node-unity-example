
set wsShell=WScript.CreateObject("wscript.shell") 
dim f_path

f_path = wsShell.CurrentDirectory 
f_path = f_path & "/"

wsShell.Exec("C:\Program Files (x86)\Unity4\Editor\Unity.exe -projectPath " & f_path )

