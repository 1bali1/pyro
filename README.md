
<p align="center">
  <a href="#">
    <img src="pyro/Assets/banner.png" alt="banner">
  </a>
</p>

<hr>


![License](https://img.shields.io/github/license/1bali1/pyro?style=for-the-badge) ![Release](https://img.shields.io/github/v/release/1bali1/pyro?style=for-the-badge) ![MongoDB](https://img.shields.io/badge/MongoDB-%234ea94b.svg?style=for-the-badge&logo=mongodb&logoColor=white) ![.Net](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white) ![Top Language](https://img.shields.io/github/languages/top/1bali1/pyro?style=for-the-badge)
<hr>

# Pyro Backend (C#)

- New C# version of Pyro Backend, instead of Python

<hr>

## Setup
**1. The database**
- Pyro backend uses MongoDB (you can download it [here](https://www.mongodb.com/docs/manual/administration/install-community/?operating-system=windows&windows-installation-method=wizard))
- I also recommend you to download MongoDB Compass, so you can manage your data (you can download it [here](https://www.mongodb.com/try/download/compass))

**2. .NET**
- The project uses .NET 10 SDK, which you can download from [here](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)

<hr>

## Run the backend
**1. Install**
- Download the dependencies with the following command:
```bash
dotnet restore
```

**2. Settings**
- **Environment variables**: `Rename .env.example to .env and add your necessary keys`
- **Config**: `Set your own settings in the Config/config.json file`
- **Itemshop**: `You can configure the itemshop and autoitem shop settings in the Config/itemshop.json file`

**3. Launch**
- Run the following command in the root folder:
```bash
dotnet run --project pyro
```

<hr>

## Discord bot
- To use admin commands, you need to set the owners of the bot in the Config/config.json file!
<details>
<summary>Current bot commands</summary>

- `/account create` - Creates an account for the user
- `/account ban` - Bans a player from the backend (needs owner permissions)
- `/account unban` - Unbans a player from the backend (needs owner permissions)
- `/account full-locker` - Gives full locker to a player (needs owner permissions)
- `/account delete` - Deletes your account 
- `/profile discord` - Retrieves a user's information by their Discord ID.
- `/profile name` - Retrieves a user's information by their player name
</details>

<hr>

## Useful information
- It is recommended to use (around) 2558px 1440px width and height for news to ensure the image is displayed properly
- (Not tested on multiple versions) The itemshop will probably always have the old UI, and the following titles will be interpreted differently there: 
  - **Featured**(In game) -> Weekly items(In config)
  - **Daily**(In game) -> Daily items(In config)
  - **Special Offers**(In game) -> Featured items(In config)
- If you need any help, open an issue
- If you want to play with friends, I recommend you to use ngrok or cloudflared tunnel

### Feautres
- [x] Auth system
- [ ] Friends
- [x] Itemshop
- [ ] Auto itemshop
- [ ] XMPP and WS services
- [ ] Advanced matchmaker
- [ ] Quests
- [ ] Battlepass
- [ ] Full locker support
- [x] Discord bot