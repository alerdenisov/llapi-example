@echo off
echo "Clone dependencies"
echo "UniRx"
call git clone https://github.com/neuecc/UniRx.git unirx
echo "Zenject"
call git clone https://github.com/modesttree/Zenject.git zenject
echo "Setup symlinks"
call mklink /D assets-folder\Zenject ..\zenject\UnityProject\Assets\Zenject
call mklink /D assets-folder\Plugins\UniRx ..\..\unirx\Assets\Plugins\UniRx
echo "Done"