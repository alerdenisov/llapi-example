@echo off
echo "Setup symlinks"
call mklink /D llapi-client\Assets ..\assets-folder
call mklink /D llapi-server\Assets ..\assets-folder
call mklink /D llapi-client\ProjectSettings ..\project-settings
call mklink /D llapi-server\ProjectSettings ..\project-settings

echo "Done"