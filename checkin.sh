#!/bin/sh
cd ..
sudo chown anon RadFramework.Libraries -R
cd RadFramework.Libraries
git add *
git gui
