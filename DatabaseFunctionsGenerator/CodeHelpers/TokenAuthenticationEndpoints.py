#generated automatically
from flask_restful import Resource
from flask_restful import wraps, abort
from SqlAlchemyMain import session
from Authentication import checkToken
class TokenAuthenticationEndpoints(Resource):
	def __init__(self, **kwargs):
		self.session = kwargs['session']
	
	#API endpoints
	#post endpoint
	def post(self):
		return Authentication.login(self.session)  


def authenticate(func):
	@wraps(func)
	def wrapper(*args, **kwargs):
		if not getattr(func, 'authenticated', True):
			return func(*args, **kwargs)

		isAuthorized, error = checkToken(session) 

		if isAuthorized:
			return func(*args, **kwargs)

		abort(401, message=error)
		return error
	return wrapper
