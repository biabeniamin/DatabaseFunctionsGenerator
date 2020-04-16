#generated automatically
from SqlAlchemy import convertToJson, dict_as_obj
from WebSocketsHelpers import checkArguments, removeClosedConnection
from SqlAlchemy import convertToJson
import Authentication

async def requestReceived(websocket, session, request):
	if request['operation'] == 'login':
		data = request['data']
		if 'username' not in data or 'password' not in data:
			await websocket.send(json.dumps({'table': 'TokenAuthentication', 'operation' : 'invalidCredentials'}))
			return
		token, isSuccessful = Authentication.login(session, data['username'], data['password'], '*')
		if isSuccessful == 0:
			await websocket.send(convertToJson({'table': 'TokenAuthentication', 'operation' : 'invalidCredentials'}))
			return
		await websocket.send(convertToJson({'table': 'TokenAuthentication', 'operation' : 'authenticationGranted', 'data' : token}))
		websocket.authenticated = True
