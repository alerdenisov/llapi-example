#!/bin/bash

echo "Clone dependencies"
echo "UniRx"
git clone https://github.com/neuecc/UniRx.git unirx
echo "Zenject"
git clone https://github.com/modesttree/Zenject.git zenject
echo "Setup symlinks"
ln -s ./assets-folder/Zenject ./zenject/UnityProject/Assets/Zenject
ln -s ./assets-folder/Plugins/UniRx ./unirx/Assets/Plugins/UniRx
ln -s ./llapi-client/Assets ./assets-folder
ln -s ./llapi-server/Assets ./assets-folder
ln -s ./llapi-client/ProjectSettings ./project-settings
ln -s ./llapi-server/ProjectSettings ./project-settings
echo "Done"