#!/bin/sh
cd ..
CurrentUser=$USER
sudo chown $CurrentUser RadFramework.Libraries -R
cd RadFramework.Libraries
echo "$CurrentUser owns the project now."
