@echo off
echo "Clone dependencies"
echo "UniRx"
call git clone https://github.com/neuecc/UniRx.git unirx
echo "Zenject"
call git clone https://github.com/modesttree/Zenject.git zenject
echo "Setup symlinks"
call mklink /D assets-folder\Zenject ..\zenject\UnityProject\Assets\Zenject
call mklink /D assets-folder\Plugins\UniRx ..\unirx\Assets\Plugins\UniRx
call mklink /D llapi-client\Assets ..\assets-folder
call mklink /D llapi-server\Assets ..\assets-folder
call mklink /D llapi-client\ProjectSettings ..\project-settings
call mklink /D llapi-server\ProjectSettings ..\project-settings
echo "Done"