@echo off
echo "Setup symlinks"
mklink /D llapi-client\Assets assets-folder
mklink /D llapi-server\Assets assets-folder
mklink /D llapi-client\ProjectSettings project-settings
mklink /D llapi-server\ProjectSettings project-settings

echo "Done"