#generated automatically
from flask_restful import Resource
import Authentication
class TokenAuthenticationEndpoints(Resource):
	def __init__(self, **kwargs):
		self.session = kwargs['session']
	
	#API endpoints
	#post endpoint
	def post(self):
		return Authentication.login(self.session)  