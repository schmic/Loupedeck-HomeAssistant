# Loupedeck-HomeAssistant

This plugin enables you to control various [Home-Assistant](https://home-assistant.io) entities with your Loupedeck.

Supported entities:
- Automations
  - toggle on & off 
  - trigger
- Buttons
  - press
- Climate
  - set `temperature`
- Cover
  - toggle open & close
  - set `position` with knob  
- Dimmers (lights with `brightness` attribute)
  - on & off
  - set `brightness` with knob
- Lights
  - toggle on & off 
- Locks
  - toggle lock & unlock
- Scene
  - turn on
- Script
  - execute, simple option via `Script` Folder or via manually configuration with data option
- Sensor
  - just visual representation of the state
- Switches
  - toggle on & off
Water Heater
  - just visual representation of the state

The plugin communicates via the [Websocket API](https://developers.home-assistant.io/docs/api/websocket/) of Home-Assistant.
State changes are immediately reflect on your Loupedeck display.

## Installation
- download the latest .lplug4 asset [here](https://github.com/schmic/Loupedeck-HomeAssistant/releases/latest)
- install into the Loupedeck software
  - right click and select `Install Plugin`

This plugin is not yet published to the official Loupedeck Store.
Only available for Windows, tested with Loupedeck Live.

## Configuration

- Create the following path & file in your home folder:
  `C:/Users/<USERNAME>/.loupedeck/homeassistant/homeassistant.json`
- Copy & Paste the example into the file
- Replace token and URL
  - for the URL you can simply use the URL from your browser, the plugin will do the rest
  - using the `Nabu Casa-URL` is possible but not recommended, especially in a LAN setup
  - you must not add a path to the URL, it will be ignored and replaced with `/api/websocket`

Examples:
```
{
    "token": "eyJhbGciOiJIUzI1NiIsInR......",
    "url": "http://your.home.assistant.host:8123"
}
```
```
{
    "token": "eyJhbGciOiJIUzI1NiIsInR......",
    "url": "https://your.home.assistant.host"
}
```

If you are already using [Lubedas Home-Assistant Plugin](https://github.com/lubeda/Loupedeck-HomeAssistantPlugin) you don't have to do anything,
this plugin uses the same type of configuration but ignores any other configuration settings except `token` and `url`.

### Home-Assistant Token
To get a token you have to log into your Home-Assistant, then go to your Profile page. At the bottom you can create a token.
Someone explained it [here](https://community.home-assistant.io/t/how-to-get-long-lived-access-token/162159/5) as well.

## Logs
From Loupedeck 6.0+ onward you can find the plugin logs in:
- `%LOCALAPPDATA%\Logi\LogiPluginService`

## Bugs & Issues
- Climate Control, current state is WIP
  - visually different to others, no icons, no colors
  - only shows the current state reported from HA
  - if you change the value with a knob you won't see the result until it is synced with the thermostat
    - with sleeping devices this can take a couple of minutes depending on your configuration

# Icons
All icons used from the [Material Design Icons](https://pictogrammers.com/docs/general/about/) group and are [licensed](https://github.com/Templarian/MaterialDesign/blob/master/LICENSE) by them.

# TODOs & IDEAs

- deal with long `friendly_name` entries better
- override `Reset` for adjustments

## Buttons
- are they actually of any use?

## Lights
- add support for different modes
  - rgbw/led 

# Commandline Paramters
- -enablelogs
