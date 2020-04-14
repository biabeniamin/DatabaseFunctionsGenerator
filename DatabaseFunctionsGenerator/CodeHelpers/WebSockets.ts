﻿import { Injectable } from '@angular/core';
import { WebSocketSubject } from 'rxjs/observable/dom/WebSocketSubject';
import { Subject } from 'rxjs';


export class Message
{
    constructor(
        public sender:string,
        public data:any
    ) { }
}

export class Request {
    constructor(
        public operation:string,
        public table:string,
        public data:any
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

	constructor()
	{
        this.socket$ = new WebSocketSubject('ws://localhost:6789');

        this.socket$
            .subscribe(
            (request) => {
                console.log(request)
                if(this.subscriber[request.table] != null)
                    this.subscriber[request.table].next(new Message(this.constructor.name, request))
            },
            (err) => console.error(err),
            () => console.warn('Completed!')
            );
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
	

}
