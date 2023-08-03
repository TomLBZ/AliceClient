# AliceClient

## Description
This is a command line utility for developing backend microservices using C#. It is designed to be used with the AliceServer project.

## Installation
1. Clone and build this repo in Release mode
1. Go to your bin/Release folder and run the alice.exe binary as Administrator (or root in Linux).
1. Enter i (stands for installation) when prompted.
1. Enter an *empty* directory as your alice installation directory. This directory will be added to the PATH environment variable.

## Get Started
Open your favourite shell and run the following command:
```bash
alice help
```
You will be able to see all commands available. For your reference, here is a quick list of the currently implemented commmands:
- help: show help message
- install installPath: install alice to the specified path
- uninstall: uninstall alice
- create lang projectPath: create a new project at the specified path. The path must be empty. lang can be CS (C#) or TS (TypeScript, currently unimplemented).
- build projectPath user: build the project at the specified path and upload the docker image under the specified user's docker hub account.

## Examples
### Install alice
```powershell
alice install C:\alice
```
### Create a new project
```powershell
# make sure you can pull things from github before trying this!
# if github is not accessible, you can use a VPN or download the project template manually
# create a new project in C-Sharp at D:\alice_projects\echo
# Echo is the default project template, feel free to modify it using Visual Studio IDE
# it is recommended to store the projects elsewhere from the installation directory
# because the installation directory will be deleted when alice is uninstalled
alice create CS D:\alice_projects\echo
```
### Build a project
```powershell
# login to docker hub first
docker login
# build the project and upload the docker image to docker hub
alice build D:\alice_projects\myproject myuser
```

## Next Step
Please refer to the AliceServer project for more information about how to deploy and run your microservices.
Some unimplemented features include:
- [ ] Support for multiple project templates (TS language)
- [ ] Being able to register the image directly with the server