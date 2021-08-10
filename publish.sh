#!/bin/bash

dotnet publish -r linux-x64 -p:PublishSingleFile=true --no-self-contained -c Release -o bin/publish/linux-x64/
dotnet publish -r win-x64 -p:PublishSingleFile=true --no-self-contained -c Release -o bin/publish/win-x64/
cp bin/publish/linux-x64/cardorganizer $HOME/.local/bin/
