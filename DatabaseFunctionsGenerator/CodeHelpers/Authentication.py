import Token
import TokenUser
from FlaskRestfulHelpers import getArguments
from flask import request
from datetime import datetime

def checkToken(session):
	isAuthorized = 1
	requestedArgs = getArguments(['token'])
	args  = requestedArgs.parse_args()
	token = Token.getTokensByValue(session, args['token'])
	if len(token) == 0:
		isAuthorized = 0
	token = token[0]
	diff = datetime.utcnow() - token.lastUpdate
	days, seconds = diff.days, diff.seconds
	hours = days * 24 + seconds // 3600
	if hours > 1:
		isAuthorized = 0

	if token.address != request.remote_addr:
		isAuthorized = 0
	print(request.remote_addr)
	return [] 