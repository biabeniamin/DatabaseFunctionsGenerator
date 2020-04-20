#generated automatically
from flask_restful import Resource
from flask_restful import wraps, abort
from SqlAlchemyMain import session
from flask import request
from FlaskRestfulHelpers import getArguments
from Authentication import checkToken, login
class TokenAuthenticationEndpoints(Resource):
	def __init__(self, **kwargs):
		self.session = kwargs['session']
	
	#API endpoints
	#get endpoint to check token
	def get(self):
		requestedArgs = getArguments(['token'])
		parsedArgs  = requestedArgs.parse_args()
		isAuthorized, error = checkToken(session, parsedArgs['token'], request.remote_addr)
		if isAuthorized:
			return {'status' : 'ok'}
		abort(401, message=error)
	#post endpoint	
	def post(self):
		requestedArgs = getArguments(['username', 'password'])
		args  = requestedArgs.parse_args()
		token, succ = login(self.session, args['username'], args['password'], request.remote_addr)  
		if succ == 0:
			abort(401, error=token)
		return token



def authenticate(func):
	@wraps(func)
	def wrapper(*args, **kwargs):
		if not getattr(func, 'authenticated', True):
			return func(*args, **kwargs)

		requestedArgs = getArguments(['token'])
		parsedArgs  = requestedArgs.parse_args()
		isAuthorized, error = checkToken(session, parsedArgs['token'], request.remote_addr)

		if isAuthorized:
			return func(*args, **kwargs)

		abort(401, message=error)
		return error
	return wrapper
