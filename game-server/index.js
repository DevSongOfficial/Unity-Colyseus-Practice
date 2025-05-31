const http = require("http");
const express = require("express");
const colyseus = require("colyseus");
const Room = require("./Room");

const app = express();
const server = http.createServer(app); // create http server based on express
const gameServer = new colyseus.Server({ server }); // Colyseus 서버를 http 서버 위에서 구동 

gameServer.define("gameRoom", Room); // gameRoom이라는 이름으로 Room 등록


// Temporarily fetching room list manually because Colyseus lobby isn't working. 도대체 왜??
app.get("/rooms", async (req, res) => {
  try {
    const rooms = await colyseus.matchMaker.query({}); // 조건 없이 전체 방

    const roomList = rooms.map((room) => ({
      roomId: room.roomId,
      name: room.name,
      clients: room.clients,
      maxClients: room.maxClients,
      //metadata: room.metadata || {},
    }));

    res.json(roomList);
  } catch (err) {
    console.error("ERROR WHILE FETCHING ROOM LIST:", err.stack || err);
    res.status(500).json({ error: "SERVER ERROR" });
  }
});


gameServer.listen(2567);

console.log("RUNNING");