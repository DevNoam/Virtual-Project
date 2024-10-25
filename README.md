# VirtualProject - Virtual world demo project
### Overview
VirtualProject is my first multiplayer attempt. I tried to clone games like Club penguin, CP-world etc.
This project is using Unity, Mirror and Playfab for the Backend (BaaS).

I've hosted this project on my website (WebGL) for a few years but open sourced it due to management complexity and management costs to run this game server.
Videos and pictures of this project available on my website: https://noamsapir.me/Virtualproject


## Deployment

To deploy this project. You should install Unity 2020.3.43f1 and compile 2 versions.
NOTE, that Backend service is not linked. You would need to setup your Playfab ID.
1. Linux headless build (Server).
You would need to configure SSL if the web server will use https.
Read Mirror WebSockets Transport for that: https://mirror-networking.gitbook.io/docs/manual/transports/websockets-transport

2. WebGL (Client).
## Screenshots

![App Screenshot](https://noamsapir.me/img/games/virtualproject/Screenshot%20(4).png)
![App Screenshot](https://noamsapir.me/img/games/virtualproject/Screenshot%20(7).png)