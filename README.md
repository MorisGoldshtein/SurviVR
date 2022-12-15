# SurviVR
SurviVR is a VR game made for the Meta Quest 2 that pits the player against enemies in a variety
of different levels. Players can choose to duke it out with enemies, slash them with swords, throw
furniture at them, or take part in a fast-faced shooting challenge.

NOTE: This game is currently only fully compatible with Oculus devices that come with dual handheld controllers as OVR toolkit was used for development.

# Group Members
* Moris Goldshtein
* Ken Ko
* Lucas Gunkel
* Dillon Phong

# Replication
1. Install Unity version 2021.3.15f1 and open the project. More recent versions may be compatible, but Unity warns against older versions.
2. In File -> Build Settings, ensure platform is set to Android 
3. In Android settings, ensure that Run Device is set to the headset you seek to build on. If it is not there, then you will need to go through online tutorials about setting up Android Studio, Oculus ADB drivers, Developer mode, etc.
4. Leave all other settings as they are and press Build And Run. The process may take a few minutes.
5. Once the build is successful, the game will automatically start up.

Alternatively, if you wish to play the game in the editor itself,
1. Ensure Quest Link is active and that you can see the desktop through the headset.
2. In Edit -> Project Settings -> XR Plug-in Management, click the Android icon. Ensure that Oculus is checked.
3. Hit play and the game should play as though it were installed onto the headset already.



