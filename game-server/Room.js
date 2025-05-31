const colyseus = require("colyseus");
const schema = require("@colyseus/schema");
 
class GameState extends schema.Schema {}

class Room extends colyseus.Room {
  
  onCreate() {
    this.state = new GameState(); 
    this.players = []; 

    this.onMessage("chat_message", (client, data) => {
      this.broadcast("chat_message", {
        sender: client.sessionId,
        message: data.chatMessage}) // 
 
    });
  }

  onJoin(client) {
    this.players.push({ sender: client.sessionId });
    this.broadcast("player_joined", { sender: client.sessionId });
  }

  onLeave(client) {
    this.players = this.players.filter(p => p.sender !== client.sessionId);
    this.broadcast("player_left", { sender: client.sessionId });
  }
}

module.exports = Room;