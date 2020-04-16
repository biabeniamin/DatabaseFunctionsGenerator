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
	#post endpoint
	def post(self):
		requestedArgs = getArguments(['username', 'password'])
		args  = requestedArgs.parse_args()
		return login(self.session, args['username'], args['password'], request.remote_addr)  


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
