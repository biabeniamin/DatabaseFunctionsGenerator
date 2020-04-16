import { Injectable } from '@angular/core';
import { WebSocketSubject } from 'rxjs/webSocket';
import { Subject } from 'rxjs';

export class Request {
    constructor(
        public operation:string,
        public table:string,
        public data:any
    ) { }
}

export class Message
{
    constructor(
        public sender:string,
        public data:Request
    ) { }
}

@Injectable({
    providedIn : 'root'
})
export class WebSockets
{
    private socket$: WebSocketSubject<Request>;
    //public limeBasket: Subject<string> = new Subject<string>();
    private subscriber : {[table : string]:Subject<Message>} = {};
    private onConnectionEstablished : (() => void)[] = [];
    private isConnected : boolean;

	constructor()
	{
        this.socket$ = new WebSocketSubject('!--url--!');

        this.socket$
            .subscribe(
            (request) => {
                console.log(request)
                if(request.table == this.constructor.name)
                {
                    if(request["operation"] == "connectedSuccessfully")
                    {
                        this.onConnectionEstablished.forEach(callback => {
                            callback();
                        })
                    }
                }
                else if(this.subscriber[request.table] != null)
                    this.subscriber[request.table].next(new Message(this.constructor.name, request))
            },
            (err) => console.error(err),
            () => console.warn('Completed!')
            );
    }
    
    public SetOnConnectionEstablished(onConnectionEstablished : () => void)
    {
        this.onConnectionEstablished.push(onConnectionEstablished);

        if(this.isConnected)
            onConnectionEstablished();
    }

    public getSubject(table)
    {
        if(this.subscriber[table] != null)
            return this.subscriber[table];

        this.subscriber[table] =  new Subject<Message>();
        this.subscriber[table].subscribe((value) => {
            if(value.sender == this.constructor.name)
                return;
            console.log(value);
            this.socket$.next(value.data);
        })
        return this.subscriber[table]
    }

	public Authenticate(username, password)
	{
		this.socket$.next(new Request('login', 'TokenAuthentication', {'username' : username, 'password': password}));
	}
	

}
