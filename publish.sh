#!/bin/sh

SCRIPT_DIR=$(dirname "$(readlink -f "$0")")

dotnet publish -r linux-x64 -p:PublishSingleFile=true --no-self-contained -c Release -o $SCRIPT_DIR/publish/linux-x64/ $SCRIPT_DIR/CardOrganizer/CardOrganizer.csproj
#dotnet publish -r win-x64 -p:PublishSingleFile=true --no-self-contained -c Release -o bin/publish/win-x64/

mkdir $HOME/.local/bin
ln -s $SCRIPT_DIR/publish/linux-x64/cardorganizer $HOME/.local/bin/
