const http = require("http");
const express = require("express");
const colyseus = require("colyseus");
const Room = require("./Room");

const app = express();
const server = http.createServer(app); // Express 기반 http 서버 생성
const gameServer = new colyseus.Server({ server }); // Colyseus 서버를 http 서버 위에서 구동 

gameServer.define("gameRoom", Room); // gameRoom이라는 이름으로 Room 등록
gameServer.listen(2567); // 임시 포트

console.log("RUNNING");