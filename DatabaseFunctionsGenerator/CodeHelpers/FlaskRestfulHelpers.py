from flask_restful import reqparse

def getArguments(arguments):
	parser = reqparse.RequestParser()
	for argument in arguments:
		parser.add_argument(argument)
	return parser