import Token
import TokenUser

def checkToken(session):
	print("checking token")
	from FlaskRestfulHelpers import getArguments
	requestedArgs = getArguments(['token'])
	args  = requestedArgs.parse_args()
	print(args)
	token = Token.getTokensByValue(session, args['token'])
	if len(token) == 0:
		print('invalid token')
	print(token)
	return [] 