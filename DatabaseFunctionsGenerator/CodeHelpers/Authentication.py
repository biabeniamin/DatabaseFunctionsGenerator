import Token
import TokenUser
from FlaskRestfulHelpers import getArguments
from flask import request
from datetime import datetime
import uuid

def login(session):
	requestedArgs = getArguments(['username', 'password'])
	args  = requestedArgs.parse_args()
	tokenUser = TokenUser.getTokenUsersByUsernamePassword(session, args['username'], args['password'])
	if len(tokenUser) == 0:
		print("invalid credentials")
		return []

	token = Token.Token(value=uuid.uuid4()), address = request.remote_addr, lastUpdate = datetime.utcnow(), tokenUserId = tokenUser[0].tokenUserId)
	Token.addToken(session, token)
	print(args)
	print(tokenUser)
	return tokenUser

def checkToken(session):
	isAuthorized = 1
	requestedArgs = getArguments(['token'])
	args  = requestedArgs.parse_args()
	token = Token.getTokensByValue(session, args['token'])
	if len(token) == 0:
		isAuthorized = 0
		return []
	token = token[0]
	diff = datetime.utcnow() - token.lastUpdate
	days, seconds = diff.days, diff.seconds
	hours = days * 24 + seconds // 3600
	if hours > 1:
		isAuthorized = 0
		print("token timeouted")

	if token.address != request.remote_addr:
		isAuthorized = 0
		print("different address")

	if isAuthorized == 0:
		Token.deleteToken(session, token.tokenId)
		return []

	token.lastUpdate = datetime.utcnow()
	Token.updateToken(session, token)	
	return [] 