# PKInfRemi

[![Build status](https://ci.appveyor.com/api/projects/status/aaut18bj6qoj7bd7/branch/faceDetection?svg=true)](https://ci.appveyor.com/project/chris579/pkinfremi/branch/faceDetection)
[![GitHub issues](https://img.shields.io/github/issues/chris579/PKInfRemi.svg)](https://github.com/chris579/PKInfRemi/issues)

A simple face detection (and recognition) app written in C#.

This application is currently **work in progress** and was created by students in a project course from a secondary school in Germany.

## Features
- Read webcam input and display a live feed on screen
- Detect faces within this live feed (front, profile or both possible)
- Save faces into a sqlite database for recognition
- Manage all saved faces
- Recognition for saved faces

## Planned Features
- Auto learning processs saving faces automatically into the database and process them
- Performance optimizations

## Requirements

Windows 7 (32/64-bit) and later

.NET Framework 4.5 and later (Download [here](https://www.microsoft.com/de-de/download/details.aspx?id=30653)).

Currently only the application itself is provided as a portable installation. You have to unpack this package somewhere to be able to run the application. 

*Installer is WIP.*

### How it was made

This is a WPF (Windows Presentation Framework) application written in pure C#. 
The wrapper for the cam processing is EmguCV which relies on OpenCV, a C++ library handling and processing camera input quite fast.

The ui was designed with [MahApps Metro](http://mahapps.com/) which enables much possibilities for interface designing. 

#### Used libraries
- [EmguCV](http://www.emgu.com/)
- [MahApps Metro](http://mahapps.com/)
- [MvvmLight](http://www.mvvmlight.net/)
- [PostSharp](https://www.postsharp.net/)



### Preview Images

**For those who are more intereseted into images than words.**

The main view:
![The main view](https://cloud.githubusercontent.com/assets/6552521/16057258/8811b2ac-3279-11e6-8aae-31a71161afde.png)

*More images coming soon*
