# Accaber-Plugin
Plugin for the AccSaber leaderboards

## Reporting Issues
* The best way to report issues is to click on the `Issues` tab at the top of the GitHub page. This allows any contributor to see the problem and attempt to fix it, and others with the same issue can contribute more information. **Please try the troubleshooting steps before reporting the issues listed there. Please only report issues after using the latest build, your problem may have already been fixed.**
* Include in your issue:
  * A detailed explanation of your problem (you can also attach videos/screenshots)
  * **Important**: The log file from the game session the issue occurred (restarting the game creates a new log file).
    * The log file can be found at `Beat Saber\Logs\_latest.log` (`Beat Saber` being the folder `Beat Saber.exe` is in).
* If you ask for help on Discord, at least include your `_latest.log` file in your help request.

## Contributing
Anyone can feel free to contribute bug fixes or enhancements to ScoreRequirement! Fork, make your changes and pull request it!
### Building
Visual Studio 2019 with the [BeatSaberModdingTools](https://github.com/Zingabopp/BeatSaberModdingTools) extension is the recommended development environment.
1. Check out the repository
2. Open `accsaber-plugin.sln`
3. Right-click the `accsaber-plugin` project, go to `Beat Saber Modding Tools` -> `Set Beat Saber Directory`
  * This assumes you have already set the directory for your Beat Saber game folder in `Extensions` -> `Beat Saber Modding Tools` -> `Settings...`
  * If you do not have the BeatSaberModdingTools extension, you will need to manually create a `accsaber-plugin.csproj.user` file to set the location of your game install. An example is showing below.
4. The project should now build.

**Example csproj.user File:**
```xml
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="Current" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <BeatSaberDir>Full\Path\To\Beat Saber</BeatSaberDir>
  </PropertyGroup>
</Project>
```
## License
This project is licensed under the GNU GPL v3.0 License - see the [LICENSE](LICENSE) file for details.
