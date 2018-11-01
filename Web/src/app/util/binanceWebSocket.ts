export class BinanceWebSocket {
    private baseUrl: string = "wss://stream.binance.com:9443/ws/!ticker@arr";
    private webSocket: WebSocket;
    private returnFuction: any;

    public static Initialize(returnFuction: any) : BinanceWebSocket {
        let socket = new BinanceWebSocket();
        socket.returnFuction = returnFuction;
        socket.openConnection();
        return socket;
    }

    private openConnection() : void {
        if (!this.webSocket || this.webSocket.readyState == WebSocket.CLOSED) {
            try {
                this.webSocket = new WebSocket(this.baseUrl);
                this.webSocket.onopen = (openEvent) => {
                };
                this.webSocket.onclose = (closeEvent) => {
                    setTimeout(() => this.openConnection(), 1000);
                };
                this.webSocket.onerror = (errorEvent) => {
                    console.error("Bnb ERROR: " + JSON.stringify(errorEvent));
                    this.webSocket.close();
                };
                this.webSocket.onmessage = (messageEvent) => {
                    if (this.returnFuction) {
                        this.returnFuction(messageEvent.data);
                    }
                };
            } catch (exception) {
                console.error(exception);
            }
        }
    }
}