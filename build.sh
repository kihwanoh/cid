#!/bin/sh
cd /project_build

. get_versions

gitver /updateassemblyinfo

xabuild Cid.sln /p:Configuration=Release

if [ "${TRAVIS_PULL_REQUEST}" = "false" ] && [ "${TRAVIS_TAG}" != "" ]; then
    nuget pack ./src/Cid/Cid.nuspec -BasePath ./src/Cid/ -Version $SYNC_VERSION -Properties Configuration=Release
fi
