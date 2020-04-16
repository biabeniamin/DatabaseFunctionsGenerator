#generated automatically
from SqlAlchemy import convertToJson, dict_as_obj
from WebSocketsHelpers import checkArguments, removeClosedConnection
from SqlAlchemy import convertToJson
import Authentication

async def requestReceived(websocket, session, request):
	if request['operation'] == 'login':
		data = request['data']
		if 'username' not in data or 'password' not in data:
			await websocket.send(convertToJson({'table': 'TokenAuthentication', 'operation' : 'invalidCredentials'}))
			return
		token, isSuccessful = Authentication.login(session, data['username'], data['password'], '::1')
		if isSuccessful == 0:
			await websocket.send(convertToJson({'table': 'TokenAuthentication', 'operation' : 'invalidCredentials'}))
			return
		await websocket.send(convertToJson({'table': 'TokenAuthentication', 'operation' : 'authenticationGranted', 'data' : token}))
		websocket.authenticated = True
	elif request['operation'] == 'setToken':
		data = request['data']
		if 'token' not in data:
			await websocket.send(convertToJson({'table': 'TokenAuthentication', 'operation' : 'invalidToken'}))
			return
		isAuthorized, error = Authentication.checkToken(session, data['token'], '::1')
		if isAuthorized == 0:
			await websocket.send(convertToJson({'table': 'TokenAuthentication', 'operation' : 'invalidToken', 'data': {'error' : error}}))
			return

		await websocket.send(convertToJson({'table': 'TokenAuthentication', 'operation' : 'validToken'}))
