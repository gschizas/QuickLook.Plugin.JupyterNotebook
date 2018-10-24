Remove-Item ..\QuickLook.Plugin.JupyterNotebook.qlplugin -ErrorAction SilentlyContinue
$config="Debug"

$files = Get-ChildItem -Path "..\QuickLook.Plugin.JupyterNotebook\bin\$config\" -Exclude *.pdb,*.xml
Compress-Archive $files ..\QuickLook.Plugin.JupyterNotebook.zip
Move-Item ..\QuickLook.Plugin.JupyterNotebook.zip ..\QuickLook.Plugin.JupyterNotebook.qlplugin