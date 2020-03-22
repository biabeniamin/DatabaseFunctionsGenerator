import {HttpClient} from '@angular/common/http';
import { ServerUrl } from './ServerUrl'
import { Injectable } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';
import { TokenService } from './TokenService';
import { Token } from './Models/Token';
import { Router } from '@angular/router';

@Injectable({
    providedIn : 'root'
})
export class AuthenticationService
{
    public locations : Location[];

    RemoveToken()
    {
        this.cookieService.delete("token");
        console.log("no token or expired");
        this.router.navigate(['/login']);
    }

	GetToken()
	{
        if(!this.cookieService.check("token"))
            this.RemoveToken();

        let token = this.cookieService.get("token"); 
        this.http.get<string>(ServerUrl.GetUrl()  + `Authentication.php?cmd=checkToken&token=${token}`).subscribe(data =>
        {
            console.log(data);
            if(data["status"] == "ok")
            {
                console.log("logged in!");
            }
            else
            {
                this.RemoveToken();
            }

        });
		
    }
    
    Login(username, password)
    {
        let login = {"username" : username, "password" : password};
		console.log(login);
		return this.http.post<Token>(ServerUrl.GetUrl()  + "Authentication.php?cmd=addToken", login).subscribe(token =>
		{
                console.log(token);
                if(token.tokenId != 0)
                {
                    this.cookieService.set("token", token.value);
                }
				
		});
    }
    
    constructor(private http:HttpClient, private cookieService: CookieService, private tokenService : TokenService,
        private router: Router)
	{
	
	}

}
