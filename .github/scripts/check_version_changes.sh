#!/bin/bash
backendVersionNumber=$(grep '<VersionPrefix>' ./PxGraf/PxGraf.csproj | grep -o "[0-9]*\.[0-9]*\.[0-9]*")
frontendVersionNumber=$(grep '"version"' ./PxGraf.Frontend/package.json | grep -o "[0-9]*\.[0-9]*\.[0-9]*")

git fetch origin dev --quiet

backendVersionInDev=$(git show origin/dev:PxGraf/PxGraf.csproj | grep '<VersionPrefix>' | grep -o "[0-9]*\.[0-9]*\.[0-9]*")
frontendVersionInDev=$(git show origin/dev:PxGraf.Frontend/package.json | grep '"version"' | grep -o "[0-9]*\.[0-9]*\.[0-9]*")

echo "Backend version: This branch $backendVersionNumber, dev branch $backendVersionInDev"
echo "Frontend version: This branch $frontendVersionNumber, dev branch $frontendVersionInDev"

smallerOrEqual() {
    if [[ $1 == $2 ]]
    then
        return 0
    fi

    local oldIFS=$IFS
    IFS='.'
    read -ra in1Vers <<< "$1"
    read -ra in2Vers <<< "$2"
    IFS=$oldIFS

    for i in 0 1 2
    do
	    if ((10#${in1Vers[i]} > 10#${in2Vers[i]}))
        then
            return 1
        fi
    done
    return 0
}

fail_pipeline=false

if ! git diff --quiet origin/dev HEAD PxGraf; then
    if smallerOrEqual $backendVersionNumber $backendVersionInDev
	then
        echo "##vso[task.logissue type=error]Backend version number needs to be updated."
        echo "##vso[task.complete result=Failed;]"
        fail_pipeline=true
	fi
fi

if ! git diff --quiet origin/dev HEAD PxGraf.Frontend; then
    if smallerOrEqual $frontendVersionNumber $frontendVersionInDev
    then 
        echo "##vso[task.logissue type=error]Frontend version number needs to be updated."
        echo "##vso[task.complete result=Failed;]"
        fail_pipeline=true
	fi
fi

if [ "$fail_pipeline" = true ]; then
    exit 1
fi