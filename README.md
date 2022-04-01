# Facebook-Video-Downloader
A simple console application (C#) to download facebook videos.  
This app uses selenium because its needs to intercept network activity to get the video url.  
If the video has audio, we need to get a video and audio url separately from the network activity, to then join them (with FFmpeg).  
The app was modeled to keep up with current facebook changes.

## Dependencies
- FFmpeg
- Chromedriver

## How to run
You need to install or update the Google Chrome navigator.  
Furthermore, all dependencies are already inside the application, so just press the F5 button and see the magic.

## Example
![image](https://user-images.githubusercontent.com/51132386/160184856-2694c83f-f579-4055-b031-026fcfd5fad1.png)
