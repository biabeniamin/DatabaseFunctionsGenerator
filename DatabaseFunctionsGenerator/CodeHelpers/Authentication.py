import Token
import TokenUser
from datetime import datetime
import uuid
from SqlAlchemyMain import session

def login(session, username, password, remote_addr):
	tokenUser = TokenUser.getTokenUsersByUsernamePassword(session, username, password)
	if len(tokenUser) == 0:
		print("invalid credentials")
		return {'error' : 'Invalid credentials'}, 0

	token = Token.Token(value=str(uuid.uuid4()), address = remote_addr, lastUpdate = datetime.utcnow(), tokenUserId = tokenUser[0].tokenUserId)
	Token.addToken(session, token)
	return token, 1

def checkToken(session, token, remote_addr):
	isAuthorized = 1
	error = None
	token = Token.getTokensByValue(session, token)
	if len(token) == 0:
		isAuthorized = 0
		error = 'Invalid token'
		return isAuthorized, error
	token = token[0]
	diff = datetime.utcnow() - token.lastUpdate
	days, seconds = diff.days, diff.seconds
	hours = days * 24 + seconds // 3600
	if hours > 1:
		isAuthorized = 0
		print("token timeouted")
		error = 'Token expired'

	if token.address != remote_addr:
		isAuthorized = 0
		print("different address")
		error = 'Token generated for other address'

	if isAuthorized == 0:
		Token.deleteToken(session, token.tokenId)
		return isAuthorized, error

	token.lastUpdate = datetime.utcnow()
	Token.updateToken(session, token)	
	return isAuthorized, error
