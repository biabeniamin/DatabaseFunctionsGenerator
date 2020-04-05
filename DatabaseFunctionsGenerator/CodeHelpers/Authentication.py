import Token
import TokenUser
from FlaskRestfulHelpers import getArguments
from flask import request
from datetime import datetime
import uuid
from flask_restful import wraps, abort
from SqlAlchemyMain import *

def login(session):
	requestedArgs = getArguments(['username', 'password'])
	args  = requestedArgs.parse_args()
	tokenUser = TokenUser.getTokenUsersByUsernamePassword(session, args['username'], args['password'])
	if len(tokenUser) == 0:
		print("invalid credentials")
		return {'error' : 'Invalid credentials'}

	token = Token.Token(value=str(uuid.uuid4()), address = request.remote_addr, lastUpdate = datetime.utcnow(), tokenUserId = tokenUser[0].tokenUserId)
	Token.addToken(session, token)
	return [token]

def checkToken(session):
	isAuthorized = 1
	error = None
	requestedArgs = getArguments(['token'])
	args  = requestedArgs.parse_args()
	token = Token.getTokensByValue(session, args['token'])
	if len(token) == 0:
		isAuthorized = 0
		error = {'error' : 'Invalid token'}
		return isAuthorized, error
	token = token[0]
	diff = datetime.utcnow() - token.lastUpdate
	days, seconds = diff.days, diff.seconds
	hours = days * 24 + seconds // 3600
	if hours > 1:
		isAuthorized = 0
		print("token timeouted")
		error = {'error' : 'Token expired'}

	if token.address != request.remote_addr:
		isAuthorized = 0
		print("different address")
		error = {'error' : 'Token generated for other address'}

	if isAuthorized == 0:
		Token.deleteToken(session, token.tokenId)
		return isAuthorized, error

	token.lastUpdate = datetime.utcnow()
	Token.updateToken(session, token)	
	return isAuthorized, error

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