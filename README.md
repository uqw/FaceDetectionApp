# PKInfRemi

[![Build status](https://ci.appveyor.com/api/projects/status/aaut18bj6qoj7bd7/branch/faceDetection?svg=true)](https://ci.appveyor.com/project/chris579/pkinfremi/branch/faceDetection)
[![GitHub issues](https://img.shields.io/github/issues/chris579/PKInfRemi.svg)](https://github.com/chris579/PKInfRemi/issues)

A simple face detection (and recognition) app written in C#.

This application is currently **work in progress** and was created by students in a project course from an secondary school in Germany.

## Features
- Read webcam input and display a live feed on screen
- Detect faces within this live feed (front, profile or both possible)
- Save faces into a sqlite database for recognition
- Manage all saved faces

## Planned Features
- Recognition for saved faces
- Auto learning processs saving faces automatically into the database and process them
- Performance optimizations

## Requirements

Windows 7 (32/64-bit) + Later

.NET Framework 4.5 or later (Download [here](https://www.microsoft.com/de-de/download/details.aspx?id=30653)).

Currently only the application itself is provided. Installer is WIP.

### How it was made

This is a WPF (Windows Presentation Framework) application written in pure C#. 
The wrapper for the cam processing is EmguCV which relies on OpenCV, a C++ library handling and processing camera input quite fast.

The ui was designed with [MahApps Metro](http://mahapps.com/) which enables much possibilities for interface designing. 
