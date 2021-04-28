# Computer Science Final Year Project 2020
### Dillon Condon 115432398

### Table of Contents
- [Introduction](#introduction)
- [Releases](#releases)
- [VR Release](#vr---windows-build)
- [AR Release](#ar---android-build)
- [Web Release](#webgl---web-build)

## Introduction
The aim of this project is to develop a application that can be used to create an interactive AR/VR experience on important and relevant areas of UCC. AR ( Augmented Reality ) and VR ( Virtual Reality ) are two very different Concepts, however they do contain similar characteristics with each other and both can reach the objectives of this project. This project outlines that the objective of the completed application would be to allow users to view and interact with a virtually created world of UCC.

Currently, within the project there exists 3 versions : [VR](#vr---windows-build) , [AR](#ar---android-build) and [Web](#webgl---web-build) . Each version is built using the [Unity Game Engine](https://unity.com/) software using [version 2019.3.10f](https://unity3d.com/unity/whats-new/2019.3.10). This repository contains all relevant files for each unity projecrt and can be opened in Unity by using the correct version as listed. 

To run the applications on their respective platforms just check out the [releases](#releases) below. However, you can access an upto date WebGL build running here : https://thedylon.com/fyp2020/

## Releases
The most recent builds of this project can be found within the "Builds" folder of the respective project, or all published releases can be accessed directly here : [All Releases](https://github.com/spicydilly/CS-FYP-2020/releases)

### VR - Windows Build 
The most recent VR build download and release notes can be accessed [here](https://github.com/spicydilly/CS-FYP-2020/releases/tag/v0.3-alpha-vr)

The download will provide you with a .zip file. Simply unzip this file to continue. If you need an unzipping program, try out this : https://www.win-rar.com/

Inside the unzipped folder will contain a VR_Test.exe which can be run on a windows machine. Running this should automatically open the application with your VR desktop applicaiton and run on your VR device, for example if you are using the Oculus Rift or the Oculus Quest with the Ouclus Link. However, if you have not already set up your Oculus device with your computer, I would recommend doing so via : https://www.oculus.com/setup/ before proceeding. 

You may need to allow "Unknown Sources" on the Oculus desktop application, to do this, simply open the Oculus Desktop application then click on "Settings" then "General" and there will be an "Unknown Sources" option here that must be enabled. 

How to play :
- Teleport with the right controller using the right trigger. A "teleport" reticle will appear showing the destination
- Continuous movement can be achieved by using the right controllers analog stick
- Object placement is done via the left controllers trigger
- Can rotate the player by using the left controllers analog stick

### AR - Android Build
The most recent AR build download and release notes can be accessed [here](https://github.com/spicydilly/CS-FYP-2020/releases/tag/v0.3-alpha-ar) 

Just download the .apk file to your Android device, and then locate that file on the device using a file manager. After clicking on the .apk you should be propted to install the application, you may get some errors which can be ignored such as the applicaiton may be potentially dangerous ( that is fine, this applicaiton is just for development and is not published through the Google Play Store ). You may get an error indicating you are unable to install unknown sources/apps , this can be fixed by just enabling this setting on the Android device.

Once the app is installed, you may be prompted to give access to your Camera, this must be accepted as this is a requirement for running the applicaiton.

How to play :
- Point your camera towards the floor. You may have to move your device around abit as AR Core needs to gather some information in order to correctly locate the floor. You will see an object appear if the placement area is valid, clicking on the screen will stick this object to the floor. 


### WebGL - Web Build
The most recent Web build download and release notes can be accessed [here](https://github.com/spicydilly/CS-FYP-2020/releases/tag/v0.3-alpha-web) 

The download will provide you with a .zip file. Simply unzip this file to continue. If you need an unzipping program, try out this : https://www.win-rar.com/

The extracted files must be placed on a server to run. After placing on the server of your choice, just access the index.html page of that directory and the project should run. Further information on running the WebGL can be found here : https://docs.unity3d.com/2019.3/Documentation/Manual/webgl-building.html 

You can access an already published version of this build here : https://thedylon.com/fyp2020/

How to play : 
- Movement is via "W, A, S, D" or "Up, Left, Down, Right" keys
- Object placement is done by left clicking
- To unlock the cursor from the web player, click the "Esc" key 
