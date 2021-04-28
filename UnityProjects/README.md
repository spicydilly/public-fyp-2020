# Computer Science Final Year Project 2020
### Dillon Condon 115432398

## AR & VR Testing

This directory contains some sample test UNITY projects I created for testing both the AR & VR implementations of this project. These projects will likely not be very refined, 
that is why they are stored separately in their own directory. Aspects of these tests will however be used in the final completed project, and some asepects of course will not.

Right now, the projects contain a simple iteration of a Portal.

Whether a project is AR or VR is clearly defined by the name of the project directory either containing AR or VR. AR is built to run on android devices, as it requires a camera. 
VR is implemented for a VR capable device, such as the Oculus Rift or Oculus Quest via the Oculus Link, I have not yet tested directly on the Oculus Quest without using the Oculus Link. 

## Builds
Each project has a completed build, which can be run without using the UNITY editor. These builds can be found in the respective **Builds** directories. 

### AR
The AR build will have a AR_Test1.apk file which can be installed on an android device. The android device at minimum should be running Android Nougat 7.0. (or API Level 24 ) as this is the requirement for using 
AR Core on android devices.

Just download the .apk file to your Android device, and then locate that file on the device using a file manager. After clicking on the .apk you should be propted to install the application, you may get some errors
which can be ignored such as the applicaiton may be potentially dangerous ( that is fine, this applicaiton is just for development and is not published through the Google Play Store ). You may get an error indicating 
you are unable to install unknown sources/apps , this can be fixed by just enabling this setting on the Android device.

Once the app is installed, you may be prompted to give access to your Camera, this must be accepted as this is a requirement for running the applicaiton.

Now, just point your camera towards the floor. You may have to move your device around abit as AR Core needs to gather some information in order to correctly locate the floor. You will see an object appear if the placement
area is valid, clicking on the screen will stick this object to the floor. The first object is a console, you must select which portal you want to create by clicking on the respective black button. After this you
will then be allowed to place a portal into the world!

### VR 
The VR build will contain a VR_Test.exe which can be run on a windows machine. Running this should automatically open the application with your VR desktop applicaiton and run on your VR device, for example if you are 
using the Oculus Rift or the Oculus Quest with the Ouclus Link.

You may however need to allow "Unknown Sources" on the Oculus desktop application, to do this, simply open the Oculus Desktop application then click on "Settings" then "General" and there will be an "Unknown Sources" 
option here that must be enabled. 

## Resources
There are many very useful videos and tutorials online on the subject of implemeting portals in UNITY. Below are some links videos which I found extremly useful in getting this set up. I have also inculuded links to 
some existing repostories I found that implement the AR Portals. 

The model I used for testing is not my own, however you can find this model here and it is freely available for eudcational purposes : https://github.com/jimmiebergmann/Sponza 

* AR Portal Tutorial with Unity - ARCore Setup - Part1 - https://www.youtube.com/watch?v=g78hQB8UKEM
* Overview - How To Unity AR Portal - https://www.youtube.com/watch?v=1cwm6sCcV_o&list=PLKIKuXdn4ZMhwJmPnYI0e7Ixv94ZFPvEP
* Introduction to VR in Unity - PART 1 : VR SETUP - https://www.youtube.com/watch?v=gGYtahQjmWQ